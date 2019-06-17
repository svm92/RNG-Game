using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestCardEffect {

    [Test]
    public void TestFirstNumberIs() {
        Assert.True(CardEffect.firstNumberIs(12345, 1));
        Assert.True(CardEffect.firstNumberIs(12345, 123));
        Assert.True(CardEffect.firstNumberIs(12345, 12345));
        Assert.False(CardEffect.firstNumberIs(12345, 2));
        Assert.False(CardEffect.firstNumberIs(12345, 123456));
        Assert.True(CardEffect.firstNumberIs(7, 7));
        Assert.False(CardEffect.firstNumberIs(7, 9));
        Assert.False(CardEffect.firstNumberIs(7, 70));
        Assert.False(CardEffect.firstNumberIs(7, 77));
    }

    [Test]
    public void TestLastNumberIs()
    {
        Assert.True(CardEffect.lastNumberIs(12345, 5));
        Assert.True(CardEffect.lastNumberIs(12345, 345));
        Assert.True(CardEffect.lastNumberIs(12345, 12345));
        Assert.False(CardEffect.lastNumberIs(12345, 2));
        Assert.False(CardEffect.lastNumberIs(12345, 212345));
        Assert.True(CardEffect.lastNumberIs(7, 7));
        Assert.False(CardEffect.lastNumberIs(7, 9));
        Assert.False(CardEffect.lastNumberIs(7, 77));
    }

    [Test]
    public void TestGetDescription()
    {
        TextScript.language = TextScript.Language.ENGLISH;

        CardEffect ce = new CardEffect(0, CardEffect.Condition.FIRST, 2, CardEffect.Effect.DICE, 7);
        string expectedDesc = "<b>Instant</b>\n\nDuring this turn, for rolls starting with 2: Dice +7";
        string actualDesc = ce.getDescription();
        Assert.AreEqual(expectedDesc, actualDesc);

        ce = new CardEffect(0, CardEffect.Condition.LAST, 523, CardEffect.Effect.HEALTH, 12);
        expectedDesc = "<b>Continuous</b>\n\nFor rolls ending in 523: Health +12";
        actualDesc = ce.getDescription();
        Assert.AreEqual(expectedDesc, actualDesc);

        ce = new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 100);
        expectedDesc = "<b>Instant</b>\n\nDice +100";
        actualDesc = ce.getDescription();
        Assert.AreEqual(expectedDesc, actualDesc);

        ce = new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE_MOD, 5);
        expectedDesc = "<b>Continuous</b>\n\nEvery turn, increase the modifier of rolls ending in 8 by +5";
        actualDesc = ce.getDescription();
        Assert.AreEqual(expectedDesc, actualDesc);
    }

    [Test]
    public void TestRemakeCardEffectWithSameModel()
    {
        CardEffect originalCardEffect;
        CardEffect expectedNewCardEffect;
        CardEffect actualNewCardEffect;

        // Instant dice
        originalCardEffect = new CardEffect(10, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 16);
        expectedNewCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 53);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(10, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Instant dice with cost
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 11057, 1000);
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 11861, 1000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Instant health
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 68);
        expectedNewCardEffect = new CardEffect(55, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 1359);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 35);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Instant health with cost
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 356, 100);
        expectedNewCardEffect = new CardEffect(55, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 3096, 100);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 35);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Last
        originalCardEffect = new CardEffect(20, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 12);
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 17);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Last with cost
        originalCardEffect = new CardEffect(20, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 60, 1000);
        expectedNewCardEffect = new CardEffect(80, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 165, 10000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // First
        originalCardEffect = new CardEffect(5, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 3);
        expectedNewCardEffect = new CardEffect(505, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 149);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(5, 500);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Card Multiplier
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 1, CardEffect.Effect.DICE_MULT, 8);
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 1, CardEffect.Effect.DICE_MULT, 11);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Card Multiplier with high delay
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 3, CardEffect.Effect.DICE_MULT, 13);
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 3, CardEffect.Effect.DICE_MULT, 19);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Card Multiplier with no delay + cost
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 18, 1000);
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 26, 1000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 10);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Card Multiplier with delay + cost
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 2, CardEffect.Effect.DICE_MULT, 12, 100);
        expectedNewCardEffect = new CardEffect(80, CardEffect.Condition.INSTANT, 2, CardEffect.Effect.DICE_MULT, 34, 1000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, 60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Dice mod
        originalCardEffect = new CardEffect(30, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE_MOD, 7);
        expectedNewCardEffect = new CardEffect(80, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE_MOD, 14);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, 50);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Dice mod with cost
        originalCardEffect = new CardEffect(50, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE_MOD, 539, 1000000000);
        expectedNewCardEffect = new CardEffect(130, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE_MOD, 929, 100000000000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, 80);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Lower max
        originalCardEffect = new CardEffect(50, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 226);
        expectedNewCardEffect = new CardEffect(200, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 410);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, 150);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Lower max with cost
        originalCardEffect = new CardEffect(50, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 344, 1000000);
        expectedNewCardEffect = new CardEffect(200, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 457, 1000000000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, 150);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Improve
        originalCardEffect = new CardEffect(100, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 10);
        expectedNewCardEffect = new CardEffect(200, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 20);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(100, 100);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Unimprovable cards
        originalCardEffect = new CardEffect(100, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FIXED_DICE, 7);
        expectedNewCardEffect = new CardEffect(180, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FIXED_DICE, 7);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(100, 80);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
        //
        originalCardEffect = new CardEffect(50, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FORBID_NUMBER, 9, 10);
        expectedNewCardEffect = new CardEffect(70, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FORBID_NUMBER, 9, 10);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, 20);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
        //
        originalCardEffect = new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 42);
        expectedNewCardEffect = new CardEffect(35, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 42);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(5, 30);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
    }

    [Test]
    public void TestRemakeSubstractWithSameModel()
    {
        CardEffect originalCardEffect;
        CardEffect expectedNewCardEffect;
        CardEffect actualNewCardEffect;

        // Substract 20
        originalCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 129);
        expectedNewCardEffect = new CardEffect(10, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 16);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -20);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Add 20 back
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 129);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(10, +20);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Multiple operations example (+30) -> (-50) -> (+20)
        originalCardEffect = new CardEffect(50, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 125, 10000);
        expectedNewCardEffect = new CardEffect(80, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 165, 10000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, +30);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // (-50)
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(30, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 85, 1000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(80, -50);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // (+20)
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(50, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 125, 10000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, +20);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
    }

    [Test]
    public void TestRemakeSubstractMinimumWithSameModel()
    {
        CardEffect originalCardEffect;
        CardEffect expectedNewCardEffect;
        CardEffect actualNewCardEffect;

        // Instant dice
        originalCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 129);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Instant health
        originalCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 176);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 7);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Last
        originalCardEffect = new CardEffect(30, CardEffect.Condition.LAST, 0, CardEffect.Effect.DICE, 17);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.LAST, 0, CardEffect.Effect.DICE, 2);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // First
        originalCardEffect = new CardEffect(30, CardEffect.Condition.FIRST, 0, CardEffect.Effect.HEALTH, 15);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.FIRST, 0, CardEffect.Effect.HEALTH, 2);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Card Multiplier
        originalCardEffect = new CardEffect(30, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 9);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 2);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Dice mod
        originalCardEffect = new CardEffect(30, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE_MOD, 7);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE_MOD, 1);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(30, -30);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Lower max
        originalCardEffect = new CardEffect(50, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 226);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 28);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(50, -150);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Improve max
        originalCardEffect = new CardEffect(100, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 10);
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 1);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(100, -150);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Last with costs
        originalCardEffect = new CardEffect(20, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 60, 1000);
        expectedNewCardEffect = new CardEffect(7, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 25, 100);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(20, -60);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Multiplier with delay + cost
        originalCardEffect = new CardEffect(100, CardEffect.Condition.INSTANT, 3, CardEffect.Effect.DICE_MULT, 1182, System.Math.Pow(10, 10));
        expectedNewCardEffect = new CardEffect(7, CardEffect.Condition.INSTANT, 3, CardEffect.Effect.DICE_MULT, 148, 10000);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(100, -1000000000);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Dice mod under minimum // check costs, then ensure it doesn't change
        originalCardEffect = new CardEffect(6, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 25, 10);
        expectedNewCardEffect = new CardEffect(6, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 25, 10);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(6, -30);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Dice mod almost at minimum
        originalCardEffect = new CardEffect(7, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 29, 10);
        expectedNewCardEffect = new CardEffect(7, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 29, 10);
        actualNewCardEffect = originalCardEffect.remakeCardEffect(6, -30);
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
    }

    [Test]
    public void TestAlterEffectPerc()
    {
        CardEffect originalCardEffect;
        CardEffect expectedNewCardEffect;
        CardEffect actualNewCardEffect;
        Card actualAlteredCard;

        // Negative multiplier
        originalCardEffect = new CardEffect(20, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 53);
        expectedNewCardEffect = new CardEffect(5, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 6);
        actualAlteredCard = new Card(originalCardEffect);
        actualAlteredCard.alterEffectPerc(0.25f);
        actualNewCardEffect = actualAlteredCard.cardEffect;
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Positive multiplier
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(8, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 11);
        actualAlteredCard = new Card(originalCardEffect);
        actualAlteredCard.alterEffectPerc(1.6f);
        actualNewCardEffect = actualAlteredCard.cardEffect;
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Positive multiplier resulting in nonexact number
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(18, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 43);
        actualAlteredCard = new Card(originalCardEffect);
        actualAlteredCard.alterEffectPerc(2.37f);
        actualNewCardEffect = actualAlteredCard.cardEffect;
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);

        // Almost null multiplier
        originalCardEffect = actualNewCardEffect;
        expectedNewCardEffect = new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3);
        actualAlteredCard = new Card(originalCardEffect);
        actualAlteredCard.alterEffectPerc(0.000000000001f);
        actualNewCardEffect = actualAlteredCard.cardEffect;
        checkAreSameCardEffect(expectedNewCardEffect, actualNewCardEffect);
    }

    [Test]
    public void TestRemakeCardEffectWithDifferentModel()
    {
        CardEffect originalCardEffect;
        CardEffect actualNewCardEffect;

        // Instant dice, check that it turns into something else
        for (int i=0; i < 300; i++)
        {
            originalCardEffect = new CardEffect(10, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 16);
            actualNewCardEffect = originalCardEffect.remakeCardEffect(10, 0, true);
            Assert.AreEqual(10, actualNewCardEffect.cardPoints);
            if (actualNewCardEffect.effect != CardEffect.Effect.FORBID_NUMBER) Assert.AreEqual(0, actualNewCardEffect.cost);
            bool differentConditions = originalCardEffect.condition != actualNewCardEffect.condition;
            bool differentEffects = originalCardEffect.effect != actualNewCardEffect.effect;
            Assert.True(differentConditions || differentEffects);
        }

        // General check for all card models
        foreach (CardFactory.CardModel cardModel in System.Enum.GetValues(typeof(CardFactory.CardModel)))
        {
            double cardPoints = Random.Range(10, 300);
            for (int i=0; i < 100; i++)
            {
                originalCardEffect = CardFactory.generateRandomCardFromModel(cardModel, cardPoints).cardEffect;
                actualNewCardEffect = originalCardEffect.remakeCardEffect(cardPoints, 0, true);
                Assert.AreEqual(cardPoints, actualNewCardEffect.cardPoints);
                Assert.AreNotEqual(CardFactory.getCardModel(originalCardEffect), CardFactory.getCardModel(actualNewCardEffect));

                // Check that conditionNumber is 0 for cards that don't use it
                CardFactory.CardModel newCardModel = CardFactory.getCardModel(actualNewCardEffect);
                if (newCardModel != CardFactory.CardModel.LAST && newCardModel != CardFactory.CardModel.FIRST &&
                    newCardModel != CardFactory.CardModel.DICE_DERIVATIVE && newCardModel != CardFactory.CardModel.MULTIPLIER)
                    Assert.AreEqual(0, actualNewCardEffect.conditionNumber);

                if (newCardModel == CardFactory.CardModel.MULTIPLIER)
                    Assert.LessOrEqual(actualNewCardEffect.conditionNumber, 3);
            }
        }

        // General check for all card models, decreasing
        foreach (CardFactory.CardModel cardModel in System.Enum.GetValues(typeof(CardFactory.CardModel)))
        {
            double cardPoints = Random.Range(10, 300);
            for (int i = 0; i < 100; i++)
            {
                originalCardEffect = CardFactory.generateRandomCardFromModel(cardModel, cardPoints).cardEffect;
                actualNewCardEffect = originalCardEffect.remakeCardEffect(cardPoints, -500, true);
                int newCardPoints = (originalCardEffect.cost == 0) ? 3 : 7;
                Assert.AreEqual(newCardPoints, actualNewCardEffect.cardPoints);
                Assert.AreNotEqual(CardFactory.getCardModel(originalCardEffect), CardFactory.getCardModel(actualNewCardEffect));
            }
        }

        // General check for all card models, decreasing perc
        foreach (CardFactory.CardModel cardModel in System.Enum.GetValues(typeof(CardFactory.CardModel)))
        {
            double cardPoints = 500;
            for (int i = 0; i < 100; i++)
            {
                Card originalCard = CardFactory.generateRandomCardFromModel(cardModel, cardPoints);
                originalCardEffect = originalCard.cardEffect;
                originalCard.swapEffect(0.25f);
                actualNewCardEffect = originalCard.cardEffect;
                Assert.AreEqual(125, actualNewCardEffect.cardPoints);
                Assert.AreNotEqual(CardFactory.getCardModel(originalCardEffect), CardFactory.getCardModel(actualNewCardEffect));
            }
        }
    }

    public void checkAreSameCardEffect(CardEffect expected, CardEffect actual)
    {
        Assert.AreEqual(expected.cardPoints, actual.cardPoints);
        Assert.AreEqual(expected.condition, actual.condition);
        Assert.AreEqual(expected.conditionNumber, actual.conditionNumber);
        Assert.AreEqual(expected.effect, actual.effect);
        Assert.AreEqual(expected.effectNumber, actual.effectNumber);
        Assert.AreEqual(expected.cost, actual.cost);
    }

}
