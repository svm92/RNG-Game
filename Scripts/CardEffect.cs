using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CostType = CardFactory.CostType;
using CardModel = CardFactory.CardModel;

[System.Serializable]
public class CardEffect {

    public enum Condition { INSTANT, FIRST, LAST, INSTANT_PERMANENT };
    public enum Effect { DICE, HEALTH, DICE_MULT, DICE_MOD, FIXED_DICE, FORBID_NUMBER, ADD_ATTACK_ROLL,
        LOWER_MAX, IMPROVE };

    public double cardPoints;

    public Condition condition;
    public int conditionNumber;
    public Effect effect;
    public double effectNumber;
    public double cost;

    public CardEffect(double cardPoints, Condition condition, int conditionNumber, Effect effect, double effectNumber)
    {
        init(cardPoints, condition, conditionNumber, effect, effectNumber, 0);
    }

    public CardEffect(double cardPoints, Condition condition, int conditionNumber, Effect effect, double effectNumber, double cost)
    {
        init(cardPoints, condition, conditionNumber, effect, effectNumber, cost);
    }

    void init(double cardPoints, Condition condition, int conditionNumber, Effect effect, double effectNumber, double cost)
    {
        this.cardPoints = cardPoints;
        this.condition = condition;
        this.conditionNumber = conditionNumber;
        this.effect = effect;
        this.effectNumber = effectNumber;
        this.cost = cost;
    }

    public static bool firstNumberIs(int rollNumber, int targetNumber)
    {
        int targetLength = (targetNumber + "").Length;
        int rollLength = (rollNumber + "").Length;
        if (targetLength > rollLength)
            return false;
        int power = (int)Mathf.Pow(10, rollLength - targetLength);
        return (rollNumber / power == targetNumber);
    }

    public static bool lastNumberIs(int rollNumber, int targetNumber)
    {
        int targetLength = (targetNumber + "").Length;
        int power = (int)Mathf.Pow(10, targetLength);
        return (rollNumber % power == targetNumber);
    }

    public void applyInstantEffect()
    {
        applyInstantEffect(1);
    }

    public void applyInstantEffect(double nActivations)
    {
        switch (effect)
        {
            case Effect.DICE:
                Player.nDice += (effectNumber * nActivations);
                break;
            case Effect.HEALTH:
                double healValue = System.Math.Round(effectNumber * nActivations * Player.healingModifier);
                if (!BattleManager.isTutorial) Player.stats[Player.Stat.TOTAL_HEALED] += healValue;
                Player.health += healValue;
                break;
            case Effect.DICE_MULT:
                if (conditionNumber == 0)
                    Player.diceMultiplier *= effectNumber;
                else
                    Player.delayedMultipliers.Add(new double[] { conditionNumber, effectNumber }); // Turn delay, dice multiplier
                break;
            case Effect.FIXED_DICE:
                DiceCluster.forcedRolls.Add((int)effectNumber);
                break;
            case Effect.FORBID_NUMBER:
                if (DiceCluster.allowedRolls.Contains((int)effectNumber))
                    DiceCluster.allowedRolls.Remove((int)effectNumber);
                break;
            case Effect.ADD_ATTACK_ROLL:
                Player.attackRolls.Add((int)effectNumber);
                break;
            case Effect.LOWER_MAX:
                Player.modifyMaxRoll((int)-effectNumber);
                break;
            case Effect.IMPROVE:
                BattleManager.bmInstance.applyCardImproveAdditive(effectNumber);
                break;
        }
    }

    public void applyEffect(Dice[] diceObjCluster, DiceGroup diceGroup)
    {
        foreach (int diceID in diceGroup.diceIDs) // Highlight each dice
            diceObjCluster[diceID].highlightDice(condition, conditionNumber);

        double nActivations = diceGroup.nRolls;
        applyInstantEffect(nActivations);
    }

