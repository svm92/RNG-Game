using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestAutoUpdater {

    string pageHTML = "<!DOCTYPE HTML><html...Download Now</a><span class=\"buy_message\"><span class=\"sub\"> Name your own price</span></span></div></div><div class=\"uploads\"><p>Click download now to get access to the following files:</p><div class=\"upload_list_widget\"><div class=\"upload\"><div class=\"info_column\"><div class=\"upload_name\">" +
        "<strong title = \"random-number-god-win-release.zip\" class=\"name\"> random-number-god-win-release.zip</strong> <span class=\"file_size\"><span>21 MB</span></span> <span class=\"download_platforms\"><span title = \"Download for Windows\" class=\"icon icon-windows8\"></span> </span></div><div class=\"build_row\"><span class=\"version_name\">Version 1.1.2</span></div></div></div><div class=\"upload\">" +
        "<strong title=\"random-number-god-linux-release.zip\" class=\"name\"> random-number-god-linux-release.zip</strong> <span class=\"file_size\"><span>42 MB</span></span> <span class=\"download_platforms\"><span title=\"Download for Linux\" class=\"icon icon-tux\"></span> </span></div><div class=\"build_row\"><span class=\"version_name\">Version 1.1.25</span></div></div></div></div></div><h2>Also available..." +
    "...</script></body></html>";
    
    const string winBuildName = "random-number-god-win-release.zip";
    const string linuxBuildName = "random-number-god-linux-release.zip";

    [Test]
	public void TestHTMLIsParsedCorrectly() {
        Assert.AreEqual("1.1.2", AutoUpdater.parseHTMLForVersion(pageHTML, winBuildName));
        Assert.AreEqual("1.1.25", AutoUpdater.parseHTMLForVersion(pageHTML, linuxBuildName));

        // Check that it doesn't crash on errors
        Assert.AreEqual("", AutoUpdater.parseHTMLForVersion("vsdvsdvsdsdv", winBuildName));
        Assert.AreEqual("", AutoUpdater.parseHTMLForVersion("random-number-god-win-release.zip>...<>", winBuildName));
        Assert.AreEqual("", AutoUpdater.parseHTMLForVersion("random-number-god-win-release.zip...<version_nameczxcxz", winBuildName));
        Assert.AreEqual("", AutoUpdater.parseHTMLForVersion("random-number-god-win-release.zip>...version_name>", winBuildName));
        Assert.AreEqual("", AutoUpdater.parseHTMLForVersion("random-number-god-win-release.zip>...<version_name>", winBuildName));
    }

}
