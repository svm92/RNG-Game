using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Arm = ArmManager.Arm;
using ArmAspect = ArmManager.ArmAspect;
using Core = CoreManager.Core;

public class Enemy : MonoBehaviour {

    public Material eyeMaterial;

    [HideInInspector] public bool finishedInitialization = false;
    [HideInInspector] public List<Arm> armList;
    [HideInInspector] public List<ArmAspect> armAspects;
    [HideInInspector] public Core currentCore;

    [HideInInspector] public Attack lastUsedAttack;
    [HideInInspector] public double damageFromLastAttack;

    [HideInInspector] public int difficultyId = 0;
    [HideInInspector] public string enemyName;
    public double maxHealth;
    public double health;
    [HideInInspector] public double damageAgainstHealth;
    [HideInInspector] public float damageAgainstDiceMin; // %
    [HideInInspector] public float damageAgainstDiceMax; // %
    [HideInInspector] public int minDiceToAllowDiceAttacks; // Enemy won't use dice-removing attacks until the player has at least this nDice
    float protectionShields = 0; // Lowers all damage received by this %

    float diceAttackLv; // Used for advanced hook-arms (knocking off dice while attacking)
    float recoilAttackLv; // Extra damage when doing a recoil attack
    double recoilAttackCost;
    double drainLv; // Used for drain attack
    float chargeLv; // Used for battery
    double curseLv = 1; // Dice multiplier for curse
    float healLv; // Heal %
    float buzzsawAttackLv; // Damage dealt by buzzsaw
    float fanAttackLv; // Damage dealt by fan
    float floeAttackLv = 1; // Card improvement multiplier for floe
    float magicianAttackLv = 1; // Card improvement multiplier for magician

    bool healEveryTurn;
    bool increaseRollEveryTurn;
    bool loseDiceEveryTurn;

    [HideInInspector] public List<Attack> possibleAttacks;
    Arm[] possibleArms;
    int nOfArms;
    Core[] possibleCores;

    public enum Attack { NOTHING, NORMAL_ATTACK, DICE_ATTACK, MILL, DISCARD, HEAL, RECOIL_ATTACK, DRAIN, CHARGE, CURSE,
        WEAKEN_CARDS, SWAP_CARDS};
    
    [HideInInspector] public Slider healthSlider;
    double healthBeforeLastDamage;
    Text healthText;
    GameObject eye;
    Animator eyeAnimator;

    public void initialize(int difficultyId)
    {
        this.difficultyId = difficultyId;
        initializeEnemyData(difficultyId);
        health = maxHealth;
        enemyName = getRandomName();

        // Get health slider/text info
        healthSlider = GameObject.FindGameObjectWithTag("BossHealthSlider").GetComponent<Slider>();
        healthText = GameObject.FindGameObjectWithTag("BossHealthText").GetComponent<Text>();
        refreshHealthHUD(true);

        // Other info
        eye = transform.GetChild(0).gameObject;
        eyeAnimator = eye.GetComponent<Animator>();
    }

    public void initializeEnemyData(int difficultyId)
    {
        initializeSpecialEnemyDict();

        // Increases faster later on
        maxHealth = System.Math.Floor(System.Math.Pow(difficultyId, 6d) * 0.01d
            + 7 * System.Math.Pow(2d, difficultyId / 4d));
        if (maxHealth > 1E+304 || difficultyId > 4050) // Upper bound to avoid overflow issues
        {
            maxHealth = 1E+304;
        }
        // Increases faster later on
        damageAgainstHealth = System.Math.Floor(4d + difficultyId + System.Math.Exp((difficultyId / 40d) * System.Math.Log(40d)));
        if (damageAgainstHealth > 1E+300 || difficultyId > 7500) // Upper bound to avoid overflow issues
        {
            damageAgainstHealth = 1E+300;
        }
        // Asymptotically approaches .4f, medium-fast (5 -> 0.09, 10 -> 0.14, 50 -> 0.32, 100 -> 0.38)
        damageAgainstDiceMin = .05f + .35f * (1 - Mathf.Exp(-0.03f * difficultyId));
        // Asymptotically approaches .8f, slow (5 -> 0.15, 10 -> 0.19, 50 -> 0.46, 100 -> 0.64)
        damageAgainstDiceMax = .1f + .7f * (1 - Mathf.Exp(-0.015f * difficultyId));
        // Asymptotically approaches 5, medium-fast (5 -> 85, 10 -> 76, 50 -> 27, 100 -> 10, 150 -> 6)
        minDiceToAllowDiceAttacks = 100 - Mathf.FloorToInt(95f * (1 - Mathf.Exp(-0.03f * difficultyId)));

        possibleAttacks = getPossibleAttacks(difficultyId);
        possibleArms = getPossibleArms(difficultyId);
        possibleCores = getPossibleCores(difficultyId);

        nOfArms = getNArms(difficultyId);
        armAspects = getArmAspects(difficultyId, nOfArms);
    }

    List<Attack> getPossibleAttacks(int difficultyId)
    {
        if (difficultyId <= 1) // 33% nothing, 66% attack
        {
            return new List<Attack>() { Attack.NOTHING, Attack.NORMAL_ATTACK, Attack.NORMAL_ATTACK };
        }
        else if (difficultyId <= 40) // 25% nothing, 50% attack, 25% knock dice
        {
            return new List<Attack>() { Attack.NOTHING, Attack.NORMAL_ATTACK, Attack.NORMAL_ATTACK, Attack.DICE_ATTACK };
        }
        else // 66% attack, 33% knock dice
        {
            return new List<Attack>() { Attack.NORMAL_ATTACK, Attack.NORMAL_ATTACK, Attack.DICE_ATTACK };
        }
    }

