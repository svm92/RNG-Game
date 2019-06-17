using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DialogueTrigger = DialogueManager.DialogueTrigger;

public class BattleManager : MonoBehaviour {

    public GameObject cardObject;
    public GameObject enemyObject;
    public GameObject splashTextObject;

    public static BattleManager bmInstance;

    public Enemy enemy;

    public static bool busy;
    public static bool playerActionAllowed;
    public static bool battleEnded;
    public static bool isTutorial;
    public static double damageDealtToEnemy;

    public static bool speedUpBattleAnimations;
    public static bool skipEnemyDialogue;

    [HideInInspector] public bool skipDrawingEveryThirdTurn;
    [HideInInspector] public bool showingInfoTextPanel;

    double healthDuringLastTurn;
    double nDiceDuringLastTurn;
    bool enemyInteractable;
    
    [HideInInspector] public Deck battleDeck;

    Transform handCanvas;
    [HideInInspector] public Transform hud;
    GameObject infoTextPanel;
    GameObject helpTextPanel;
    GameObject dialoguePanel;
    Text healthText;
    Text diceText;
    Text deckText;
    Text infoText;
    Text helpText;
    Text dialogueText;
    RectTransform healthIcon;
    RectTransform diceIcon;
    RectTransform deckIcon;
    GameObject enemyInfoPanel;
    GameObject victoryPanel;
    GameObject surrenderButton;
    [HideInInspector] public Animator flashImageAnimator;
    [HideInInspector] public Transform tutorialGizmos;
    
    Text[] diceEndModifierTexts; // 0~9

    private void Awake()
    {
        handCanvas = GameObject.FindGameObjectWithTag("HandCanvas").transform;
        hud = GameObject.FindGameObjectWithTag("HUD").transform;
        infoTextPanel = hud.Find("InfoTextPanel").gameObject;
        helpTextPanel = hud.Find("HelpTextPanel").gameObject;
        dialoguePanel = GameObject.Find("DialoguePanel");
        healthText = hud.Find("HealthText").GetComponent<Text>();
        diceText = hud.Find("DiceText").GetComponent<Text>();
        deckText = hud.Find("DeckText").GetComponent<Text>();
        infoText = infoTextPanel.transform.Find("InfoText").GetComponent<Text>();
        helpText = helpTextPanel.transform.Find("HelpText").GetComponent<Text>();
        dialogueText = dialoguePanel.transform.Find("DialogueText").GetComponent<Text>();
        healthIcon = hud.Find("HealthIcon").GetComponent<RectTransform>();
        diceIcon = hud.Find("DiceIcon").GetComponent<RectTransform>();
        deckIcon = hud.Find("DeckIcon").GetComponent<RectTransform>();
        enemyInfoPanel = GameObject.FindGameObjectWithTag("EnemyInfoCanvas").transform.GetChild(0).gameObject;
        victoryPanel = GameObject.FindGameObjectWithTag("VictoryCanvas").transform.GetChild(0).gameObject;
        flashImageAnimator = GameObject.FindGameObjectWithTag("FlashImage").GetComponent<Image>().GetComponent<Animator>();
        if (isTutorial) tutorialGizmos = GameObject.Find("TutorialGizmos").transform;
        surrenderButton = GameObject.Find("GiveUpButton");
        hideInfoTextPanel();

        busy = true;
        playerActionAllowed = false;
        battleEnded = false;
        damageDealtToEnemy = 0;

        skipDrawingEveryThirdTurn = false;

        enemyInteractable = false;
        showingInfoTextPanel = false;
        diceEndModifierTexts = new Text[10]; // 0~9
        Player.currentTurn = 0;
        battleDeck = Player.deck.clone();
        
        Transform diceEndRollTr = GameObject.FindGameObjectWithTag("DiceEndRoll").transform;
        for (int i = 0; i < diceEndModifierTexts.Length; i++)
            diceEndModifierTexts[i] = diceEndRollTr.GetChild(i).GetComponent<Text>();

        if (isTutorial)
            Destroy(surrenderButton);
        else
            surrenderButton.GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.GIVE_UP);

