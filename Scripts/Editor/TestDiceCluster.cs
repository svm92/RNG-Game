using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestDiceCluster {

    int diceLimit = 361;

    [Test]
	public void TestDecideDiceScale() {
        Player.restoreValuesToDefault();
        Player.diceMultiplier = 1;
        int[] nDices = new int[] { 1, 13, 25, 36, 50, 100, 121, 122, 169, 170, 225, 226,
            256, 289, 290, 500, 1000000 };
        float[] expectedScales = new float[] { 1.5f, 1.25f, 1f, 1f, .767f, .55f, .459f, 0.457f, 0.457f, 0.4f, 0.4f, 0.375f,
            0.375f, 0.35f, 0.325f, 0.325f, 0.325f };

        for (int i=0; i < nDices.Length; i++)
        {
            Player.nDice = nDices[i];
            float actualScale = DiceCluster.decideDiceScale(Player.nDice);
            float expectedScale = expectedScales[i];
            Assert.AreEqual(expectedScale, actualScale, 0.001f); // 3-digit precision
        }
	}

    [Test]
    public void TestDecideDicePositions()
    {
        Player.restoreValuesToDefault();
        Player.diceMultiplier = 1;
        int[] nDices = new int[] { 1, 2, 3, 4 };
        List<Vector2[]> expectedPositionsArray = new List<Vector2[]>() { new Vector2[] { Vector2.zero },
            new Vector2[] { new Vector2(-200, 0), new Vector2(200, 0) },
            new Vector2[] { new Vector2(-200, 55), new Vector2(200, 55), new Vector2(0, -55)},
            new Vector2[] { new Vector2(-200, 55), new Vector2(200, 55), new Vector2(-200, -55), new Vector2(200, -55)} };

        for (int i=0; i < nDices.Length; i++)
        {
            Player.nDice = nDices[i];
            float scale = DiceCluster.decideDiceScale(Player.nDice);
            int nDiceObjs = (Player.nDice > diceLimit) ? diceLimit : (int)Player.nDice;
            Vector2[] actualPositions = DiceCluster.decideDicePositions(nDiceObjs, scale, diceLimit);
            Vector2[] expectedPositions = expectedPositionsArray[i];
            Assert.AreEqual(actualPositions, expectedPositions);
        }

        // Check that the dice are always within the margins of the screen
        int screenWidth = 1600;
        int screenHeight = 900;
        int minimumWidthMargin = 20;
        nDices = new int[] { 10, 25, 50, 75, 100, 200, 250, 300, 500, 1000000 };
        for (int i = 0; i < nDices.Length; i++)
        {
            Player.nDice = nDices[i];
            float scale = DiceCluster.decideDiceScale(Player.nDice);
            int nDiceObjs = (Player.nDice > diceLimit) ? diceLimit : (int)Player.nDice;
            Vector2[] actualPositions = DiceCluster.decideDicePositions(nDiceObjs, scale, diceLimit);
            foreach (Vector2 dicePosition in actualPositions)
            {
                Assert.GreaterOrEqual(dicePosition.x, -screenWidth/2 + minimumWidthMargin);
                Assert.LessOrEqual(dicePosition.x, screenWidth / 2 - minimumWidthMargin);
                Assert.GreaterOrEqual(dicePosition.y, -screenHeight / 2);
                Assert.LessOrEqual(dicePosition.y, screenHeight / 2);
            }
        }
    }

    [Test]
    public void TestGetNColsRows()
    {
        int[] nDices = new int[] { 1, 2, 3,
            4, 5, 7, 10, 20,
            30, 50, 75, 100, 195,
            196, 197, 250, 361 };
        List<int[]> nColsRows = new List<int[]> { new int[] { 1, 1}, new int[] { 2, 1 }, new int[] { 2, 2 },
            new int[] { 2, 2 }, new int[] { 3, 2 }, new int[] { 3, 3 }, new int[] { 4, 3 }, new int[] { 5, 4 },
            new int[] {6, 5}, new int[] { 8, 7 }, new int[] { 9, 9 }, new int[] {10, 10 }, new int[] { 14, 14 },
            new int[] { 14, 14 }, new int[] { 15, 14 }, new int[] { 16, 16 }, new int[] { 19, 19 } };

        for (int i=0; i < nColsRows.Count; i++)
        {
            int actualNCols = DiceCluster.getNCols(nDices[i]);
            int actualNRows = DiceCluster.getNRows(nDices[i], actualNCols);
            int expectedNCols = nColsRows[i][0];
            int expectedNRows = nColsRows[i][1];
            Assert.AreEqual(expectedNCols, actualNCols);
            Assert.AreEqual(expectedNRows, actualNRows);
        }
    }

    [Test]
    public void TestAddDiceToArrays()
    {
        int maxRoll = 1000;
        // Initialize values
        int[] rolls = new int[] { 2, 7, 15, 7, 20, 1000, 7, 2, 2 };
        int nDiceObjs = Mathf.Min(rolls.Length, diceLimit);
        DiceCluster.diceObjArray = new Dice[nDiceObjs];
        DiceCluster.diceGroupArray = new DiceGroup[maxRoll+1];
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
            DiceCluster.diceGroupArray[i] = new DiceGroup();

        DiceGroup[] expectedGroupArray = new DiceGroup[maxRoll+1];
        for (int i=0; i < expectedGroupArray.Length; i++)
            expectedGroupArray[i] = new DiceGroup();
        expectedGroupArray[2] = new DiceGroup(3, new List<int>() { 0, 7, 8 });
        expectedGroupArray[7] = new DiceGroup(3, new List<int>() { 1, 3, 6 });
        expectedGroupArray[15] = new DiceGroup(1, new List<int>() { 2 });
        expectedGroupArray[20] = new DiceGroup(1, new List<int>() { 4 });
        expectedGroupArray[1000] = new DiceGroup(1, new List<int>() { 5 });

        // Fill arrays
        for (int i=0; i < rolls.Length; i++)
        {
            GameObject dummyDiceObject = new GameObject();
            dummyDiceObject.AddComponent<Dice>();
            DiceCluster.addDiceToArrays(rolls[i], i, dummyDiceObject);
            DiceCluster.diceGroupArray[rolls[i]].nRolls++;
            DiceCluster.diceObjArray[i].value = rolls[i];
        }

        // Assert same diceObjArrays
        for (int i = 0; i < rolls.Length; i++)
            Assert.AreEqual(rolls[i], DiceCluster.diceObjArray[i].value);

        // Assert same diceGroupArrays
        for (int i = 0; i < expectedGroupArray.Length; i++)
        {
            Assert.AreEqual(expectedGroupArray[i].nRolls, DiceCluster.diceGroupArray[i].nRolls);
            Assert.AreEqual(expectedGroupArray[i].diceIDs, DiceCluster.diceGroupArray[i].diceIDs);
        }
    }

    [Test]
    public void TestSwapDiceForRandomOne()
    {
        Player.restoreValuesToDefault();
        Player.diceMultiplier = 1;
        // Initialize values
        int maxRoll = 1000;
        int nDiceObjs = diceLimit;
        DiceCluster.diceObjArray = new Dice[nDiceObjs];
        DiceCluster.diceGroupArray = new DiceGroup[maxRoll+1];
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
            DiceCluster.diceGroupArray[i] = new DiceGroup();

        // Fill with random numbers
        GameObject dummyDiceObject;
        int rollResult = 100;
        for (int i=0; i < 500; i++)
        {
            DiceCluster.diceGroupArray[rollResult].nRolls++;

            if (i < diceLimit)
            {
                dummyDiceObject = new GameObject();
                dummyDiceObject.AddComponent<Dice>();
                DiceCluster.addDiceToArrays(rollResult, i, dummyDiceObject);
                DiceCluster.diceObjArray[i].value = rollResult;
            }
        }

        dummyDiceObject = new GameObject();
        dummyDiceObject.AddComponent<Dice>();
        int randomIndex = Random.Range(0, diceLimit);
        DiceCluster.swapDiceForRandomOne(randomIndex, Player.attackRolls[0], dummyDiceObject);

        Assert.AreEqual(1 , DiceCluster.diceGroupArray[Player.attackRolls[0]].diceIDs.Count);
        Assert.AreEqual(diceLimit - 1, DiceCluster.diceGroupArray[rollResult].diceIDs.Count);
    }

}
