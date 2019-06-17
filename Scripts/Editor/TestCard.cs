using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestCard {

	[Test]
	public void TestOrderCards() {
        List<Card> cardCollection = new List<Card>();

        // Fill with cards
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 5, CardEffect.Effect.DICE_MOD, 10)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 100, 1)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 1, CardEffect.Effect.DICE_MULT, 2, 5)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 600)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FORBID_NUMBER, 7)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FORBID_NUMBER, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 8)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 4)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 4)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FIXED_DICE, 42)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 5)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 200)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE_MOD, 10)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 350)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FIXED_DICE, 50)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 5, CardEffect.Effect.DICE, 1, 30)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 10, 30)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE_MOD, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 800)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.FIRST, 2, CardEffect.Effect.HEALTH, 5, 10)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 7)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.FIRST, 2, CardEffect.Effect.HEALTH, 10, 10)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 1, CardEffect.Effect.DICE_MULT, 2)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 50)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 5)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2, 15)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.FIRST, 8, CardEffect.Effect.HEALTH, 5, 10)));
        cardCollection.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 4, 5)));

        // Order
        cardCollection = Card.orderCards(cardCollection);

        // Check correct order
        int n = 0;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(5, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(8, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.HEALTH, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(5, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(3, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(3, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(7, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(9, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(4, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(9, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(15, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(5, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(1, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(30, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.FIRST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(2, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.HEALTH, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(5, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(10, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.FIRST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(2, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.HEALTH, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(10, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(10, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.FIRST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(8, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.HEALTH, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(5, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(10, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MULT, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(0, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MULT, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(4, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(0, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(1, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MULT, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(0, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MULT, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(4, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(5, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(1, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MULT, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(5, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(5, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MOD, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(10, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(8, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MOD, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.LAST, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(8, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE_MOD, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(10, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.FIXED_DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(42, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.FIXED_DICE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(50, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.FORBID_NUMBER, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(2, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.FORBID_NUMBER, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(7, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.ADD_ATTACK_ROLL, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(200, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(0, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.ADD_ATTACK_ROLL, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(600, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(0, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.ADD_ATTACK_ROLL, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(100, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(1, cardCollection[n].cardEffect.cost);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.LOWER_MAX, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(350, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.LOWER_MAX, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(800, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.IMPROVE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(50, cardCollection[n].cardEffect.effectNumber);

        n++;
        Assert.AreEqual(CardEffect.Condition.INSTANT_PERMANENT, cardCollection[n].cardEffect.condition);
        Assert.AreEqual(0, cardCollection[n].cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.IMPROVE, cardCollection[n].cardEffect.effect);
        Assert.AreEqual(10, cardCollection[n].cardEffect.effectNumber);
        Assert.AreEqual(30, cardCollection[n].cardEffect.cost);
    }

}
