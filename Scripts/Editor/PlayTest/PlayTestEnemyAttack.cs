using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Arm = ArmManager.Arm;
using ArmAspect = ArmManager.ArmAspect;

public class PlayTestEnemyAttack {

    IEnumerator initialize()
    {
        // Initialize
        Player.restoreValuesToDefault();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        yield return null;

        // Hide hand canvas
        GameObject.Find("HandCanvas").SetActive(false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestNormalAttack() {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.NORMAL, Arm.NORMAL };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        yield return enemy.launchAttack(Enemy.Attack.NORMAL_ATTACK);
        Assert.AreEqual(25, Player.health);
        yield return enemy.launchAttack(Enemy.Attack.NORMAL_ATTACK);
        Assert.AreEqual(20, Player.health);
    }

    [UnityTest]
    public IEnumerator TestDiceAttack()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.NORMAL, Arm.NORMAL };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        for (int i = 0; i < 20; i++) // There's some amount of randomness in this attack, so do this test a handful of times
        { // 5 ~ 10 dice damage
            Player.nDice = 100;
            yield return enemy.launchAttack(Enemy.Attack.DICE_ATTACK);
            Assert.AreEqual(92.5, Player.nDice, 2.5);
        }

        enemy.initializeEnemyData(0);
        armList = new List<Arm> { Arm.GRAPNEL, Arm.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        for (int i = 0; i < 20; i++) // 10 ~ 30 dice damage
        {
            Player.nDice = 100;
            yield return enemy.launchAttack(Enemy.Attack.DICE_ATTACK);
            Assert.AreEqual(80, Player.nDice, 10);
        }

        enemy.initializeEnemyData(0);
        armList = new List<Arm> { Arm.GRAPNEL, Arm.GRAPNEL, Arm.GRAPNEL, Arm.GRAPNEL };
        armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL, ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        for (int i = 0; i < 20; i++) // 25 ~ 90 dice damage
        {
            Player.nDice = 100;
            yield return enemy.launchAttack(Enemy.Attack.DICE_ATTACK);
            Assert.AreEqual(42.5, Player.nDice, 32.5);
        }

        enemy.initializeEnemyData(0);
        armList = new List<Arm> { Arm.GRAPNEL, Arm.GRAPNEL, Arm.NORMAL, Arm.GRAPNEL };
        armAspects = new List<ArmAspect> { ArmAspect.GOLD, ArmAspect.NORMAL, ArmAspect.NORMAL, ArmAspect.SILVER };
        enemy.applyAllArmEffects(armList, armAspects);

        for (int i = 0; i < 20; i++) // 40 ~ 90 dice damage
        {
            Player.nDice = 100;
            yield return enemy.launchAttack(Enemy.Attack.DICE_ATTACK);
            Assert.AreEqual(35, Player.nDice, 25);
        }
        Assert.AreEqual(30, Player.health);

        enemy.initializeEnemyData(0);
        armList = new List<Arm> { Arm.GRAPNEL, Arm.HOOK, Arm.HOOK, Arm.GRAPNEL };
        armAspects = new List<ArmAspect> { ArmAspect.GOLD, ArmAspect.GOLD, ArmAspect.SILVER, ArmAspect.SILVER };
        enemy.applyAllArmEffects(armList, armAspects);
        Player.nDice = 100;
        yield return enemy.launchAttack(Enemy.Attack.DICE_ATTACK);
        Assert.AreEqual(25, Player.health);
    }

