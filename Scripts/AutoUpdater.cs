using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoUpdater {

	public static string parseHTMLForVersion(string pageHTML, string buildName)
    {
        // Find first instance of build name, find first instance of "version_name" after that, then get the number
        
        // Find part of the HTML that mentions the build name
        int buildIndex = pageHTML.IndexOf(buildName);
        if (buildIndex < 0) return ""; // Error check
        pageHTML = pageHTML.Substring(buildIndex); //random-number-god-win-release.zip" class="name">...
        
        // Find part of the HTML that mentions the version name
        int versionIndex = pageHTML.IndexOf("version_name");
        if (versionIndex < 0) return ""; // Error check
        pageHTML = pageHTML.Substring(versionIndex); //version_name">Version 1.1.2</span>...

        if (pageHTML.Split('>').Length < 2) return ""; // Error check
        pageHTML = pageHTML.Split('>')[1]; //Version 1.1.2</span>...

        if (pageHTML.Split('<').Length < 1) return ""; // Error check
        pageHTML = pageHTML.Split('<')[0]; //Version 1.1.2

        if (pageHTML.Length < 8) return "";
        string newestVersion = pageHTML.Substring(8); //1.1.2
        return newestVersion;
    }

    public static bool isNewestVersion(string newestVersion)
    {
        return (Player.saveVersion == newestVersion);
    }

}
