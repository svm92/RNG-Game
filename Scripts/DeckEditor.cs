using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Origin = DeckEditorCardObject.Origin;

public class DeckEditor : MonoBehaviour {

    public GameObject cardObj;

    public static DeckEditor deInstance;

    public bool deckView = true; // If false, trade view
    public bool lockModeOn = false;

    List<GameObject> cardPool;
    List<Card> cardsToSell = new List<Card>();

    Transform collectionPanel;
    Transform deckPanel;
    RectTransform collectionPanelRectTr;
    RectTransform deckPanelRectTr;
    Text helpText;
    Text deckLabelText;
    Text expText;
    Vector2 expTextLocalPos;
    Button changeViewButton;
    Text changeViewText;
    GameObject lockButton;
    GameObject sellAllButton;
    GameObject scrollbarVertical;
    Button returnButton;
    Text returnButtonText;

    Camera cam;

	void Awake () {
        collectionPanel = GameObject.FindGameObjectWithTag("CollectionPanel").transform;
        collectionPanelRectTr = collectionPanel.GetComponent<RectTransform>();
        deckPanel = GameObject.FindGameObjectWithTag("DeckPanel").transform;
        deckPanelRectTr = deckPanel.GetComponent<RectTransform>();
        helpText = GameObject.Find("HelpText").GetComponent<Text>();
        deckLabelText = GameObject.Find("DeckLabel").GetComponent<Text>();
        expText = GameObject.Find("ExpLabel").GetComponent<Text>();
        expTextLocalPos = expText.transform.localPosition;
        changeViewButton = GameObject.Find("ChangeViewButton").GetComponent<Button>();
        changeViewText = changeViewButton.GetComponentInChildren<Text>();
        lockButton = GameObject.Find("LockButton");
        sellAllButton = GameObject.Find("SellAllButton");
        returnButton = GameObject.Find("ReturnButton").GetComponent<Button>();
        returnButtonText = returnButton.GetComponentInChildren<Text>();
        scrollbarVertical = GameObject.Find("Scrollbar Vertical");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        deInstance = this;

        GameObject.Find("CollectionLabel").GetComponent<Text>().text = TextScript.get(TextScript.Sentence.COLLECTION);
        changeViewText.text = TextScript.get(TextScript.Sentence.CHANGE_TO_TRADE_VIEW);
        deckLabelText.text = TextScript.get(TextScript.Sentence.DECK);

        expText.gameObject.SetActive(false);
        lockButton.SetActive(false);
        sellAllButton.SetActive(false);

        cardPool = new List<GameObject>();
        for (int i = 0; i < Player.collection.Count + 15 + 1; i++)
        {
            GameObject newCardObj = Instantiate(cardObj);
            newCardObj.SetActive(false);
            cardPool.Add(newCardObj);
        }
    }

    private void Start()
    {
        AudioManager.amInstance.playDeckMusic();
        StartCoroutine(waitThenInitialize());
    }
    
    IEnumerator waitThenInitialize()
    {
        yield return null; // Wait a frame for canvas calculations
        initializeHUD();
        if (Player.keyboardModeOn)
            selectFirstButton();
    }