        bmInstance = this;
    }

    private void Start()
    {
        AudioManager.amInstance.playBattleMusic();
        assignEnemy();

        Player.health = (!isTutorial) ? Player.initialHealth : 20;
        healthDuringLastTurn = Player.health;

        Player.diceMultiplier = (!isTutorial) ? Player.firstTurnDiceMultiplier : 1;
        nDiceDuringLastTurn = Player.nDice;

        Hand.hand = new List<GameObject>();
        battleDeck.shuffle();

        changeHelpText("");
        hideDialoguePanel();
        StartCoroutine(waitForEnemyPreparationsThenStartPlayerTurn());
    }

    IEnumerator waitForEnemyPreparationsThenStartPlayerTurn()
    {
        while (!enemy.finishedInitialization)
            yield return null;
        startPlayerTurn();
    }

    private void Update()
    {
        // If left/right pressed, activate keyboard mode (only if there are cards in hand)
        if (Input.GetAxisRaw("Horizontal") != 0 && !Player.keyboardModeOn)
        {
            if (!battleEnded && Hand.hand.Count > 0)
            {
                Player.keyboardModeOn = true;
                if (!EventSystem.current.alreadySelecting)
                    selectFirstCard();
            }
            else if (battleEnded)
            {
                Player.keyboardModeOn = true;
                victoryPanel.GetComponentInChildren<Button>().Select();
            }
        }

        if (Input.GetButtonDown("Submit"))
            setShowingInfoTextPanelToFalse();
    }

    public void startPlayerTurn()
    {
        busy = true;
        Player.currentTurn++;
        removeOldCardEffects();
        applyModEffects();
        applyDelayedMultipliers();

        showHand();
        if (!isTutorial) drawTurnCards();
        checkWhatCardsArePlayable();

        // If not first turn, apply regen
        if (!isTutorial && Player.currentTurn != 1) Player.applyRegen();

        // If not first turn, apply enemy core effects
        if (Player.currentTurn != 1)
        {
            enemy.applyRegen();
            enemy.applyRollIncrease();
            enemy.applyAutoLoseDice();
        }

        updateHUD();
        if (!isTutorial)
        {
            makeEnemyInteractable(true); // Must be before selectFirstCard
            showEnemyDialogue(DialogueTrigger.TURN_START);
        }
        else
        {
            if (Player.currentTurn >= 8)
                makeEnemyInteractable(true);
            StartCoroutine(TutorialManager.doRoutine(Player.currentTurn));
        }  
        selectFirstCard();
        showSurrenderButton();

        busy = false;
        if (!isTutorial) playerActionAllowed = true;
    }

    void drawTurnCards()
    {
        if (Player.currentTurn == 1) // Draw hand in first turn
        {
            drawInitialHand();
        }
        else // Draw single card in other turns
        {
            // Except every 3rd turn if the corresponding bool is active
            if (!(skipDrawingEveryThirdTurn && Player.currentTurn % 3 == 0))
                drawNCards(1);
        }
    }

    public void initiateBattleLogicFlow(CardEffect cardEffect)
    {
        StartCoroutine(battleLogicFlow(cardEffect));
    }

    IEnumerator battleLogicFlow(CardEffect cardEffect)
    {
        // CARD IS CHOSEN
        playerActionAllowed = false;
        makeEnemyInteractable(false);
        hideSurrenderButton();
        hideDialoguePanel();

        // HUD is active, Hand canvas isn't, Info Textbox isn't
        // If the last card had an instant non-delayed effect or a cost, wait for effect to show
        if (cardEffect != null)
        {
            if ((cardEffect.condition == CardEffect.Condition.INSTANT && cardEffect.conditionNumber == 0)
                || cardEffect.cost > 0)
            {
                
                if (isTutorial || !speedUpBattleAnimations)
                {
                    updateHUD();
                    if (cardEffect.cost == 0 || (cardEffect.condition == CardEffect.Condition.INSTANT))
                        AudioManager.amInstance.playPositiveChime();
                    yield return new WaitForSeconds(1.5f);
                } else if (speedUpBattleAnimations && cardEffect.effect == CardEffect.Effect.DICE_MULT 
                    && cardEffect.conditionNumber == 0)
                {
                    updateHUDQuick();
                    yield return new WaitForSeconds(0.75f);
                }
                
                
            }
        }
        hideHUD();

        // ROLL DICE
        // All HUDs are inactive
        busy = true;
        changeHelpText("");
        StartCoroutine(DiceCluster.dcInstance.rollAll());

        yield return new WaitWhile(() => busy);

        Player.diceMultiplier = 1;
        // DEAL DAMAGE TO ENEMY (optional)
        // All HUDs are inactive
        if (!(isTutorial && TutorialManager.freezeScreenAfterRollWithoutAttacking()))
            showHUD();
        if (damageDealtToEnemy > 0)
        {
            AudioManager.amInstance.playAttackChime();
            damageDealtToEnemy = enemy.receiveDamage(damageDealtToEnemy);
            if (!isTutorial)
                showEnemyDialogue(DialogueTrigger.HIT_AGAINST_ENEMY);
            else
            {
                TutorialManager.isCombatLine = true;
                string enemyHurtLine = TutorialManager.getEnemyHurtLine();
                StartCoroutine(TutorialManager.forceEnemyDialogue(enemyHurtLine, 4f));
                TutorialManager.isCombatLine = false;
            }
            if (skipEnemyDialogue)
                prepareWaitForInfoTextPanel(2f);
            else
                prepareWaitForInfoTextPanel(5f);
            yield return new WaitWhile(() => showingInfoTextPanel); // Wait for info text box
            if (enemy.health == 0)
            {
                if (!isTutorial)
                    showEnemyDialogue(DialogueTrigger.VICTORY);
                else
                {
                    TutorialManager.isCombatLine = true;
                    StartCoroutine(TutorialManager.forceEnemyDialogue(TextScript.get(TextScript.Sentence.T85), 5f));
                    TutorialManager.isCombatLine = false;
                }
                StartCoroutine(win());
                yield break;
            }
        }
        else if (diceText.transform.childCount > 0 || healthText.transform.childCount > 0)
        {
            AudioManager.amInstance.playPositiveChime();
            if (isTutorial && TutorialManager.freezeScreenAfterRoll()) yield return new WaitForSeconds(1f);

            if (speedUpBattleAnimations) // Wait if there's some popup text
                yield return new WaitForSeconds(1f);
            else
                yield return new WaitForSeconds(2f);
        }  
        else
            yield return new WaitForSeconds(0.75f); // Wait for screen transition otherwise

        // ENEMY TURN
        // Main HUD active
        if (!TutorialManager.skipEnemyTurn)
        {
            busy = true;
            enemy.decideAttack();
            yield return new WaitWhile(() => busy);
            if (!isTutorial)
                showEnemyDialogue(DialogueTrigger.ENEMY_ATTACK);
            else
            {
                TutorialManager.isCombatLine = true;
                string enemyAttackLine = TutorialManager.getEnemyAttackLine();
                StartCoroutine(TutorialManager.forceEnemyDialogue(enemyAttackLine, 4f));
                TutorialManager.isCombatLine = false;
            }
            yield return new WaitForSeconds(0.5f);
            enemy.playEnemyAttackSound(enemy.lastUsedAttack);
            updateHUD();
            if (skipEnemyDialogue)
                prepareWaitForInfoTextPanel(1.5f);
            else
                prepareWaitForInfoTextPanel(4.5f);
            yield return new WaitWhile(() => showingInfoTextPanel);

            if (Player.health <= 0)
            {
                showEnemyDialogue(DialogueTrigger.LOSE_NO_HEALTH);
                lose("NoHealth");
                yield break;
            }
        }

        if (isTutorial && TutorialManager.freezeScreenAfterRollWithoutEnemyAttack()) yield break;

        // PLAYER TURN
        // Main HUD + info text box active
        hideInfoTextPanel();
        startPlayerTurn();
    }

    public void prepareWaitForInfoTextPanel(float waitTime)
    {
        StopCoroutine("waitForInfoTextPanel");
        showingInfoTextPanel = true;
        if (waitTime != 0) StartCoroutine("waitForInfoTextPanel", waitTime);
    }

    IEnumerator waitForInfoTextPanel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        showingInfoTextPanel = false;
    }

    void drawInitialHand()
    {
        int nCardsToDraw = Mathf.Max(Player.initialHandSize, 0);
        drawNCards(nCardsToDraw);
    }

    void drawNCards(int n)
    {
        for (int i=0; i<n; i++)
        {
            if (battleDeck.cards.Count == 0)
                break;
            Card cardEffect = battleDeck.drawFirstCard();
            GameObject newCardObj = Instantiate(cardObject, Vector3.zero, Quaternion.identity);
            newCardObj.GetComponent<CardObject>().initialize(cardEffect);
            newCardObj.transform.SetParent(handCanvas);
            newCardObj.GetComponent<RectTransform>().localScale = Vector3.one;
            Hand.hand.Add(newCardObj);
        }
        repositionCardsInHand();
    }

    public void forceDrawSpecificCard(Card cardEffect)
    {
        GameObject newCardObj = Instantiate(cardObject, Vector3.zero, Quaternion.identity);
        newCardObj.GetComponent<CardObject>().initialize(cardEffect);
        newCardObj.transform.SetParent(handCanvas);
        newCardObj.GetComponent<RectTransform>().localScale = Vector3.one;
        Hand.hand.Add(newCardObj);
    }

    public static void repositionCardsInHand()
    {
        Vector2[] cardPositions = getCardPositionsInHand();
        for (int i = 0; i < Hand.hand.Count; i++)
        {
            RectTransform rt = Hand.hand[i].GetComponent<RectTransform>();
            rt.anchoredPosition = cardPositions[i];
            // Solve z taking weird values for wide screens
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        }
    }

    public static Vector2[] getCardPositionsInHand()
    {
        int nOfCards = Hand.hand.Count;
        Vector2[] cardPositions = new Vector2[nOfCards];

        float yPos = 75;
        float centralCard = (nOfCards / 2f) + 0.5f; // For 3 cards is 2, for 4 is 2.5, etc.
        // Distance so that 45px out of 1600px are left free on the sides of the screen
        float distanceBetweenCardEdges = Mathf.Infinity;
        if (centralCard != 1)
            distanceBetweenCardEdges = (755f - (centralCard - 0.5f) * CardObject.WIDTH) / (centralCard - 1);
        distanceBetweenCardEdges = Mathf.Min(distanceBetweenCardEdges, 65f); // Set a maximum separation for <5 cards
        float distanceBetweenCards = CardObject.WIDTH + distanceBetweenCardEdges;

        for (int i=0; i < nOfCards; i++)
        {
            float xPos = (i + 1 - centralCard) * distanceBetweenCards;
            cardPositions[i] = new Vector2(xPos, yPos);
        }
        return cardPositions;
    }

    public void selectFirstCard()
    {
        if (Hand.hand.Count > 0)
        {
            foreach (GameObject card in Hand.hand)
            {
                Button b = card.GetComponent<Button>();
                if (b.interactable)
                {
                    // Except on Android, select the button
                    if (!Player.isAndroid() && Player.keyboardModeOn)
                        b.Select();
                    return; // Return in either case
                }
            }
        }
        if (isTutorial) return;
        // If the function reaches here, there are no playable cards in hand
        makeEnemyInteractable(false);
        if (!loseIfNoCardsLeft()) // If you still have cards left in the deck
        {
            showEnemyDialogue(DialogueTrigger.NO_PLAYABLE_CARDS);
            StartCoroutine(autoRollAfterWait());
        }
    }

    IEnumerator autoRollAfterWait()
    {
        if (skipEnemyDialogue)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitForSeconds(3.5f);
        hideHand();

        if (!battleEnded)
            initiateBattleLogicFlow(null);
    }

    void checkWhatCardsArePlayable()
    {
        foreach (GameObject cardObject in Hand.hand)
        {
            double cost = cardObject.GetComponent<CardObject>().card.cardEffect.cost;
            if (cost >= Player.nDice)
                cardObject.GetComponent<Button>().interactable = false;
            else
                cardObject.GetComponent<Button>().interactable = true;
        }
    }

    public void applyCardImproveAdditive(double improvementPoints)
    {
        foreach (GameObject cardGO in Hand.hand)
        {
            CardObject cardObj = cardGO.GetComponent<CardObject>();
            cardObj.card.alterEffect(improvementPoints);
            cardObj.initialize(cardObj.card);
        }
    }

    public void applyCardImproveMultiplicative(float improvementPerc)
    {
        foreach (GameObject cardGO in Hand.hand)
        {
            CardObject cardObj = cardGO.GetComponent<CardObject>();
            cardObj.card.alterEffectPerc(improvementPerc);
            cardObj.initialize(cardObj.card);
        }
    }

    public void applyCardSwap(float improvementPerc)
    {
        foreach (GameObject cardGO in Hand.hand)
        {
            CardObject cardObj = cardGO.GetComponent<CardObject>();
            cardObj.card.swapEffect(improvementPerc);
            cardObj.initialize(cardObj.card);
        }
    }

    void assignEnemy()
    {
        GameObject enemyObj = Instantiate(enemyObject);
        enemyObj.transform.SetParent(hud);
        enemyObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
        enemyObj.GetComponent<RectTransform>().localScale = Vector3.one;
        enemy = enemyObj.GetComponent<Enemy>();
        enemy.initialize(Player.currentEnemyDifficultyId);
    }

    public void hideAll()
    {
        hideHUD();
        hideHand();
    }

    public void hideHUD()
    {
        hud.gameObject.SetActive(false);
    }

    public void showHUD()
    {
        updateHUD();
        if (!hud.gameObject.activeInHierarchy)
        {
            hud.gameObject.SetActive(true);
            flashImageAnimator.SetTrigger("fadeFromBlack");
        }
    }

    void updateHUDTextWithPopup(Text text, double value, RectTransform textIcon, double valueDuringLastTurn)
    {
        updateHUDTextWithoutPopup(text, value, textIcon);
        checkIfCreateSplashText(text, value, valueDuringLastTurn);
    }

    void updateHUDTextWithoutPopup(Text text, double value, RectTransform textIcon)
    {
        if (value < 0) value = 0; // If value is below 0, make it 0
        text.text = NumberStringConverter.convert(value);
        if (textIcon == diceIcon && Player.diceMultiplier != 1)
            text.text += " (x" + NumberStringConverter.convert(Player.diceMultiplier) + ")";
        float textWidth = text.preferredWidth;
        float textX = text.GetComponent<RectTransform>().anchoredPosition.x;
        float iconWidth = textIcon.rect.width;
        float padding = 20;
        float newIconX = textX - (textWidth / 2) - (iconWidth / 2) - padding;
        textIcon.anchoredPosition = new Vector2(newIconX, textIcon.anchoredPosition.y);
    }

    void checkIfCreateSplashText(Text text, double value, double valueDuringLastTurn)
    {
        double addedValue = value - valueDuringLastTurn;
        if (addedValue != 0)
            createSplashTextAt(text.GetComponent<RectTransform>(), addedValue);
    }

    public void updateHUD()
    {
        updateModifiersInHUD();
        updateHUDTextWithPopup(healthText, Player.health, healthIcon, healthDuringLastTurn);
        updateHUDTextWithPopup(diceText, Player.nDice, diceIcon, nDiceDuringLastTurn);
        updateHUDTextWithoutPopup(deckText, battleDeck.getCardsLeft(), deckIcon);
        healthDuringLastTurn = Player.health;
        nDiceDuringLastTurn = Player.nDice;
    }

    public void updateHUDQuick()
    {
        updateModifiersInHUD();
        updateHUDTextWithoutPopup(healthText, Player.health, healthIcon);
        updateHUDTextWithoutPopup(diceText, Player.nDice, diceIcon);
        updateHUDTextWithoutPopup(deckText, battleDeck.getCardsLeft(), deckIcon);
    }

    public void updateModifiersInHUD()
    {
        updateModifiersInHUDFromArray(diceEndModifierTexts, CardEffect.Condition.LAST);
    }

    public static void updateModifiersInHUDFromArray(Text[] modifierTexts, CardEffect.Condition condition)
    {
        foreach (Text t in modifierTexts) t.text = "+0";
        foreach (CardEffect ce in Player.cardEffects)
        {
            if (ce.condition == condition && ce.effect != CardEffect.Effect.DICE_MOD)
            {
                int arrayIndex = ce.conditionNumber;
                double modValue = ce.effectNumber;//double.Parse(modifierTexts[arrayIndex].text) + ce.effectNumber;
                modifierTexts[arrayIndex].text = "+" + NumberStringConverter.convert(modValue);
                Color textColor = (modValue == 0) ? Color.white : (condition == CardEffect.Condition.FIRST) ? 
                    Color.green : Color.blue;
                modifierTexts[arrayIndex].color = textColor;
            }
        }
    }

    public void hideHand()
    {
        handCanvas.gameObject.SetActive(false);
    }

    public void showHand()
    {
        handCanvas.gameObject.SetActive(true);
    }

    public void showSurrenderButton()
    {
        if (surrenderButton != null)
            surrenderButton.SetActive(true);
    }

    public void hideSurrenderButton()
    {
        if (surrenderButton != null)
            surrenderButton.SetActive(false);
    }

    public void hideInfoTextPanel()
    {
        infoTextPanel.SetActive(false);
    }

    public void showInfoTextPanel()
    {
        infoTextPanel.SetActive(true);
    }

    public void hideHelpTextPanel()
    {
        helpTextPanel.SetActive(false);
    }

    public void showHelpTextPanel()
    {
        helpTextPanel.SetActive(true);
    }

    public void changeInfoText(string text)
    {
        infoText.text = text;
    }

    public void changeHelpText(string text)
    {
        if (!helpTextPanel.activeInHierarchy) showHelpTextPanel();
        helpText.text = text;
        if (text == "") hideHelpTextPanel();
    }

    void createSplashTextAt(RectTransform parentTransform, double value)
    {
        GameObject splashText = Instantiate(splashTextObject);
        splashText.transform.SetParent(parentTransform);
        splashText.GetComponent<RectTransform>().localPosition = Vector3.zero;
        splashText.GetComponent<RectTransform>().localScale = Vector3.one;

        string valueSign = (value >= 0) ? "+" : "-";
        Color valueColor = (value >= 0) ? Color.green : Color.red;

        splashText.GetComponent<Text>().text = valueSign + NumberStringConverter.convert(System.Math.Abs(value));
        splashText.GetComponent<Text>().color = valueColor;

        if (speedUpBattleAnimations) splashText.GetComponent<Animator>().speed = 1.5f;
    }

    public void flash()
    {
        flashImageAnimator.SetTrigger("startFlashing");
    }

    void removeOldCardEffects()
    {
        // Remove card effects with an activation condition of "first"
        Player.cardEffects.RemoveAll(ce => (ce.condition == CardEffect.Condition.FIRST));
    }

    void applyModEffects()
    {
        List<CardEffect> cardEffectsToAdd = new List<CardEffect>();
        foreach (CardEffect ce in Player.cardEffects)
            if (ce.effect == CardEffect.Effect.DICE_MOD)
            {
                cardEffectsToAdd.Add(new CardEffect(0, CardEffect.Condition.LAST, ce.conditionNumber,
                    CardEffect.Effect.DICE, ce.effectNumber));
            }

        foreach (CardEffect ce in cardEffectsToAdd)
            Player.addCardEffect(ce);
    }

    void applyDelayedMultipliers()
    {
        foreach (double[] diceMultiplier in Player.delayedMultipliers)
        {
            diceMultiplier[0]--; // Reduce turns until activation by 1
            if (diceMultiplier[0] <= 0)
                Player.diceMultiplier *= diceMultiplier[1];
        }

        Player.delayedMultipliers.RemoveAll(d => (d[0] <= 0));
    }

    public void discard()
    {
        if (noCardsLeftInHand()) return;
        int rnd = Random.Range(0, handCanvas.childCount);
        GameObject cardToDiscard = handCanvas.GetChild(rnd).gameObject;
        Hand.removeCard(cardToDiscard);
        cardToDiscard.transform.SetParent(null);
    }

    public int mill(int n)
    {
        int nMilledCards = 0;
        for (int i=0; i < n; i++)
        {
            if (noCardsLeftInDeck())
            {
                showDeckMillPopup(nMilledCards);
                return nMilledCards;
            }
            int rnd = Random.Range(0, battleDeck.cards.Count);
            battleDeck.cards.RemoveAt(rnd);
            nMilledCards++;
        }
        showDeckMillPopup(nMilledCards);
        return nMilledCards;
    }

    void showDeckMillPopup(int nMilledCards)
    {
        int currentNCards = battleDeck.getCardsLeft();
        updateHUDTextWithPopup(deckText, currentNCards, deckIcon, currentNCards + nMilledCards);
    }

    public void makeEnemyInteractable(bool interactable)
    {
        enemy.GetComponent<Button>().interactable = interactable;
        enemyInteractable = interactable;
    }

    public void showEnemyInfoPanel()
    {
        if (!enemyInteractable) return;

        makeEnemyInteractable(false);
        playerActionAllowed = false;
        handCanvas.gameObject.SetActive(false);
        hideSurrenderButton();
        enemyInfoPanel.SetActive(true);
        enemyInfoPanel.GetComponent<Animator>().SetBool("Show", true);
        Text returnText = enemyInfoPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        returnText.text = TextScript.get(TextScript.Sentence.RETURN);

        if (Player.keyboardModeOn) enemyInfoPanel.GetComponentInChildren<Button>().Select();
    }

    public void hideEnemyInfoPanel()
    {
        playerActionAllowed = true;
        handCanvas.gameObject.SetActive(true);
        showSurrenderButton();
        enemyInfoPanel.GetComponent<Animator>().SetBool("Show", false);

        if (Player.keyboardModeOn) selectFirstCard();
    }

    public void setShowingInfoTextPanelToFalse()
    {
        showingInfoTextPanel = false;
    }

    public void hideDialoguePanel()
    {
        StopCoroutine("showEnemyDialogueAsync");
        dialoguePanel.SetActive(false);
    }

    public void showEnemyDialogue(string dialogue)
    {
        if (!isTutorial && skipEnemyDialogue) return;
        hideDialoguePanel();
        StartCoroutine("showEnemyDialogueAsync", dialogue);
    }

    void showEnemyDialogue(DialogueTrigger dialogueTrigger)
    {
        string dialogue = DialogueManager.getDialogue(dialogueTrigger);
        showEnemyDialogue(dialogue);
    }

    IEnumerator showEnemyDialogueAsync(string dialogue)
    {
        // Initial wait
        if (!isTutorial || TutorialManager.isCombatLine)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.2f);

        dialoguePanel.SetActive(true);

        // Change text
        dialogueText.text = dialogue;

        // Change dialogue panel dimensions
        StartCoroutine(waitAFrameThenRedimensionDialoguePanel());

        // Fade in
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();
        Color panelBaseColor = dialoguePanelImage.color;
        Color panelOriginalColor = new Color(panelBaseColor.r, panelBaseColor.g, panelBaseColor.b, 0);
        Color panelFinalColor = new Color(panelBaseColor.r, panelBaseColor.g, panelBaseColor.b, .88f);

        Color textBaseColor = dialogueText.color;
        Color textOriginalColor = new Color(textBaseColor.r, textBaseColor.g, textBaseColor.b, 0);
        Color textFinalColor = new Color(textBaseColor.r, textBaseColor.g, textBaseColor.b, 1f);

        float transitionTime = 0.35f;
        float timer = 0;
        while (timer <= transitionTime)
        {
            dialoguePanelImage.color = Color.Lerp(panelOriginalColor, panelFinalColor, timer / transitionTime);
            dialogueText.color = Color.Lerp(textOriginalColor, textFinalColor, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        dialoguePanelImage.color = panelFinalColor;

        // Wait
        if (isTutorial) yield break;
        yield return new WaitForSeconds(3.8f);

        // Fade out
        timer = 0;
        while (timer <= transitionTime)
        {
            dialoguePanelImage.color = Color.Lerp(panelFinalColor, panelOriginalColor, timer / transitionTime);
            dialogueText.color = Color.Lerp(textFinalColor, textOriginalColor, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }

        dialoguePanel.SetActive(false);
    }

    public void hideDialoguePanelSlow()
    {
        StartCoroutine(hideDialoguePanelAsync());
    }

    IEnumerator hideDialoguePanelAsync() // Must wait 0.35 seconds after this
    {
        StopCoroutine("showEnemyDialogueAsync");
        // Fade out
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();
        Color panelBaseColor = dialoguePanelImage.color;
        Color panelOriginalColor = new Color(panelBaseColor.r, panelBaseColor.g, panelBaseColor.b, 0);
        Color panelFinalColor = new Color(panelBaseColor.r, panelBaseColor.g, panelBaseColor.b, .88f);

        Color textBaseColor = dialogueText.color;
        Color textOriginalColor = new Color(textBaseColor.r, textBaseColor.g, textBaseColor.b, 0);
        Color textFinalColor = new Color(textBaseColor.r, textBaseColor.g, textBaseColor.b, 1f);

        float transitionTime = 0.35f;
        float timer = 0;
        while (timer <= transitionTime)
        {
            dialoguePanelImage.color = Color.Lerp(panelFinalColor, panelOriginalColor, timer / transitionTime);
            dialogueText.color = Color.Lerp(textFinalColor, textOriginalColor, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        dialoguePanel.SetActive(false);
    }

    IEnumerator waitAFrameThenRedimensionDialoguePanel()
    {
        yield return null; // Needs to wait a frame for canvas calculations
        RectTransform dialoguePanelRt = dialoguePanel.GetComponent<RectTransform>();

        // Change size
        Vector2 dialogueTextDimensions = dialogueText.GetComponent<RectTransform>().sizeDelta;
        float xSize = dialogueTextDimensions.x + 40f;
        float ySize = dialogueTextDimensions.y + 30f;
        dialoguePanelRt.sizeDelta = new Vector2(xSize, ySize);

        // Reposition
        float yPos = dialoguePanelRt.localPosition.y;
        if (xSize <= 400f)
            dialoguePanelRt.localPosition = new Vector2(-220f, yPos);
        else if (xSize <= 550f)
            dialoguePanelRt.localPosition = new Vector2(-185, yPos);
        else
            dialoguePanelRt.localPosition = new Vector2(-95, yPos);
    }

    bool loseIfNoCardsLeft()
    {
        if (noCardsLeft())
        {
            showEnemyDialogue(DialogueTrigger.LOSE_NO_CARDS);
            lose("NoCardsLeft");
            return true;
        }
        return false;
    }

    bool noCardsLeft()
    {
        return (battleDeck.cards.Count == 0 && Hand.hand.Count == 0);
    }

    public bool noCardsLeftInHand()
    {
        return Hand.hand.Count == 0;
    }

    public bool noCardsLeftInDeck()
    {
        return battleDeck.cards.Count == 0;
    }

    enum Result { VICTORY, DEFEAT };

    public IEnumerator win()
    {
        if (!isTutorial) Player.stats[Player.Stat.VICTORIES]++;
        AudioManager.amInstance.playVictoryTune();

        battleEnded = true;
        showHUD();
        showInfoTextPanel();
        
        changeInfoText(TextScript.get(TextScript.Sentence.WIN));
        if (skipEnemyDialogue)
            prepareWaitForInfoTextPanel(2f);
        else
            prepareWaitForInfoTextPanel(5f);
        yield return new WaitWhile(() => showingInfoTextPanel); // Wait for info text box

        showWinLosePanel(new Color(0.6f, 1f, 0.6f), new Color(0.2f, 0.75f, 0.2f), Result.VICTORY);
    }

    public void lose(string defeatReason)
    {
        if (!isTutorial) Player.stats[Player.Stat.DEFEATS]++;
        StartCoroutine(gameOver(defeatReason));
    }

    public void giveUp()
    {
        hideSurrenderButton();
        hideHand();
        makeEnemyInteractable(false);
        hideHelpTextPanel();
        showEnemyDialogue(DialogueTrigger.LOSE_NO_HEALTH);
        lose("GiveUp");
    }

    IEnumerator gameOver(string defeatReason)
    {
        AudioManager.amInstance.playDefeatTune();

        battleEnded = true;
        
        showInfoTextPanel();
        if (defeatReason == "NoCardsLeft")
            changeInfoText(TextScript.get(TextScript.Sentence.LOSE_NO_CARDS));
        else if (defeatReason == "NoHealth")
            changeInfoText(TextScript.get(TextScript.Sentence.LOSE));
        else if (defeatReason == "GiveUp")
            changeInfoText(TextScript.get(TextScript.Sentence.SURRENDER));

        if (skipEnemyDialogue)
            prepareWaitForInfoTextPanel(2f);
        else
            prepareWaitForInfoTextPanel(5f);
        yield return new WaitWhile(() => showingInfoTextPanel); // Wait for info text box

        showWinLosePanel(new Color(1f, 0.6f, 0.6f), new Color(0.6f, 0.1f, 0.1f) , Result.DEFEAT);
    }

    void showWinLosePanel(Color panelColor, Color textColor, Result result)
    {
        // Show panel
        victoryPanel.SetActive(true);
        victoryPanel.GetComponent<Image>().color = panelColor;
        Text victoryText = victoryPanel.GetComponentInChildren<Text>();
        victoryText.text = (result == Result.VICTORY) ? TextScript.get(TextScript.Sentence.VICTORY) 
            : TextScript.get(TextScript.Sentence.DEFEAT);
        victoryText.color = textColor;
        victoryPanel.GetComponentInChildren<Outline>().effectColor = panelColor;
        Text returnText = victoryPanel.GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        returnText.text = TextScript.get(TextScript.Sentence.RETURN);

        // Move help text panel
        RectTransform helpTextPanelrt = helpTextPanel.GetComponent<RectTransform>();
        helpTextPanel.transform.SetParent(victoryPanel.transform);
        helpTextPanelrt.anchoredPosition = new Vector2(50, 0);
        helpText.text = "";
        helpTextPanel.SetActive(true);

        // Add exp
        if (result == Result.VICTORY && !isTutorial)
        {
            double rewardExp = CardFactory.getAbsoluteCardPoints(enemy.difficultyId);
            // Enemy difficulty bonus
            rewardExp = System.Math.Floor(rewardExp * (1 + System.Math.Pow(enemy.difficultyId / 5d, 1.75d)) * Player.expModifier);
            // Arm aspect bonus
            foreach (ArmManager.ArmAspect armAspect in enemy.armAspects)
            {
                switch (armAspect)
                {
                    case ArmManager.ArmAspect.SILVER:
                        rewardExp = System.Math.Floor(rewardExp * 1.25f);
                        break;
                    case ArmManager.ArmAspect.GOLD:
                        rewardExp = System.Math.Floor(rewardExp * 1.5f);
                        break;
                }
            }
            // Add exp to player
            rewardExp = Player.gainExp(rewardExp);
            GameObject.Find("ExpText").GetComponent<Text>().text = "(+" + NumberStringConverter.convert(rewardExp) + 
                " " + TextScript.get(TextScript.Sentence.EXP) + ")";
        }
        else
            GameObject.Find("ExpText").GetComponent<Text>().text = "";

        // Show prize cards
        Card[] prizeCards = getPrizeCards(result, enemy.difficultyId);
        Transform prizeCardPanel = victoryPanel.transform.Find("PrizeCardPanel");
        foreach (Card c in prizeCards)
        {
            Player.getNewCard(c);
            GameObject cardObj = Instantiate(cardObject, Vector3.zero, Quaternion.identity);
            cardObj.GetComponent<CardObject>().initialize(c);
            cardObj.transform.SetParent(prizeCardPanel);
            cardObj.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        GridLayoutGroup glg = prizeCardPanel.GetComponent<GridLayoutGroup>();
        // Alignment
        if (prizeCards.Length <= 4) // Change alignment (1 row -> center it, 2 rows -> one on top of another)
            glg.childAlignment = TextAnchor.MiddleCenter;
        else
            glg.childAlignment = TextAnchor.UpperCenter;

        // Spacing
        if (prizeCards.Length == 2)
            glg.spacing = new Vector2(150, 0);
        else if (prizeCards.Length == 3)
            glg.spacing = new Vector2(50, 0);

        // Deactivate the Grid Layout to avoid reordering when hovering over a card
        StartCoroutine(removeGridLayoutGroupAfterAFrame(glg));

        // Unlock new enemies
        if (result == Result.VICTORY && !isTutorial && enemy.difficultyId == HubControl.maxUnlockedDifficultyId)
        {
            HubControl.maxUnlockedDifficultyId++;
            if (enemy.difficultyId < Player.stats[Player.Stat.GREATEST_RANK])
            {
                HubControl.maxUnlockedDifficultyId += RerollManager.getRerollSkip(Player.rerollPoints); // Skip enemies
                // Only up to the greatest rank ever reached
                if (HubControl.maxUnlockedDifficultyId > Player.stats[Player.Stat.GREATEST_RANK])
                {
                    HubControl.maxUnlockedDifficultyId = (int)Player.stats[Player.Stat.GREATEST_RANK];
                }
            }
            
            // Update greatest rank
            if (HubControl.maxUnlockedDifficultyId > Player.stats[Player.Stat.GREATEST_RANK])
                Player.stats[Player.Stat.GREATEST_RANK] = HubControl.maxUnlockedDifficultyId;

            // Move onwards
            HubControl.currentDifficultyId = HubControl.maxUnlockedDifficultyId;
        }

        // Update defeated silver/gold enemies
        if (result == Result.VICTORY && !isTutorial)
        {
            foreach (ArmManager.ArmAspect aspect in enemy.armAspects)
            {
                if (aspect == ArmManager.ArmAspect.SILVER) Player.stats[Player.Stat.SILVER_DEFEATED]++;
                if (aspect == ArmManager.ArmAspect.GOLD) Player.stats[Player.Stat.GOLD_DEFEATED]++;
            }
        }

        if (Player.keyboardModeOn)
            victoryPanel.GetComponentInChildren<Button>().Select();
    }

    IEnumerator removeGridLayoutGroupAfterAFrame(GridLayoutGroup glg)
    {
        yield return null;
        glg.enabled = false;
    }

    Card[] getPrizeCards(Result result, int enemyId)
    {
        if (isTutorial) return new Card[0];
        // Max 8
        int nPrizeCards = 0;
        if (result == Result.VICTORY) // Victory
        {
            nPrizeCards = 3;
            // Extra cards on higher levels
            if (Player.currentEnemyDifficultyId > 11)
            {
                nPrizeCards++;
            }
            if (Player.currentEnemyDifficultyId > 30)
            {
                nPrizeCards++;
            }
            if (Player.currentEnemyDifficultyId > 73)
            {
                nPrizeCards++;
            }
            if (Player.currentEnemyDifficultyId > 101)
            {
                nPrizeCards++;
            }
            // Apply fortune effect
            float rnd = Random.Range(0f, 1f);
            if (rnd < Player.chanceExtraCard)
            {
                nPrizeCards++;
            }
        }
        else // Defeat
        {
            //% chance of prize card equal to inverse of enemy's health (25% health left becomes 75% chance)
            float prizeChance = 1 - (float)(enemy.health / enemy.maxHealth);
            // Up to 2 prize cards max
            for (int i=0; i < 2; i++)
            {
                float rnd = Random.Range(0f, 1f);
                if (rnd < prizeChance) nPrizeCards++;
            }
            
        }
        Card[] prizeCards = CardFactory.generateNCards(nPrizeCards, enemyId);
        return prizeCards;
    }

    public void returnToHub()
    {
        battleEnded = false;
        Savegame.save();
        SceneManager.LoadScene("Hub");
    }

}
