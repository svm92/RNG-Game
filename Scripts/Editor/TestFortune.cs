using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestFortune {

	[Test]
	public void TestEndurancePrice() {
        Fortune fortune = new Fortune(Fortune.FortuneId.ENDURANCE);

        fortune.updatePrice();
        Assert.AreEqual(5, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(10, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(21, fortune.priceToNextLevel);

        fortune.level = 3; fortune.updatePrice();
        Assert.AreEqual(43, fortune.priceToNextLevel);

        fortune.level = 10; fortune.updatePrice();
        Assert.AreEqual(1315, fortune.priceToNextLevel);

        fortune.level = 100; fortune.updatePrice();
        Assert.AreEqual(29703442, fortune.priceToNextLevel);
    }

    [Test]
    public void TestEnduranceEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.ENDURANCE);

        fortune.level = 0; fortune.applyEffect();
        Assert.AreEqual(30, Player.initialHealth);

        fortune.level = 1; fortune.applyEffect();
        Assert.AreEqual(35, Player.initialHealth);

        fortune.level = 5; fortune.applyEffect();
        Assert.AreEqual(64, Player.initialHealth);

        fortune.level = 10; fortune.applyEffect();
        Assert.AreEqual(140, Player.initialHealth);

        fortune.level = 50; fortune.applyEffect();
        Assert.AreEqual(66751, Player.initialHealth);
    }

    [Test]
    public void TestQuickStartPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.QUICK_START);

        fortune.updatePrice();
        Assert.AreEqual(10, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(22, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(51, fortune.priceToNextLevel);

        fortune.level = 3; fortune.updatePrice();
        Assert.AreEqual(113, fortune.priceToNextLevel);

        fortune.level = 100; fortune.updatePrice();
        Assert.AreEqual(302916845, fortune.priceToNextLevel);
    }

    [Test]
    public void TestQuickStartEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.QUICK_START);

        for (int i=0; i <= 10; i++)
        {
            fortune.level = i; fortune.applyEffect();
            Assert.AreEqual(i+1, Player.firstTurnDiceMultiplier);
        }

        fortune.level = 20; fortune.applyEffect();
        Assert.AreEqual(24, Player.firstTurnDiceMultiplier);

        fortune.level = 100; fortune.applyEffect();
        Assert.AreEqual(10101, Player.firstTurnDiceMultiplier, 1d);
    }

    [Test]
    public void TestShieldPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.SHIELD);

        fortune.updatePrice();
        Assert.AreEqual(50, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(87, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(153, fortune.priceToNextLevel);

        fortune.level = 3; fortune.updatePrice();
        Assert.AreEqual(267, fortune.priceToNextLevel);

        fortune.level = 6; fortune.updatePrice();
        Assert.AreEqual(1436, fortune.priceToNextLevel);
    }

    [Test]
    public void TestShieldEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.SHIELD);

        fortune.level = 0; fortune.applyEffect();
        Assert.AreEqual(0, Player.shields);
        Assert.AreEqual(150, Player.applyShieldDamageReduction(150));

        fortune.level = 1; fortune.applyEffect();
        Assert.AreEqual(.051f, Player.shields, 1E-3f);
        Assert.AreEqual(142, Player.applyShieldDamageReduction(150));

        fortune.level = 2; fortune.applyEffect();
        Assert.AreEqual(.1f, Player.shields, 1E-5f);
        Assert.AreEqual(135, Player.applyShieldDamageReduction(150));

        fortune.level = 3; fortune.applyEffect();
        Assert.AreEqual(.146f, Player.shields, 1E-3f);
        Assert.AreEqual(128, Player.applyShieldDamageReduction(150));

        fortune.level = 10; fortune.applyEffect();
        Assert.AreEqual(.409f, Player.shields, 1E-3f);
        Assert.AreEqual(89, Player.applyShieldDamageReduction(150));
    }

    [Test]
    public void TestRegenPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.REGEN);

        fortune.updatePrice();
        Assert.AreEqual(100, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(150, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(225, fortune.priceToNextLevel);

        fortune.level = 3; fortune.updatePrice();
        Assert.AreEqual(337, fortune.priceToNextLevel);

        fortune.level = 10; fortune.updatePrice();
        Assert.AreEqual(5766, fortune.priceToNextLevel);
    }

    [Test]
    public void TestRegenEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.REGEN);
        Player.initialHealth = 150;

        fortune.level = 0; Player.health = 150; fortune.applyEffect(); Player.applyRegen();
        Assert.AreEqual(150, Player.health);

        fortune.level = 1; Player.health = 150; fortune.applyEffect(); Player.applyRegen();
        Assert.AreEqual(153, Player.health);

        fortune.level = 2; Player.health = 150; fortune.applyEffect(); Player.applyRegen();
        Assert.AreEqual(157, Player.health);

        fortune.level = 5; Player.health = 150; fortune.applyEffect(); Player.applyRegen();
        Assert.AreEqual(168, Player.health);
    }

    [Test]
    public void TestLowerCeilEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.LOWER_CEIL);

        /*for (int i=0; i < 500; i++)
        {
            fortune.level = i; fortune.applyEffect();
            Debug.Log(Player.maxRoll);
        }*/
 
        fortune.level = 0; fortune.applyEffect();
        Assert.AreEqual(1000, Player.maxRoll);

        fortune.level = 1; fortune.applyEffect();
        Assert.AreEqual(950, Player.maxRoll);

        fortune.level = 5; fortune.applyEffect();
        Assert.AreEqual(833, Player.maxRoll);

        fortune.level = 50; fortune.applyEffect();
        Assert.AreEqual(470, Player.maxRoll);

        fortune.level = 100; fortune.applyEffect();
        Assert.AreEqual(301, Player.maxRoll);

        fortune.level = 200; fortune.applyEffect();
        Assert.AreEqual(100, Player.maxRoll);

        fortune.level = 500; fortune.applyEffect();
        Assert.AreEqual(100, Player.maxRoll);
    }

    [Test]
    public void TestLowerCielPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.LOWER_CEIL);

        fortune.updatePrice();
        Assert.AreEqual(30, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(72, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(129, fortune.priceToNextLevel);
    }

    [Test]
    public void TestExtraExpPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.EXTRA_EXP);

        fortune.updatePrice();
        Assert.AreEqual(15, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(37, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(70, fortune.priceToNextLevel);

        fortune.level = 3; fortune.updatePrice();
        Assert.AreEqual(117, fortune.priceToNextLevel);

        fortune.level = 10; fortune.updatePrice();
        Assert.AreEqual(1536, fortune.priceToNextLevel);
    }

    [Test]
    public void TestExtraPrizeCardPrice()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.EXTRA_PRIZE_CARD);

        fortune.updatePrice();
        Assert.AreEqual(25, fortune.priceToNextLevel);

        fortune.level = 1; fortune.updatePrice();
        Assert.AreEqual(57, fortune.priceToNextLevel);

        fortune.level = 2; fortune.updatePrice();
        Assert.AreEqual(99, fortune.priceToNextLevel);
    }

    [Test]
    public void TestExtraPrizeCardEffect()
    {
        Fortune fortune = new Fortune(Fortune.FortuneId.EXTRA_PRIZE_CARD);

        fortune.applyEffect();
        Assert.AreEqual(0, Player.chanceExtraCard);

        fortune.level = 1; fortune.applyEffect();
        Assert.AreEqual(0.25f, Player.chanceExtraCard);

        fortune.level = 2; fortune.applyEffect();
        Assert.AreEqual(0.5f, Player.chanceExtraCard);

        fortune.level = 3; fortune.applyEffect();
        Assert.AreEqual(0.625f, Player.chanceExtraCard);

        fortune.level = 10; fortune.applyEffect();
        Assert.AreEqual(0.863f, Player.chanceExtraCard, 0.001f);

        /*for (int i=0; i <100; i++)
        {
            fortune.level = i; fortune.applyEffect();
            Debug.Log(Player.chanceExtraCard);
        }*/
    }

    [Test]
    public void TestGetMultiplierAsString()
    {
        Assert.AreEqual("0", Fortune.getMultiplierAsString(1));
        Assert.AreEqual("25", Fortune.getMultiplierAsString(1.25));
        Assert.AreEqual("50", Fortune.getMultiplierAsString(1.5));
        Assert.AreEqual("72", Fortune.getMultiplierAsString(1.725));
        Assert.AreEqual("87", Fortune.getMultiplierAsString(1.8724));
        Assert.AreEqual("80", Fortune.getMultiplierAsString(1.8));
        Assert.AreEqual("150", Fortune.getMultiplierAsString(2.5));
        Assert.AreEqual("2900", Fortune.getMultiplierAsString(30));
        Assert.AreEqual("50K", Fortune.getMultiplierAsString(501));
        Assert.AreEqual("500K", Fortune.getMultiplierAsString(5001));
        Assert.AreEqual("5M", Fortune.getMultiplierAsString(50001));

        /*for (int i=0; i <= 100000; i++) // Check that no value is wrongly approximated as .99999
        {
            float mult = 1 + 0.2f * i;
            string multString = Fortune.getMultiplierAsString(mult);
            if (multString.EndsWith("9"))
            {
                Debug.Log(multString);
            }
        }*/
    }

    [Test]
    public void TestGetFractionAsString()
    {
        Assert.AreEqual("0", Fortune.getFractionAsString(0f));
        Assert.AreEqual("25", Fortune.getFractionAsString(0.25f));
        Assert.AreEqual("50", Fortune.getFractionAsString(0.5f));
        Assert.AreEqual("50.05", Fortune.getFractionAsString(0.5005f));
        Assert.AreEqual("50.5", Fortune.getFractionAsString(0.5050f));
        Assert.AreEqual("72.5", Fortune.getFractionAsString(0.725f));
        Assert.AreEqual("87.25", Fortune.getFractionAsString(0.8725f));
        Assert.AreEqual("97.05", Fortune.getFractionAsString(0.9705f));
    }
}
