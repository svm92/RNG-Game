using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestBattleManager {

	[Test]
	public void TestGetCardPositionsInHand() {
        Player.restoreValuesToDefault();
        Hand.hand = new List<GameObject>();
        Hand.hand.Clear();
        float yPos = 75;
        GameObject dummyCard = new GameObject();

        // 1 card
        Hand.hand.Add(dummyCard);
        Vector2[] actualPositions = BattleManager.getCardPositionsInHand();
        Vector2[] expectedPositions = new Vector2[] { new Vector3(0, yPos, 0) };
        Assert.AreEqual(expectedPositions, actualPositions);

        // 2 cards
        Hand.hand.Add(dummyCard);
        actualPositions = BattleManager.getCardPositionsInHand();
        expectedPositions = new Vector2[] { new Vector3(-157.5f, yPos, 0), new Vector3(157.5f, yPos, 0) };
        Assert.AreEqual(expectedPositions, actualPositions);

        // 3 cards
        Hand.hand.Add(dummyCard);
        actualPositions = BattleManager.getCardPositionsInHand();
        expectedPositions = new Vector2[] { new Vector3(-315, yPos, 0), new Vector3(0, yPos, 0), new Vector3(315, yPos, 0) };
        Assert.AreEqual(expectedPositions, actualPositions);

        // 4 cards
        Hand.hand.Add(dummyCard);
        actualPositions = BattleManager.getCardPositionsInHand();
        expectedPositions = new Vector2[] { new Vector3(-472.5f, yPos, 0), new Vector3(-157.5f, yPos, 0),
            new Vector3(157.5f, yPos, 0), new Vector3(472.5f, yPos, 0) };
        Assert.AreEqual(expectedPositions, actualPositions);

        // 5 cards
        Hand.hand.Add(dummyCard);
        actualPositions = BattleManager.getCardPositionsInHand();
        expectedPositions = new Vector2[] { new Vector3(-630, yPos, 0), new Vector3(-315, yPos, 0),
            new Vector3(0, yPos, 0), new Vector3(315, yPos, 0), new Vector3(630, yPos, 0) };
        Assert.AreEqual(expectedPositions, actualPositions);

        // 6+ cards
        float screenBorder = 755f;
        for (int i=0; i < 10; i++)
        {
            Hand.hand.Add(dummyCard);
            actualPositions = BattleManager.getCardPositionsInHand();
            foreach (Vector2 position in actualPositions)
            {
                Assert.GreaterOrEqual(position.x, -screenBorder);
                Assert.LessOrEqual(position.x, screenBorder);
            }
        }
    }

    [Test]
    public void TestUpdateModifiersInHUDFromArray()
    {
        Player.cardEffects = new List<CardEffect>();
        Text[] modifierTexts = new Text[9];
        for (int i=0; i < modifierTexts.Length; i++)
            modifierTexts[i] = (new GameObject()).AddComponent<Text>();
        CardEffect.Condition condition = CardEffect.Condition.FIRST;

        // No card effects
        BattleManager.updateModifiersInHUDFromArray(modifierTexts, condition);
        foreach (Text t in modifierTexts)
            Assert.AreEqual("+0", t.text);

        // Some card effects
        CardEffect ce1 = new CardEffect(0, CardEffect.Condition.FIRST, 4, CardEffect.Effect.HEALTH, 3);
        CardEffect ce2 = new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 2);
        Player.addCardEffect(ce1); Player.addCardEffect(ce2);
        BattleManager.updateModifiersInHUDFromArray(modifierTexts, condition);
        for (int i=0; i < modifierTexts.Length; i++)
        {
            Text t = modifierTexts[i];
            if (i == 4)
                Assert.AreEqual("+3", t.text);
            else if (i == 7)
                Assert.AreEqual("+2", t.text);
            else
                Assert.AreEqual("+0", t.text);
        }

        // Repeated card effects
        CardEffect ce3 = new CardEffect(0, CardEffect.Condition.FIRST, 4, CardEffect.Effect.HEALTH, 6);
        CardEffect ce4 = new CardEffect(0, CardEffect.Condition.FIRST, 4, CardEffect.Effect.HEALTH, 3);
        CardEffect ce5 = new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 5);
        Player.addCardEffect(ce3); Player.addCardEffect(ce4); Player.addCardEffect(ce5);
        BattleManager.updateModifiersInHUDFromArray(modifierTexts, condition);
        for (int i = 0; i < modifierTexts.Length; i++)
        {
            Text t = modifierTexts[i];
            if (i == 1)
                Assert.AreEqual("+5", t.text);
            else if (i == 4)
                Assert.AreEqual("+12", t.text);
            else if (i == 7)
                Assert.AreEqual("+2", t.text);
            else
                Assert.AreEqual("+0", t.text);
        }

        // No "last" card effects
        modifierTexts = new Text[10];
        for (int i = 0; i < modifierTexts.Length; i++)
            modifierTexts[i] = (new GameObject()).AddComponent<Text>();
        condition = CardEffect.Condition.LAST;
        BattleManager.updateModifiersInHUDFromArray(modifierTexts, condition);
        foreach (Text t in modifierTexts)
            Assert.AreEqual("+0", t.text);
        CardEffect ce6 = new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.HEALTH, 7);
        CardEffect ce7 = new CardEffect(0, CardEffect.Condition.LAST, 4, CardEffect.Effect.HEALTH, 1);
        CardEffect ce8 = new CardEffect(0, CardEffect.Condition.LAST, 8, CardEffect.Effect.HEALTH, 15);
        Player.addCardEffect(ce6); Player.addCardEffect(ce7); Player.addCardEffect(ce8);
        BattleManager.updateModifiersInHUDFromArray(modifierTexts, condition);
        for (int i = 0; i < modifierTexts.Length; i++)
        {
            Text t = modifierTexts[i];
            if (i == 4)
                Assert.AreEqual("+1", t.text);
            else if (i == 8)
                Assert.AreEqual("+22", t.text);
            else
                Assert.AreEqual("+0", t.text);
        }
    }

}