    Arm[] getPossibleArms(int difficultyId)
    {
        if (isSpecialId(difficultyId)) return new Arm[] { getSpecialIdArmType(difficultyId) };

        List<Arm> allowedArms = new List<Arm>();
        if (difficultyId % 5 != 4) // Normal arm not allowed on 4, 8, 12...
        {
            allowedArms.Add(Arm.NORMAL);
        }
        if (difficultyId >= 2)
        {
            allowedArms.Add(Arm.BLADE);
        }
        if (difficultyId >= 3)
        {
            allowedArms.Add(Arm.HOOK);
            allowedArms.Add(Arm.SHIELD);
        }
        if (difficultyId >= 5)
        {
            allowedArms.Add(Arm.BUZZSAW);
            allowedArms.Add(Arm.GRAPNEL);
        }
        if (difficultyId >= 7)
        {
            allowedArms.Add(Arm.SALVE);
            allowedArms.Add(Arm.BATTERY);
        }
        if (difficultyId >= 10)
        {
            allowedArms.Add(Arm.FAN);
            allowedArms.Add(Arm.AEGIS);
            allowedArms.Add(Arm.CARTILAGE);
        }
        if (difficultyId >= 15)
        {
            allowedArms.Add(Arm.SIPHON);
        }
        if (difficultyId >= 20)
        {
            allowedArms.Add(Arm.POWER_GLOW);
        }
        if (difficultyId >= 25)
        {
            allowedArms.Add(Arm.MIASMA);
        }
        if (difficultyId >= 35)
        {
            allowedArms.Add(Arm.FORBIDDEN_RELIC);
        }
        if (difficultyId >= 75)
        {
            allowedArms.Add(Arm.FLOE);
        }
        if (difficultyId >= 100)
        {
            allowedArms.Add(Arm.ILLUSIONIST);
        }

        return allowedArms.ToArray();
    }

    Core[] getPossibleCores(int difficultyId)
    {
        if (isSpecialId(difficultyId)) return new Core[] { Core.GOLD };

        List<Core> allowedCores = new List<Core>();
        if (this.difficultyId % 5 != 4) // Normal core not allowed on 4, 8, 12...
        {
            allowedCores.Add(Core.NORMAL);
        }
        if (this.difficultyId >= 4)
        {
            allowedCores.Add(Core.RED);
        }
        if (this.difficultyId >= 8)
        {
            allowedCores.Add(Core.GREEN);
        }
        if (this.difficultyId >= 15)
        {
            allowedCores.Add(Core.BLUE);
        }
        if (this.difficultyId >= 65)
        {
            allowedCores.Add(Core.CYAN);
        }
        if (this.difficultyId >= 105)
        {
            allowedCores.Add(Core.MAGENTA);
        }
        if (this.difficultyId >= 140)
        {
            allowedCores.Add(Core.ORANGE);
        }
        return allowedCores.ToArray();
    }

    public static int getNArms(int difficultyId)
    {
        if (isSpecialId(difficultyId)) return getSpecialIdNArms(difficultyId);

        // Except for 5 and 10, every 5th (15, 20, 25...)
        // From 30 on, every 4th and 5th (34, 35, 49, 50...)
        // From 50 on, every 3/4/5th
        // From 100 on, all
        if (difficultyId >= 100) return 4;
        if (difficultyId > 9 && (difficultyId % 5 == 4
            || (difficultyId >= 29 && (difficultyId % 5 == 3))
            || (difficultyId >= 49 && (difficultyId % 5 == 2))))
            return 4;
        return 2;
    }

    static List<ArmAspect> getArmAspects(int difficultyId, int nArms)
    {
        if (isSpecialId(difficultyId)) return getSpecialIdAspects(difficultyId, nArms);

        List<ArmAspect> armAspects = new List<ArmAspect>();
        for (int i = 0; i < nArms; i++)
        {
            armAspects.Add(getRandomArmAspect(difficultyId));
        }
        return armAspects;
    }

    static ArmAspect getRandomArmAspect(int difficultyId)
    {
        float rnd;

        // Gold check
        if (difficultyId >= 249)
        {
            rnd = Random.Range(0f, 1f);
            if (rnd < getChanceOfGoldArm(difficultyId))
                return ArmAspect.GOLD;
        }

        // Silver check
        if (difficultyId >= 99)
        {
            rnd = Random.Range(0f, 1f);
            if (rnd < getChanceOfSilverArm(difficultyId))
                return ArmAspect.SILVER;
        }

        return ArmAspect.NORMAL;
    }

    public static float getChanceOfSilverArm(int difficultyId) // Starts at 99, maxes out at 500
    {
        if (difficultyId < 99) return 0;
        return Mathf.Min(0.01f + 0.99f * (difficultyId - 99f) / 401f, 1f);
    }

    public static float getChanceOfGoldArm(int difficultyId) // Starts at 249, maxes out at 2000
    {
        if (difficultyId < 249) return 0;
        return Mathf.Min(0.01f + 0.99f * (difficultyId - 249f) / 1751f, 1f);
    }

