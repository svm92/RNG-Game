using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageChoiceManager : MonoBehaviour {

    AsyncOperation ao;

    private void Awake()
    {
        if (Player.isAndroid()) // Make flags appear non-faded on Android
        {
            GameObject flagEN = GameObject.Find("FlagEN");
            GameObject flagES = GameObject.Find("FlagES");
            GameObject[] flags = new GameObject[] { flagEN, flagES };
            foreach (GameObject flag in flags)
            {
                flag.GetComponent<Button>().transition = Selectable.Transition.None;
                flag.GetComponent<Animator>().enabled = false;
                flag.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private IEnumerator Start()
    {
        // If there is a save file
        if (Savegame.checkIfSaveExists())
        {
            Savegame.load();

            yield return null;

            if (Player.checkForUpdatesOnStart) yield return checkIfLastVersion();

            yield return null;

            SceneManager.LoadScene("TitleScreen");
        }
        else // If no save file
        {
            Destroy(GameObject.Find("BlankImage"));
            ao = SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Single);
            ao.allowSceneActivation = false;
        }
    }

    IEnumerator checkIfLastVersion()
    {
        string url = "";
        string buildName = "";

        switch (Application.platform) // Detect platform in order to fetch appropriate update
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                url = "https://justacicada.itch.io/random-number-god";
                buildName = "random-number-god-win-release.zip";
                break;
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.Android:
                url = "https://justacicada.itch.io/random-number-god";
                buildName = "random-number-god-linux-release.zip";
                break;
        }

        if (url == "" || buildName == "") yield break; // If there's any issue, abort

        using (WWW www = new WWW(url))
        {
            yield return www; // Wait until it loads
            string pageHTML = www.text;
            string newestVersion = AutoUpdater.parseHTMLForVersion(pageHTML, buildName);

            if (www.error != null && www.error != "") // There's some error
            {
                TitleManager.connectionErrorMessage = www.error;
            }
            else if (newestVersion == "") // If it cannot parse for whatever reason
            {
                TitleManager.connectionErrorMessage = "Update checking failed";
            }
            else if (!AutoUpdater.isNewestVersion(newestVersion)) // There is a newer version
            {
                TitleManager.thereIsANewerVersion = true;
            }
            // Otherwise we're up to date, so do nothing
        }
    }

    private void Update()
    {
        if ((Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0) && !Player.keyboardModeOn)
        {
            Player.keyboardModeOn = true;
            GameObject.Find("FlagEN").GetComponent<Button>().Select();
        }
    }

    public void setFlagUS()
    {
        TextScript.language = TextScript.Language.ENGLISH;
        StartCoroutine(waitUntilSceneLoaded(ao));
    }

    public void setFlagES()
    {
        TextScript.language = TextScript.Language.SPANISH;
        StartCoroutine(waitUntilSceneLoaded(ao));
    }

    IEnumerator waitUntilSceneLoaded(AsyncOperation ao)
    {
        while (ao.progress <= 0.89f)
        {
            yield return null;
        }
        ao.allowSceneActivation = true;
    }

}