    private void Update()
    {
        // If left/right/down/up pressed, activate keyboard mode
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !Player.keyboardModeOn)
        {
            Player.keyboardModeOn = true;
            // If not already selecting a card (selecting the scrollbar is allowed)
            if (!EventSystem.current.alreadySelecting || EventSystem.current.gameObject.GetComponent<Scrollbar>() != null)
                selectFirstButton();
        }
    }

    void initializeHUD()
    {
        Player.collection = Card.orderCards(Player.collection);
        Player.deck.cards = Card.orderCards(Player.deck.cards);

        displayCollection();
        displayDeck();

        updateHUD();
    }

    void updateHUDAfterMovement(DeckEditorCardObject movedDeco, Origin newOrigin)
    {
        // Sort
        Player.collection = Card.orderCards(Player.collection);
        if (deckView)
            Player.deck.cards = Card.orderCards(Player.deck.cards);
        else
            cardsToSell = Card.orderCards(cardsToSell);

        // Get origin/index info
        List<Card> newOriginList = (newOrigin == Origin.COLLECTION) ? Player.collection : 
            (deckView) ? Player.deck.cards : cardsToSell;
        int nthPosition = newOriginList.IndexOf(movedDeco.card);

        // Update data for recently moved card with new origin
        movedDeco.initialize(movedDeco.card, newOrigin);
        Transform newPanel = (newOrigin == Origin.COLLECTION) ? collectionPanel : deckPanel;
        movedDeco.transform.SetParent(newPanel);
        movedDeco.transform.SetSiblingIndex(nthPosition);
        movedDeco.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(changeColliderSizeAfterOneFrame(movedDeco.gameObject));

        updateHUD();
    }

    void updateHUD()
    {
        updateCollectionPanelHeight();
        updateDeckPanelHeight();
        orderCollectionAsAGrid();
        orderDeckAsAGrid();
        if (!deckView)
        {
            updateExpText();
            StartCoroutine(updateMiddleButtonsPositionsAfterFrame());
            updateChangeViewButton();
        }
        decideIfReturnButtonIsInteractable();
    }

    void displayCollection()
    {
        displayCardsIn(collectionPanel, Player.collection, Origin.COLLECTION);
    }

    void displayDeck()
    {
        displayCardsIn(deckPanel, Player.deck.cards, Origin.DECK);
    }

    void displayCardsToSell()
    {
        displayCardsIn(deckPanel, cardsToSell, Origin.DECK);
    }

    void displayCardsIn(Transform displayPanel, List<Card> cardsToDisplay, Origin origin)
    {
        foreach (Card card in cardsToDisplay)
        {
            GameObject poolCardObj = getItemFromPool(cardPool);
            poolCardObj.GetComponent<DeckEditorCardObject>().initialize(card, origin);
            poolCardObj.transform.SetParent(displayPanel);
            poolCardObj.GetComponent<RectTransform>().localScale = Vector3.one;
            StartCoroutine(changeColliderSizeAfterOneFrame(poolCardObj));
            poolCardObj.SetActive(true);
        }
    }

    IEnumerator changeColliderSizeAfterOneFrame(GameObject poolCardObj)
    {
        yield return null;
        poolCardObj.GetComponent<BoxCollider2D>().size = poolCardObj.GetComponent<RectTransform>().sizeDelta;
    }

    void hideCardsInDeckPanel()
    {
        foreach (Transform cardTr in deckPanel)
        {
            cardTr.gameObject.SetActive(false);
        }
        deckPanel.DetachChildren();
    }

    GameObject getItemFromPool(List<GameObject> pool)
    {
        foreach (GameObject poolObj in pool)
        {
            if (!poolObj.activeInHierarchy)
                return poolObj;
        }
        Debug.Log("Pool doesn't have enough items. Check getItemFromPool");
        return null;
    }

    void updateCollectionPanelHeight()
    {
        // +160 for each row of cards, plus 20 as margin, minimum 640
        float nRows = Mathf.Ceil(Player.collection.Count / 4f);
        float collectionPanelHeight = nRows * 160f + 20f;
        collectionPanelHeight = Mathf.Max(collectionPanelHeight, 640f);
        if (collectionPanelHeight == 660f) collectionPanelHeight = 640f; // Solve edge case for exactly 4 rows (scrollbar showing up when it's not needed)
        collectionPanelRectTr.sizeDelta = new Vector2(collectionPanelRectTr.sizeDelta.x, collectionPanelHeight);
    }

    void updateDeckPanelHeight()
    {
        // +140 for each row of cards, plus 20 as margin, minimum 440
        int cardsInDeckPanel = (deckView) ? Player.deck.cards.Count : cardsToSell.Count;
        float nRows = Mathf.Ceil(cardsInDeckPanel / 5f);
        float deckPanelHeight = nRows * 140f + 20f;
        deckPanelHeight = Mathf.Max(deckPanelHeight, 440f);
        deckPanelRectTr.sizeDelta = new Vector2(deckPanelRectTr.sizeDelta.x, deckPanelHeight);
    }

    void orderCollectionAsAGrid()
    {
        orderAsAGrid(collectionPanel, 140, 4);
    }

    void orderDeckAsAGrid()
    {
        orderAsAGrid(deckPanel, 120, 5);
    }

    void orderAsAGrid(Transform panel, int cellSize, int nCols)
    {
        for (int i=0; i < panel.childCount; i++)
        {
            int row = (int)Mathf.Floor(i / (float)nCols);
            int col = i % nCols;
            float xPos = 20 + (cellSize/2) + col * (cellSize + 30);
            float yPos = 20 + (cellSize/2) + row * (cellSize + 20);
            Transform child = panel.GetChild(i);
            child.localPosition = new Vector2(xPos, -yPos);
            child.GetComponent<DeckEditorCardObject>().nextDestination = child.localPosition;
            child.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
        }
    }

    void selectFirstButton()
    {
        if (Player.collection.Count > 0)
            selectClosestButton(0, Origin.COLLECTION);
        else
            selectClosestButton(0, Origin.DECK);
    }

    void selectClosestButton(int cardIndex, Origin destination)
    {
        Transform destinationTr = (destination == Origin.COLLECTION) ? collectionPanel : deckPanel;
        if (cardIndex < destinationTr.childCount) // Select next (right) if available
            destinationTr.GetChild(cardIndex).GetComponent<Button>().Select();
        else if (destinationTr.childCount > 0) // Select last element (left) otherwise
            destinationTr.GetChild(destinationTr.childCount - 1).GetComponent<Button>().Select();
        else // If empty, choose the opposite
        {
            if (destination == Origin.COLLECTION)
                deckPanel.GetChild(0).GetComponent<Button>().Select();
            else
                collectionPanel.GetChild(0).GetComponent<Button>().Select();
        }
    }

    public void moveCardBetweenCollectionAndDeck(DeckEditorCardObject deco)
    {
        // Get data
        int oldCardIndex;
        Card card = deco.card;
        Origin oldOrigin = deco.origin;
        bool cardMoved;

        Vector2 oldLocalCardPosition = (oldOrigin == Origin.COLLECTION) ?
            deckPanel.InverseTransformPoint(deco.transform.position) :
            collectionPanel.InverseTransformPoint(deco.transform.position);

        // Move card
        if (oldOrigin == Origin.COLLECTION)
        {
            oldCardIndex = Player.collection.IndexOf(card);
            cardMoved = moveCardFromCollectionToDeck(deco);
        } 
        else
        {
            oldCardIndex = (deckView) ? Player.deck.cards.IndexOf(card) : cardsToSell.IndexOf(card);
            cardMoved = moveCardFromDeckToCollection(deco);
        }

        if (!cardMoved) return;

        StartCoroutine(moveCardAnimation(deco, oldLocalCardPosition));

        if (Player.keyboardModeOn)
            selectClosestButton(oldCardIndex, oldOrigin);
    }

    bool moveCardFromCollectionToDeck(DeckEditorCardObject deco)
    {
        // Forbid if the deck is full, the card is locked,
        // or if this could cause the player to be left with less than 15 cards
        if ((deckView && Player.deck.cards.Count >= 15) || (!deckView && deco.card.isLocked) ||
            (!deckView && (Player.collection.Count + Player.deck.cards.Count <= 15)))
        {
            AudioManager.amInstance.playBuzz();
            return false;
        } else
        {
            if (deckView)
                Player.deck.cards.Add(deco.card);
            else
                cardsToSell.Add(deco.card);
            Player.collection.Remove(deco.card);
            updateHUDAfterMovement(deco, Origin.DECK);
            return true;
        }
    }

    bool moveCardFromDeckToCollection(DeckEditorCardObject deco)
    {
        Player.collection.Add(deco.card);
        if (deckView)
            Player.deck.cards.Remove(deco.card);
        else
            cardsToSell.Remove(deco.card);
        updateHUDAfterMovement(deco, Origin.COLLECTION);
        return true;
    }

    IEnumerator moveCardAnimation(DeckEditorCardObject deco, Vector2 oldLocalCardPosition)
    {
        if (deco.gameObject.GetComponent<Canvas>() != null) // If it is already moving (has a canvas), skip
            yield break;

        Transform cardTr = deco.transform;
        Canvas cardCanvas = deco.gameObject.AddComponent<Canvas>();
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = 10;

        float upperScreenLimit = Screen.height * 0.9f;
        float lowerScreenLimit = (deco.origin == Origin.COLLECTION || deckView) ? 
            Screen.height * 0.2f : Screen.height * 0.4f;

        float speed = 2000f;
        float transitionTime = Vector2.Distance(oldLocalCardPosition, deco.nextDestination) / speed;
        float timer = 0;
        while (timer < transitionTime)
        {
            cardTr.localPosition = Vector3.Lerp(oldLocalCardPosition, deco.nextDestination, timer / transitionTime);
            float cardScreenPosition = RectTransformUtility.WorldToScreenPoint(cam, cardTr.position).y;
            if (cardScreenPosition < lowerScreenLimit || cardScreenPosition > upperScreenLimit) // If not inside the viewport
            {
                Destroy(cardCanvas);
                cardTr.localPosition = deco.nextDestination;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(cardCanvas);
        cardTr.localPosition = deco.nextDestination;
    }

    public void moveAllToSell()
    {
        List<DeckEditorCardObject> decoList = new List<DeckEditorCardObject>();
        foreach (Transform child in collectionPanelRectTr)
        {
            DeckEditorCardObject deco = child.GetComponent<DeckEditorCardObject>();
            if (deco != null && !deco.card.isLocked)
                decoList.Add(deco);
        }
        foreach (DeckEditorCardObject deco in decoList)
        {
            moveCardBetweenCollectionAndDeck(deco);
        }
    }

    public void setLock()
    {
        lockModeOn = !lockModeOn;
        lockButton.GetComponent<Image>().color = (lockModeOn) ? new Color(.5f, .5f, .5f, .5f) : Color.white;
    }

    public void lockCard(DeckEditorCardObject deco)
    {
        deco.card.isLocked = !deco.card.isLocked;
        CardImageManager.ciInstance.updateImage(deco.gameObject, deco.card.cardEffect);
        orderCollectionAsAGrid();
    }

    IEnumerator updateMiddleButtonsPositionsAfterFrame()
    {
        yield return null; // Wait a frame for UI to update
        Transform lockTr = lockButton.transform;
        Transform sellAllTr = sellAllButton.transform;
        if (scrollbarVertical.activeInHierarchy)
        {
            lockTr.localPosition = new Vector3(-10, lockTr.localPosition.y, lockTr.localPosition.z);
            sellAllTr.localPosition = new Vector3(-5, sellAllTr.localPosition.y, sellAllTr.localPosition.z);
        }
        else
        {
            lockTr.localPosition = new Vector3(-40, lockTr.localPosition.y, lockTr.localPosition.z);
            sellAllTr.localPosition = new Vector3(-35, sellAllTr.localPosition.y, sellAllTr.localPosition.z);
        }
        
    }

    public void changeHelpText(string text)
    {
        helpText.text = text;
    }

    public void activateChangeViewButton()
    {
        double totalSellValue = getTotalSellValue();
        if (totalSellValue > 0)
        {
            tradeCards(totalSellValue);
        } else
        {
            changeView();
        }
    }

    void tradeCards(double totalSellValue)
    {
        AudioManager.amInstance.playSellSound();
        expText.GetComponent<Animator>().SetTrigger("Zoom");
        Player.stats[Player.Stat.TOTAL_TRADED_CARDS] += cardsToSell.Count;
        Player.gainExp(totalSellValue);
        cardsToSell.RemoveRange(0, cardsToSell.Count);
        hideCardsInDeckPanel();
        updateHUD();
    }

    void changeView()
    {
        if (deckView) // Deck view -> Trade view
        {
            deckView = false;
            changeViewText.text = TextScript.get(TextScript.Sentence.CHANGE_TO_DECK_VIEW);
            deckLabelText.text = TextScript.get(TextScript.Sentence.TRADE_FOR_EXP);
            deckLabelText.color = new Color(1f, 0.09f, 0);
            deckPanel.GetComponent<Image>().color = new Color(0.784f, 0.392f, 0.392f, 0.294f);
            resetExpTextScaleAndPosition();

            expText.gameObject.SetActive(true);
            updateExpText();

            lockButton.SetActive(true);
            sellAllButton.SetActive(true);
            StartCoroutine(updateMiddleButtonsPositionsAfterFrame());

            hideCardsInDeckPanel();
            displayCardsToSell();
            orderDeckAsAGrid();
        } else // Trade view -> Deck view
        {
            deckView = true;
            changeViewText.text = TextScript.get(TextScript.Sentence.CHANGE_TO_TRADE_VIEW);
            deckLabelText.text = TextScript.get(TextScript.Sentence.DECK);
            deckLabelText.color = new Color(0, 0.09f, 1f);
            deckPanel.GetComponent<Image>().color = new Color(0.392f, 0.392f, 0.784f, 0.294f);

            expText.gameObject.SetActive(false);

            lockButton.SetActive(false);
            sellAllButton.SetActive(false);

            hideCardsInDeckPanel();
            displayDeck();
            orderDeckAsAGrid();
        }
    }

    void updateChangeViewButton()
    {
        double totalSellValue = getTotalSellValue();
        if (totalSellValue > 0)
        {
            if (TextScript.language == TextScript.Language.ENGLISH)
                changeViewText.text = "<size=80>" + TextScript.get(TextScript.Sentence.TRADE) + "</size>";
            else
                changeViewText.text = "<size=65>" + TextScript.get(TextScript.Sentence.TRADE) + "</size>";
            changeViewButton.GetComponent<Image>().color = Color.yellow;
        } else
        {
            changeViewText.text = TextScript.get(TextScript.Sentence.CHANGE_TO_DECK_VIEW);
            changeViewButton.GetComponent<Image>().color = Color.white;
        }
    }

    void updateExpText()
    {
        double totalSellValue = getTotalSellValueToShow();
        expText.text = NumberStringConverter.convert(Player.experience) + " " + TextScript.get(TextScript.Sentence.EXP);
        if (totalSellValue > 0)
        {
            expText.text += "\n<color=#559911>(+" + 
                NumberStringConverter.convert(totalSellValue) + " " + TextScript.get(TextScript.Sentence.EXP) + ")</color>";
        }
    }

    void resetExpTextScaleAndPosition()
    {
        expText.transform.localScale = Vector3.one;
        expText.transform.localPosition = expTextLocalPos;
    }

    double getTotalSellValue()
    {
        double totalSellValue = 0;
        foreach (Card c in cardsToSell)
        {
            totalSellValue += c.cardEffect.cardPoints;
        }
        return totalSellValue;
    }

    double getTotalSellValueToShow()
    {
        double totalSellValue = 0;
        foreach (Card c in cardsToSell)
        {
            totalSellValue += System.Math.Floor(c.cardEffect.cardPoints * RerollManager.getRerollExpMult(Player.rerollPoints));
        }
        return totalSellValue;
    }

    void decideIfReturnButtonIsInteractable()
    {
        if (Player.deck.cards.Count != 15)
        {
            returnButton.interactable = false;
            returnButtonText.text = TextScript.get(TextScript.Sentence.YOUR_DECK_MUST_HAVE_15_CARDS);
            returnButtonText.fontSize = 50;
        }
        else
        {
            returnButton.interactable = true;
            returnButtonText.text = TextScript.get(TextScript.Sentence.RETURN);
            returnButtonText.fontSize = 65;
        }
    }

    public void returnToHub()
    {
        Player.collection.AddRange(cardsToSell); // Recover any cards that weren't sold
        Savegame.save();
        SceneManager.LoadScene("Hub");
    }

}
