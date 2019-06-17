using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    public static bool thereIsANewerVersion = false;
    public static string connectionErrorMessage = "";

    GameObject startGameButton;
    GameObject loadingText;
    GameObject newerVersionContainer;

    Outline[] letterOutline = new Outline[3];
    Vector2[] lettersFinalPos = new Vector2[3] { new Vector2(423, 324), new Vector2(485, 108), new Vector2(530, -107) };

    AsyncOperation ao;

    private void Awake()
    {
        newerVersionContainer = GameObject.Find("NewerVersionContainer");
        if (!thereIsANewerVersion && connectionErrorMessage == "") // If no new version and no error, don't show
        {
            newerVersionContainer.SetActive(false);
        }
        else if (connectionErrorMessage != "") // If error
        {
            Transform tr = newerVersionContainer.transform;
            tr.GetChild(0).GetComponent<Text>().text = connectionErrorMessage;
            tr.GetChild(1).gameObject.SetActive(false);
        }
        else // If there is new version and no error, translate and show
        {
            Transform tr = newerVersionContainer.transform;
            tr.GetChild(0).GetComponent<Text>().text = TextScript.get(TextScript.Sentence.NEW_VERSION_AVAILABLE);
            tr.GetChild(1).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.DOWNLOAD_LINK);
        }

        startGameButton = GameObject.Find("StartGameButton");
        loadingText = GameObject.Find("LoadingText");
        loadingText.SetActive(false);

        Text startGameButtonText = startGameButton.GetComponentInChildren<Text>();
        startGameButtonText.text = TextScript.get(TextScript.Sentence.START);
        if (TextScript.language == TextScript.Language.SPANISH) startGameButtonText.fontSize = 80;

        GameObject.Find("VersionText").GetComponent<Text>().text = "v" + Player.saveVersion;

        letterOutline[0] = GameObject.Find("TitleR").GetComponent<Outline>();
        letterOutline[1] = GameObject.Find("TitleN").GetComponent<Outline>();
        letterOutline[2] = GameObject.Find("TitleG").GetComponent<Outline>();
    }

    private void Start()
    {
        StartCoroutine(animateAllLetters());
        
        ao = SceneManager.LoadSceneAsync("Hub", LoadSceneMode.Single);
        ao.allowSceneActivation = false;

        //debugSimulateFightNEnemies(1, 30);
    }

    void debugSimulateFightNEnemies(int firstId, int lastId)
    {
        for (int i=firstId; i <= lastId; i++)
        {
            // Battle EXP
            double rewardExp = CardFactory.getAbsoluteCardPoints(i);
            rewardExp = System.Math.Floor(rewardExp * Player.expModifier);
            Player.gainExp(rewardExp);

            // Cards
            int nPrizeCards = 3;
            // Extra cards on higher levels
            if (i > 11)
            {
                nPrizeCards++;
            }
            if (i > 20)
            {
                nPrizeCards++;
            }
            if (i > 35)
            {
                nPrizeCards++;
            }
            if (i > 50)
            {
                nPrizeCards++;
            }
            // Apply fortune effect
            float rnd = Random.Range(0f, 1f);
            if (rnd < Player.chanceExtraCard)
            {
                nPrizeCards++;
            }
            Card[] prizeCards = CardFactory.generateNCards(nPrizeCards, i);
            foreach (Card c in prizeCards)
            {
                Player.getNewCard(c);
            }
        }
        HubControl.maxUnlockedDifficultyId = lastId;
        HubControl.currentDifficultyId = lastId;
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            startGame();
        }
    }

    IEnumerator animateAllLetters()
    {
        Color[] randomColors = getThreeRandomDifferentColors();
        yield return new WaitForSeconds(0.95f);
        for (int i=0; i < 3; i++)
            StartCoroutine(moveLetter(letterOutline[i].gameObject, lettersFinalPos[i]));
        yield return new WaitForSeconds(0.75f);
        for (int i = 0; i < 3; i++)
            StartCoroutine(recolorLetter(letterOutline[i], randomColors[i]));
    }

    IEnumerator moveLetter(GameObject letter, Vector2 finalPos)
    {
        // Since the letter objects have their pivot on the left side, move them left half of the canvas width (800)
        finalPos += Vector2.left * 800;

        Vector2 startPos = letter.transform.localPosition;
        float duration = 0.5f;
        float timer = 0;
        while (timer <= duration)
        {
            letter.transform.localPosition = Vector2.Lerp(startPos, finalPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        letter.transform.localPosition = finalPos;
    }

    IEnumerator recolorLetter(Outline letterOutline, Color destinationColor)
    {
        Color originalColor = letterOutline.effectColor;

        float duration = Random.Range(1f, 1.5f);
        float timer = 0;
        while (timer <= duration)
        {
            letterOutline.effectColor = Color.Lerp(originalColor, destinationColor, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(1f, 1.5f));
        StartCoroutine(recolorLetter(letterOutline, differentRandomColor(destinationColor)));
    }

    Color[] getThreeRandomDifferentColors()
    {
        List<Color> possibleColors = new List<Color>() { Color.red, Color.green, Color.blue,
            Color.cyan, Color.magenta, Color.yellow };

        Color colorA = possibleColors[Random.Range(0, possibleColors.Count)];
        possibleColors.Remove(colorA);
        Color colorB = possibleColors[Random.Range(0, possibleColors.Count)];
        possibleColors.Remove(colorB);
        Color colorC = possibleColors[Random.Range(0, possibleColors.Count)];

        return new Color[] { colorA, colorB, colorC };
    }

    Color differentRandomColor(Color color)
    {
        List<Color> possibleColors = new List<Color>() { Color.red, Color.green, Color.blue,
            Color.cyan, Color.magenta, Color.yellow };
        possibleColors.Remove(color);
        return possibleColors[Random.Range(0, possibleColors.Count)];
    }

    public void startGame()
    {
        Destroy(startGameButton);
        loadingText.SetActive(true);
        StartCoroutine(waitUntilSceneLoaded(ao));
    }

    IEnumerator waitUntilSceneLoaded(AsyncOperation ao)
    {
        Text text = loadingText.GetComponent<Text>();
        while (ao.progress <= 0.89f)
        {
            string progress = Mathf.FloorToInt(ao.progress * 100f) + "";
            text.text = TextScript.get(TextScript.Sentence.LOADING) + " (" + progress + "%) ...";
            yield return null;
        }
        ao.allowSceneActivation = true;
    }

    public void linkToUpdatePage()
    {
        string url = "";
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
                url = "https://justacicada.itch.io/random-number-god";
                break;
            case RuntimePlatform.Android:
                url = "https://play.google.com/store/apps/details?id=samuelVazquez.randomNumberGod";
                break;
        }

        if (url != "")
        {
            Application.OpenURL(url);
            Application.Quit();
        }
        
    }

}
