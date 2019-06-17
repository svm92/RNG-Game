using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using CostType = CardFactory.CostType;

//[Ignore("")]
public class TestCardFactory {

    int nLoops = 1000;
    List<int> cardPoints = new List<int> { 100 };

    [Test]
    public void TestGetAbsoluteCardPoints()
    {
        Enemy.initializeSpecialEnemyDict();

        // 5, 6, 6, 7, 7 | 8, 8, 9, 9, 10 | 10, 11, 11, 12, <12> | 13 ...
        Assert.AreEqual(5, CardFactory.getAbsoluteCardPoints(0));
        Assert.AreEqual(6, CardFactory.getAbsoluteCardPoints(1));
        Assert.AreEqual(6, CardFactory.getAbsoluteCardPoints(2));
        Assert.AreEqual(7, CardFactory.getAbsoluteCardPoints(3));
        Assert.AreEqual(7, CardFactory.getAbsoluteCardPoints(4));

        Assert.AreEqual(10, CardFactory.getAbsoluteCardPoints(9));

        Assert.AreEqual(12, CardFactory.getAbsoluteCardPoints(13));
        Assert.AreEqual(13, CardFactory.getAbsoluteCardPoints(14)); // Normally 12, but is a boss
        Assert.AreEqual(13, CardFactory.getAbsoluteCardPoints(15));
    }

    [Test]
    public void TestRequiredPointsForExtraCardLast()
    {
        Assert.AreEqual(2f, CardFactory.requiredPointsForExtraCardLast(0));
        Assert.AreEqual(2.1f, CardFactory.requiredPointsForExtraCardLast(10), 0.01f);
        Assert.AreEqual(2.22f, CardFactory.requiredPointsForExtraCardLast(20), 0.01f);
        Assert.AreEqual(2.64f, CardFactory.requiredPointsForExtraCardLast(50), 0.01f);
        Assert.AreEqual(3.71f, CardFactory.requiredPointsForExtraCardLast(100), 0.01f);
        Assert.AreEqual(4f, CardFactory.requiredPointsForExtraCardLast(500));
    }

    [Test]
    public void TestGetLowerMaxValue()
    {
        Assert.AreEqual(78, CardFactory.getLowerMaxValue(10));
        Assert.AreEqual(106, CardFactory.getLowerMaxValue(15));
        Assert.AreEqual(169, CardFactory.getLowerMaxValue(30));
        Assert.AreEqual(213, CardFactory.getLowerMaxValue(45));
        Assert.AreEqual(314, CardFactory.getLowerMaxValue(100));
        Assert.AreEqual(545, CardFactory.getLowerMaxValue(500));
        Assert.AreEqual(650, CardFactory.getLowerMaxValue(1000));
        Assert.AreEqual(897, CardFactory.getLowerMaxValue(5000));
    }

    [Test]
    public void TestGetCost()
    {
        // Blue card effect: 2
        Assert.AreEqual(0, CardFactory.getCost(CostType.NONE, 3));
        Assert.AreEqual(1, CardFactory.getCost(CostType.LOW, 3));
        Assert.AreEqual(1, CardFactory.getCost(CostType.MEDIUM, 3));
        Assert.AreEqual(10, CardFactory.getCost(CostType.HIGH, 3));
        Assert.AreEqual(100, CardFactory.getCost(CostType.ABSURD, 3));

        // Blue card effect: 3
        Assert.AreEqual(0, CardFactory.getCost(CostType.NONE, 5));
        Assert.AreEqual(1, CardFactory.getCost(CostType.LOW, 5));
        Assert.AreEqual(10, CardFactory.getCost(CostType.MEDIUM, 5));
        Assert.AreEqual(100, CardFactory.getCost(CostType.HIGH, 5));
        Assert.AreEqual(1000, CardFactory.getCost(CostType.ABSURD, 5));

        // Blue card effect: 36
        Assert.AreEqual(0, CardFactory.getCost(CostType.NONE, 100));
        Assert.AreEqual(System.Math.Pow(10, 3), CardFactory.getCost(CostType.LOW, 100));
        Assert.AreEqual(System.Math.Pow(10, 4), CardFactory.getCost(CostType.MEDIUM, 100));
        Assert.AreEqual(System.Math.Pow(10, 7), CardFactory.getCost(CostType.HIGH, 100));
        Assert.AreEqual(System.Math.Pow(10, 10), CardFactory.getCost(CostType.ABSURD, 100));

        // Blue card effect: 333
        Assert.AreEqual(0, CardFactory.getCost(CostType.NONE, 1000));
        Assert.AreEqual(System.Math.Pow(10, 5), CardFactory.getCost(CostType.LOW, 1000));
        Assert.AreEqual(System.Math.Pow(10, 7), CardFactory.getCost(CostType.MEDIUM, 1000));
        Assert.AreEqual(System.Math.Pow(10, 12), CardFactory.getCost(CostType.HIGH, 1000));
        Assert.AreEqual(System.Math.Pow(10, 17), CardFactory.getCost(CostType.ABSURD, 1000));

        // Check no same costs for different rarities
        for (int i=3; i < 1000; i++)
        {
            double costA = CardFactory.getCost(CostType.NONE, i);
            double costB = CardFactory.getCost(CostType.LOW, i);
            double costC = CardFactory.getCost(CostType.MEDIUM, i);
            double costD = CardFactory.getCost(CostType.HIGH, i);
            double costE = CardFactory.getCost(CostType.ABSURD, i);
            if (i == 3 || i == 6) continue; // These do get same costs for different rarities
            if (costA == costB || costB == costC || costC == costD || costD == costE)
            {
                Debug.Log("Same costs for different rarities at i: " + i);
            }
        }
    }

