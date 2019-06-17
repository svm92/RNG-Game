using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestDice {

    string firstColor = "#00ff00ff";
    string lastColor = "#0000ffff";

    [Test]
	public void TestGetHighlightedChars() {
        string[] actualColors = Dice.getHighlightedChars(CardEffect.Condition.FIRST, 21, new string[] { null, null, null });
        string[] expectedColors = new string[] { firstColor, firstColor, null};
        CollectionAssert.AreEqual(expectedColors, actualColors);

        actualColors = Dice.getHighlightedChars(CardEffect.Condition.FIRST, 777, new string[] { null, null, null });
        expectedColors = new string[] { firstColor, firstColor, firstColor };
        CollectionAssert.AreEqual(expectedColors, actualColors);

        actualColors = Dice.getHighlightedChars(CardEffect.Condition.LAST, 123, new string[] { null, null, null, null, null });
        expectedColors = new string[] { null, null, lastColor, lastColor, lastColor };
        CollectionAssert.AreEqual(expectedColors, actualColors);

        actualColors = Dice.getHighlightedChars(CardEffect.Condition.LAST, 531, new string[] { null, null, null });
        expectedColors = new string[] { lastColor, lastColor, lastColor };
        CollectionAssert.AreEqual(expectedColors, actualColors);

        actualColors = Dice.getHighlightedChars(CardEffect.Condition.LAST, 22, new string[] { firstColor, null, null, null, null });
        expectedColors = new string[] { firstColor, null, null, lastColor, lastColor };
        CollectionAssert.AreEqual(expectedColors, actualColors);

        actualColors = Dice.getHighlightedChars(CardEffect.Condition.FIRST, 123, new string[] { firstColor, null, null, lastColor, lastColor, lastColor });
        expectedColors = new string[] { firstColor, firstColor, firstColor, lastColor, lastColor, lastColor };
        CollectionAssert.AreEqual(expectedColors, actualColors);
    }

    [Test]
    public void TestGetTextWithColorCodes()
    {
        string actualText = Dice.getTextWithColorCodes(12345, new string[] { null, null, null, null, null});
        string expectedText = "12345";
        Assert.AreEqual(expectedText, actualText);

        actualText = Dice.getTextWithColorCodes(1234, new string[] { firstColor, null, null, lastColor});
        expectedText = "<color=" + firstColor + ">1</color>23<color=" + lastColor + ">4</color>";
        Assert.AreEqual(expectedText, actualText);

        actualText = Dice.getTextWithColorCodes(1234, new string[] { firstColor, firstColor, firstColor, lastColor });
        expectedText = "<color=" + firstColor + ">1</color>" +
            "<color=" + firstColor + ">2</color>" +
            "<color=" + firstColor + ">3</color>" +
            "<color=" + lastColor + ">4</color>";
        Assert.AreEqual(expectedText, actualText);
    }

    [Test]
    public void TestColorCodeLerp()
    {
        // Test for no opacity change
        float originalAlpha = 1; float finalAlpha = 1;
        string[] inputCharColors = new string[] { null, null, null };
        string[] expectedCharColors = new string[] { null, null, null };
        string[] actualCharColors = Dice.colorCodeLerp(inputCharColors, 1, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        actualCharColors = Dice.colorCodeLerp(inputCharColors, .5f, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        inputCharColors = new string[] { "#00ffffff", null, "#005500ff" };
        expectedCharColors = new string[] { "#00ffffff", null, "#005500ff" };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0.5f, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        // Opacity changes
        finalAlpha = 0;
        inputCharColors = new string[] { null, null, null, null };
        expectedCharColors = new string[] { null, null, null, null };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0.5f, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        inputCharColors = new string[] { null, "#00ff00ff", null, "#ff0000ff" };
        expectedCharColors = new string[] { null, "#00ff00FF", null, "#ff0000FF" };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        inputCharColors = new string[] { null, "#00ff00ff", null, "#ff0000ff" };
        expectedCharColors = new string[] { null, "#00ff0000", null, "#ff000000" };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 1, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        inputCharColors = new string[] { null, "#00ff00ff", null, "#ff0000ff" };
        expectedCharColors = new string[] { null, "#00ff007F", null, "#ff00007F" };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0.5f, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

        inputCharColors = new string[] { "#ffffffff", "#00ff00ff", null, "#ff0000ff" };
        expectedCharColors = new string[] { "#ffffffBF", "#00ff00BF", null, "#ff0000BF" };
        actualCharColors = Dice.colorCodeLerp(inputCharColors, 0.25f, originalAlpha, finalAlpha);
        Assert.AreEqual(expectedCharColors, actualCharColors);

    }

    [Test]
    public void TestQuickRoll()
    {
        // Only half allowed
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 5, 6, 7, 8, 9 };
        List<int> allowedNumbers = DiceCluster.prepareAllowedNumbers();
        for (int i=0; i < 10000; i++)
        {
            int diceRoll = Dice.quickRoll(DiceCluster.allowedRolls, allowedNumbers);
            if (diceRoll % 10 <= 4)
            {
                Assert.Fail("Roll ended in " + (diceRoll % 10));
            }
        }

        // Only even allowed
        DiceCluster.allowedRolls = new List<int>() { 0, 2, 4, 6, 8 };
        allowedNumbers = DiceCluster.prepareAllowedNumbers();
        for (int i = 0; i < 10000; i++)
        {
            int diceRoll = Dice.quickRoll(DiceCluster.allowedRolls, allowedNumbers);
            if (diceRoll % 2 == 1)
            {
                Assert.Fail("Roll ended in " + (diceRoll % 10));
            }
        }

        // Only even allowed, low ceil
        Player.maxRoll = 5;
        DiceCluster.allowedRolls = new List<int>() { 0, 2, 4, 6, 8 };
        allowedNumbers = DiceCluster.prepareAllowedNumbers();
        for (int i = 0; i < 10000; i++)
        {
            int diceRoll = Dice.quickRoll(DiceCluster.allowedRolls, allowedNumbers);
            if (diceRoll != 2 && diceRoll != 4)
            {
                Assert.Fail("Roll ended in " + (diceRoll % 10));
            }
        }

        // Impossible combination, just to make sure there is no infinite loop
        Player.maxRoll = 3;
        DiceCluster.allowedRolls = new List<int>() { 0, 4, 5, 6, 7, 8, 9 };
        allowedNumbers = DiceCluster.prepareAllowedNumbers();
        for (int i = 0; i < 5; i++)
        {
            int diceRoll = Dice.quickRoll(DiceCluster.allowedRolls, allowedNumbers);
        }

        // Ensure that all dice rolls happen with the same likelihood
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4 };
        allowedNumbers = DiceCluster.prepareAllowedNumbers();
        int[] nRolls = new int[1001];
        for (int i = 0; i < 1000000; i++) // Roll dice and save their results
        {
            int diceRoll = Dice.quickRoll(DiceCluster.allowedRolls, allowedNumbers);
            nRolls[diceRoll]++;
        }
        for (int i = 0; i < nRolls.Length; i++) // Check that all non-forbidden dice roll with the same chance
        {
            if (i % 10 <= 4 && i != 0)
                Assert.AreEqual(2000f, nRolls[i], 200, "Dice rolled on " + i + " -> " + nRolls[i] + " times");
        }
    }

}
