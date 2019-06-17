using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Player {

    public static List<string> allVersions = new List<string>() { "1.0.0", "1.1.0", "1.1.1", "1.1.2", "1.1.3", "1.1.4",
        "1.1.5"};
    public static string saveVersion = "1.1.5";
    public static bool checkForUpdatesOnStart = true;

    // Mouse is always usable. This only determines if the cursor automatically highlights a card at beginning of turn or not
    public static bool keyboardModeOn = false;
    
    public static double initialHealth = 30;
    public static double health;
    public static double nDice;
    public static int maxRoll;
    public static List<int> attackRolls;
    public static int initialHandSize;

    public static double experience;
    public static double expModifier = 1;
    public static float chanceExtraCard = 0;
    public static float shields = 0;
    public static float regen = 0;
    public static int lowerMaxRoll = 0;

    public static double firstTurnDiceMultiplier = 1;
    public static double diceMultiplier;
    public static List<double[]> delayedMultipliers;
    public static float healingModifier;

    public static Deck deck;
    public static List<Card> collection;
    public static List<CardEffect> cardEffects;
    public static int currentEnemyDifficultyId;
    public static int currentTurn;

    public static double rerollPoints = 0;

    public static Dictionary<Stat, double> stats;

    public static void restoreValuesToDefault()
    {
        nDice = 1;
        recalculateInitialMaxRoll();
        attackRolls = new List<int> { 1, 2, 3, 4, 5 };
        initialHandSize = 5;

        delayedMultipliers = new List<double[]>();
        healingModifier = 1;

        if (deck == null) deck = new Deck();
        if (collection == null) collection = new List<Card>();
        cardEffects = new List<CardEffect>();
        currentEnemyDifficultyId = 0;
        currentTurn = 0;

        if (stats == null) stats = initializeStats();
    }

    public enum Stat { BATTLES, VICTORIES, DEFEATS, TOTAL_ACCUMULATED_CARDS, TOTAL_TRADED_CARDS,
        FORTUNES_BOUGHT, TOTAL_ACCUMULATED_EXP, TOTAL_EXP_SPENT, TOTAL_DAMAGE_DEALT, TOTAL_DAMAGE_RECEIVED, TOTAL_HEALED,
        TOTAL_ROLLED_DICE, HIGHEST_N_OF_ROLLED_DICE, TOTAL_ATTACK_ROLLS, GREATEST_RANK, SILVER_DEFEATED, GOLD_DEFEATED};

    static Dictionary<Stat, double> initializeStats()
    {
        Dictionary<Stat, double> stats = new Dictionary<Stat, double>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            stats.Add(stat, 0);
        }
        stats[Stat.TOTAL_ACCUMULATED_CARDS] = 15; // Player starts with 15 cards
        return stats;
    }

    public static double receiveDamage(double damage)
    {
        BattleManager.bmInstance.flash();
        double actualDamage = damage;

        if (!BattleManager.isTutorial) actualDamage = applyShieldDamageReduction(actualDamage);
        if (actualDamage < 0) actualDamage = 0; // Make damage nonnegative after applying shields

        //if (health - actualDamage < 0) actualDamage = health; // If damage is lethal, make it equal to player's health

        health -= actualDamage;
        if (!BattleManager.isTutorial) stats[Stat.TOTAL_DAMAGE_RECEIVED] += actualDamage;
        return actualDamage;
    }

    public static double loseDice(double damage)
    {
        BattleManager.bmInstance.flash();
        double actualDamage = damage;
        if (nDice - damage < 1)
            actualDamage = nDice - 1;
        nDice -= actualDamage;
        return actualDamage;
    }

    public static void applyRegen()
    {
        health += System.Math.Floor(initialHealth * regen);
    }

    public static double applyShieldDamageReduction(double damage)
    {
        return System.Math.Round(damage * (1 - shields));
    }

    public static void addCardEffect(CardEffect newCardEffect)
    {
        if (newCardEffect.condition == CardEffect.Condition.FIRST || newCardEffect.condition == CardEffect.Condition.LAST)
        {
            foreach (CardEffect ce in cardEffects) // Search if there's any ce with same condition, and cond/eff numbers
            {
                if (ce.condition == newCardEffect.condition && ce.conditionNumber == newCardEffect.conditionNumber
                    && ce.effect == newCardEffect.effect)
                {
                    ce.effectNumber += newCardEffect.effectNumber; // If there is, add new effect to the old one
                    return;
                }
            }
        }
        // If there isn't create a new one (to break the reference and avoid changes to the effect in deck's card)
        newCardEffect = new CardEffect(0, newCardEffect.condition, newCardEffect.conditionNumber, newCardEffect.effect, newCardEffect.effectNumber);
        cardEffects.Add(newCardEffect);
    }

    public static double gainExp(double expValue)
    {
        expValue = System.Math.Floor(expValue * RerollManager.getRerollExpMult(rerollPoints));
        stats[Stat.TOTAL_ACCUMULATED_EXP] += expValue;
        experience += expValue;
        return expValue;
    }

    public static void getNewCard(Card card)
    {
        stats[Stat.TOTAL_ACCUMULATED_CARDS]++;
        collection.Add(card);
    }

    public static void recalculateInitialMaxRoll() // Assuming initial max of 1000 (other modifiers are applied later)
    {
        maxRoll = 1000 - lowerMaxRoll;
        if (maxRoll <= 0) maxRoll = 1;
        Dice.restoreValuesToDefault();
    }

    public static void modifyMaxRoll(int value)
    {
        maxRoll += value;
        if (maxRoll <= 0) maxRoll = 1;
        Dice.restoreValuesToDefault();
    }

    public static bool isOlderSaveThan(string oldSaveVersion, string saveToCheck)
    {
        int oldSaveIndex = allVersions.IndexOf(oldSaveVersion);
        int saveToCheckIndex = allVersions.IndexOf(saveToCheck);
        return (oldSaveIndex < saveToCheckIndex);
    }

    public static bool isAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }

}
