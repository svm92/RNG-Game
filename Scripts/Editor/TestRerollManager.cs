using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestRerollManager {

	[Test]
	public void TestGetRerollPoints() {
        Assert.AreEqual(5, RerollManager.getRerollPoints(49));
        Assert.AreEqual(5, RerollManager.getRerollPoints(50));
        Assert.AreEqual(7, RerollManager.getRerollPoints(60));
        Assert.AreEqual(10, RerollManager.getRerollPoints(75));
        Assert.AreEqual(15, RerollManager.getRerollPoints(100));
        Assert.AreEqual(176, RerollManager.getRerollPoints(500));
    }

    [Test]
    public void TestGetRerollExpMult()
    {
        Assert.AreEqual(1, RerollManager.getRerollExpMult(0));
        Assert.AreEqual(1.625, RerollManager.getRerollExpMult(5));
        Assert.AreEqual(1.875, RerollManager.getRerollExpMult(7));
        Assert.AreEqual(2.25, RerollManager.getRerollExpMult(10));
        Assert.AreEqual(2.875, RerollManager.getRerollExpMult(15));
        Assert.AreEqual(23, RerollManager.getRerollExpMult(176));
    }

    [Test]
    public void TestGetRerollSkip()
    {
        Assert.AreEqual(0, RerollManager.getRerollSkip(0));
        Assert.AreEqual(0, RerollManager.getRerollSkip(1));
        Assert.AreEqual(2, RerollManager.getRerollSkip(5));
        Assert.AreEqual(3, RerollManager.getRerollSkip(10));
        Assert.AreEqual(5, RerollManager.getRerollSkip(50));
        Assert.AreEqual(6, RerollManager.getRerollSkip(100));
        Assert.AreEqual(9, RerollManager.getRerollSkip(1000));
    }

}
