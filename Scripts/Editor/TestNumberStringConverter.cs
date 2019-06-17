using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestNumberStringConverter {

	[Test]
	public void TestConvert() {
        Assert.AreEqual("1", NumberStringConverter.convert(1d));
        Assert.AreEqual("7", NumberStringConverter.convert(7d));
        Assert.AreEqual("123", NumberStringConverter.convert(123d));
        Assert.AreEqual("5432", NumberStringConverter.convert(5432d));
        Assert.AreEqual("54.3K", NumberStringConverter.convert(54321d));
        Assert.AreEqual("128K", NumberStringConverter.convert(128325d));
        Assert.AreEqual("1.23M", NumberStringConverter.convert(1234567d));
        Assert.AreEqual("20M", NumberStringConverter.convert(20000000d));
        Assert.AreEqual("146M", NumberStringConverter.convert(146761842d));
        Assert.AreEqual("7.21B", NumberStringConverter.convert(7216325741d));
        Assert.AreEqual("127B", NumberStringConverter.convert(127832952030d));
        Assert.AreEqual("73.4T", NumberStringConverter.convert(73462631752923d));
        Assert.AreEqual("73.4q", NumberStringConverter.convert(73462631752923000d));
        Assert.AreEqual("73.4Q", NumberStringConverter.convert(73462631752923000000d));
        Assert.AreEqual("73.4s", NumberStringConverter.convert(73462631752000923000000d));
        Assert.AreEqual("73.4S", NumberStringConverter.convert(73462630001752000923000000d));
        Assert.AreEqual("73.4O", NumberStringConverter.convert(73462630001752000923000000000d));
        Assert.AreEqual("73.4N", NumberStringConverter.convert(73460002630001752000923000000000d));
        Assert.AreEqual("73.4D", NumberStringConverter.convert(73460002630001752000923000000000000d));

        // Check that trailing zeros are removed
        Assert.AreEqual("1T", NumberStringConverter.convert(1000000000000d));
        Assert.AreEqual("10T", NumberStringConverter.convert(10000000000000d));
        Assert.AreEqual("100T", NumberStringConverter.convert(100000000000000d));
        Assert.AreEqual("1.20T", NumberStringConverter.convert(1200000000000d));
        Assert.AreEqual("12T", NumberStringConverter.convert(12000000000000d));

        // Check that non-trailing zeros aren't removed
        Assert.AreEqual("1.05T", NumberStringConverter.convert(1050000000000d));

        // Values represented via scientific notation
        Assert.AreEqual("712D", NumberStringConverter.convert(712860002630001752000923000000000000d));
        Assert.AreEqual("7.12E36", NumberStringConverter.convert(7128600026300017520009230000000000000d));
        Assert.AreEqual("7.12E37", NumberStringConverter.convert(71286000263000175200092300000000000000d));
        Assert.AreEqual("7.12E38", NumberStringConverter.convert(712860002630001752000923000000000000000d));
        Assert.AreEqual("7.12E42", NumberStringConverter.convert(7128600026300017520009230000000000000000000d));
        Assert.AreEqual("1E100", NumberStringConverter.convert(1E+100));
        Assert.AreEqual("1.53E100", NumberStringConverter.convert(1.53523452E+100));
        Assert.AreEqual("1.03E100", NumberStringConverter.convert(1.03523452E+100));
        Assert.AreEqual("1.70E100", NumberStringConverter.convert(1.70523452E+100));
    }

}
