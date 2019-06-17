using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Arm = ArmManager.Arm;
using ArmAspect = ArmManager.ArmAspect;

public class TestEnemy {

    [Test]
    public void TestHealth()
    {
        GameObject enemyObj = new GameObject();
        Enemy enemy = enemyObj.AddComponent<Enemy>();

        /*for (int i=0; i < 100; i++)
        {
            enemy.initializeEnemyData(i);
            Debug.Log(enemy.maxHealth);
        }*/

        enemy.initializeEnemyData(0);
        Assert.AreEqual(7, enemy.maxHealth);
        enemy.initializeEnemyData(1);
        Assert.AreEqual(8, enemy.maxHealth);
        enemy.initializeEnemyData(2);
        Assert.AreEqual(10, enemy.maxHealth);
        enemy.initializeEnemyData(10);
        Assert.AreEqual(10039, enemy.maxHealth);
        enemy.initializeEnemyData(50);
        Assert.AreEqual(156290548, enemy.maxHealth);
        enemy.initializeEnemyData(100);
        Assert.AreEqual(10234881024, enemy.maxHealth);
    }

    [Test]
    public void TestPower()
    {
        GameObject enemyObj = new GameObject();
        Enemy enemy = enemyObj.AddComponent<Enemy>();

        enemy.initializeEnemyData(0);
        Assert.AreEqual(5, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(1);
        Assert.AreEqual(6, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(2);
        Assert.AreEqual(7, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(5);
        Assert.AreEqual(10, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(20);
        Assert.AreEqual(30, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(50);
        Assert.AreEqual(154, enemy.damageAgainstHealth);
        enemy.initializeEnemyData(100);
        Assert.AreEqual(10223, enemy.damageAgainstHealth, 1d);
    }

    [Test]
    public void TestDiceDamage()
    {
        GameObject enemyObj = new GameObject();
        Enemy enemy = enemyObj.AddComponent<Enemy>();

        enemy.initializeEnemyData(0);
        Assert.AreEqual(.05f, enemy.damageAgainstDiceMin);
        Assert.AreEqual(.1f, enemy.damageAgainstDiceMax);
        Assert.AreEqual(100, enemy.minDiceToAllowDiceAttacks);

        enemy.initializeEnemyData(5);
        Assert.AreEqual(.09f, enemy.damageAgainstDiceMin, 0.01f);
        Assert.AreEqual(.15f, enemy.damageAgainstDiceMax, 0.01f);
        Assert.AreEqual(87, enemy.minDiceToAllowDiceAttacks);

        enemy.initializeEnemyData(10);
        Assert.AreEqual(.14f, enemy.damageAgainstDiceMin, 0.01f);
        Assert.AreEqual(.19f, enemy.damageAgainstDiceMax, 0.01f);
        Assert.AreEqual(76, enemy.minDiceToAllowDiceAttacks);

        enemy.initializeEnemyData(50);
        Assert.AreEqual(.32f, enemy.damageAgainstDiceMin, 0.01f);
        Assert.AreEqual(.46f, enemy.damageAgainstDiceMax, 0.01f);
        Assert.AreEqual(27, enemy.minDiceToAllowDiceAttacks);

        enemy.initializeEnemyData(100);
        Assert.AreEqual(.38f, enemy.damageAgainstDiceMin, 0.01f);
        Assert.AreEqual(.64f, enemy.damageAgainstDiceMax, 0.01f);
        Assert.AreEqual(10, enemy.minDiceToAllowDiceAttacks);

        enemy.initializeEnemyData(1000);
        Assert.AreEqual(.4f, enemy.damageAgainstDiceMin, 0.01f);
        Assert.AreEqual(.8f, enemy.damageAgainstDiceMax, 0.01f);
        Assert.AreEqual(5, enemy.minDiceToAllowDiceAttacks);

        for (int i=0; i <= 250; i++)
        {
            enemy.initializeEnemyData(i);
            if (enemy.damageAgainstDiceMin >= enemy.damageAgainstDiceMax)
            {
                Assert.Fail("Greater 'minDiceDamage' than 'maxDiceDamage' at " + i);
            }
        }
    }

    [Test]
    public void TestGetNArms()
    {
        for (int i=0; i <= 13; i++)
            Assert.AreEqual(2, Enemy.getNArms(i));

        Assert.AreEqual(4, Enemy.getNArms(14));
        Assert.AreEqual(2, Enemy.getNArms(15));
        
        Assert.AreEqual(2, Enemy.getNArms(23));
        Assert.AreEqual(4, Enemy.getNArms(24));
        Assert.AreEqual(2, Enemy.getNArms(25));

        Assert.AreEqual(2, Enemy.getNArms(32));
        Assert.AreEqual(4, Enemy.getNArms(33));
        Assert.AreEqual(4, Enemy.getNArms(34));
        Assert.AreEqual(2, Enemy.getNArms(35));

        Assert.AreEqual(2, Enemy.getNArms(66));
        Assert.AreEqual(4, Enemy.getNArms(67));
        Assert.AreEqual(4, Enemy.getNArms(68));
        Assert.AreEqual(4, Enemy.getNArms(69));
        Assert.AreEqual(2, Enemy.getNArms(70));
    }

    [Test]
    public void TestGetChanceOfSilverArm()
    {
        Assert.AreEqual(0, Enemy.getChanceOfSilverArm(98));
        Assert.AreEqual(0.01f, Enemy.getChanceOfSilverArm(99));
        Assert.AreEqual(0.5f, Enemy.getChanceOfSilverArm(300), 0.01f);
        Assert.AreEqual(1f, Enemy.getChanceOfSilverArm(500));
    }

    [Test]
    public void TestGetChanceOfGoldArm()
    {
        Assert.AreEqual(0, Enemy.getChanceOfGoldArm(248));
        Assert.AreEqual(0.01f, Enemy.getChanceOfGoldArm(249));
        Assert.AreEqual(0.5f, Enemy.getChanceOfGoldArm(1125), 0.01f);
        Assert.AreEqual(1f, Enemy.getChanceOfGoldArm(2000));
    }

    [Test]
    public void TestSpecialEnemyDict()
    {
        Enemy.initializeSpecialEnemyDict();
        Arm firstArm = Arm.NORMAL; Arm lastArm = Arm.FAN;

        // Nº special enemies (11 arms * 6 variants (6N, 8N, 6S, 8S, 6G, 8G))
        Assert.AreEqual(12 * 6, Enemy.specialEnemyDict.Keys.Count);

        // Ranks of first and last 6-armed enemy
        int first6Rank = 110; int last6Rank = 220;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first6Rank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[first6Rank - 1].nArms);
        Assert.AreEqual(ArmAspect.NORMAL, Enemy.specialEnemyDict[first6Rank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last6Rank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[last6Rank - 1].nArms);
        Assert.AreEqual(ArmAspect.NORMAL, Enemy.specialEnemyDict[last6Rank - 1].aspect);

        // Ranks of first and last 8-armed enemy
        int first8Rank = 230; int last8Rank = 340;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first8Rank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[first8Rank - 1].nArms);
        Assert.AreEqual(ArmAspect.NORMAL, Enemy.specialEnemyDict[first8Rank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last8Rank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[last8Rank - 1].nArms);
        Assert.AreEqual(ArmAspect.NORMAL, Enemy.specialEnemyDict[last8Rank - 1].aspect);

        // Ranks of first and last silver 6-armed enemy
        int first6SRank = 350; int last6SRank = 460;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first6SRank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[first6SRank - 1].nArms);
        Assert.AreEqual(ArmAspect.SILVER, Enemy.specialEnemyDict[first6SRank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last6SRank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[last6SRank - 1].nArms);
        Assert.AreEqual(ArmAspect.SILVER, Enemy.specialEnemyDict[last6SRank - 1].aspect);

        // Ranks of first and last silver 8-armed enemy
        int first8SRank = 470; int last8SRank = 580;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first8SRank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[first8SRank - 1].nArms);
        Assert.AreEqual(ArmAspect.SILVER, Enemy.specialEnemyDict[first8SRank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last8SRank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[last8SRank - 1].nArms);
        Assert.AreEqual(ArmAspect.SILVER, Enemy.specialEnemyDict[last8SRank - 1].aspect);

        // Ranks of first and last gold 6-armed enemy
        int first6GRank = 590; int last6GRank = 700;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first6GRank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[first6GRank - 1].nArms);
        Assert.AreEqual(ArmAspect.GOLD, Enemy.specialEnemyDict[first6GRank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last6GRank - 1].arm);
        Assert.AreEqual(6, Enemy.specialEnemyDict[last6GRank - 1].nArms);
        Assert.AreEqual(ArmAspect.GOLD, Enemy.specialEnemyDict[last6GRank - 1].aspect);

        // Ranks of first and last gold 8-armed enemy
        int first8GRank = 710; int last8GRank = 820;
        Assert.AreEqual(firstArm, Enemy.specialEnemyDict[first8GRank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[first8GRank - 1].nArms);
        Assert.AreEqual(ArmAspect.GOLD, Enemy.specialEnemyDict[first8GRank - 1].aspect);
        Assert.AreEqual(lastArm, Enemy.specialEnemyDict[last8GRank - 1].arm);
        Assert.AreEqual(8, Enemy.specialEnemyDict[last8GRank - 1].nArms);
        Assert.AreEqual(ArmAspect.GOLD, Enemy.specialEnemyDict[last8GRank - 1].aspect);
    }

}