    public static Dictionary<int, SpecialEnemyInfo> specialEnemyDict;
    public static void initializeSpecialEnemyDict ()
    {
        if (specialEnemyDict != null) return;

        int startingBase = 109;
        int specialDelta = 10;
        specialEnemyDict = new Dictionary<int, SpecialEnemyInfo>();
        List<Arm> armOrder = new List<Arm>() { Arm.NORMAL, Arm.BLADE, Arm.HOOK, Arm.SHIELD, Arm.MIASMA, Arm.CARTILAGE,
            Arm.FORBIDDEN_RELIC, Arm.POWER_GLOW, Arm.FLOE, Arm.ILLUSIONIST, Arm.BUZZSAW, Arm.FAN};

        // Enemies with 6 arms
        for (int i=0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(6, armOrder[i], ArmAspect.NORMAL));
        }

        // Enemies with 8 arms
        startingBase = startingBase + armOrder.Count * specialDelta;
        for (int i = 0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(8, armOrder[i], ArmAspect.NORMAL));
        }

        // Enemies with 6 silver arms
        startingBase = startingBase + armOrder.Count * specialDelta;
        for (int i = 0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(6, armOrder[i], ArmAspect.SILVER));
        }

        // Enemies with 8 silver arms
        startingBase = startingBase + armOrder.Count * specialDelta;
        for (int i = 0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(8, armOrder[i], ArmAspect.SILVER));
        }

        // Enemies with 6 gold arms
        startingBase = startingBase + armOrder.Count * specialDelta;
        for (int i = 0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(6, armOrder[i], ArmAspect.GOLD));
        }

        // Enemies with 8 gold arms
        startingBase = startingBase + armOrder.Count * specialDelta;
        for (int i = 0; i < armOrder.Count; i++)
        {
            specialEnemyDict.Add(startingBase + i * specialDelta, new SpecialEnemyInfo(8, armOrder[i], ArmAspect.GOLD));
        }
    }

    static bool isSpecialId(int difficultyId)
    {
        if (specialEnemyDict == null) initializeSpecialEnemyDict();
        return (specialEnemyDict.ContainsKey(difficultyId));
    }

    static int getSpecialIdNArms(int difficultyId)
    {
        return specialEnemyDict[difficultyId].nArms;
    }

    static Arm getSpecialIdArmType(int difficultyId)
    {
        return specialEnemyDict[difficultyId].arm;
    }

    static List<ArmAspect> getSpecialIdAspects(int difficultyId, int nArms)
    {
        List<ArmAspect> listArmAspects = new List<ArmAspect>();
        ArmAspect specialIdAspect = specialEnemyDict[difficultyId].aspect;
        for (int i=0; i < nArms; i++)
        {
            listArmAspects.Add(specialIdAspect);
        }
        return listArmAspects;
    }

    public struct SpecialEnemyInfo
    {
        public int nArms;
        public Arm arm;
        public ArmAspect aspect;
        public SpecialEnemyInfo(int nArms, Arm arm, ArmAspect aspect)
        {
            this.nArms = nArms;
            this.arm = arm;
            this.aspect = aspect;
        }
    }

    private void Start()
    {
        if (possibleArms == null) // Debug, to allow initializing from outside battle, can be deleted
        {
            initializeEnemyData(0);
        }

        // Assign body
        int rnd = Random.Range(0, possibleCores.Length);
        currentCore = possibleCores[rnd];
        GetComponent<Image>().sprite = CoreManager.cmInstance.getSpriteFromCore(currentCore);
        if (currentCore == Core.MAGENTA)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(230, 230);
        }
        applyCoreEffects(currentCore);

        // Assign arms
        armList = ArmManager.amInstance.assignArms(transform, possibleArms, nOfArms, armAspects);
        foreach (EnemyLimb el in GetComponentsInChildren<EnemyLimb>())
            el.initialize();
        applyAllArmEffects(armList, armAspects);
        finishedInitialization = true;

        // Change scale (purely cosmetic) for more than 2 arms
        if (nOfArms > 2)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale *= (nOfArms == 4) ? 1.2f : (nOfArms == 6) ? 1.4f : 1.6f;
            // Scale correction for arms (so that they stay at x1)
            foreach (Transform child in transform)
                if (child.GetComponent<EnemyLimb>() != null)
                    child.GetComponent<RectTransform>().localScale *= 1f / rt.localScale.x;
        }

        // Eye
        changeEyeColor();
        Invoke("animateEye", 0.2f);
    }

    void changeEyeColor()
    {
        if (currentCore == Core.NORMAL) return;
        if (eye == null) eye = transform.GetChild(0).gameObject;
        eye.GetComponent<Image>().material = eyeMaterial;
        Color eyeColor = CoreManager.getEyeColor(currentCore);
        eyeMaterial.SetFloat("_R", eyeColor.r);
        eyeMaterial.SetFloat("_G", eyeColor.g);
        eyeMaterial.SetFloat("_B", eyeColor.b);
    }

    void animateEye()
    {
        // If inactive or already animating, wait
        if (!eyeAnimator.isActiveAndEnabled || !eyeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Invoke("animateEye", 0.4f);
            return;
        }

        float rnd = Random.Range(0f, 1f);
        if (rnd < 0.04f) // Blink animation
        {
            eyeAnimator.SetTrigger("Blink");
            Invoke("animateEye", 2.5f);
        } else if (rnd < 0.055f) // Shift eye animation (random direction)
        {
            bool flipHorizontalDirection = (Random.Range(0, 2) == 0);
            if (flipHorizontalDirection) // Flip the horizontal direction half of the time
            {
                Vector3 currentScale = eye.GetComponent<RectTransform>().localScale;
                eye.GetComponent<RectTransform>().localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
            }
            eyeAnimator.SetTrigger("ShiftEye");
            Invoke("animateEye", 3.5f);
        } else
        {
            Invoke("animateEye", 0.2f);
        }
    }

    public void decideAttack()
    {
        int rnd = Random.Range(0, possibleAttacks.Count);
        Attack attack = possibleAttacks[rnd];
        StartCoroutine(launchAttack(attack));
    }

    public IEnumerator launchAttack(Attack attack)
    {
        string attackDescription = TextScript.get(TextScript.Sentence.EXCL) + enemyName + " " + 
            attackDescriptor(attack) + "\n";
        double actualDamage = 0; double extraDamage = 0;
        switch (attack)
        {
            case Attack.NOTHING:
                break;
            case Attack.NORMAL_ATTACK: // Attack against health
                actualDamage = Player.receiveDamage(damageAgainstHealth);
                attackDescription += TextScript.get(TextScript.Sentence.LOSE_HEALTH_A) + " " + 
                    NumberStringConverter.convert(actualDamage) + " " + TextScript.get(TextScript.Sentence.LOSE_HEALTH_B) + ".";
                break;
            case Attack.DICE_ATTACK: // Attack against dice
                if (minDiceToAllowDiceAttacks > Player.nDice) // If the player has too few dice, choose another attack
                {
                    decideAttack();
                    yield break;
                }

                float dicePercDamage = Random.Range(damageAgainstDiceMin, damageAgainstDiceMax);
                double diceDamage = System.Math.Round(Player.nDice * dicePercDamage);
                actualDamage = Player.loseDice(diceDamage);
                string diceSingOrPlural = (actualDamage == 1) ? TextScript.get(TextScript.Sentence.SINGLE_DICE) :
                   TextScript.get(TextScript.Sentence.DICE);

                attackDescription += TextScript.get(TextScript.Sentence.LOSE_DICE) + " " +
                    NumberStringConverter.convert(actualDamage) + " " + diceSingOrPlural;

                if (diceAttackLv > 0) // Advanced attack, deal damage while knocking off dice
                {
                    extraDamage = Player.receiveDamage(damageAgainstHealth * diceAttackLv);
                    attackDescription += " " + TextScript.get(TextScript.Sentence.AND) + " " + 
                        NumberStringConverter.convert(extraDamage) + " " + TextScript.get(TextScript.Sentence.HEALTH);
                }

                attackDescription += ".";
                break;
            case Attack.MILL:
                if (BattleManager.bmInstance.noCardsLeftInDeck()) // If the player has no cards in deck, choose another attack
                {
                    decideAttack();
                    yield break;
                }

                int nMilledCards = (buzzsawAttackLv < 0.7f) ? 1 : 2;
                actualDamage = BattleManager.bmInstance.mill(nMilledCards);
                string cardSingOrPlural = (actualDamage == 1) ? TextScript.get(TextScript.Sentence.CARD_SINGLE) :
                    TextScript.get(TextScript.Sentence.CARD_PLURAL);

                if (buzzsawAttackLv == 0) // Only milling
                {
                    attackDescription += TextScript.get(TextScript.Sentence.YOUR_DECK_LOSES) + " " +
                        actualDamage + " " + cardSingOrPlural + ".";
                }
                else // Mill + damage
                {
                    extraDamage = Player.receiveDamage(damageAgainstHealth * buzzsawAttackLv);
                    attackDescription += TextScript.get(TextScript.Sentence.YOUR_DECK_LOSES) + " " +
                        actualDamage + " " + cardSingOrPlural + ". " + TextScript.get(TextScript.Sentence.LOSE_HEALTH_A) + " " +
                        NumberStringConverter.convert(extraDamage) + " " + TextScript.get(TextScript.Sentence.LOSE_HEALTH_B) + "."; ;
                }
                break;
            case Attack.DISCARD:
                if (BattleManager.bmInstance.noCardsLeftInHand()) // If the player has no cards in hand, choose another attack
                {
                    decideAttack();
                    yield break;
                }

                BattleManager.bmInstance.discard();
                if (fanAttackLv == 0)
                {
                    attackDescription += TextScript.get(TextScript.Sentence.DISCARD);
                }  
                else
                {
                    extraDamage = Player.receiveDamage(damageAgainstHealth * fanAttackLv);
                    attackDescription += TextScript.get(TextScript.Sentence.DISCARD_PLUS_DAMAGE_A) + " " + 
                        TextScript.get(TextScript.Sentence.DISCARD_PLUS_DAMAGE_B) + " " + NumberStringConverter.convert(extraDamage) + 
                        " " + TextScript.get(TextScript.Sentence.LOSE_HEALTH_B);
                }
                break;
            case Attack.HEAL:
                if (health >= maxHealth && healLv < 0.35) // Cannot overheal with normal arms
                {
                    decideAttack();
                    yield break;
                }

                bool allowExcedeMax = (healLv > 0.35f);
                double healedAmount = healPerc(healLv, allowExcedeMax);
                attackDescription += TextScript.get(TextScript.Sentence.RECOVER) + " " +
                    NumberStringConverter.convert(healedAmount) + " " + TextScript.get(TextScript.Sentence.HEALTH) + ".";
                break;
            case Attack.RECOIL_ATTACK:
                double cost = System.Math.Round(maxHealth * recoilAttackCost);
                if (cost >= health)
                {
                    decideAttack();
                    yield break;
                }

                health -= cost;
                refreshHealthHUD(false);
                double recoilPower = damageAgainstHealth * recoilAttackLv;
                actualDamage = Player.receiveDamage(recoilPower);
                attackDescription += TextScript.get(TextScript.Sentence.LOSE_HEALTH_A) + " " +
                    NumberStringConverter.convert(actualDamage) + " " + TextScript.get(TextScript.Sentence.LOSE_HEALTH_B) + ".";
                break;
            case Attack.DRAIN:
                if (health >= maxHealth)
                {
                    decideAttack();
                    yield break;
                }

                double drainDamage = System.Math.Round(damageAgainstHealth * drainLv);
                actualDamage = Player.receiveDamage(drainDamage);
                healValue(actualDamage, false);
                attackDescription += TextScript.get(TextScript.Sentence.DRAIN_A) + " " +
                    NumberStringConverter.convert(actualDamage) + " " + TextScript.get(TextScript.Sentence.DRAIN_B) + ".";
                break;
            case Attack.CHARGE:
                damageAgainstHealth = System.Math.Round(damageAgainstHealth * chargeLv);
                attackDescription += TextScript.get(TextScript.Sentence.ATTACK_PLUS);
                break;
            case Attack.CURSE:
                if (Player.nDice == 1)
                {
                    decideAttack();
                    yield break;
                }

                Player.diceMultiplier *= curseLv;
                if (curseLv == 0.5)
                    attackDescription += TextScript.get(TextScript.Sentence.CURSE_HALF);
                else
                    attackDescription += TextScript.get(TextScript.Sentence.CURSE_GENERIC);
                break;
            case Attack.WEAKEN_CARDS:
                BattleManager.bmInstance.applyCardImproveMultiplicative(floeAttackLv);
                attackDescription += TextScript.get(TextScript.Sentence.WEAKEN_CARDS);
                break;
            case Attack.SWAP_CARDS:
                BattleManager.bmInstance.applyCardSwap(magicianAttackLv);
                attackDescription += TextScript.get(TextScript.Sentence.SWAP_CARDS);
                break;
        }
        lastUsedAttack = attack;
        damageFromLastAttack = actualDamage;
        BattleManager.bmInstance.showInfoTextPanel();
        BattleManager.bmInstance.changeInfoText(attackDescription);
        BattleManager.busy = false;
    }

    string attackDescriptor(Attack attack)
    {
        switch (attack)
        {
            case Attack.NOTHING:
                return TextScript.get(TextScript.Sentence.DESC_NOTHING);
            case Attack.NORMAL_ATTACK:
                return TextScript.get(TextScript.Sentence.DESC_NORMAL);
            case Attack.DICE_ATTACK:
                return TextScript.get(TextScript.Sentence.DESC_DICE);
            case Attack.MILL:
                return TextScript.get(TextScript.Sentence.DESC_MILL);
            case Attack.DISCARD:
                return TextScript.get(TextScript.Sentence.DESC_DISCARD);
            case Attack.HEAL:
                return TextScript.get(TextScript.Sentence.DESC_HEAL);
            case Attack.RECOIL_ATTACK:
                return TextScript.get(TextScript.Sentence.DESC_RECOIL);
            case Attack.DRAIN:
                return TextScript.get(TextScript.Sentence.DESC_DRAIN);
            case Attack.CHARGE:
                return TextScript.get(TextScript.Sentence.DESC_CHARGE);
            case Attack.CURSE:
                return TextScript.get(TextScript.Sentence.DESC_CURSE);
            case Attack.WEAKEN_CARDS:
                return TextScript.get(TextScript.Sentence.DESC_WEAKEN_CARDS);
            case Attack.SWAP_CARDS:
                return TextScript.get(TextScript.Sentence.DESC_SWAP_CARDS);
        }
        return "";
    }

    void applyCoreEffects(Core core)
    {
        switch (core)
        {
            case Core.NORMAL: // No effect
            case Core.GOLD:
                break;
            case Core.BLUE: // Skip drawing every 3rd turn (in turns 3, 6, 9...)
                BattleManager.bmInstance.skipDrawingEveryThirdTurn = true;
                break;
            case Core.RED: // Increase max roll by 200
                Player.modifyMaxRoll(200);
                break;
            case Core.GREEN: // Draw 1 less card as battle begins
                Player.initialHandSize -= 1;
                break;
            case Core.CYAN: // Heals every turn
                healEveryTurn = true;
                break;
            case Core.MAGENTA: // Increases max roll every turn
                increaseRollEveryTurn = true;
                break;
            case Core.ORANGE: // Lose dice every turn
                loseDiceEveryTurn = true;
                break;

        }
    }

    public void applyAllArmEffects(List<Arm> armTypes, List<ArmAspect> armAspects)
    {
        for (int i=0; i < armTypes.Count; i++)
        {
            applyArmEffect(armTypes[i], armAspects[i]);
        }
    }

    void applyArmEffect(Arm arm, ArmAspect armAspect)
    {
        switch (arm)
        {
            case Arm.NORMAL: // No effect
                break;

            case Arm.HOOK: // Increases chances of dice attack
                possibleAttacks.Add(Attack.DICE_ATTACK);
                if (armAspect == ArmAspect.SILVER) diceAttackLv = Mathf.Max(0.5f, diceAttackLv);
                else if (armAspect == ArmAspect.GOLD) diceAttackLv = Mathf.Max(1f, diceAttackLv);
                break;

            case Arm.POWER_GLOW: // +20% damage
                double extraAttack = 1;
                if (armAspect == ArmAspect.NORMAL) extraAttack = 1.2;
                else if (armAspect == ArmAspect.SILVER) extraAttack = 1.5;
                else if (armAspect == ArmAspect.GOLD) extraAttack = 2.5;
                damageAgainstHealth = System.Math.Round(damageAgainstHealth * extraAttack);
                break;

            case Arm.BLADE: // Increases chances of normal attack
                possibleAttacks.Add(Attack.NORMAL_ATTACK);
                extraAttack = 1;
                if (armAspect == ArmAspect.NORMAL) extraAttack = 1;
                else if (armAspect == ArmAspect.SILVER) extraAttack = 1.2;
                else if (armAspect == ArmAspect.GOLD) extraAttack = 1.5;
                damageAgainstHealth = System.Math.Round(damageAgainstHealth * extraAttack);
                break;
            
            case Arm.BUZZSAW: // Allows milling attacks
                possibleAttacks.Add(Attack.MILL);
                if (armAspect == ArmAspect.SILVER) buzzsawAttackLv = Mathf.Max(0.5f, buzzsawAttackLv);
                else if (armAspect == ArmAspect.GOLD) buzzsawAttackLv = Mathf.Max(0.75f, buzzsawAttackLv);
                break;

            case Arm.SHIELD: // +20% health
                float extraHealth = 1;
                if (armAspect == ArmAspect.NORMAL) extraHealth = 1.2f;
                else if (armAspect == ArmAspect.SILVER) extraHealth = 2f;
                else if (armAspect == ArmAspect.GOLD) extraHealth = 4f;
                multiplyMaxHealth(extraHealth);
                break;

            case Arm.FAN: // Allows discarding attacks
                possibleAttacks.Add(Attack.DISCARD);
                if (armAspect == ArmAspect.SILVER) fanAttackLv = Mathf.Max(0.5f, fanAttackLv);
                else if (armAspect == ArmAspect.GOLD) fanAttackLv = Mathf.Max(0.75f, fanAttackLv);
                break;

            case Arm.GRAPNEL: // Increases dice damage (default is min .4, max .8)
                if (armAspect == ArmAspect.NORMAL) damageAgainstDiceMin += 0.05f;
                else if (armAspect == ArmAspect.SILVER) damageAgainstDiceMin += 0.10f;
                else if (armAspect == ArmAspect.GOLD) damageAgainstDiceMin += 0.20f;
                damageAgainstDiceMax += 0.2f;
                damageAgainstDiceMin = Mathf.Min(0.99f, damageAgainstDiceMin);
                damageAgainstDiceMax = Mathf.Min(1, damageAgainstDiceMax);
                break;

            case Arm.SALVE: // Allows healing
                possibleAttacks.Add(Attack.HEAL);
                if (armAspect == ArmAspect.NORMAL) healLv = Mathf.Max(0.33f, healLv);
                else if (armAspect == ArmAspect.SILVER) healLv = Mathf.Max(0.4f, healLv);
                else if (armAspect == ArmAspect.GOLD) healLv = Mathf.Max(0.75f, healLv);
                break;

            case Arm.FORBIDDEN_RELIC: // Deal high damage by paying health
                possibleAttacks.Add(Attack.RECOIL_ATTACK);
                if (armAspect == ArmAspect.NORMAL) {
                    recoilAttackLv = Mathf.Max(2, recoilAttackLv);
                    recoilAttackCost = System.Math.Max(0.2, recoilAttackCost);
                }
                else if (armAspect == ArmAspect.SILVER) {
                    recoilAttackLv = Mathf.Max(4, recoilAttackLv);
                    recoilAttackCost = System.Math.Max(0.25, recoilAttackCost);
                }
                else if (armAspect == ArmAspect.GOLD) {
                    recoilAttackLv = Mathf.Max(8, recoilAttackLv);
                    recoilAttackCost = System.Math.Max(0.3, recoilAttackCost);
                }
                break;

            case Arm.SIPHON: // Drain health
                possibleAttacks.Add(Attack.DRAIN);
                if (armAspect == ArmAspect.NORMAL) drainLv = System.Math.Max(0.33, drainLv);
                else if (armAspect == ArmAspect.SILVER) drainLv = System.Math.Max(0.8, drainLv);
                else if (armAspect == ArmAspect.GOLD) drainLv = System.Math.Max(1.5, drainLv);
                break;

            case Arm.BATTERY: // Allows boosting attack
                possibleAttacks.Add(Attack.CHARGE);
                if (armAspect == ArmAspect.NORMAL) chargeLv = Mathf.Max(1.2f, chargeLv);
                else if (armAspect == ArmAspect.SILVER) chargeLv = Mathf.Max(2f, chargeLv);
                else if (armAspect == ArmAspect.GOLD) chargeLv = Mathf.Max(3.5f, chargeLv);
                break;

            case Arm.MIASMA: // Halve all health gains
                float healMod = 1;
                if (armAspect == ArmAspect.NORMAL) healMod = 0.5f;
                else if (armAspect == ArmAspect.SILVER) healMod = 0.25f;
                else if (armAspect == ArmAspect.GOLD) healMod = 0.1f;
                Player.healingModifier *= healMod;
                break;

            case Arm.AEGIS: // Lower damage of attack rolls
                float damageReduction = 1;
                if(armAspect == ArmAspect.NORMAL) damageReduction = 0.8f;
                else if (armAspect == ArmAspect.SILVER) damageReduction = 0.5f;
                else if (armAspect == ArmAspect.GOLD) damageReduction = 0.25f;
                protectionShields = 1 - ((1 - protectionShields) * damageReduction);
                break;

            case Arm.CARTILAGE: // Can curse (x0.5 dice for one turn)
                possibleAttacks.Add(Attack.CURSE);
                if (armAspect == ArmAspect.NORMAL) curseLv = System.Math.Min(0.5, curseLv);
                else if (armAspect == ArmAspect.SILVER) curseLv = System.Math.Min(0.25, curseLv);
                else if (armAspect == ArmAspect.GOLD) curseLv = System.Math.Min(0.1, curseLv);
                break;

            case Arm.FLOE: // Weaken cards in hand
                possibleAttacks.Add(Attack.WEAKEN_CARDS);
                if (armAspect == ArmAspect.NORMAL) floeAttackLv = Mathf.Min(0.85f, floeAttackLv);
                else if (armAspect == ArmAspect.SILVER) floeAttackLv = Mathf.Min(0.7f, floeAttackLv);
                else if (armAspect == ArmAspect.GOLD) floeAttackLv = Mathf.Min(0.5f, floeAttackLv);
                break;

            case Arm.ILLUSIONIST: // Swap cards in hand for random ones
                possibleAttacks.Add(Attack.SWAP_CARDS);
                if (armAspect == ArmAspect.SILVER) magicianAttackLv = Mathf.Min(0.85f, magicianAttackLv);
                else if (armAspect == ArmAspect.GOLD) magicianAttackLv = Mathf.Min(0.7f, magicianAttackLv);
                break;
        }
    }

    public void playEnemyAttackSound(Attack attack)
    {
        switch (attack)
        {
            case Attack.NOTHING:
            case Attack.CURSE:
            case Attack.WEAKEN_CARDS:
                break;
            case Attack.NORMAL_ATTACK:
            case Attack.RECOIL_ATTACK:
            case Attack.DRAIN:
                AudioManager.amInstance.playEnemyAttackChime();
                break;
            case Attack.DICE_ATTACK:
                AudioManager.amInstance.playDiceAttackChime();
                break;
            case Attack.MILL:
            case Attack.DISCARD:
            case Attack.SWAP_CARDS:
                AudioManager.amInstance.playWindAttackChime();
                break;
            case Attack.HEAL:
                AudioManager.amInstance.playEnemyHealChime();
                break;
            case Attack.CHARGE:
                AudioManager.amInstance.playEnemyBuffChime();
                break;
        }
    }

    string getRandomName()
    {
        string firstWord = getFirstWord();
        string secondWord = getSecondWord();
        string thirdWord = getThirdWord();
        return firstWord + " " + secondWord + " " + thirdWord;
    }

    string getFirstWord()
    {
        string[] possibleWords = new string[] { "Random", "Racy", "Radiating", "Rabid", "Radiant", "Rackety", "Radical",
            "Ragged", "Raging", "Rainproof", "Rakish", "Rallying", "Rambling", "Rambunctious", "Rampant", "Rancorous",
            "Rangy", "Rapacious", "Rapid", "Rare", "Raring", "Rash", "Rational", "Rattled", "Rattling", "Raucous", "Ravening",
            "Ravenous", "Reactionary", "Reactive", "Reasonable", "Reasonless", "Rebellious", "Recalcitrant", "Recent",
            "Reckless", "Reclusive", "Redeemed", "Redundant", "Refined", "Reflexive", "Regal", "Regular", "Reigning",
            "Reinforced", "Reinvigorated", "Relaxed", "Relentless", "Reliable", "Relinquished", "Reluctant", "Remarkable",
            "Renascent", "Renegade", "Renowned", "Reprehensible", "Repressed", "Reputable", "Resilient", "Resistant",
            "Resistive", "Resolute", "Resonant", "Resourceful", "Respected", "Respectful", "Resplendent", "Responsible",
            "Restful", "Restless", "Restrained", "Resurgent", "Retaliative", "Reticent", "Returning", "Revered",
            "Revitalized", "Revolutionary", "Riant", "Riemannian", "Rigorous", "Risky", "Ritzy", "Rivalrous", "Robust",
            "Rococo", "Roguish", "Romantic", "Rough", "Rowdy", "Royal", "Rumbustious", "Runaway", "Rustless", "Rusty"};
        int rnd = Random.Range(0, possibleWords.Length);
        return possibleWords[rnd];
        /*string longestWord = "";
        foreach (string word in possibleWords)
            if (longestWord.Length < word.Length)
                longestWord = word;
        return longestWord;*/
    }

    string getSecondWord()
    {
        string[] possibleWords = new string[] { "Number", "Niceness", "Niche", "Nickname", "Night", "Nightfall", "Nihil",
            "Nimbus", "Ninja", "Nobility", "Noble", "Nuance", "Nucleus", "Numeral", "Naive", "Nasty", "Natural", "Neat",
            "Nefarious", "Nervous", "Nonchalant", "Nonplused", "Normal", "Nitid", "Native", "Nascent", "Neutral", "Nimble",
            "Numeric"};
        int rnd = Random.Range(0, possibleWords.Length);
        return possibleWords[rnd];
    }

    string getThirdWord()
    {
        string[] possibleWords = new string[] { "God", "Gadabout", "Gadget", "Gadgeteer", "Gaffer", "Gale", "Gallant",
            "Galvanizer", "Gambler", "Gamekeeper", "Gangster", "Gardener", "Gatecrasher", "Gatekeeper", "Gatherer",
            "Gendarme", "Genealogist", "Generator", "Geneticist", "Genie", "Genius", "Geographer", "Geomancer",
            "Geometrician", "Ghost", "Giant", "Gladiator", "Glider", "Globetrotter", "Glutton", "Goalkeeper", "Grader",
            "Grandmaster", "Grappler", "Groundkeeper", "Gymnast"};
        int rnd = Random.Range(0, possibleWords.Length);
        return possibleWords[rnd];
    }

    public void applyRollIncrease()
    {
        if (!increaseRollEveryTurn) return;
        int maxRollMod = Mathf.CeilToInt(Player.maxRoll * 0.25f);
        Player.modifyMaxRoll(maxRollMod);
    }

    public void applyAutoLoseDice()
    {
        if (!loseDiceEveryTurn) return;
        Player.nDice -= System.Math.Floor(Player.nDice * 0.1f);
        if (Player.nDice < 1) Player.nDice = 1;
    }

    double applyShieldDamageReduction(double damage)
    {
        return System.Math.Floor(damage * (1 - protectionShields));
    }

    public double receiveDamage(double damage)
    {
        damage = applyShieldDamageReduction(damage);
        if (damage < 0) damage = 0;

        health -= damage;
        if (health < 0) health = 0;

        if (!BattleManager.isTutorial) Player.stats[Player.Stat.TOTAL_DAMAGE_DEALT] += damage;

        refreshHealthHUD(false);
        
        BattleManager.bmInstance.showInfoTextPanel();
        string attackDescription = TextScript.get(TextScript.Sentence.CAUSE_DAMAGE_A) + " " + 
            NumberStringConverter.convert(damage) + " " + TextScript.get(TextScript.Sentence.CAUSE_DAMAGE_B);
        BattleManager.bmInstance.changeInfoText(attackDescription);

        return damage;
    }

    double healPerc(float perc, bool allowExcedeMax)
    {
        double healedAmount = System.Math.Ceiling(maxHealth * perc);
        return healValue(healedAmount, allowExcedeMax);
    }

    double healValue(double healedAmount, bool allowExcedeMax)
    {
        health += healedAmount;
        if (health > maxHealth && !allowExcedeMax) health = maxHealth;
        refreshHealthHUD(false);
        return healedAmount;
    }

    void multiplyMaxHealth(float multiplier)
    {
        maxHealth = System.Math.Floor(maxHealth * multiplier);
        health = maxHealth;
        refreshHealthHUD(true);
    }

    public void applyRegen()
    {
        if (!healEveryTurn) return;
        healPerc(0.2f, true);
    }

    void refreshHealthHUD(bool instant)
    {
        refreshHealthSlider(instant);
        refreshHealthText(instant);
    }

    void refreshHealthSlider(bool instant)
    {
        float currentHealthValue = healthSlider.value;
        float nextHealthValue = (float)(health / maxHealth);
        StartCoroutine(healthSliderLerp(currentHealthValue, nextHealthValue, instant));
    }

    IEnumerator healthSliderLerp(float currentHealthValue, float nextHealthValue, bool instant)
    {
        float transitionTime = (instant) ? 0 : 1f;
        float timer = 0;
        while (timer < transitionTime)
        {
            healthSlider.value = Mathf.Lerp(currentHealthValue, nextHealthValue, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        healthSlider.value = nextHealthValue;
    }

    void refreshHealthText(bool instant)
    {
        double currentHealthValue = healthBeforeLastDamage;
        double nextHealthValue = health;
        StartCoroutine(healthTextLerp(currentHealthValue, nextHealthValue, instant));
    }

    IEnumerator healthTextLerp(double currentHealthValue, double nextHealthValue, bool instant)
    {
        float transitionTime = (instant) ? 0 : 1f;
        float timer = 0;
        while (timer < transitionTime)
        {
            double transitionHealthValue = doubleLerp(currentHealthValue, nextHealthValue, timer / transitionTime);
            transitionHealthValue = System.Math.Round(transitionHealthValue);
            healthText.text = NumberStringConverter.convert(transitionHealthValue) + " / " 
                + NumberStringConverter.convert(maxHealth);
            timer += Time.deltaTime;
            yield return null;
        }
        healthText.text = NumberStringConverter.convert(health) + " / "
                + NumberStringConverter.convert(maxHealth);
        healthBeforeLastDamage = health;
    }

    double doubleLerp(double start, double end, double percentage)
    {
        return start + ((end - start) * percentage);
    }

    public void showEnemyInfoPanel()
    {
        BattleManager.bmInstance.showEnemyInfoPanel();
    }

}