    [Test]
    public void TestGetCostType()
    {
        // Blue card effect: 2
        Assert.AreEqual(CostType.NONE, CardFactory.getCostType(3, 0));
        Assert.AreEqual(CostType.LOW, CardFactory.getCostType(3, 1));
        //Assert.AreEqual(CostType.MEDIUM, CardFactory.getCostType(3, 1)); // This one won't work (same cost)
        Assert.AreEqual(CostType.HIGH, CardFactory.getCostType(3, 10));
        Assert.AreEqual(CostType.ABSURD, CardFactory.getCostType(3, 100));

        // Blue card effect: 3
        Assert.AreEqual(CostType.NONE, CardFactory.getCostType(5, 0));
        Assert.AreEqual(CostType.LOW, CardFactory.getCostType(5, 1));
        Assert.AreEqual(CostType.MEDIUM, CardFactory.getCostType(5, 10));
        Assert.AreEqual(CostType.HIGH, CardFactory.getCostType(5, 100));
        Assert.AreEqual(CostType.ABSURD, CardFactory.getCostType(5, 1000));

        // Blue card effect: 333
        Assert.AreEqual(CostType.NONE, CardFactory.getCostType(1000, 0));
        Assert.AreEqual(CostType.LOW, CardFactory.getCostType(1000, System.Math.Pow(10, 5)));
        Assert.AreEqual(CostType.MEDIUM, CardFactory.getCostType(1000, System.Math.Pow(10, 7)));
        Assert.AreEqual(CostType.HIGH, CardFactory.getCostType(1000, System.Math.Pow(10, 12)));
        Assert.AreEqual(CostType.ABSURD, CardFactory.getCostType(1000, System.Math.Pow(10, 17)));
    }

    [Test]
    [Ignore("")]
    public void TestGenerateInstantDice() { // 5 possibilities per cardpoint value
        for (int i=0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardInstantDice(cardPoint);
                Debug.Log("+" + card.cardEffect.effectNumber + " dice, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateInstantHealth() // 5 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardInstantHealth(cardPoint);
                Debug.Log("+" + card.cardEffect.effectNumber + " health, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateLast() // 50 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardLast(cardPoint);
                Debug.Log("For " + card.cardEffect.conditionNumber + ", +" + card.cardEffect.effectNumber + " dice, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateFirst() // 45 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardFirst(cardPoint);
                Debug.Log("For " + card.cardEffect.conditionNumber + ", +" + card.cardEffect.effectNumber + " health, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateMultiplier() // ? possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardMultiplier(cardPoint);
                Debug.Log("In " + card.cardEffect.conditionNumber + " turns, x" + card.cardEffect.effectNumber + " dice, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateDerivative() // 50 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardDerivative(cardPoint);
                Debug.Log("For " + card.cardEffect.conditionNumber + ", +" + card.cardEffect.effectNumber + " mult, " + " cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateFixedDice() // 1000 possibilities
    {
        for (int i = 0; i < nLoops; i++)
        {
            Card card = CardFactory.generateRandomCardFixedDice(0);
            Debug.Log("Fixed " + card.cardEffect.effectNumber);
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateForbidNumber() // 10 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardForbidNumber(cardPoint);
                Debug.Log("Forbidden " + card.cardEffect.effectNumber + " , cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateAddAttackRoll() // 1000 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardAddAttackRoll(cardPoint);
                Debug.Log("Added " + card.cardEffect.effectNumber + " , cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateLowerMax() // 5 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardLowerMax(cardPoint);
                Debug.Log("Lower by " + card.cardEffect.effectNumber + " , cost: " + card.cardEffect.cost);
            }
        }
    }

    [Test]
    [Ignore("")]
    public void TestGenerateImprove() // 5 possibilities per cardpoint value
    {
        for (int i = 0; i < nLoops; i++)
        {
            foreach (int cardPoint in cardPoints)
            {
                Card card = CardFactory.generateRandomCardImprove(cardPoint);
                Debug.Log("Improve by " + card.cardEffect.effectNumber + ", cost: " + card.cardEffect.cost);
            }
        }
    }

}
