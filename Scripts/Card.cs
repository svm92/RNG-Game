using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card {

    public CardEffect cardEffect = null;
    public bool isLocked = false;
    string description = ""; // Unused, needed for compatibility with old saves

    public Card(CardEffect cardEffect)
    {
        this.cardEffect = cardEffect;
    }

    public string getDescription()
    {
        return cardEffect.getDescription();
    }

    public Card clone()
    {
        CardEffect cloneCardEffect = new CardEffect(cardEffect.cardPoints, cardEffect.condition, cardEffect.conditionNumber,
            cardEffect.effect, cardEffect.effectNumber, cardEffect.cost);
        Card cardClone = new Card(cloneCardEffect);
        return cardClone;
    }

    public void alterEffectPerc(float perc)
    {
        double alteration = System.Math.Floor(cardEffect.cardPoints * Mathf.Abs(1f - perc));
        if (perc < 1) alteration = -alteration;
        alterEffect(alteration);
    }

    public void alterEffect(double alteration)
    {
        cardEffect = cardEffect.remakeCardEffect(cardEffect.cardPoints, alteration);
    }

    public void swapEffect(float perc)
    {
        double alteration = System.Math.Floor(cardEffect.cardPoints * Mathf.Abs(1f - perc));
        if (perc < 1) alteration = -alteration;
        cardEffect = cardEffect.remakeCardEffect(cardEffect.cardPoints, alteration, true);
    }

    public static List<Card> orderCards(List<Card> listCards)
    {
        listCards = listCards.OrderBy(x => x.cardEffect, new CardConditionComparer())
            .ThenBy(x => x.cardEffect.effect, new CardEffectComparer())
            .ThenBy(x => x.cardEffect.cost)
            .ThenBy(x => x.cardEffect.conditionNumber)
            .ThenBy(x => x.cardEffect.effectNumber)
            .ThenBy(x => !x.isLocked) // For equal cards, put locked ones first
            .ToList();
        return listCards;
    }

    public class CardConditionComparer : IComparer<CardEffect>
    {
        public int Compare(CardEffect a, CardEffect b)
        {
            int aNumericalValue = getAssociatedInt(a);
            int bNumbericalValue = getAssociatedInt(b);
            return aNumericalValue.CompareTo(bNumbericalValue);
        }

        // Health/Dice Instants, Health/Dice Continuous, everything else
        int getAssociatedInt(CardEffect cardEffect)
        {
            int assocatedInt = 0;

            if (cardEffect.effect == CardEffect.Effect.DICE || cardEffect.effect == CardEffect.Effect.HEALTH)
                assocatedInt -= 100;

            switch (cardEffect.condition)
            {
                case CardEffect.Condition.INSTANT:
                    assocatedInt += 0;
                    break;
                case CardEffect.Condition.LAST:
                    assocatedInt += 1;
                    break;
                case CardEffect.Condition.FIRST:
                    assocatedInt += 2;
                    break;
                case CardEffect.Condition.INSTANT_PERMANENT:
                    assocatedInt += 3;
                    break;
                default:
                    assocatedInt += 100;
                    break;
            }
            return assocatedInt;
        }
    }

    public class CardEffectComparer : IComparer<CardEffect.Effect>
    {
        public int Compare(CardEffect.Effect a, CardEffect.Effect b)
        {
            int aNumericalValue = getAssociatedInt(a);
            int bNumbericalValue = getAssociatedInt(b);
            return aNumericalValue.CompareTo(bNumbericalValue);
        }

        int getAssociatedInt(CardEffect.Effect effect)
        {
            switch (effect)
            {
                case CardEffect.Effect.DICE:
                    return 0;
                case CardEffect.Effect.HEALTH:
                    return 1;
                case CardEffect.Effect.DICE_MULT:
                    return 2;
                case CardEffect.Effect.DICE_MOD:
                    return 3;
                case CardEffect.Effect.FIXED_DICE:
                    return 4;
                case CardEffect.Effect.FORBID_NUMBER:
                    return 5;
                case CardEffect.Effect.ADD_ATTACK_ROLL:
                    return 6;
                case CardEffect.Effect.LOWER_MAX:
                    return 7;
                case CardEffect.Effect.IMPROVE:
                    return 8;
                default:
                    return 100;
            }
        }
    }

}
