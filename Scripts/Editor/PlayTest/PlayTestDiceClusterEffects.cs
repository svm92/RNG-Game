using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class PlayTestDiceClusterEffects {

    public DiceCluster dc;

    IEnumerator initialize()
    {
        // Initialize
        Player.restoreValuesToDefault();
        DiceCluster.allowedRolls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // Skip anims
        Dice.minPreRollTime = 0f;
        DiceCluster.skipPostAttackAnimation = true;
        BattleManager.speedUpBattleAnimations = true;
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        yield return null;

        // Find DiceCluster and hide other HUDs
        dc = GameObject.Find("DiceCluster").GetComponent<DiceCluster>();
        DiceCluster.isAnEffectTest = true;
        GameObject.Find("HUD").SetActive(false);
        yield return null;
        GameObject.Find("HandCanvas").SetActive(false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestEffectGeneric()
    {
        yield return initialize();

        // Roll 1,000,000
        Player.nDice = 1000000;
        Player.maxRoll = 1000;
        Player.health = 1;
        Player.cardEffects = new List<CardEffect>() {
            new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 7),
            new CardEffect(0, CardEffect.Condition.LAST, 5, CardEffect.Effect.DICE, 2),
            new CardEffect(0, CardEffect.Condition.LAST, 0, CardEffect.Effect.DICE, 5),

            new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 3),
            new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 8)
        };
        yield return dc.rollAll();

        Assert.AreEqual(1000000 + ((100000) * (7+2+5)), Player.nDice, 4000);
        Assert.AreEqual(1 + ((111111) * (3+8)), Player.health, 8000);

        // Roll 1,000,000,000
        Player.nDice = 1000000000;
        Player.maxRoll = 1000;
        Player.health = 1;
        Player.cardEffects = new List<CardEffect>() {
            new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 7),
            new CardEffect(0, CardEffect.Condition.LAST, 5, CardEffect.Effect.DICE, 2),
            new CardEffect(0, CardEffect.Condition.LAST, 0, CardEffect.Effect.DICE, 5),

            new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 3),
            new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 8)
        };
        yield return dc.rollAll();

        Assert.AreEqual(1000000000f + ((100000000) * (7 + 2 + 5)), Player.nDice, 5000);
        Assert.AreEqual(1 + ((111111111) * (3 + 8)), Player.health, 10000000);

    }

    [UnityTest]
    public IEnumerator TestEffectGeneric1q()
    {
        yield return initialize();

        // Roll 1q
        Player.nDice = Mathf.Pow(10, 15);
        Player.maxRoll = 1000;
        Player.health = 1;
        Player.cardEffects = new List<CardEffect>() {
            new CardEffect(0, CardEffect.Condition.LAST, 1, CardEffect.Effect.DICE, 4),
            new CardEffect(0, CardEffect.Condition.LAST, 2, CardEffect.Effect.DICE, 3),
            new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 2),
            new CardEffect(0, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE, 1),

            new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 2),
            new CardEffect(0, CardEffect.Condition.FIRST, 8, CardEffect.Effect.HEALTH, 1),
            new CardEffect(0, CardEffect.Condition.FIRST, 9, CardEffect.Effect.HEALTH, 2),
        };
        yield return dc.rollAll();

        Assert.AreEqual(Mathf.Pow(10, 15) + (Mathf.Pow(10, 14) * (4 + 3 + 2 + 1)), Player.nDice, Mathf.Pow(10, 9));
        Assert.AreEqual(1 + (Mathf.Pow(10, 15) / 9 * (2 + 1 + 2)), Player.health, Mathf.Pow(10, 12));

        // Roll E250
        Player.nDice = System.Math.Pow(10, 250);
        Player.maxRoll = 1000;
        Player.health = 1;
        Player.cardEffects = new List<CardEffect>() {
            new CardEffect(0, CardEffect.Condition.LAST, 1, CardEffect.Effect.DICE, 4),
            new CardEffect(0, CardEffect.Condition.LAST, 2, CardEffect.Effect.DICE, 3),
            new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 2),
            new CardEffect(0, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE, 1),

            new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 2),
            new CardEffect(0, CardEffect.Condition.FIRST, 8, CardEffect.Effect.HEALTH, 1),
            new CardEffect(0, CardEffect.Condition.FIRST, 9, CardEffect.Effect.HEALTH, 2),
        };
        yield return dc.rollAll();

        Assert.AreEqual(System.Math.Pow(10, 250) + (System.Math.Pow(10, 249) * (4 + 3 + 2 + 1)), 
            Player.nDice, System.Math.Pow(10, 244));
        Assert.AreEqual(1 + (System.Math.Pow(10, 250) / 9 * (2 + 1 + 2)), Player.health, System.Math.Pow(10, 247));

    }

    [UnityTest]
    public IEnumerator TestEffectOnly1s()
    {
        yield return initialize();

        // Roll 1q
        Player.nDice = Mathf.Pow(10, 15);
        Player.maxRoll = 1;
        Player.health = 1;
        Player.cardEffects = new List<CardEffect>() {
            new CardEffect(0, CardEffect.Condition.LAST, 1, CardEffect.Effect.DICE, 4),
            new CardEffect(0, CardEffect.Condition.LAST, 2, CardEffect.Effect.DICE, 3),
            new CardEffect(0, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 2),
            new CardEffect(0, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE, 1),

            new CardEffect(0, CardEffect.Condition.FIRST, 7, CardEffect.Effect.HEALTH, 2),
            new CardEffect(0, CardEffect.Condition.FIRST, 8, CardEffect.Effect.HEALTH, 1),
            new CardEffect(0, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 2),
        };
        yield return dc.rollAll();

        Assert.AreEqual(Mathf.Pow(10, 15) + (Mathf.Pow(10, 15) * (4)), Player.nDice, Mathf.Pow(10, 10));
        Assert.AreEqual(1 + (Mathf.Pow(10, 15) * (2)), Player.health, Mathf.Pow(10, 10));
    }

}
