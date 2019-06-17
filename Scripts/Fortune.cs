using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fortune {

    public enum FortuneId { ENDURANCE, QUICK_START, SHIELD, REGEN, LOWER_CEIL, EXTRA_EXP, EXTRA_PRIZE_CARD };

    public FortuneId id;
    public double priceToNextLevel;
    public double level;
    public bool isAvailable = false;
    public bool isNew = true;
    
    string name;

    public Fortune(FortuneId id)
    {
        this.id = id;
        initialize(id);
    }

    void initialize(FortuneId id)
    {
        switch (id)
        {
            case FortuneId.ENDURANCE:
                isNew = false;
                break;
        }
        updateName();
        level = 0;
    }

    void updateName()
    {
        switch (id)
        {
            case FortuneId.ENDURANCE:
                name = TextScript.get(TextScript.Sentence.F0_NAME);
                break;
            case FortuneId.QUICK_START:
                name = TextScript.get(TextScript.Sentence.F1_NAME);
                break;
            case FortuneId.SHIELD:
                name = TextScript.get(TextScript.Sentence.F2_NAME);
                break;
            case FortuneId.REGEN:
                name = TextScript.get(TextScript.Sentence.F3_NAME);
                break;
            case FortuneId.LOWER_CEIL:
                name = TextScript.get(TextScript.Sentence.F4_NAME);
                break;
            case FortuneId.EXTRA_EXP:
                name = TextScript.get(TextScript.Sentence.F5_NAME);
                break;
            case FortuneId.EXTRA_PRIZE_CARD:
                name = TextScript.get(TextScript.Sentence.F6_NAME);
                break;
        }
    }

    public string getDescription()
    {
        updateName();
        string description = "<b><size=60>" + name + "</size></b>";
        if (level > 0)
        {
            description += "<size=50> (" + TextScript.get(TextScript.Sentence.LV) + " " +
                NumberStringConverter.convert(level) + ")</size>";
        }
            
        description += " [" + TextScript.get(TextScript.Sentence.PRICE) + ": " + 
            NumberStringConverter.convert(priceToNextLevel) + " " + TextScript.get(TextScript.Sentence.EXP) + "]\n";
        switch (id)
        {
            case FortuneId.ENDURANCE:
                description += TextScript.get(TextScript.Sentence.F0_DESC) + "\n" +
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: " + NumberStringConverter.convert(getEffectFormula(level)) + " " + TextScript.get(TextScript.Sentence.HEALTH) +
                    "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: " + NumberStringConverter.convert(getEffectFormula(level+1)) + " " + TextScript.get(TextScript.Sentence.HEALTH);
                break;
            case FortuneId.QUICK_START:
                description += TextScript.get(TextScript.Sentence.F1_DESC) + "\n" +
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: x" + NumberStringConverter.convert(getEffectFormula(level)) + " " + TextScript.get(TextScript.Sentence.DICE) +
                    "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: x" + NumberStringConverter.convert(getEffectFormula(level + 1)) + " " + TextScript.get(TextScript.Sentence.DICE);
                break;
            case FortuneId.SHIELD:
                description += TextScript.get(TextScript.Sentence.F2_DESC) + "\n";
                description += (level == 0) ? "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: " + TextScript.get(TextScript.Sentence.NONE) :
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: -" + getFractionAsString((float)getEffectFormula(level)) + "% " + TextScript.get(TextScript.Sentence.DMG);
                description += "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: -" + getFractionAsString((float)getEffectFormula(level + 1)) + "% " + TextScript.get(TextScript.Sentence.DMG);
                break;
            case FortuneId.REGEN:
                description += TextScript.get(TextScript.Sentence.F3_DESC) + "\n";
                description += (level == 0) ? "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: " + TextScript.get(TextScript.Sentence.NONE) :
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: " + getFractionAsString((float)getEffectFormula(level)) + "% " + TextScript.get(TextScript.Sentence.HEALTH);
                description += "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: " + getFractionAsString((float)getEffectFormula(level + 1)) + "% " + TextScript.get(TextScript.Sentence.HEALTH);
                break;
            case FortuneId.LOWER_CEIL:
                description += TextScript.get(TextScript.Sentence.F4_DESC) + "\n" +
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: -" + getEffectFormula(level) +
                    "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: -" + getEffectFormula(level + 1);
                break;
            case FortuneId.EXTRA_EXP:
                description += TextScript.get(TextScript.Sentence.F5_DESC) + "\n" +
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: +" + getMultiplierAsString(getEffectFormula(level)) + "% " + TextScript.get(TextScript.Sentence.EXP) +
                    "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: +" + getMultiplierAsString(getEffectFormula(level + 1)) + "% " + TextScript.get(TextScript.Sentence.EXP);
                break;
            case FortuneId.EXTRA_PRIZE_CARD:
                description += TextScript.get(TextScript.Sentence.F6_DESC) + "\n" +
                    "<b>" + TextScript.get(TextScript.Sentence.CURRENT) + "</b>: " + getFractionAsString((float)getEffectFormula(level)) + "%" +
                    "\t\t\t\t\t<b>" + TextScript.get(TextScript.Sentence.NEXT) + "</b>: " + getFractionAsString((float)getEffectFormula(level + 1)) + "%";
                break;
        }
        return description;
    }

    public void updatePrice()
    {
        switch (id)
        {
            case FortuneId.ENDURANCE:
                // To Lv1 = 5; Lv2 = 10; Lv3 = 21; Lv4 = 43...
                priceToNextLevel = System.Math.Floor(2.5d * (1 + System.Math.Pow((level + 4d) / 4d, 5d)));
                break;
            case FortuneId.QUICK_START:
                // To Lv1 = 10; Lv2 = 22; Lv3 = 51...
                priceToNextLevel = System.Math.Floor(5d * (1 + System.Math.Pow((level + 4d) / 4d, 5.5d)));
                break;
            case FortuneId.SHIELD:
                // To Lv1 = 50; Lv2 = 87; Lv3 = 153
                priceToNextLevel = System.Math.Floor(50d * System.Math.Pow(1.75, level));
                break;
            case FortuneId.REGEN:
                // To Lv1 = 100; Lv2 = 150; Lv3 = 225
                priceToNextLevel = System.Math.Floor(100d * System.Math.Pow(1.5, level));
                break;
            case FortuneId.LOWER_CEIL:
                // To Lv1 = 30; Lv2 = 72, Lv3 = 129
                priceToNextLevel = System.Math.Floor((level+1) * 30d * System.Math.Pow(1.2, level));
                break;
            case FortuneId.EXTRA_EXP:
                // To Lv1 = 15; Lv2 = 37, Lv3 = 70
                priceToNextLevel = System.Math.Floor((level+1) * 15d * System.Math.Pow(1.25, level));
                break;
            case FortuneId.EXTRA_PRIZE_CARD:
                // To Lv1 = 25; Lv2 = 57, Lv3 = 99
                priceToNextLevel = System.Math.Floor((level+1) * 25d * System.Math.Pow(1.15, level));
                break;
        }
    }

    public void levelUpAndApplyEffect()
    {
        level++;
        applyEffect();
    }

    public void applyEffect()
    {
        switch (id)
        {
            case FortuneId.ENDURANCE:
                Player.initialHealth = getEffectFormula(level);
                break;
            case FortuneId.QUICK_START:
                Player.firstTurnDiceMultiplier = getEffectFormula(level);
                break;
            case FortuneId.SHIELD:
                Player.shields = (float)getEffectFormula(level);
                break;
            case FortuneId.REGEN:
                Player.regen = (float)getEffectFormula(level);
                break;
            case FortuneId.LOWER_CEIL:
                Player.lowerMaxRoll = (int)getEffectFormula(level);
                Player.recalculateInitialMaxRoll();
                break;
            case FortuneId.EXTRA_EXP:
                Player.expModifier =  getEffectFormula(level);
                break;
            case FortuneId.EXTRA_PRIZE_CARD:
                Player.chanceExtraCard = (float)getEffectFormula(level);
                break;
        }
    }

    public double getEffectFormula(double level)
    {
        switch (id)
        {
            case FortuneId.ENDURANCE:
                return System.Math.Floor(30d * System.Math.Pow((1d + (1 / 6d)), level));
            case FortuneId.QUICK_START:
                return System.Math.Floor(1 + level + System.Math.Pow(level, 6d) / (0.00001d + level * 1000000d));
            case FortuneId.SHIELD:
                return 1d - System.Math.Pow(0.9, level / 2d);
            case FortuneId.REGEN:
                return level * 0.025d;
            case FortuneId.LOWER_CEIL:
                if (level == 0) return 0;
                float value = (float)(900d * 
                    System.Math.Pow(System.Math.Log((level)*27d), 3d) / System.Math.Pow(System.Math.Log(5400d), 3d));
                value = Mathf.Floor(value);
                return Mathf.Clamp(value, 0, 900);
            case FortuneId.EXTRA_EXP:
                return System.Math.Pow(1.25, level * 0.35);
            case FortuneId.EXTRA_PRIZE_CARD:
                if (level == 0)
                    return 0;
                else
                    return 0.25d + (1d - 1d / (0.5d * (level + 1d))) * 0.75d;
            default:
                return 0;
        }
    }

    public bool fulfillsAvailableCondition()
    {
        switch (id)
        {
            case FortuneId.ENDURANCE: // Unlocked from beginning
                return true;
            case FortuneId.QUICK_START: // Unlocked after beating enemy of rank 4
                return (HubControl.maxUnlockedDifficultyId >= 4);
            case FortuneId.SHIELD:
                return (Player.stats[Player.Stat.TOTAL_DAMAGE_RECEIVED] >= 500
                    && HubControl.maxUnlockedDifficultyId >= 10);
            case FortuneId.REGEN:
                return (Player.stats[Player.Stat.TOTAL_HEALED] >= 5000000
                    && HubControl.maxUnlockedDifficultyId >= 15);
            case FortuneId.LOWER_CEIL:
                return (Player.stats[Player.Stat.FORTUNES_BOUGHT] >= 20
                    && HubControl.maxUnlockedDifficultyId >= 20);
            case FortuneId.EXTRA_EXP:
                return (Player.stats[Player.Stat.TOTAL_ACCUMULATED_EXP] >= 150
                    && HubControl.maxUnlockedDifficultyId >= 5);
            case FortuneId.EXTRA_PRIZE_CARD:
                return (Player.stats[Player.Stat.TOTAL_ACCUMULATED_CARDS] >= 33
                    && HubControl.maxUnlockedDifficultyId >= 4);
            default:
                return false;
        }
    }

    public static string getMultiplierAsString(double multiplier)
    {
        multiplier -= 1;
        multiplier *= 100;
        return NumberStringConverter.convert(System.Math.Floor(multiplier));
    }

    public static string getFractionAsString(float fraction)
    {
        fraction *= 100;
        if (Mathf.Abs(Mathf.Round(fraction) - fraction) <= 0.001f) // Round if extremely close to an integer (solves issues)
        {
            fraction = Mathf.Round(fraction);
        }
        int integerPart = Mathf.FloorToInt(fraction);
        int decimalPart = Mathf.FloorToInt((fraction - integerPart) * 100);
        // Add 0 to the left if the decimal part is a single digit ("5" becomes "05")
        string decimalZero = (decimalPart < 10) ? "0" : "";
        // If decimal part ends in 0, remove it
        if (decimalPart % 10 == 0)
            decimalPart /= 10;
        return (decimalPart == 0) ? (integerPart + "") : (integerPart + "." + decimalZero + decimalPart);
    }

}