    [UnityTest]
    public IEnumerator TestMill()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.BUZZSAW, Arm.BUZZSAW };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);
        Assert.AreEqual(10, BattleManager.bmInstance.battleDeck.cards.Count); // Draw 5

        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(9, BattleManager.bmInstance.battleDeck.cards.Count);
        Assert.AreEqual(30, Player.health);

        armAspects = new List<ArmAspect> { ArmAspect.SILVER, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(8, BattleManager.bmInstance.battleDeck.cards.Count);
        Assert.AreEqual(28, Player.health);
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(7, BattleManager.bmInstance.battleDeck.cards.Count);
        Assert.AreEqual(26, Player.health);

        armAspects = new List<ArmAspect> { ArmAspect.GOLD, ArmAspect.SILVER };
        enemy.applyAllArmEffects(armList, armAspects);
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(5, BattleManager.bmInstance.battleDeck.cards.Count);
        Assert.AreEqual(22, Player.health);

        // Try getting deck down to 0
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(3, BattleManager.bmInstance.battleDeck.cards.Count);
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(1, BattleManager.bmInstance.battleDeck.cards.Count);
        yield return enemy.launchAttack(Enemy.Attack.MILL);
        Assert.AreEqual(0, BattleManager.bmInstance.battleDeck.cards.Count);
    }

    [UnityTest]
    public IEnumerator TestDiscard()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.FAN, Arm.FAN };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        Assert.AreEqual(5, Hand.hand.Count);
        yield return enemy.launchAttack(Enemy.Attack.DISCARD);
        Assert.AreEqual(4, Hand.hand.Count);
        Assert.AreEqual(30, Player.health);

        armAspects = new List<ArmAspect> { ArmAspect.SILVER, ArmAspect.SILVER };
        enemy.applyAllArmEffects(armList, armAspects);
        yield return enemy.launchAttack(Enemy.Attack.DISCARD);
        Assert.AreEqual(3, Hand.hand.Count);
        Assert.AreEqual(28, Player.health);

        armAspects = new List<ArmAspect> { ArmAspect.SILVER, ArmAspect.GOLD };
        enemy.applyAllArmEffects(armList, armAspects);
        yield return enemy.launchAttack(Enemy.Attack.DISCARD);
        Assert.AreEqual(2, Hand.hand.Count);
        Assert.AreEqual(24, Player.health);
    }

    [UnityTest]
    public IEnumerator TestHeal()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.SALVE, Arm.SALVE };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        Assert.AreEqual(7, enemy.health);
        enemy.receiveDamage(5);
        yield return enemy.launchAttack(Enemy.Attack.HEAL);
        Assert.AreEqual(5, enemy.health);
        yield return enemy.launchAttack(Enemy.Attack.HEAL);
        Assert.AreEqual(7, enemy.health);

        armAspects = new List<ArmAspect> { ArmAspect.SILVER, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);
        enemy.receiveDamage(5);
        yield return enemy.launchAttack(Enemy.Attack.HEAL);
        Assert.AreEqual(5, enemy.health);
        yield return enemy.launchAttack(Enemy.Attack.HEAL);
        Assert.AreEqual(8, enemy.health);
    }

    [UnityTest]
    public IEnumerator TestRecoilAttack()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.FORBIDDEN_RELIC, Arm.FORBIDDEN_RELIC };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        yield return enemy.launchAttack(Enemy.Attack.RECOIL_ATTACK);
        Assert.AreEqual(20, Player.health);
        Assert.AreEqual(6, enemy.health);

        armAspects = new List<ArmAspect> { ArmAspect.GOLD, ArmAspect.SILVER };
        enemy.applyAllArmEffects(armList, armAspects);

        Player.health = 50;
        yield return enemy.launchAttack(Enemy.Attack.RECOIL_ATTACK);
        Assert.AreEqual(10, Player.health);
        Assert.AreEqual(4, enemy.health);
    }

    [UnityTest]
    public IEnumerator TestDrain()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.SIPHON, Arm.SIPHON };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        enemy.receiveDamage(6);
        yield return enemy.launchAttack(Enemy.Attack.DRAIN);
        Assert.AreEqual(28, Player.health);
        Assert.AreEqual(3, enemy.health);

        armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.GOLD };
        enemy.applyAllArmEffects(armList, armAspects);
        yield return enemy.launchAttack(Enemy.Attack.DRAIN);
        Assert.AreEqual(20, Player.health);
        Assert.AreEqual(7, enemy.health);
    }

    [UnityTest]
    public IEnumerator TestWeakenCards()
    {
        yield return initialize();

        Enemy enemy = BattleManager.bmInstance.enemy;
        enemy.initializeEnemyData(0);
        List<Arm> armList = new List<Arm> { Arm.FLOE, Arm.FLOE };
        List<ArmAspect> armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);

        Card cardA = Hand.hand[0].GetComponent<CardObject>().card;
        cardA.cardEffect = new CardEffect(55, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 804);

        yield return enemy.launchAttack(Enemy.Attack.WEAKEN_CARDS);
        Assert.AreEqual(47, cardA.cardEffect.cardPoints);
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardA.cardEffect.condition);
        Assert.AreEqual(0, cardA.cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardA.cardEffect.effect);
        Assert.AreEqual(465, cardA.cardEffect.effectNumber);

        armList = new List<Arm> { Arm.FLOE, Arm.FLOE, Arm.FLOE, Arm.FLOE };
        armAspects = new List<ArmAspect> { ArmAspect.NORMAL, ArmAspect.GOLD, ArmAspect.SILVER, ArmAspect.NORMAL };
        enemy.applyAllArmEffects(armList, armAspects);
        cardA.cardEffect = new CardEffect(55, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 804);

        yield return enemy.launchAttack(Enemy.Attack.WEAKEN_CARDS);
        Assert.AreEqual(28, cardA.cardEffect.cardPoints);
        Assert.AreEqual(CardEffect.Condition.INSTANT, cardA.cardEffect.condition);
        Assert.AreEqual(0, cardA.cardEffect.conditionNumber);
        Assert.AreEqual(CardEffect.Effect.DICE, cardA.cardEffect.effect);
        Assert.AreEqual(109, cardA.cardEffect.effectNumber);
    }

}
