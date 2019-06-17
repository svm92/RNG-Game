using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Condition = CardEffect.Condition;
using Effect = CardEffect.Effect;

public static class CardFactory {

    public enum CardModel { INSTANT_DICE, INSTANT_HEALTH, LAST, FIRST, MULTIPLIER, DICE_DERIVATIVE,
        FIXED_DICE, FORBID_NUMBER, ADD_ATTACK_ROLL, LOWER_MAX, IMPROVE };
    public enum CostType { NONE, LOW, MEDIUM, HIGH, ABSURD };

    public static Card[] generateNCards(int n, int enemyId)
    {
        Card[] generatedCards = new Card[n];
        for (int i = 0; i < generatedCards.Length; i++)
            generatedCards[i] = generateRandomCard(enemyId);
        return generatedCards;
    }

    public static Card generateRandomCard(int enemyId)
    {
        CardModel cardModel = getRandomCardModel(enemyId);
        double cardPoints = getCardPoints(enemyId);
        Card newCard = generateRandomCardFromModel(cardModel, cardPoints);
        return newCard;
    }

    static CardModel getRandomCardModel(int enemyId)
    {
        List<CardModel> possibleCardModels = new List<CardModel>();
        if (enemyId <= 3)
        {
            for (int i = 0; i < 30; i++) possibleCardModels.Add(CardModel.INSTANT_DICE);
            for (int i = 0; i < 20; i++) possibleCardModels.Add(CardModel.INSTANT_HEALTH);
            for (int i = 0; i < 30; i++) possibleCardModels.Add(CardModel.LAST);
            for (int i = 0; i < 20; i++) possibleCardModels.Add(CardModel.FIRST);
        } else if (enemyId <= 5)
        {
            for (int i = 0; i < 30; i++) possibleCardModels.Add(CardModel.INSTANT_DICE);
            for (int i = 0; i < 15; i++) possibleCardModels.Add(CardModel.INSTANT_HEALTH);
            for (int i = 0; i < 30; i++) possibleCardModels.Add(CardModel.LAST);
            for (int i = 0; i < 15; i++) possibleCardModels.Add(CardModel.FIRST);
            for (int i = 0; i < 10; i++) possibleCardModels.Add(CardModel.MULTIPLIER);
        } else if (enemyId <= 15)
        {
            for (int i = 0; i < 25; i++) possibleCardModels.Add(CardModel.INSTANT_DICE);
            for (int i = 0; i < 10; i++) possibleCardModels.Add(CardModel.INSTANT_HEALTH);
            for (int i = 0; i < 25; i++) possibleCardModels.Add(CardModel.LAST);
            for (int i = 0; i < 10; i++) possibleCardModels.Add(CardModel.FIRST);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.MULTIPLIER);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.FIXED_DICE);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.FORBID_NUMBER);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.ADD_ATTACK_ROLL);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.LOWER_MAX);
        } else
        {
            for (int i = 0; i < 22; i++) possibleCardModels.Add(CardModel.INSTANT_DICE);
            for (int i = 0; i < 10; i++) possibleCardModels.Add(CardModel.INSTANT_HEALTH);
            for (int i = 0; i < 22; i++) possibleCardModels.Add(CardModel.LAST);
            for (int i = 0; i < 10; i++) possibleCardModels.Add(CardModel.FIRST);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.MULTIPLIER);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.DICE_DERIVATIVE);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.FIXED_DICE);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.FORBID_NUMBER);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.ADD_ATTACK_ROLL);
            for (int i = 0; i < 6; i++) possibleCardModels.Add(CardModel.LOWER_MAX);
        }

        if (enemyId >= 100)
            for (int i = 0; i < 5; i++) possibleCardModels.Add(CardModel.IMPROVE);

        int rnd = Random.Range(0, possibleCardModels.Count);
        return possibleCardModels[rnd];
    }

    public static double getAbsoluteCardPoints(int enemyId)
    {
        // Get max points depending on enemy
        double enemyPoints = 5 + Mathf.CeilToInt(enemyId / 2f); // 5, 6, 6, 7, 7...

        if (Enemy.getNArms(enemyId) > 2) // If enemy has more than 2 arms, +15% points
        {
            enemyPoints = System.Math.Floor(enemyPoints * 1.15f);
        }

        return enemyPoints;
    }

    static double getCardPoints(int enemyId) // Minimum 3
    {
        // Get max points depending on enemy
        double enemyPoints = getAbsoluteCardPoints(enemyId); // 5, 6, 6, 7, 7...
        // Return 60%~100% of the max points
        // The lower bound (60%) will asymptotically approach 95% as the game progresses (82% at enemyId = 50, 90% at 100)
        float lowerBound = 0.6f + .35f * (1 - Mathf.Exp(-0.02f * enemyId));
        enemyPoints = System.Math.Round(enemyPoints * Random.Range(lowerBound, 1f));

        enemyPoints = applyFlatBonus(enemyPoints);

        return enemyPoints;
    }

    static double applyFlatBonus(double points)
    {
        points += Player.rerollPoints;
        return points;
    }

    public static Card generateRandomCardFromModel(CardModel cardModel, double cardPoints)
    {
        switch (cardModel)
        {
            default:
            case CardModel.INSTANT_DICE:
                return generateRandomCardInstantDice(cardPoints);
            case CardModel.INSTANT_HEALTH:
                return generateRandomCardInstantHealth(cardPoints);
            case CardModel.LAST:
                return generateRandomCardLast(cardPoints);
            case CardModel.FIRST:
                return generateRandomCardFirst(cardPoints);
            case CardModel.MULTIPLIER:
                return generateRandomCardMultiplier(cardPoints);
            case CardModel.DICE_DERIVATIVE:
                return generateRandomCardDerivative(cardPoints);
            case CardModel.FIXED_DICE:
                return generateRandomCardFixedDice(cardPoints);
            case CardModel.FORBID_NUMBER:
                return generateRandomCardForbidNumber(cardPoints);
            case CardModel.ADD_ATTACK_ROLL:
                return generateRandomCardAddAttackRoll(cardPoints);
            case CardModel.LOWER_MAX:
                return generateRandomCardLowerMax(cardPoints);
            case CardModel.IMPROVE:
                return generateRandomCardImprove(cardPoints);
        }
    }

    public static Card generateRandomCardInstantDice(double cardPoints)
    {
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardInstantDice(cardPoints, costType, cost);
    }

    public static Card generateRandomCardInstantDice(double cardPoints, CostType costType, double cost)
    {
        double effect = System.Math.Floor((cardPoints * System.Math.Pow(1.05, cardPoints) + cost)
            * System.Math.Pow(10.5, (int)costType / 2d));

        // Edge case for when effect might near the overflow limit
        if (effect > 1E+300 || cardPoints > 14000)
        {
            effect = 1E+300;
        }

        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT, 0, Effect.DICE, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardInstantHealth(double cardPoints)
    {
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardInstantHealth(cardPoints, costType, cost);
    }

    public static Card generateRandomCardInstantHealth(double cardPoints, CostType costType, double cost)
    {
        double effect = System.Math.Floor((4 + cardPoints * System.Math.Pow(1.06, cardPoints) + cost)
            * System.Math.Pow(4.5, (int)costType / 2d));

        // Edge case for when effect might near the overflow limit
        if (effect > 1E+300 || cardPoints > 12000)
        {
            effect = 1E+300;
        }

        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT, 0, Effect.HEALTH, effect, cost));
        return newCard;
    }

    public static float requiredPointsForExtraCardLast(double cardPoints)
    {
        return Mathf.Min(1f + Mathf.Exp(0.01f * (float)cardPoints), 4f);
    }

    public static Card generateRandomCardLast(double cardPoints)
    {
        int cond = Random.Range(0, 10);
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardLast(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardLast(double cardPoints, CostType costType, double cost)
    {
        int cond = Random.Range(0, 10);
        return generateRandomCardLast(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardLast(double cardPoints, int cond, CostType costType, double cost)
    {
        double effect = System.Math.Round(cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 0.75f))
            * getCostModifier(costType);
        Card newCard = new Card(new CardEffect(cardPoints, Condition.LAST, cond, Effect.DICE, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardFirst(double cardPoints)
    {
        int cond = Random.Range(1, 10);
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardFirst(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardFirst(double cardPoints, CostType costType, double cost)
    {
        int cond = Random.Range(1, 10);
        return generateRandomCardFirst(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardFirst(double cardPoints, int cond, CostType costType, double cost)
    {
        double effect = System.Math.Round(cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 0.85f)
            * getCostModifier(costType, 1.75));
        Card newCard = new Card(new CardEffect(cardPoints, Condition.FIRST, cond, Effect.HEALTH, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardMultiplier(double cardPoints)
    {
        int delay = getRandomDelay(); // 0 ~ 3
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardMultiplier(cardPoints, delay, costType, cost);
    }

    public static Card generateRandomCardMultiplier(double cardPoints, CostType costType, double cost)
    {
        int delay = getRandomDelay(); // 0 ~ 3
        return generateRandomCardMultiplier(cardPoints, delay, costType, cost);
    }

    public static Card generateRandomCardMultiplier(double cardPoints, int delay, CostType costType, double cost)
    {
        double effect = System.Math.Round((cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 1.5))
                * getCostModifier(costType, 0.6)
                * System.Math.Pow(1.3, delay));

        if (effect < 2) effect = 2;

        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT, delay, Effect.DICE_MULT, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardDerivative(double cardPoints)
    {
        int cond = Random.Range(0, 10);
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardDerivative(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardDerivative(double cardPoints, CostType costType, double cost)
    {
        int cond = Random.Range(0, 10);
        return generateRandomCardDerivative(cardPoints, cond, costType, cost);
    }

    public static Card generateRandomCardDerivative(double cardPoints, int cond, CostType costType, double cost)
    {
        double effect = System.Math.Round(cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 1.75f)
            * getCostModifier(costType));
        if (effect < 1) effect = 1; // Minimum effect of 1
        Card newCard = new Card(new CardEffect(cardPoints, Condition.LAST, cond, Effect.DICE_MOD, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardFixedDice(double cardPoints)
    {
        int effect = Random.Range(1, 11);
        return generateRandomCardFixedDice(cardPoints, effect);
    }

    public static Card generateRandomCardFixedDice(double cardPoints, int effect)
    {
        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT_PERMANENT, 0, Effect.FIXED_DICE, effect));
        return newCard;
    }

    public static Card generateRandomCardForbidNumber(double cardPoints)
    {
        int effect = Random.Range(0, 10);
        float chanceOfNoCost = (float)(System.Math.Pow(cardPoints / 10, 2) / 100); // 25% chance at 50, 100% chance at 100
        float rnd = Random.Range(0f, 1f);
        double cost;

        if (chanceOfNoCost > rnd)
        {
            cost = 0;
        }
        else
        {
            float chanceOfLowCost = (float)(cardPoints / 100); // 50% chance at 50, 100% chance at 100
            rnd = Random.Range(0f, 1f);
            if (chanceOfLowCost > rnd)
            {
                cost = 10;
            }
            else
            {
                cost = 100;
            }
        }

        if (cost < 0) cost = 0;
        return generateRandomCardForbidNumber(cardPoints, effect, cost);
    }

    public static Card generateRandomCardForbidNumber(double cardPoints, int effect, double cost)
    {
        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT_PERMANENT, 0, Effect.FORBID_NUMBER, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardAddAttackRoll(double cardPoints)
    {
        int effect = Random.Range(2, 1001);
        return generateRandomCardAddAttackRoll(cardPoints, effect);
    }

    public static Card generateRandomCardAddAttackRoll(double cardPoints, int effect)
    {
        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT_PERMANENT, 0, Effect.ADD_ATTACK_ROLL, effect));
        return newCard;
    }

    public static float getLowerMaxValue(double cardPoints) // ~9500 card points to reach -999 sides
    {
        return Mathf.FloorToInt(Mathf.Log((1 / 15f) * ((float)cardPoints + 15), 1.0065f));
    }

    public static Card generateRandomCardLowerMax(double cardPoints)
    {
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardLowerMax(cardPoints, costType, cost);
    }

    public static Card generateRandomCardLowerMax(double cardPoints, CostType costType, double cost)
    {
        double effect = getLowerMaxValue(cardPoints + 25 * ((int)costType));
        if (effect > 999) effect = 999; // Can't go above 999
        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT_PERMANENT, 0, Effect.LOWER_MAX, effect, cost));
        return newCard;
    }

    public static Card generateRandomCardImprove(double cardPoints)
    {
        CostType costType = getRandomCostType(cardPoints);
        double cost = getCost(costType, cardPoints);
        return generateRandomCardImprove(cardPoints, costType, cost);
    }

    public static Card generateRandomCardImprove(double cardPoints, CostType costType, double cost)
    {
        double effect = System.Math.Floor(cardPoints / 10d * System.Math.Pow(1.25d, (int)costType));
        if (effect < 1) effect = 1;
        Card newCard = new Card(new CardEffect(cardPoints, Condition.INSTANT_PERMANENT, 0, Effect.IMPROVE, effect, cost));
        return newCard;
    }

    static int getRandomDelay()
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd <= 0.40f)
            return 0; // 40%
        if (rnd <= 0.85f)
            return 1; // 45%
        if (rnd <= 0.96f)
            return 2; // 11%
        return 3; // 4%
    }

    static CostType getRandomCostType(double cardPoints)
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd <= 0.8f)// 80% - No cost
        {
            return CostType.NONE;
        } 
        if (rnd <= 0.9f) // 10% - Low cost
        {
            if (cardPoints <= 10) return CostType.NONE;
            return CostType.LOW;
        }
        if (rnd <= 0.96f) // 6% - Medium cost
        {
            if (cardPoints <= 10) return CostType.NONE;
            if (cardPoints <= 15) return CostType.LOW;
            return CostType.MEDIUM;
        }
        if (rnd <= 0.99f) // 3% - High cost
        {
            if (cardPoints <= 10) return CostType.NONE;
            if (cardPoints <= 15) return CostType.LOW;
            if (cardPoints <= 20) return CostType.MEDIUM;
            return CostType.HIGH;
        }
        // 1% - Absurd cost
        if (cardPoints <= 10) return CostType.NONE;
        if (cardPoints <= 15) return CostType.LOW;
        if (cardPoints <= 20) return CostType.MEDIUM;
        if (cardPoints <= 25) return CostType.HIGH;
        return CostType.ABSURD;
    }

    // Might not work correctly for 3 or 6 points (check getCost test)
    public static CostType getCostType(double cardPoints, double cost)
    {
        if (cost == 0) return CostType.NONE;

        double blueCardEffect = System.Math.Round(cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 0.75f));
        int costExponent = (int)System.Math.Log10(cost);
        if (System.Math.Floor(System.Math.Log10(System.Math.Pow(blueCardEffect, 2))) == costExponent)
            return CostType.LOW;
        if (System.Math.Floor(System.Math.Log10(System.Math.Pow(blueCardEffect, 3))) == costExponent)
            return CostType.MEDIUM;
        if (System.Math.Floor(System.Math.Log10(System.Math.Pow(blueCardEffect, 5))) == costExponent)
            return CostType.HIGH;
        if (System.Math.Floor(System.Math.Log10(System.Math.Pow(blueCardEffect, 7))) == costExponent)
            return CostType.ABSURD;

        Debug.Log("Check getCostType");
        return CostType.LOW;
    }

    public static double getCost(CostType costType, double cardPoints)
    {
        double blueCardEffect = System.Math.Round(cardPoints / (requiredPointsForExtraCardLast(cardPoints) * 0.75f));

        int blueCardExponent = 0;
        switch (costType)
        {
            case CostType.NONE:
                return 0;
            case CostType.LOW:
                blueCardExponent = 2;
                break;
            case CostType.MEDIUM:
                blueCardExponent = 3;
                break;
            case CostType.HIGH:
                blueCardExponent = 5;
                break;
            case CostType.ABSURD:
                blueCardExponent = 7;
                break;
        }
        double cardPointsPower = System.Math.Pow(blueCardEffect, blueCardExponent);
        int exponent = (int)System.Math.Floor(System.Math.Log10(cardPointsPower));
        return System.Math.Pow(10, exponent);
    }

    static double getCostModifier(CostType costType)
    {
        return getCostModifier(costType, 1);
    }

    static double getCostModifier(CostType costType, double innateMultiplier)
    {
        switch (costType)
        {
            default:
            case CostType.NONE:
                return 1;
            case CostType.LOW:
                return 2 * innateMultiplier;
            case CostType.MEDIUM:
                return 5 * innateMultiplier;
            case CostType.HIGH:
                return 10 * innateMultiplier;
            case CostType.ABSURD:
                return 50 * innateMultiplier;
        }
    }

    public static CardModel getCardModel(CardEffect cardEffect)
    {
        if (cardEffect.condition == Condition.INSTANT && cardEffect.effect == Effect.DICE)
        {
            return CardModel.INSTANT_DICE;
        }
        else if (cardEffect.condition == Condition.INSTANT && cardEffect.effect == Effect.HEALTH)
        {
            return CardModel.INSTANT_HEALTH;
        }
        else if (cardEffect.condition == Condition.LAST && cardEffect.effect == Effect.DICE)
        {
            return CardModel.LAST;
        }
        else if (cardEffect.condition == Condition.FIRST && cardEffect.effect == Effect.HEALTH)
        {
            return CardModel.FIRST;
        }
        else if (cardEffect.condition == Condition.INSTANT && cardEffect.effect == Effect.DICE_MULT)
        {
            return CardModel.MULTIPLIER;
        }
        else if (cardEffect.condition == Condition.LAST && cardEffect.effect == Effect.DICE_MOD)
        {
            return CardModel.DICE_DERIVATIVE;
        }
        else if (cardEffect.condition == Condition.INSTANT_PERMANENT)
        {
            if (cardEffect.effect == Effect.FIXED_DICE)
            {
                return CardModel.FIXED_DICE;
            }
            else if (cardEffect.effect == Effect.FORBID_NUMBER)
            {
                return CardModel.FORBID_NUMBER;
            }
            else if (cardEffect.effect == Effect.ADD_ATTACK_ROLL)
            {
                return CardModel.ADD_ATTACK_ROLL;
            }
            else if (cardEffect.effect == Effect.LOWER_MAX)
            {
                return CardModel.LOWER_MAX;
            }
            else if (cardEffect.effect == Effect.IMPROVE)
            {
                return CardModel.IMPROVE;
            }
        }

        Debug.Log("Unknown card model. Check CardFactory.getCardModel");
        return CardModel.LAST;
    }

}