    CardEffect remakeCardIntoModel(double cardPoints, CostType costType, double newCost, CardModel cardModel, bool isNewModel)
    {
        Card remadeCard = null;
        switch (cardModel)
        {
            case CardModel.INSTANT_DICE:
                remadeCard = CardFactory.generateRandomCardInstantDice(cardPoints, costType, newCost);
                break;
            case CardModel.INSTANT_HEALTH:
                remadeCard = CardFactory.generateRandomCardInstantHealth(cardPoints, costType, newCost);
                break;
            case CardModel.LAST:
                remadeCard = isNewModel ? CardFactory.generateRandomCardLast(cardPoints, costType, newCost) :
                    CardFactory.generateRandomCardLast(cardPoints, conditionNumber, costType, newCost);
                break;
            case CardModel.FIRST:
                remadeCard = isNewModel ? CardFactory.generateRandomCardFirst(cardPoints, costType, newCost) :
                    CardFactory.generateRandomCardFirst(cardPoints, conditionNumber, costType, newCost);
                break;
            case CardModel.MULTIPLIER:
                remadeCard = isNewModel ? CardFactory.generateRandomCardMultiplier(cardPoints, costType, newCost) :
                    CardFactory.generateRandomCardMultiplier(cardPoints, conditionNumber, costType, newCost);
                break;
            case CardModel.DICE_DERIVATIVE:
                remadeCard = isNewModel ? CardFactory.generateRandomCardDerivative(cardPoints, costType, newCost) :
                    CardFactory.generateRandomCardDerivative(cardPoints, conditionNumber, costType, newCost);
                break;
            case CardModel.LOWER_MAX:
                remadeCard = CardFactory.generateRandomCardLowerMax(cardPoints, costType, newCost);
                break;
            case CardModel.IMPROVE:
                remadeCard = CardFactory.generateRandomCardImprove(cardPoints, costType, newCost);
                break;
            case CardModel.FIXED_DICE:
                remadeCard = isNewModel ? CardFactory.generateRandomCardFixedDice(cardPoints):
                    CardFactory.generateRandomCardFixedDice(cardPoints, (int)effectNumber);
                break;
            case CardModel.FORBID_NUMBER:
                remadeCard = isNewModel ? CardFactory.generateRandomCardForbidNumber(cardPoints):
                    CardFactory.generateRandomCardForbidNumber(cardPoints, (int)effectNumber, cost);
                break;
            case CardModel.ADD_ATTACK_ROLL:
                remadeCard = isNewModel ? CardFactory.generateRandomCardAddAttackRoll(cardPoints):
                    CardFactory.generateRandomCardAddAttackRoll(cardPoints, (int)effectNumber);
                break;
        }
        return (remadeCard != null) ? remadeCard.cardEffect : this;
    }

    public CardEffect remakeCardEffect(double cardPoints, double alteration)
    {
        return remakeCardEffect(cardPoints, alteration, false);
    }

    public CardEffect remakeCardEffect(double cardPoints, double alteration, bool forceDifferentModel)
    {
        CardEffect remadeCardEffect = null;
        CardModel cardModel = CardFactory.getCardModel(this);
        // The cost of "forbid number" cards has no association with the card points, so consider it low
        CostType costType = (effect != Effect.FORBID_NUMBER) ? CardFactory.getCostType(cardPoints, cost) : 
            (cost > 0) ? CostType.LOW : CostType.NONE;

        // 6 and values under 3 give trouble when reconverting costs, so return those without changing
        if (cost > 0 && cardPoints < 7 && alteration < 0) return this;

        cardPoints += alteration;
        if (cardPoints < 3) cardPoints = 3; // Should be the minimum
        if (cost > 0 && cardPoints < 7) cardPoints = 7; // 6 and values under 3 give trouble when reconverting costs
        double newCost = CardFactory.getCost(costType, cardPoints);

        if (forceDifferentModel) // If true, pick any random model other than the current one
        {
            List<CardModel> availableCardModels = new List<CardModel>();
            // Get all possible models
            foreach (CardModel model in System.Enum.GetValues(typeof(CardModel)))
            {
                availableCardModels.Add(model);
            }
            availableCardModels.Remove(cardModel); // Remove model of this card, then pick one at random
            cardModel = availableCardModels[Random.Range(0, availableCardModels.Count)];
        }

        // Get card effect
        remadeCardEffect = remakeCardIntoModel(cardPoints, costType, newCost, cardModel, forceDifferentModel);

        // Return if not null, otherwise return default card effect (should not happen)
        return remadeCardEffect ?? this;
    }

