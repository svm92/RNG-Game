using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardImageManager : MonoBehaviour {

    public static CardImageManager ciInstance;

    public GameObject cardObjectTemplate;
    public GameObject standardModel;
    public GameObject standardCompactModel;
    public GameObject adderModel;
    public GameObject adderCompactModel;
    public GameObject adderCompacterModel;
    public GameObject delayedModel;
    public GameObject delayedCompactModel;
    public GameObject singleDiceModel;
    public GameObject forbidModel;
    public GameObject singleValueModel;
    public GameObject lowerMaxModel;

    public Sprite greenBackground;

    public Sprite diceCondition;
    public Sprite clock;

    public Sprite diceEffect;
    public Sprite heartEffect;
    public Sprite multiDiceEffect;
    public Sprite forbidEffect;
    public Sprite attackEffect;
    public Sprite pentagonEffect;

    public Sprite plusSign;
    public Sprite multiplicationSign;

    public GameObject costLabel;
    public GameObject lockIcon;

    static float templateHalfHeight = 0;
    static bool inDeckEditor;

    enum Model { STANDARD, STANDARD_COMPACT, ADDER, ADDER_COMPACT, ADDER_COMPACTER,
        DELAYED, DELAYED_COMPACT, SINGLE_DICE, FORBID, SINGLE_VALUE, LOWER_MAX };

    private void Awake()
    {
        ciInstance = this;
    }

    private void Start()
    {
        inDeckEditor = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DeckEditor");

        if (templateHalfHeight == 0)
            templateHalfHeight = cardObjectTemplate.GetComponent<RectTransform>().rect.height / 2;
    }

    public void updateImage(GameObject cardObj, CardEffect cardEffect)
    {
        // Delete old model
        foreach (Transform child in cardObj.transform)
            Destroy(child.gameObject);
        cardObj.transform.DetachChildren();

        // Get the card's model
        Model cardModel = getCardImageModel(cardEffect);
        GameObject modelObjPrefab = getCardModelObject(cardModel);
        GameObject modelObj = Instantiate(modelObjPrefab);
        modelObj.transform.SetParent(cardObj.transform);

        // Position/scale the model
        RectTransform modelRt = modelObj.GetComponent<RectTransform>();
        modelRt.localScale = Vector3.one;
        modelRt.localPosition = modelObjPrefab.GetComponent<RectTransform>().localPosition;
        modelRt.localPosition += Vector3.up * templateHalfHeight; // Push up icons (since cards are south-aligned)

        Image[] images = cardObj.GetComponentsInChildren<Image>();
        Text[] texts = cardObj.GetComponentsInChildren<Text>();
        // Fill in the images for that model
        switch (cardModel)
        {
            case Model.STANDARD:
            case Model.STANDARD_COMPACT:
            case Model.DELAYED:
            case Model.DELAYED_COMPACT:
                // 0 - Background, 1 - Condition, 2 - Effect , 3 - Effect sign; 0 - Condition digit, 1 - Effect digit
                images[0].sprite = getBackground(cardEffect);
                images[1].sprite = getConditionImage(cardEffect);
                images[2].sprite = getEffectImage(cardEffect.effect, images[2]);
                images[3].sprite = getEffectSignImage(cardEffect);
                texts[0].text = cardEffect.conditionNumber + "";
                texts[1].text = NumberStringConverter.convert(cardEffect.effectNumber);

                if (cardModel == Model.DELAYED)
                        changeTextSize(texts[1]);

                if (cardModel == Model.STANDARD || cardModel == Model.STANDARD_COMPACT)
                    texts[0].GetComponent<Outline>().effectColor = getEffectColor(cardEffect);
                break;
            case Model.ADDER:
            case Model.ADDER_COMPACT:
            case Model.ADDER_COMPACTER:
                // 0 - Background, 3 - Effect , 4 - Effect sign; 0 - Effect digit
                images[0].sprite = getBackground(cardEffect);
                images[1].sprite = getEffectImage(cardEffect.effect, images[1]);
                images[2].sprite = getEffectSignImage(cardEffect);
                texts[0].text = NumberStringConverter.convert(cardEffect.effectNumber);
                if (cardModel == Model.ADDER) changeTextSize(texts[0]);
                break;
            case Model.SINGLE_DICE:
                // 0 - Background, 1 - Condition Image ; 0 - Effect digit
                images[0].sprite = getBackground(cardEffect);
                images[1].sprite = getConditionImage(cardEffect);
                texts[0].text = cardEffect.effectNumber + "";
                break;
            case Model.FORBID:
            case Model.SINGLE_VALUE:
                // 0 - Background, 1 - Effect Image ; 0 - Effect digit
                images[0].sprite = getBackground(cardEffect);
                images[1].sprite = getEffectImage(cardEffect.effect, images[1]);
                if (cardEffect.effect == CardEffect.Effect.IMPROVE)
                {
                    texts[0].text = "+" + NumberStringConverter.convert(cardEffect.effectNumber) + "";
                    if (texts[0].text.Length == 4) texts[0].fontSize = 130;
                    else if (texts[0].text.Length >= 5) texts[0].fontSize = 100;
                } 
                else
                    texts[0].text = cardEffect.effectNumber + "";
                break;
            case Model.LOWER_MAX:
                // 0 - Background ; 1 - Effect digit
                images[0].sprite = getBackground(cardEffect);
                texts[1].text = "-" + cardEffect.effectNumber;
                break;
        }

        // If the card has a cost, add the cost label
        if (cardEffect.cost > 0)
        {
            GameObject costLabelObj = Instantiate(costLabel);
            costLabelObj.transform.SetParent(modelObj.transform); // Set as child of the model, not the card itself
            Text costText = costLabelObj.GetComponentInChildren<Text>();
            costText.text = NumberStringConverter.convert(cardEffect.cost);
            switch (costText.text.Length)
            {
                case 2:
                    costText.fontSize = 60;
                    break;
                case 3:
                    costText.fontSize = 50;
                    break;
                case 4:
                case 5:
                    costText.fontSize = 45;
                    break;
            }
            costLabelObj.GetComponent<RectTransform>().localPosition = costLabel.GetComponent<RectTransform>().localPosition;
            costLabelObj.GetComponent<RectTransform>().localScale = Vector3.one;
            images = cardObj.GetComponentsInChildren<Image>(); // Add new image to array (text is already included)
        }

        // If the card is locked, add the icon (only on deck editor)
        if (inDeckEditor && cardObj.GetComponent<DeckEditorCardObject>().card.isLocked)
        {
            GameObject lockIconObj = Instantiate(lockIcon);
            lockIconObj.transform.SetParent(modelObj.transform); // Set as child of the model, not the card itself
            lockIconObj.GetComponent<RectTransform>().localPosition = lockIcon.GetComponent<RectTransform>().localPosition;
            lockIconObj.GetComponent<RectTransform>().localScale = Vector3.one;
            images = cardObj.GetComponentsInChildren<Image>(); // Add new image to array
        }

        // If in deck editor, fix scaling issues
        if (inDeckEditor)
        {
            DeckEditorCardObject.Origin cardOrigin = cardObj.GetComponent<DeckEditorCardObject>().origin;
            float scaledSize = (cardOrigin == DeckEditorCardObject.Origin.COLLECTION) ? 140 : 120;
            float newScale = getNewScale(scaledSize);
            fixScalingIssues(newScale, images, texts);
        }
    }

    Model getCardImageModel(CardEffect ce)
    {
        if (ce.condition == CardEffect.Condition.INSTANT)
        {
            if (ce.effect == CardEffect.Effect.DICE_MULT)
            {
                if (ce.effectNumber <= 99)
                    return Model.DELAYED;
                else
                    return Model.DELAYED_COMPACT;
            } 
            else
            {
                if (ce.effectNumber <= 99)
                    return Model.ADDER;
                else if (ce.effectNumber < 1E+36)
                    return Model.ADDER_COMPACT;
                else
                    return Model.ADDER_COMPACTER;
            }
        }
        
        if (ce.condition == CardEffect.Condition.INSTANT_PERMANENT)
        {
            if (ce.effect == CardEffect.Effect.FIXED_DICE)
                return Model.SINGLE_DICE;
            if (ce.effect == CardEffect.Effect.FORBID_NUMBER)
                return Model.FORBID;
            if (ce.effect == CardEffect.Effect.ADD_ATTACK_ROLL)
                return Model.SINGLE_VALUE;
            if (ce.effect == CardEffect.Effect.LOWER_MAX)
                return Model.LOWER_MAX;
            if (ce.effect == CardEffect.Effect.IMPROVE)
                return Model.SINGLE_VALUE;
        }

        if (ce.effectNumber <= 99)
            return Model.STANDARD;
        else
            return Model.STANDARD_COMPACT;
    }

    GameObject getCardModelObject(Model model)
    {
        switch (model)
        {
            case Model.STANDARD:
                return standardModel;
            case Model.STANDARD_COMPACT:
                return standardCompactModel;
            case Model.ADDER:
                return adderModel;
            case Model.ADDER_COMPACT:
                return adderCompactModel;
            case Model.ADDER_COMPACTER:
                return adderCompacterModel;
            case Model.DELAYED:
                return delayedModel;
            case Model.DELAYED_COMPACT:
                return delayedCompactModel;
            case Model.SINGLE_DICE:
                return singleDiceModel;
            case Model.FORBID:
                return forbidModel;
            case Model.SINGLE_VALUE:
                return singleValueModel;
            case Model.LOWER_MAX:
                return lowerMaxModel;
        }
        Debug.Log("No model assigned, check 'getCardModelObject'");
        return standardModel;
    }

    Sprite getBackground(CardEffect cardEffect)
    {
        return greenBackground;
    }

    Sprite getConditionImage(CardEffect ce)
    {
        switch (ce.condition)
        {
            case CardEffect.Condition.FIRST:
            case CardEffect.Condition.LAST:
                return diceCondition;
            case CardEffect.Condition.INSTANT:
                return clock;
            case CardEffect.Condition.INSTANT_PERMANENT:
                if (ce.effect == CardEffect.Effect.FIXED_DICE)
                    return diceCondition;
                break;
        }
        return null;
    }

    Sprite getEffectImage(CardEffect.Effect effect, Image image)
    {
        switch (effect)
        {
            case CardEffect.Effect.DICE:
            case CardEffect.Effect.DICE_MULT:
                return diceEffect;
            case CardEffect.Effect.HEALTH:
                return heartEffect;
            case CardEffect.Effect.DICE_MOD:
                image.GetComponent<RectTransform>().localScale *= 1.33f;
                return multiDiceEffect;
            case CardEffect.Effect.FORBID_NUMBER:
                return forbidEffect;
            case CardEffect.Effect.ADD_ATTACK_ROLL:
                return attackEffect;
            case CardEffect.Effect.IMPROVE:
                return pentagonEffect;
        }
        return null;
    }

    Sprite getEffectSignImage(CardEffect ce)
    {
        if (ce.effect == CardEffect.Effect.DICE_MULT)
            return multiplicationSign;
        return plusSign;
    }

    Color getEffectColor(CardEffect ce)
    {
        switch (ce.effect)
        {
            case CardEffect.Effect.DICE:
                return new Color(0, 0.08f, 0.89f, 0.5f);
            case CardEffect.Effect.HEALTH:
                return new Color(0.85f, 0, 0.08f, 0.5f);
            case CardEffect.Effect.DICE_MOD:
                return new Color(0.08f, 0.9f, 0.05f, 0.5f);
        }
        return Color.white;
    }

    void changeTextSize(Text text)
    {
        int textLength = text.text.Length;
        if (textLength <= 1) return;
        if (textLength == 2)
            text.fontSize = 100;
        else if (textLength == 3)
            text.fontSize = 70;
        else if (textLength >= 4)
            text.fontSize = 50;
    }

    void fixScalingIssues(float newScale, Image[] images, Text[] texts)
    {
        foreach (Image img in images)
            fixScalingIn(newScale, img.gameObject.GetComponent<RectTransform>());
        foreach (Text txt in texts)
            fixScalingIn(newScale, txt.gameObject.GetComponent<RectTransform>());
    }

    float getNewScale(float scaledSize)
    {
        float actualSize = templateHalfHeight * 2;
        return scaledSize / actualSize;
    }

    void fixScalingIn(float newScale, RectTransform rectTr)
    {
        rectTr.localScale *= newScale;
        rectTr.localPosition *= newScale;
        rectTr.localPosition += Vector3.down * templateHalfHeight * newScale;
    }

}
