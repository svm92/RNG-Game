using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestPlayer {

	[Test]
	public void TestAddCardEffect() {
        Player.cardEffects = new List<CardEffect>();
        CardEffect ce1 = new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 5);
        CardEffect ce2 = new CardEffect(0, CardEffect.Condition.LAST, 1, CardEffect.Effect.DICE, 4);
        CardEffect ce3 = new CardEffect(0, CardEffect.Condition.FIRST, 3, CardEffect.Effect.HEALTH, 5);
        CardEffect ce4 = new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 2);

        Player.addCardEffect(ce1);
        Assert.AreEqual(1, Player.cardEffects.Count);
        Player.addCardEffect(ce2);
        Assert.AreEqual(2, Player.cardEffects.Count);
        Player.addCardEffect(ce3);
        Assert.AreEqual(3, Player.cardEffects.Count);
        Player.addCardEffect(ce4);
        Assert.AreEqual(3, Player.cardEffects.Count);
        Assert.AreEqual(1, Player.cardEffects[0].conditionNumber);
        Assert.AreEqual(7, Player.cardEffects[0].effectNumber);
        Assert.AreEqual(1, Player.cardEffects[1].conditionNumber);
        Assert.AreEqual(4, Player.cardEffects[1].effectNumber);
        Assert.AreEqual(3, Player.cardEffects[2].conditionNumber);
        Assert.AreEqual(5, Player.cardEffects[2].effectNumber);
    }

    [Test]
    public void TestIsOlderSaveThan()
    {
        Assert.AreEqual(true, Player.isOlderSaveThan("1.1.1", "1.1.2"));
        Assert.AreEqual(false, Player.isOlderSaveThan("1.1.1", "1.1.0"));
        Assert.AreEqual(false, Player.isOlderSaveThan("1.1.1", "1.1.1"));
    }

}
