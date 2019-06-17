using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    public GameObject fortunePrefab;

    const int FORTUNE_HEIGHT = 170;

    RectTransform fortuneContainer;
    Text expText;
    Button returnButton;

    List<GameObject> fortuneObjPool = new List<GameObject>();

    public static Shop shopInstance;

    private void Awake()
    {
        fortuneContainer = GameObject.Find("FortuneContainer").GetComponent<RectTransform>();
        expText = GameObject.Find("ExpText").GetComponent<Text>();
        returnButton = GameObject.Find("ReturnButton").GetComponent<Button>();

        returnButton.GetComponentInChildren<Text>().text = TextScript.get(TextScript.Sentence.RETURN);
        GameObject.Find("Label").GetComponent<Text>().text = TextScript.get(TextScript.Sentence.FORTUNES_UPPERCASE);

        shopInstance = this;
    }

    private void Start()
    {
        AudioManager.amInstance.playShopMusic();
        instantiateFortunePool();
        updateFortunes();
        // Scroll to top
        GameObject.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;

        if (Player.keyboardModeOn) selectFirstButton();
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

    void instantiateFortunePool()
    {
        foreach (Fortune fortune in FortuneManager.allFortunes)
        {
            GameObject fortuneObj = Instantiate(fortunePrefab);
            Transform fortuneTr = fortuneObj.transform;
            fortuneTr.SetParent(fortuneContainer);
            fortuneTr.localScale = Vector3.one;
            if (Player.isAndroid() || (float)Screen.width / Screen.height < 1.33f)
                fortuneTr.GetComponentInChildren<Text>().fontSize = 40;
            fortuneObj.GetComponent<FortuneObject>().fortune = fortune;
            fortuneObjPool.Add(fortuneObj);
        }
    }

    public void updateFortunes()
    {
        FortuneManager.updateAvailableFortunes();
        updateFortuneInfo();
        updateFortuneContainerSize();
        updatePlayerExperience();
        updateKeyboardNavigation();
    }

    void updateFortuneInfo()
    {
        foreach (GameObject fortuneObj in fortuneObjPool)
        {
            Fortune fortune = fortuneObj.GetComponent<FortuneObject>().fortune;
            if (fortune.isAvailable)
            {
                fortune.updatePrice(); // Update price
                fortuneObj.GetComponentInChildren<Text>().text = fortune.getDescription(); // Update description

                fortuneObj.SetActive(true);
                // Make buyable/unbuyable
                bool isBuyable = (Player.experience >= fortune.priceToNextLevel);
                fortuneObj.GetComponent<Button>().interactable = isBuyable;

                // Show/Hide 'NEW' icon
                GameObject newText = fortuneObj.transform.GetChild(1).gameObject;
                newText.SetActive(fortune.isNew);
            }
        }
    }

    void updateFortuneContainerSize()
    {
        // 20 from top plus (fortuneHeight + 10 of spacing) for each fortune plus 20 from lower area
        int nOfFortunes = FortuneManager.getNOfAvailableFortunes();
        float ySize = 20 + (FORTUNE_HEIGHT + 10) * nOfFortunes + 20;
        if (ySize < 600) ySize = 600; // Minimum size
        fortuneContainer.sizeDelta = new Vector2(fortuneContainer.sizeDelta.x, ySize);
    }

    void updatePlayerExperience()
    {
        expText.text = NumberStringConverter.convert(Player.experience) + " " + TextScript.get(TextScript.Sentence.EXP);
    }

    Button getFirstAvailableFortuneButton()
    {
        foreach (Transform fortuneTr in fortuneContainer.transform) // Select first available fortune object
        {
            Button fortuneButton = fortuneTr.GetComponent<Button>();
            if (fortuneButton.gameObject.activeInHierarchy && fortuneButton.interactable)
            {
                return fortuneButton;
            }
        }
        return null;
    }

    Button getLastAvailableFortuneButton()
    {
        for (int i = fortuneContainer.childCount - 1; i >= 0; i--) // Select last available fortune object
        {
            Button fortuneButton = fortuneContainer.GetChild(i).GetComponent<Button>();
            if (fortuneButton.gameObject.activeInHierarchy && fortuneButton.interactable)
            {
                return fortuneButton;
            }
        }
        return null;
    }

    public void selectFirstButton()
    {
        Button firstAvailableButton = getFirstAvailableFortuneButton();
        if (firstAvailableButton != null)
        {
            firstAvailableButton.Select();
            return;
        }
        // If no fortune available, select return button by default
        returnButton.Select();
    }

    void updateKeyboardNavigation()
    {
        updateReturnButtonKeyboardNavigation();
        updateAllFortunesKeyboardNavigation();
    }

    void updateReturnButtonKeyboardNavigation() // Make return button connect to first (DOWN) and last (UP) fortunes
    {
        Navigation nav = returnButton.navigation;

        nav.selectOnUp = getLastAvailableFortuneButton();
        nav.selectOnDown = getFirstAvailableFortuneButton();

        returnButton.navigation = nav;
    }

    void updateAllFortunesKeyboardNavigation() // Make fortunes connect between themselves and the return button
    {
        foreach (Transform fortuneTr in fortuneContainer)
        {
            Button fortuneButton = fortuneTr.GetComponent<Button>();
            if (fortuneButton.gameObject.activeInHierarchy && fortuneButton.interactable)
                updateFortuneKeyboardNavigation(fortuneButton);
        }
    }

    void updateFortuneKeyboardNavigation(Button fortuneButton)
    {
        Navigation nav = fortuneButton.navigation;

        nav.selectOnUp = getPreviousFortune(fortuneButton);
        nav.selectOnDown = getNextFortune(fortuneButton);

        // If there is no button up or down, make it connect to the return button
        if (nav.selectOnUp == null) nav.selectOnUp = returnButton;
        if (nav.selectOnDown == null) nav.selectOnDown = returnButton;

        fortuneButton.navigation = nav;
    }

    Button getNextFortune(Button fortuneButton)
    {
        int index = fortuneButton.transform.GetSiblingIndex();
        for (int i=index+1; i < fortuneContainer.childCount; i++) // Starting at next index, search for an interactable button
        {
            Button nextFortuneButton = fortuneContainer.GetChild(i).GetComponent<Button>();
            if (nextFortuneButton.gameObject.activeInHierarchy && nextFortuneButton.interactable)
            {
                return nextFortuneButton;
            }
        }
        return null;
    }

    Button getPreviousFortune(Button fortuneButton)
    {
        int index = fortuneButton.transform.GetSiblingIndex();
        for (int i = index - 1; i >= 0; i--) // Starting at previous index, search for an interactable button
        {
            Button previousFortuneButton = fortuneContainer.GetChild(i).GetComponent<Button>();
            if (previousFortuneButton.gameObject.activeInHierarchy && previousFortuneButton.interactable)
            {
                return previousFortuneButton;
            }
        }
        return null;
    }

    public void returnToHub()
    {
        FortuneManager.markAllFortunesAsSeen();
        Savegame.save();
        SceneManager.LoadScene("Hub");
    }

}
