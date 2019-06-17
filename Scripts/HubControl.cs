using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubControl : MonoBehaviour {

    Button battleButton;
    Button leftArrowButton;
    Button rightArrowButton;
    Transform shopTr;
    GameObject options;
    GameObject rerollButton;

    [HideInInspector] public static bool isAboutToSave = false;

    [HideInInspector] public static int currentDifficultyId = 0;
    [HideInInspector] public static int maxUnlockedDifficultyId = 0;

    bool isPressingArrowButton = false;

    int nTimesPressedDeleteButton = 0;

    private void Awake()
    {
        battleButton = GameObject.Find("BattleButton").GetComponent<Button>();
        leftArrowButton = GameObject.Find("ArrowLeft").GetComponent<Button>();
        rightArrowButton = GameObject.Find("ArrowRight").GetComponent<Button>();
        shopTr = GameObject.Find("ShopButton").transform;
        options = GameObject.Find("OptionsContainer").transform.GetChild(0).gameObject;
        rerollButton = GameObject.Find("RerollButton");

        Player.restoreValuesToDefault();
        BattleManager.isTutorial = false;
        Dice.applyGlobalAnimSetting();

        updateTextInButtons();

        if (!isRerollUnlocked()) Destroy(rerollButton);

        /*for (int i = 0; i < 300; i++) {
            int enemyId = Random.Range(100, 101);
            Card randomCard = CardFactory.generateRandomCard(enemyId);
            Player.collection.Add(randomCard);
        }*/

        /*foreach (KeyValuePair<Player.Stat, double> statPair in Player.stats)
        {
            Debug.Log(statPair.Key + ": " + statPair.Value);
        }*/
    }

    private void Start()
    {
        AudioManager.amInstance.playMainMenuMusic();

        if (isAboutToSave)
        {
            isAboutToSave = false; // Save text autodestroys
            // Push message up if reroll button is unlocked
            if (isRerollUnlocked())
                GameObject.Find("SaveText").GetComponent<RectTransform>().localPosition += Vector3.up * 175;
        } 
        else
            Destroy(GameObject.Find("SaveText"));

        checkIfMarkShopAsNew();
        if (isRerollUnlocked()) checkIfMarkRerollAsNew();

        if (Application.platform != RuntimePlatform.Android && Player.keyboardModeOn)
            battleButton.Select();
    }

    private void Update()
    {
        // If left/right pressed while selecting battle button, scroll appropriately
        if (!isPressingArrowButton && Player.keyboardModeOn)
        {
            if (Input.GetAxisRaw("Horizontal") < 0 && leftArrowButton.interactable &&
                EventSystem.current.currentSelectedGameObject == battleButton.gameObject)
            {
                isPressingArrowButton = true;
                leftArrowButton.onClick.Invoke();
                leftArrowButton.GetComponent<Animator>().SetBool("Pressed", true);
                StartCoroutine(unpressArrowAfterWait(leftArrowButton));
            } 
            else if (Input.GetAxisRaw("Horizontal") > 0 && rightArrowButton.interactable &&
                EventSystem.current.currentSelectedGameObject == battleButton.gameObject)
            {
                isPressingArrowButton = true;
                rightArrowButton.onClick.Invoke();
                rightArrowButton.GetComponent<Animator>().SetBool("Pressed", true);
                StartCoroutine(unpressArrowAfterWait(rightArrowButton));
            }
        }

        // Allow pressing arrows again once horizontal axis is set to neutral
        if (isPressingArrowButton && Input.GetAxisRaw("Horizontal") == 0)
            isPressingArrowButton = false;

        // If left/right/up/down pressed, activate keyboard mode
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !Player.keyboardModeOn)
        {
            Player.keyboardModeOn = true;

            battleButton.Select();

            isPressingArrowButton = true;
        }

        // If mouse clicked, deactivate keyboard mode
        if (Input.GetMouseButtonDown(0))
            Player.keyboardModeOn = false;
    }

    IEnumerator unpressArrowAfterWait(Button arrowButton)
    {
        yield return new WaitForSeconds(0.5f);
        arrowButton.GetComponent<Animator>().SetBool("Pressed", false);
    }

    void updateTextInButtons()
    {
        updateBattleButtons();

        GameObject.Find("TutorialButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.TUTORIAL);
        GameObject.Find("DeckEditorButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.DECK_EDITOR);
        GameObject.Find("ShopButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.FORTUNES);
        GameObject.Find("ExitButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.QUIT);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.OPTIONS);
        if (GameObject.Find("RerollButton") != null)
        {
            GameObject.Find("RerollButton").GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.REROLL);
        }

        Transform optionsPanel = options.transform.GetChild(1);
        Transform creditsPanel = options.transform.GetChild(2).GetChild(0).GetChild(1);
        Transform deleteSavePanel = options.transform.GetChild(3).GetChild(0).GetChild(1);
        optionsPanel.GetChild(0).GetChild(1).GetComponent<Text>().text = TextScript.get(TextScript.Sentence.MUTE);
        optionsPanel.GetChild(1).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.CREDITS);
        optionsPanel.GetChild(2).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.RETURN);
        optionsPanel.GetChild(7).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.SKIP_ANIM);
        optionsPanel.GetChild(8).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.DELETE_SAVE);
        optionsPanel.GetChild(9).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.SKIP_ENEMY_DIALOGUE);
        optionsPanel.GetChild(10).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.CHECK_UPDATES_ON_START);
        creditsPanel.GetChild(0).GetComponent<Text>().text = TextScript.get(TextScript.Sentence.CREDITS_CONTENT);
        creditsPanel.GetChild(1).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.RETURN);
        if (Player.isAndroid())
            deleteSavePanel.GetChild(0).GetComponent<Text>().text = TextScript.get(TextScript.Sentence.DELETE_WARNING_TAP);
        else
            deleteSavePanel.GetChild(0).GetComponent<Text>().text = TextScript.get(TextScript.Sentence.DELETE_WARNING_CLICK);
        deleteSavePanel.GetChild(1).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.RETURN);
        deleteSavePanel.GetChild(2).GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.DELETE_BUTTON);

        GameObject saveText = GameObject.Find("SaveText"); // Can be destroyed (null)
        if (saveText != null)
        {
            saveText.GetComponent<Text>().text = TextScript.get(TextScript.Sentence.SAVING_TEXT);
        }
    }

    public void goToDeckEditor()
    {
        SceneManager.LoadScene("DeckEditor");
    }

    public void goToBattle()
    {
        Player.stats[Player.Stat.BATTLES]++;
        decideBattleParameters();
        BattleManager.isTutorial = false;
        SceneManager.LoadScene("Battle");
    }

    void decideBattleParameters()
    {
        Player.currentEnemyDifficultyId = currentDifficultyId;
        int mod = currentDifficultyId % 5; // Cycling value between 0~4
        switch (mod)
        {
            case 0:
                Player.attackRolls = new List<int> { 1, 2, 3, 4, 5 };
                break;
            case 1:
                Player.attackRolls = new List<int> { 1, 2, 3, 4 };
                break;
            case 2:
                Player.attackRolls = new List<int> { 1, 2, 3 };
                break;
            case 3:
                Player.attackRolls = new List<int> { 1, 2 };
                break;
            case 4:
                Player.attackRolls = new List<int> { 1 };
                break;
        }
    }

    public void cycleBattleButtonLeft()
    {
        if (Player.keyboardModeOn && EventSystem.current.currentSelectedGameObject != battleButton.gameObject)
            return;

        if (currentDifficultyId == 0)
        {
            AudioManager.amInstance.playBuzz();
        } else
        {
            currentDifficultyId--;
            updateBattleButtons();
        }
    }

    public void cycleBattleButtonRight()
    {
        if (Player.keyboardModeOn && EventSystem.current.currentSelectedGameObject != battleButton.gameObject)
            return;

        if (currentDifficultyId == maxUnlockedDifficultyId)
        {
            AudioManager.amInstance.playBuzz();
        }
        else
        {
            currentDifficultyId++;
            updateBattleButtons();
        }
    }

    void updateBattleButtons()
    {
        updateMainBattleButton();
        updateArrowButtons();
    }

    void updateMainBattleButton()
    {
        Text battleButtonText = battleButton.GetComponentInChildren<Text>();
        battleButtonText.text = TextScript.get(TextScript.Sentence.RANK) + " " + (currentDifficultyId+1)  + "\n\n";
        int mod = currentDifficultyId % 5; // Cycling value between 0~4
        switch (mod)
        {
            case 0:
                battleButtonText.text += TextScript.get(TextScript.Sentence.ATTACK_ROLLS) + ": 1~5";
                break;
            case 1:
                battleButtonText.text += TextScript.get(TextScript.Sentence.ATTACK_ROLLS) + ": 1~4";
                break;
            case 2:
                battleButtonText.text += TextScript.get(TextScript.Sentence.ATTACK_ROLLS) + ": 1~3";
                break;
            case 3:
                battleButtonText.text += TextScript.get(TextScript.Sentence.ATTACK_ROLLS) + ": 1~2";
                break;
            case 4:
                battleButtonText.text += TextScript.get(TextScript.Sentence.ATTACK_ROLL) + ": 1";
                break;
        }
    }

    void updateArrowButtons()
    {
        // Left arrow
        if (currentDifficultyId == 0)
            leftArrowButton.interactable = false;
        else
            leftArrowButton.interactable = true;
        // Right arrow
        if (currentDifficultyId == maxUnlockedDifficultyId)
            rightArrowButton.interactable = false;
        else
            rightArrowButton.interactable = true;
    }

    public void goToTutorialBattle()
    {
        Player.currentEnemyDifficultyId = 0;
        Player.attackRolls = new List<int> { 1, 2, 3, 4, 5 };
        BattleManager.isTutorial = true;

        Dice.minPreRollTime = 1f;
        DiceCluster.skipPostAttackAnimation = false;
        BattleManager.speedUpBattleAnimations = false;

        SceneManager.LoadScene("Battle");
    }

    public void goToShop()
    {
        SceneManager.LoadScene("Shop");
    }

    void checkIfMarkShopAsNew()
    {
        FortuneManager.updateAvailableFortunes();
        if (FortuneManager.checkIfAnyFortuneIsNew())
            shopTr.GetChild(1).gameObject.SetActive(true);
    }

    public void goToReroll()
    {
        SceneManager.LoadScene("Reroll");
    }

    static bool isRerollUnlocked()
    {
        return maxUnlockedDifficultyId >= 50;
    }

    void checkIfMarkRerollAsNew()
    {
        if (maxUnlockedDifficultyId == 49 && !RerollManager.alreadyVisitedSinceLoading)
            rerollButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void goToOptions()
    {
        options.SetActive(true);
        if (Player.keyboardModeOn)
        {
            GameObject.Find("OptionsReturnButton").GetComponent<Button>().Select();
        }
        updateToggles();
    }

    void updateToggles()
    {
        // Check/Uncheck mute button
        GameObject.Find("MuteToggle").GetComponent<Toggle>().isOn = AudioManager.globlalMute;
        // Check/Uncheck anim button
        GameObject.Find("AnimToggle").GetComponent<Toggle>().isOn = Dice.skipAnim;
        // Check/Uncheck enemy dialogue button
        GameObject.Find("EnemyDialogueToggle").GetComponent<Toggle>().isOn = BattleManager.skipEnemyDialogue;
        // Check/Uncheck update on start
        GameObject.Find("CheckForUpdatesToggle").GetComponent<Toggle>().isOn = Player.checkForUpdatesOnStart;
        // Check/Uncheck flag buttons
        GameObject.Find("FlagToggleEN").GetComponent<Toggle>().isOn = (TextScript.language == TextScript.Language.ENGLISH);
        GameObject.Find("FlagToggleES").GetComponent<Toggle>().isOn = (TextScript.language == TextScript.Language.SPANISH);
    }

    public void returnFromOptions()
    {
        options.SetActive(false);
        if (Player.keyboardModeOn)
        {
            battleButton.Select();
        }
    }

    public void openCredits()
    {
        GameObject credits = GameObject.Find("CreditsContainer").transform.GetChild(0).gameObject;
        credits.SetActive(true);
        if (Player.keyboardModeOn)
        {
            GameObject.Find("CreditsReturnButton").GetComponent<Button>().Select();
        }
    }

    public void returnFromCredits()
    {
        GameObject credits = GameObject.Find("CreditsContainer").transform.GetChild(0).gameObject;
        credits.SetActive(false);
        if (Player.keyboardModeOn)
        {
            GameObject.Find("OptionsReturnButton").GetComponent<Button>().Select();
        }
    }

    public void openDeleteSave()
    {
        GameObject deleteSave = GameObject.Find("DeleteSaveContainer").transform.GetChild(0).gameObject;
        deleteSave.SetActive(true);
        Text deleteText = GameObject.Find("FinalDeleteButton").GetComponentInChildren<Text>();
        deleteText.text = TextScript.get(TextScript.Sentence.DELETE_BUTTON);
        if (Player.keyboardModeOn)
        {
            GameObject.Find("DeleteSaveReturnButton").GetComponent<Button>().Select();
        }
    }

    public void returnFromDeleteSave()
    {
        GameObject deleteSave = GameObject.Find("DeleteSaveContainer").transform.GetChild(0).gameObject;
        deleteSave.SetActive(false);
        nTimesPressedDeleteButton = 0;
        if (Player.keyboardModeOn)
        {
            GameObject.Find("OptionsReturnButton").GetComponent<Button>().Select();
        }
    }

    public void checkIfDeleteSavefile()
    {
        nTimesPressedDeleteButton++;
        Text deleteText = GameObject.Find("FinalDeleteButton").GetComponentInChildren<Text>();
        deleteText.text = TextScript.get(TextScript.Sentence.DELETE_BUTTON) + " (" + nTimesPressedDeleteButton + ")";

        if (nTimesPressedDeleteButton >= 7)
        {
            Savegame.deleteSaveFile();
        }
    }

    public void setGlobalMute(bool toggle)
    {
        AudioManager.globlalMute = toggle;
        AudioManager.amInstance.applyGlobalMuteSetting();
    }

    public void setGlobalAnim(bool toggle)
    {
        Dice.skipAnim = toggle;
        Dice.applyGlobalAnimSetting();
    }

    public void setSkipEnemyDialogue(bool toggle)
    {
        BattleManager.skipEnemyDialogue = toggle;
    }

    public void setCheckForUpdatesOnStart(bool toggle)
    {
        Player.checkForUpdatesOnStart = toggle;
    }

    public void setFlagUS(bool toggle)
    {
        if (toggle)
        {
            TextScript.language = TextScript.Language.ENGLISH;
            updateTextInButtons();
        }
        updateToggles();
    }

    public void setFlagES(bool toggle)
    {
        if (toggle)
        {
            TextScript.language = TextScript.Language.SPANISH;
            updateTextInButtons();
        }
        updateToggles();
    }

    public void quitGame()
    {
        Application.Quit();
    }

    void addDebugCollection()
    {
        // Random fodder
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 6, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 5, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 4, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 3, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 2, CardEffect.Effect.DICE, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 1, CardEffect.Effect.DICE, 2)));

        // Examples of types of cards
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT, 1, CardEffect.Effect.DICE_MULT, 2)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT, 2, CardEffect.Effect.DICE_MULT, 3)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE_MULT, 3, 50)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE_MOD, 1)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FIXED_DICE, 42)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.FORBID_NUMBER, 9)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.ADD_ATTACK_ROLL, 42)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.LOWER_MAX, 100)));
        Player.collection.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT_PERMANENT, 0, CardEffect.Effect.IMPROVE, 100)));
        // Card that returns next/last used card to deck/hand
        // Card that shuffles all used cards into deck (for high dice cost?)
        // Dice <-> Health transfusion
    }

}
