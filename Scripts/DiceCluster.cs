using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCluster : MonoBehaviour {

    public GameObject dice;

    public static DiceCluster dcInstance;

    public static Dice[] diceObjArray;
    public static DiceGroup[] diceGroupArray; // Starts from 1, not 0

    public static List<int> forcedRolls; // Forced takes preference over forbidden
    public static List<int> allowedRolls;

    public bool isCalculating;
    public static bool showingVictoryAnimation;

    const int diceLimit = 361; // Approximate number of dice that can be emulated without significant lag / space issues
    List<GameObject> dicePool;

    public static bool skipPostAttackAnimation = false;

    public static bool isARollTest = false;
    public static bool isAnEffectTest = false;

    public void Awake()
    {
        forcedRolls = new List<int>();
        allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        showingVictoryAnimation = false;

        dicePool = new List<GameObject>();
        for (int i=0; i < diceLimit + 1; i++) // 1 extra dice above the limit, to allow swapping dice
        {
            GameObject obj = Instantiate(dice);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            dicePool.Add(obj);
        }

        dcInstance = this;
    }

    GameObject getDiceFromPool()
    {
        foreach (GameObject obj in dicePool)
            if (!obj.activeInHierarchy) return obj;
        return null;
    }

    public IEnumerator rollAll()
    {
        yield return null; // Wait a frame for graphical updates

        deleteCurrentDice();

        AudioManager.amInstance.playRollingDiceSound();

        // Prepare variables
        double actualNDice = System.Math.Ceiling(Player.nDice * Player.diceMultiplier);
        if (actualNDice < 1) actualNDice = 1;
        if (!BattleManager.isTutorial)
        {
            Player.stats[Player.Stat.TOTAL_ROLLED_DICE] += actualNDice;
            if (actualNDice > Player.stats[Player.Stat.HIGHEST_N_OF_ROLLED_DICE])
                Player.stats[Player.Stat.HIGHEST_N_OF_ROLLED_DICE] = actualNDice;
        }
        int nDiceObjs = (actualNDice > diceLimit) ? diceLimit : (int)actualNDice; // Between nDice ~ diceLimit
        if (nDiceObjs < 1) nDiceObjs = 1;
        diceObjArray = new Dice[nDiceObjs];
        int diceGroupArraySize = Mathf.Max(1000, Player.maxRoll); // Between 1000 ~ maxRoll
        diceGroupArray = new DiceGroup[diceGroupArraySize + 1];
        for (int i = 0; i < diceGroupArray.Length; i++)
            diceGroupArray[i] = new DiceGroup();

        float diceScale = decideDiceScale(actualNDice);
        Vector2[] diceLocalPositions = decideDicePositions(nDiceObjs, diceScale, diceLimit);

        // Start calculating all dice final values
        isCalculating = true;
        StartCoroutine(getRandomValuesForDice(actualNDice, diceScale, diceLocalPositions));

        yield return new WaitForSeconds(Dice.minPreRollTime); // Rolling animation time

        yield return new WaitWhile(() => isCalculating);

        foreach (Dice d in diceObjArray)
        {
            d.stopRoll();
            d.forceValue(d.value);
        }

        AudioManager.amInstance.stopSound();
        AudioManager.amInstance.playRollEndSound();

        if (isARollTest) yield break;

        bool isAttackRoll = checkForAttackRolls();
        yield return new WaitForSeconds(0.25f);
        applyRollEffects();

        if (isAnEffectTest) yield break;

        if (BattleManager.isTutorial && TutorialManager.freezeScreenAfterRoll())
        {
            TutorialManager.showingDiceRolledText = true;
            yield return new WaitWhile(() => TutorialManager.showingDiceRolledText);
        }

        if (isAttackRoll)
        {
            applyVictoryColor();
            if (BattleManager.speedUpBattleAnimations)
                yield return new WaitForSeconds(.25f);
            else
                yield return new WaitForSeconds(.75f);
            AudioManager.amInstance.playPreAttackChime();

            if (!skipPostAttackAnimation)
            {
                showingVictoryAnimation = true;
                showVictoryHighlight();
                yield return new WaitForSeconds(1f);
                yield return new WaitWhile(() => showingVictoryAnimation);
            }

            double nAttackRolls = getNAttackRolls();
            deleteCurrentDice();
            BattleManager.damageDealtToEnemy = nAttackRolls;
        } else
        {
            if (BattleManager.speedUpBattleAnimations)
                yield return new WaitForSeconds(.25f);
            else
                yield return new WaitForSeconds(1f);
            deleteCurrentDice();
            BattleManager.damageDealtToEnemy = 0;
        }
        BattleManager.busy = false;
    }

    int getRandomRoll(double i, List<int> allowedNumbers)
    {
        if (forcedRolls.Count > i) return forcedRolls[(int)i]; // If forcedRolls[i] exists, return that
        
        return Dice.quickRoll(allowedRolls, allowedNumbers);
    }

    public static List<int> prepareAllowedNumbers()
    {
        List<int> allowedNumbers = new List<int>();
        for (int i = 1; i <= Player.maxRoll; i++) allowedNumbers.Add(i);
        for (int i = 0; i <= 9; i++)
        {
            if (!allowedRolls.Contains(i))
            {
                allowedNumbers.RemoveAll(x => (x % 10) == i);
            }
        }
        return allowedNumbers;
    }

    public IEnumerator getRandomValuesForDice(double actualNDice, float diceScale, Vector2[] diceLocalPositions)
    {
        List<int> allowedNumbers = prepareAllowedNumbers();

        // Performance trick: This cannot possibly run 1,000,000+ RNGs within a reasonable time frame,
        // so from a million onwards, will only roll dice until the leftover dice is a multiple of nº of allowed numbers,
        // then will apply statistical averages for those dice in sets (each allowed number will get +1 for each set)
        int diceSet = allowedNumbers.Count;
        if (diceSet == 0) diceSet = Player.maxRoll; // For impossible combinations, just consider each roll a set
        double cutoffPoint = 1000000 + ((actualNDice - 1000000) % diceSet);

        // Attack rolls created after the screen is filled will be randomly changed with a screen dice
        // In order to avoid slowdowns, there is a limit to how far we will look into when taking new dice to swap
        // (The dice's effects will still be applied, just no shown on screen)
        int diceSwapDepthLimit = 500 * Mathf.Min(Player.maxRoll, 1200);

        // Get random values for each individual dice
        for (double i = 0; i < actualNDice; i++)
        {
            // From a million+ onwards, simulate rolling via statistical averages. Too slow otherwise
            if (i >= cutoffPoint)
            {
                yield return null;
                // Divide leftover dice by dice per set -> Example: For each 1000d set, each number gets +1
                double leftoverDice = actualNDice - i;
                double nSets = leftoverDice / diceSet;
                if (allowedNumbers.Count != 0) // If no impossible rolls
                {
                    for (int j = 1; j <= Player.maxRoll; j++)
                    {
                        if (allowedRolls.Contains(j % 10))
                            diceGroupArray[j].nRolls += nSets;
                    }
                }
                else // If impossible rolls, just roll whatever to avoid freezing/infinite loops
                {
                    for (int j = 1; j <= Player.maxRoll; j++)
                    {
                        diceGroupArray[j].nRolls += nSets;
                    }
                }
                break;
            }

            // Wait a frame every 50,000 dice to avoid freezes or slowdowns
            if (i % 50000 == 0 && i > 0 && !skipPostAttackAnimation) yield return null;

            // Decide whether the roll should be random or fixed (for tutorials, etc)
            int rollResult = getRandomRoll(i, allowedNumbers);

            // If the dice lands on an attack roll, make sure to show it onscreen
            bool forceShowDice = false;
            if (!skipPostAttackAnimation && i < diceSwapDepthLimit && Player.attackRolls.Contains(rollResult))
                forceShowDice = true;

            diceGroupArray[rollResult].nRolls++;


            // Create dice object to show on screen (only for the first few dice that can fit in the screen)
            if (i < diceLimit || (forceShowDice && i < diceSwapDepthLimit))
            {
                int diceIndex = (int)i;
                GameObject newDice = getDiceFromPool();
                newDice.SetActive(true);
                newDice.GetComponent<RectTransform>().localScale = Vector3.one * diceScale;

                // Add to array (object) and dictionary (array id)
                if (i < diceLimit)
                {
                    addDiceToArrays(rollResult, diceIndex, newDice);
                }
                else // If there are too many dice in the screen, swap it for a random one
                {
                    int randomIndex = Random.Range(0, diceLimit);
                    GameObject oldDice = swapDiceForRandomOne(randomIndex, rollResult, newDice);
                    oldDice.SetActive(false);
                    diceIndex = randomIndex;
                }

                newDice.GetComponent<RectTransform>().anchoredPosition = diceLocalPositions[diceIndex];
                // Set z to 0 to solve issues in wide screens
                Vector3 dicePosition = newDice.GetComponent<RectTransform>().localPosition;
                newDice.GetComponent<RectTransform>().localPosition = new Vector3(dicePosition.x, dicePosition.y, 0);

                // Preassign value for later dice checks
                diceObjArray[diceIndex].value = rollResult;

                // Show dice rolling animation
                StartCoroutine(diceObjArray[diceIndex].roll());
            }
        }
        isCalculating = false;
        yield break;
    }

    // The dice will be distributed in a grid
    // N of columns will be the closest perfect square matching or above nOfDice
    public static int getNCols(int nOfDice)
    {
        return (int)Mathf.Ceil(Mathf.Sqrt(nOfDice));
    }

    // N of rows will be the minimum needed to include all dice
    public static int getNRows(int nOfDice, int nCols)
    {
        return (int)Mathf.Ceil(nOfDice / (float)nCols);
    }

    public static Vector2[] decideDicePositions(int nDiceObjs, float diceScale, int diceLimit)
    {
        Vector2[] dicePositions = new Vector2[nDiceObjs];

        int nCols = getNCols(nDiceObjs);
        int nRows = getNRows(nDiceObjs, nCols);

        // If the n of rows or cols is even, the middle row/col will be decimal
        // (i.e.: the middle row for 3 is 2, for 4 is 2.5, for 5 is 3)
        float middleRow = (nRows / 2f) + 0.5f;
        float middleCol = (nCols / 2f) + 0.5f;

        float diceSeparationH = Mathf.Infinity;
        if (middleCol != 1)
            diceSeparationH = (780f - (0.5f * Dice.WIDTH * diceScale)) / (middleCol - 1f);
        diceSeparationH = Mathf.Min(diceSeparationH, 400f); // Set a maximum separation for <4 rows

        float diceSeparationV = -Mathf.Infinity;
        if (middleRow != 1)
            diceSeparationV = -1f * (450f - (0.5f * Dice.HEIGHT * diceScale)) / (middleRow - 1f); // Negative so that it grows downwards
        diceSeparationV = Mathf.Max(diceSeparationV, -110f); // Set a maximum separation for <8 cols

        int nEmptySpacesInLastRow = (nRows * nCols) - nDiceObjs;
        float xLastRowDisplacement = nEmptySpacesInLastRow * 0.5f * diceSeparationH;

        for (int i=0; i < nDiceObjs; i++)
        {
            int row = (int)Mathf.Floor(i / nCols) + 1; // Die in the same row have the same quotient (respect to nCols)
            int col = i % nCols + 1; // Die in the same column have the same remainder (respect to nCols)

            float xPos = (col - middleCol) * diceSeparationH;
            float yPos = (row - middleRow) * diceSeparationV;

            if (row == nRows) // If last row, center dice (since they might not completely fill it)
            {
                xPos += xLastRowDisplacement;
            }

            dicePositions[i] = new Vector2( xPos, yPos );
        }
        return dicePositions;
    }

    public static float decideDiceScale(double actualNDice)
    {
        if (actualNDice <= 25)
            // Scale 1.5 for 1d, scale 1 for 25d
            return (1f + (25 - (float)actualNDice) / 48);
        if (actualNDice <= 36)
            return 1;
        if (actualNDice <= 121)
            // Scale 0.85 for 36d, scale 0.55 for 100d
            return Mathf.Max(.55f + (100 - (float)actualNDice) / 230);
        if (actualNDice <= 169)
            return 0.457f;
        if (actualNDice <= 225)
            return 0.4f;
        if (actualNDice <= 256)
            return 0.375f;
        if (actualNDice <= 289)
            return 0.35f;
        // Works well up to 19^2 = 361. Breaks from 362 onwards
        return 0.325f;
    }

    public static void addDiceToArrays(int rollResult, int diceIndex, GameObject newDice)
    {
        addDiceToObjArray(diceIndex, newDice);
        addDiceToGroupArray(rollResult, diceIndex);
    }

    static void addDiceToObjArray(int diceIndex, GameObject newDice)
    {
        diceObjArray[diceIndex] = newDice.GetComponent<Dice>();
    }

    static void addDiceToGroupArray(int rollResult, int diceIndex)
    {
        diceGroupArray[rollResult].diceIDs.Add(diceIndex);
    }

    public static GameObject swapDiceForRandomOne(int rnd, int rollResult, GameObject newDice)
    {
        // Remove old dice object ID from the dictionary
        GameObject oldDiceObj = diceObjArray[rnd].gameObject;
        int oldDiceValue = diceObjArray[rnd].value;
        diceGroupArray[oldDiceValue].diceIDs.Remove(rnd);

        // Add new dice to arrays
        addDiceToObjArray(rnd, newDice);
        addDiceToGroupArray(rollResult, rnd);

        return oldDiceObj;
    }

    void deleteCurrentDice()
    {
        foreach (GameObject dice in dicePool)
            dice.SetActive(false);
    }

    void applyRollEffects()
    {
        foreach (CardEffect ce in Player.cardEffects)
        {
            if (ce.effect == CardEffect.Effect.DICE_MOD) continue; // Dice mods are applied only once per turn, skip
            switch (ce.condition)
            {
                case CardEffect.Condition.FIRST:
                    for (int i = 0; i < diceGroupArray.Length; i++)
                    {
                        if (diceGroupArray[i].nRolls > 0 && CardEffect.firstNumberIs(i, ce.conditionNumber) && i != 0)
                            ce.applyEffect(diceObjArray, diceGroupArray[i]);
                    }
                    break;
                case CardEffect.Condition.LAST:
                    for (int i = 0; i < diceGroupArray.Length; i++)
                    {
                        if (diceGroupArray[i].nRolls > 0 && CardEffect.lastNumberIs(i, ce.conditionNumber) && i != 0)
                            ce.applyEffect(diceObjArray, diceGroupArray[i]);
                    }
                    break;
            }
        }
    }

    double getNAttackRolls()
    {
        double nAttackRolls = 0;
        foreach (int attackRoll in Player.attackRolls)
        {
            if (diceGroupArray.Length > attackRoll)
                nAttackRolls += diceGroupArray[attackRoll].nRolls;
        }
        if (!BattleManager.isTutorial) Player.stats[Player.Stat.TOTAL_ATTACK_ROLLS] += nAttackRolls;
        return nAttackRolls;
    }

    bool checkForAttackRolls()
    {
        foreach (int attackRoll in Player.attackRolls)
        {
            if (diceGroupArray[attackRoll].nRolls > 0)
                return true;
        }
        return false;
    }

    void applyVictoryColor()
    {
        foreach (int attackRoll in Player.attackRolls)
            if (diceGroupArray.Length > attackRoll)
                foreach (int winnerID in diceGroupArray[attackRoll].diceIDs)
                    diceObjArray[winnerID].changeTextColor(Dice.victoryColor);
    }

    void showVictoryHighlight()
    {
        // Show winners
        foreach (int attackRoll in Player.attackRolls)
            if (diceGroupArray.Length > attackRoll)
                foreach (int winnerID in diceGroupArray[attackRoll].diceIDs)
                    diceObjArray[winnerID].highlightVictoryRoll();

        // Fade out non-winners
        for (int roll = 0; roll < diceGroupArray.Length; roll++)
        {
            if (Player.attackRolls.Contains(roll)) // Skip winner roll
                continue;
            foreach (int nonWinnerID in diceGroupArray[roll].diceIDs)
                diceObjArray[nonWinnerID].fadeOut(0.35f);
        }
                    
    }

}