    public string getDescription()
    {
        string conditionText;
        switch (condition)
        {
            case Condition.INSTANT:
            case Condition.FIRST:
                conditionText = TextScript.get(TextScript.Sentence.INSTANT);
                break;
            default:
                conditionText = TextScript.get(TextScript.Sentence.CONTINUOUS);
                break;
        }
        string effectText;
        switch (condition)
        {
            default:
            case Condition.INSTANT:
            case Condition.INSTANT_PERMANENT:
                effectText = getTextEffect();
                break;
            case Condition.FIRST:
                effectText = TextScript.get(TextScript.Sentence.COND0) + " " + conditionNumber + ": " + getTextEffect();
                break;
            case Condition.LAST:
                if (effect == Effect.DICE_MOD)
                    effectText = TextScript.get(TextScript.Sentence.COND1) + " " + conditionNumber + " " + 
                        TextScript.get(TextScript.Sentence.COND1_A) + " +" + NumberStringConverter.convert(effectNumber);
                else
                    effectText = TextScript.get(TextScript.Sentence.COND2) + " " + conditionNumber + ": " + getTextEffect();
                break;
        }
        string costText = (cost == 0) ? "" : "[" + TextScript.get(TextScript.Sentence.COST) + 
            ": " + NumberStringConverter.convert(cost) + TextScript.get(TextScript.Sentence.DICE_ABBR) + "] ";
        string text = "<b>" + conditionText + "</b>\n\n" + costText + effectText;
        return text;
    }

    string getTextEffect()
    {
        switch (effect)
        {
            case Effect.DICE:
                return TextScript.get(TextScript.Sentence.DICE_UPPER) + " +" + NumberStringConverter.convert(effectNumber);
            case Effect.HEALTH:
                return TextScript.get(TextScript.Sentence.HEALTH_UPPER) + " +" + NumberStringConverter.convert(effectNumber);
            case Effect.DICE_MULT:
                string turnString = (conditionNumber == 0) ? " " + TextScript.get(TextScript.Sentence.THIS_TURN) + "" : 
                    (conditionNumber == 1) ? " " + TextScript.get(TextScript.Sentence.NEXT_TURN) :
                    " " + TextScript.get(TextScript.Sentence.IN) + " " + 
                    conditionNumber + " " + TextScript.get(TextScript.Sentence.TURNS);
                return TextScript.get(TextScript.Sentence.DICE_UPPER) + " x" + effectNumber + 
                    turnString + " (" + TextScript.get(TextScript.Sentence.TEMPORARY) + ")";
            case Effect.FIXED_DICE:
                return TextScript.get(TextScript.Sentence.FIXED_DICE_A) + " " + 
                    effectNumber + " " + TextScript.get(TextScript.Sentence.FIXED_DICE_B);
            case Effect.FORBID_NUMBER:
                return TextScript.get(TextScript.Sentence.FORBID_NUMBER_A) + " " + 
                    effectNumber + " " + TextScript.get(TextScript.Sentence.FORBID_NUMBER_B);
            case Effect.ADD_ATTACK_ROLL:
                return TextScript.get(TextScript.Sentence.ADD_ATTACK_ROLL_A) + " " + 
                    effectNumber + " " + TextScript.get(TextScript.Sentence.ADD_ATTACK_ROLL_B);
            case Effect.LOWER_MAX:
                return TextScript.get(TextScript.Sentence.LOWER_MAX) + " " + effectNumber;
            case Effect.IMPROVE:
                return TextScript.get(TextScript.Sentence.IMPROVE);
            default:
                return "";
        }
    }

}
