using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour {

    public const int WIDTH = 250;
    public const int HEIGHT = 100;
    public static Color victoryColor = Color.yellow;
    public static Color originalColor = Color.clear;

    public static float minPreRollTime;
    public static int maxPreRollValue = Player.maxRoll + 1;
    public static bool skipAnim = false;

    public int value;

    bool isRolling = false;

    string[] charColors;

    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        if (originalColor == Color.clear) originalColor = text.color;
    }

    private void OnEnable()
    {
        text.color = originalColor;
    }

    public static void restoreValuesToDefault()
    {
        minPreRollTime = (skipAnim && !BattleManager.isTutorial) ? 0f : 1f;
        maxPreRollValue = Player.maxRoll + 1;
    }

    public static void applyGlobalAnimSetting()
    {
        restoreValuesToDefault();
        DiceCluster.skipPostAttackAnimation = skipAnim;
        BattleManager.speedUpBattleAnimations = skipAnim;
    }

    public IEnumerator roll()
    {
        isRolling = true;
        // Rolling animation
        while (isRolling)
        {
            int preRollValue = Random.Range(1, maxPreRollValue);
            if (isRolling) text.text = preRollValue + "";
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void stopRoll()
    {
        isRolling = false;
    }

    public static bool isSafeRoll(List<int> allowedRolls)
    {
        // Check for edge case that causes infinite loop
        // If the player has a low max ceil for rolls and many forbidden rolls, it may happen that there are
        // no possible allowed rolls (i.e.: with a maxRoll of 3 the player can only roll 1,2,3, but if those
        // happen to be forbidden, this gets stuck in an infinite loop)
        bool safeRoll = true;
        if (Player.maxRoll < 10 && allowedRolls.Count < 10)
        {
            safeRoll = false;
            for (int i = 1; i <= Player.maxRoll; i++)
            {
                if (allowedRolls.Contains(i)) // If a possible roll is found, all is well
                {
                    safeRoll = true;
                    break;
                }
            }
        }
        return safeRoll;
    }

    public static int quickRoll(List<int> allowedRolls, List<int> allowedNumbers)
    {
        bool safeRoll = isSafeRoll(allowedRolls);

        // If all rolls are allowed return something totally random. If all rolls are somehow forbidden, do that too
        if (allowedRolls.Count == 10 || allowedRolls.Count == 0 || !safeRoll)
        {
            return Random.Range(1, Player.maxRoll + 1);
        }
        else // If some rolls are forbidden, avoid them
        {
            int roll = allowedNumbers[Random.Range(0, allowedNumbers.Count)];
            return roll;
        }
    }

    public void forceValue(int newValue)
    {
        value = newValue;
        text.text = value + "";
        charColors = createCharColors();
    }

    string[] createCharColors()
    {
        string valueString = value + "";
        return new string[valueString.Length];
    }

    public void highlightDice(CardEffect.Condition condition, int conditionNumber)
    {
        charColors = getHighlightedChars(condition, conditionNumber, charColors);
        applyTextColor();
    }

    public static string[] getHighlightedChars(CardEffect.Condition condition, int conditionNumber, string[] charColors)
    {
        int numberLength = (conditionNumber + "").Length;
        switch (condition)
        {
            case CardEffect.Condition.FIRST:
                for (int i = 0; i < numberLength; i++)
                    charColors[i] = "#00ff00ff";
                break;
            case CardEffect.Condition.LAST:
                int lastPosition = charColors.Length - 1;
                for (int i = 0; i < numberLength; i++)
                    charColors[lastPosition - i] = "#0000ffff";
                break;
        }
        return charColors;
    }

    void applyTextColor()
    {
        string newText = getTextWithColorCodes(value, charColors);
        text.text = newText;
    }

    public static string getTextWithColorCodes(int value, string[] charColors)
    {
        string newText = "";
        char[] charArray = (value + "").ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            if (charColors[i] == null)
                newText += charArray[i];
            else
                newText += "<color=" + charColors[i] + ">" + charArray[i] + "</color>";
        }
        return newText;
    }

    public void changeTextColor(Color c)
    {
        text.color = c;
    }

    public void highlightVictoryRoll()
    {
        text.text = value + ""; // First, remove color codes
        for (int i = 0; i < charColors.Length; i++) charColors[i] = null;

        Color finalColor = victoryColor;
        float finalScale = 3f;
        float transitionTime = .5f;
        StartCoroutine(transitionTextColor(finalColor, transitionTime));
        StartCoroutine(transitionScale(finalScale, transitionTime));
        StartCoroutine(moveToCenterOfScreenThenDisappear(transitionTime));
    }

    public void fadeOut(float transitionTime)
    {
        Color originalColor = text.color;
        Color finalColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        StartCoroutine(transitionTextColor(finalColor, transitionTime));
    }

    IEnumerator transitionTextColor(Color finalColor, float transitionTime)
    {
        Color originalColor = text.color;
        float timer = 0;
        float lastTime = Time.time;

        while (timer <= transitionTime)
        {
            text.color = Color.Lerp(originalColor, finalColor, timer / transitionTime);

            charColors = colorCodeLerp(charColors, timer / transitionTime, originalColor.a, finalColor.a);
            applyTextColor();
            
            timer += Time.time - lastTime;
            lastTime = Time.time;
            if (!Player.isAndroid())
                yield return new WaitForSeconds(0.05f);
            else
                yield return new WaitForSeconds(0.25f); // Longer delay between updates in Android, to avoid lag
        }
        text.color = finalColor;
    }

    IEnumerator transitionScale(float finalScaleFloat, float transitionTime)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 originalScale = rt.localScale;
        Vector2 finalScale = Vector2.one * finalScaleFloat;
        float timer = 0;

        while (timer <= transitionTime)
        {
            rt.localScale = Vector2.Lerp(originalScale, finalScale, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        rt.localScale = finalScale;
    }

    IEnumerator moveToCenterOfScreenThenDisappear(float transitionTime)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 originalPosition = rt.anchoredPosition;
        Vector2 finalPosition = new Vector2(0, 0);
        float timer = 0;

        while (timer <= transitionTime)
        {
            rt.anchoredPosition = Vector2.Lerp(originalPosition, finalPosition, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = finalPosition;
        yield return new WaitForSeconds(0.15f);
        fadeOut(0.25f);
        yield return new WaitForSeconds(0.4f);
        DiceCluster.showingVictoryAnimation = false;
    }

    public static string[] colorCodeLerp(string[] charColors, float t, float originalAlpha, float finalAlpha)
    {
        if (originalAlpha == finalAlpha) return charColors;
        for (int i = 0; i < charColors.Length; i++) // Color codes in the text must be treated separatedly
        {
            if (charColors[i] == null) continue;
            string colorCodeRGB = charColors[i].Substring(0, 7);
            string colorCodeAlpha = charColors[i].Substring(7);
            //int colorIntAlpha = System.Convert.ToInt32(colorCodeAlpha, 16); // Convert #hex to decimal
            //float colorFloatAlpha = (float)colorIntAlpha / 255f; // Alphas are measured from 0~1
            float newColorFloatAlpha = Mathf.Lerp(originalAlpha, finalAlpha, t);
            colorCodeAlpha = ((int)(newColorFloatAlpha * 255f)).ToString("X"); // Convert from decimal to #hex
            if (colorCodeAlpha.Length == 1)
                colorCodeAlpha = "0" + colorCodeAlpha; // Add padding 0 to the left if needed
            charColors[i] = colorCodeRGB + colorCodeAlpha; // Form the new code
        }
        return charColors;
    }
}
