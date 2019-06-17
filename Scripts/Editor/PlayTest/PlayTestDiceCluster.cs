using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class PlayTestDiceCluster {

    public DiceCluster dc;

    IEnumerator initialize()
    {
        // Initialize
        Player.restoreValuesToDefault();
        // Skip anims
        Dice.minPreRollTime = 0f;
        DiceCluster.skipPostAttackAnimation = true;
        BattleManager.speedUpBattleAnimations = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        yield return null;

        // Find DiceCluster and hide other HUDs
        dc = GameObject.Find("DiceCluster").GetComponent<DiceCluster>();
        DiceCluster.isARollTest = true;
        GameObject.Find("HUD").SetActive(false);
        yield return null;
        GameObject.Find("HandCanvas").SetActive(false);
        yield return null;
    }

    void assertRolled(double realNDice)
    {
        double totalDiceRolled = 0;
        foreach (DiceGroup dg in DiceCluster.diceGroupArray)
        {
            totalDiceRolled += dg.nRolls;
        }

        if (realNDice < System.Math.Pow(10, 15)) // For "low" rolls (under 10^15) do precise check
            Assert.AreEqual(realNDice, totalDiceRolled);
        else // For high rolls, don't ask for as much precision
            Assert.AreEqual(realNDice, totalDiceRolled, realNDice / 100);
    }

    [UnityTest]
    public IEnumerator TestRollGeneric()
    {
        yield return initialize();

        // Roll 100,000
        Player.nDice = 100000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(100000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(100, DiceCluster.diceGroupArray[i].nRolls, 40);
            }
        }

        // Roll 1,000,000
        Player.nDice = 1000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1000, DiceCluster.diceGroupArray[i].nRolls, 150);
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollGeneric1B()
    {
        yield return initialize();
        
        // Roll 1B
        Player.nDice = 1000000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1000000, DiceCluster.diceGroupArray[i].nRolls, 1000);
            }
        }

        // Roll 10^30
        Player.nDice = System.Math.Pow(10, 30);
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(System.Math.Pow(10, 30));
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(System.Math.Pow(10, 27), DiceCluster.diceGroupArray[i].nRolls, System.Math.Pow(10, 24));
            }
        }

        // Roll 10^300
        Player.nDice = System.Math.Pow(10, 300);
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(System.Math.Pow(10, 300));
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(System.Math.Pow(10, 297), DiceCluster.diceGroupArray[i].nRolls, System.Math.Pow(10, 294));
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollWithForbidden()
    {
        yield return initialize();

        // Roll 1,000,000 with half of the values forbidden
        Player.nDice = 1000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 >= 5 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2000, DiceCluster.diceGroupArray[i].nRolls, 400, "Failed for dice of side: " + i);
            }
        }

        // Roll 1,000,000 with only 0 allowed
        Player.nDice = 1000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 != 0 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(10000, DiceCluster.diceGroupArray[i].nRolls, 400);
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollWithForbidden1B()
    {
        yield return initialize();

        // Roll 1,000,000,000 with half of the values forbidden
        Player.nDice = 1000000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 >= 5 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2000000, DiceCluster.diceGroupArray[i].nRolls, 400, "Failed for dice of side: " + i);
            }
        }

        // Roll 1,000,000,000 with only 0 allowed
        Player.nDice = 1000000000;
        Player.maxRoll = 1000;
        DiceCluster.allowedRolls = new List<int>() { 0 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 != 0 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(10000000, DiceCluster.diceGroupArray[i].nRolls, 400);
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollWithLowerMax()
    {
        yield return initialize();

        // Roll 1,000,000 with max 500
        Player.nDice = 1000000;
        Player.maxRoll = 500;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i >= 501 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2000, DiceCluster.diceGroupArray[i].nRolls, 400, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000 with max 127
        Player.nDice = 1000000;
        Player.maxRoll = 127;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i >= 128 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(7874, DiceCluster.diceGroupArray[i].nRolls, 400, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000 with max 1
        Player.nDice = 1000000;
        Player.maxRoll = 1;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i != 1)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1000000, DiceCluster.diceGroupArray[i].nRolls);
            }
        }

        // Roll 1,000,000 with max 1234
        Player.nDice = 1000000;
        Player.maxRoll = 1234;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(810, DiceCluster.diceGroupArray[i].nRolls, 150);
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollWithLowerMax1B()
    {
        yield return initialize();

        // Roll 1,000,000,000 with max 500
        Player.nDice = 1000000000;
        Player.maxRoll = 500;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i >= 501 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2000000, DiceCluster.diceGroupArray[i].nRolls, 400, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000,000 with max 389
        Player.nDice = 1000000000;
        Player.maxRoll = 389;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i >= 390 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2570694, DiceCluster.diceGroupArray[i].nRolls, 400, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000,000 with max 1
        Player.nDice = 1000000000;
        Player.maxRoll = 1;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i != 1)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1000000000, DiceCluster.diceGroupArray[i].nRolls);
            }
        }

        // Roll 1,000,000,000 with max 1567
        Player.nDice = 1000000000;
        Player.maxRoll = 1567;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(638162, DiceCluster.diceGroupArray[i].nRolls, 150);
            }
        }

        yield break;
    }

    [UnityTest]
    public IEnumerator TestRollMixed()
    {
        yield return initialize();

        // Roll 1,000,000 with max 750, allowing only half of the rolls
        Player.nDice = 1000000;
        Player.maxRoll = 750;
        DiceCluster.allowedRolls = new List<int>() { 0, 2, 4, 6, 8 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 2 == 1 || i >= 751 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2666, DiceCluster.diceGroupArray[i].nRolls, 250, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }
        // Roll 1,000,000 with max 2750, allowing only 0, 1, 2
        Player.nDice = 1000000;
        Player.maxRoll = 2750;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 >= 3 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1212, DiceCluster.diceGroupArray[i].nRolls, 200, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000 so that only 3 an 6 are possible
        Player.nDice = 1000000;
        Player.maxRoll = 6;
        DiceCluster.allowedRolls = new List<int>() { 0, 3, 6 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i != 3 && i != 6)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(500000, DiceCluster.diceGroupArray[i].nRolls, 1500, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000 so that the nº of possibilities isn't divisible by 10
        Player.nDice = 1000000;
        Player.maxRoll = 371;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3 };
        yield return dc.rollAll();

        assertRolled(1000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 >= 4 || i >= 372 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(6711, DiceCluster.diceGroupArray[i].nRolls, 250, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

    }

    [UnityTest]
    public IEnumerator TestRollMixed1B()
    {
        yield return initialize();

        // Roll 1,000,000,00 with max 750, allowing only half of the rolls
        Player.nDice = 1000000000;
        Player.maxRoll = 750;
        DiceCluster.allowedRolls = new List<int>() { 1, 3, 5, 7, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 2 == 0 || i >= 751)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2666666, DiceCluster.diceGroupArray[i].nRolls, 300, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000,000 with max 2750, allowing only 0, 1, 2, 3
        Player.nDice = 1000000000;
        Player.maxRoll = 5430;
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 10 >= 4 || i == 0)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(460405, DiceCluster.diceGroupArray[i].nRolls, 400, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000,000 so that only 3 an 6 are possible
        Player.nDice = 1000000000;
        Player.maxRoll = 6;
        DiceCluster.allowedRolls = new List<int>() { 0, 3, 6 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i != 3 && i != 6)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(500000000, DiceCluster.diceGroupArray[i].nRolls, 4000, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

        // Roll 1,000,000,000 so that the nº of possibilities isn't divisible by 10
        Player.nDice = 1000000000;
        Player.maxRoll = 771;
        DiceCluster.allowedRolls = new List<int>() { 3, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if ((i % 10 != 3 && i % 10 <= 7) || i >= 772)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(4329004, DiceCluster.diceGroupArray[i].nRolls, 1000, "Rolled " + i + " a total " +
                    DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

    }

    [UnityTest]
    public IEnumerator TestRollMixedE300()
    {
        yield return initialize();

        // Roll E300
        Player.nDice = System.Math.Pow(10, 300);
        Player.maxRoll = 1273;
        DiceCluster.allowedRolls = new List<int>() { 1, 5, 7, 9 };
        yield return dc.rollAll();

        assertRolled(System.Math.Pow(10, 300));
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i % 2 == 0 || i % 10 == 3)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(System.Math.Pow(10, 300) / (1273 * 0.4), DiceCluster.diceGroupArray[i].nRolls,
                    System.Math.Pow(10, 294), 
                    "Rolled " + i + " a total " + DiceCluster.diceGroupArray[i].nRolls + " times");
            }
        }

    }

    [UnityTest]
    public IEnumerator TestImpossibleRoll() // Edge cases that result in impossible rolls
    {
        yield return initialize();

        // Roll 1,000 with max 1, forbidding 1
        Player.nDice = 1000;
        Player.maxRoll = 1;
        DiceCluster.allowedRolls = new List<int>() { 0, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0 || i >= 2)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1000, DiceCluster.diceGroupArray[i].nRolls);
            }
        }

        // Roll 10,000,000 with max 1, forbidding 1
        Player.nDice = 10000000;
        Player.maxRoll = 1;
        DiceCluster.allowedRolls = new List<int>() { 0, 2, 3, 4, 5, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(10000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0 || i >= 2)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(10000000, DiceCluster.diceGroupArray[i].nRolls);
            }
        }

        // Roll 10,000,000 with max 5, forbidding 1, 2, 3, 4, 5
        Player.nDice = 10000000;
        Player.maxRoll = 5;
        DiceCluster.allowedRolls = new List<int>() { 0, 6, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(10000000);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0 || i >= 6)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(2000000, DiceCluster.diceGroupArray[i].nRolls, 1000);
            }
        }

        // Roll E20 with max 6, forbidding 1, 2, 3, 4, 5, 6 (non divisible by E20)
        Player.nDice = 1E+20;
        Player.maxRoll = 6;
        DiceCluster.allowedRolls = new List<int>() { 0, 7, 8, 9 };
        yield return dc.rollAll();

        assertRolled(1E+20);
        for (int i = 0; i < DiceCluster.diceGroupArray.Length; i++)
        {
            if (i == 0 || i >= 7)
            {
                Assert.AreEqual(0, DiceCluster.diceGroupArray[i].nRolls);
            }
            else
            {
                Assert.AreEqual(1E+20/6, DiceCluster.diceGroupArray[i].nRolls, 1E+15);
            }
        }

    }

}
