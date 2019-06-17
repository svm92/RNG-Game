using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardObject : MonoBehaviour, ISelectHandler {
    
    public static int WIDTH = 250;

    [HideInInspector] public Card card;

    public void initialize(Card card)
    {
        this.card = card;
        updateImage(card.cardEffect);
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        showCardInfo();
    }

    void showCardInfo()
    {
        BattleManager.bmInstance.changeHelpText(card.getDescription());
        solveVictoryScreenIssues();
    }

    // On victory screen, make selected card the last child (so that it appears in front of others)
    void solveVictoryScreenIssues()
    {
        if (BattleManager.battleEnded)
        {
            if (transform.parent != null && transform.parent.name == "PrizeCardPanel")
            {
                transform.SetSiblingIndex(transform.parent.childCount-1);
            }
        }
    }

    public void playCard() // Gets called when the card is clicked/touched
    {
        if (!Player.isAndroid())
            StartCoroutine(playCardAsync()); // If not on Android -> Click = Play
        else
            showCardInfo(); // If on Android -> Click = Show card info
    }

    public IEnumerator playCardAsync()
    {
        if (!BattleManager.playerActionAllowed || BattleManager.battleEnded) yield break;
        if (BattleManager.isTutorial && Player.currentTurn == 4 && card.cardEffect.condition == CardEffect.Condition.INSTANT)
        {
            StartCoroutine(TutorialManager.forceEnemyDialogue(TextScript.get(TextScript.Sentence.T24), 4f));
            yield break;
        }

        BattleManager.bmInstance.hideSurrenderButton();

        yield return waitForCardAnimation();

        Hand.removeCard(gameObject);
        BattleManager.bmInstance.hideHand();
        activateEffect();
        if (!BattleManager.isTutorial)
            BattleManager.bmInstance.initiateBattleLogicFlow(card.cardEffect);
        else
        {
            TutorialManager.playedCardInTutorial = true;
            BattleManager.bmInstance.showingInfoTextPanel = false;
        }
    }

    IEnumerator waitForCardAnimation()
    {
        Button button = GetComponent<Button>();

        button.interactable = false;
        button.transition = Selectable.Transition.None;
        GetComponent<Animator>().enabled = false;
        transform.localScale = Vector3.one;

        foreach (GameObject cardObj in Hand.hand)
        {
            if (cardObj != gameObject)
                cardObj.SetActive(false);
        }

        Vector3 originalPosition = transform.localPosition;
        float transitionTime = 0.1f;
        float timer = 0;
        while (timer <= transitionTime)
        {
            transform.localPosition = Vector2.Lerp(originalPosition, Vector2.zero, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector2.zero;

        yield return new WaitForSeconds(0.15f);

        float finalScale = .1f;
        Vector2 destination = getCardDestination(finalScale);
        if (destination == Vector2.zero) finalScale = 0;
        originalPosition = transform.localPosition;
        transitionTime = 0.25f;
        timer = 0;
        while (timer <= transitionTime)
        {
            transform.localPosition = Vector2.Lerp(originalPosition, destination, timer / transitionTime);
            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * finalScale, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject cardObj in Hand.hand)
        {
            if (cardObj != gameObject)
                cardObj.SetActive(true);
        }
    }
    
    Vector2 getCardDestination(float finalScale)
    {
        // y-125 at some because the card's origin is on the lower side (y-half-height)
        float halfHeight = 125f * finalScale;
        CardEffect cardEffect = card.cardEffect;
        switch (cardEffect.condition)
        {
            case CardEffect.Condition.INSTANT:
                if (cardEffect.effect == CardEffect.Effect.DICE) return new Vector2(0, -410);
                if (cardEffect.effect == CardEffect.Effect.HEALTH) return new Vector2(-450, -410);
                break;
            case CardEffect.Condition.LAST:
                // 180: Panel y position; 280: Half-height of child container (because pivot is on upper side);
                // -46: Position of 0; -52: Distance between numbers
                int yPos = 180 + 280 - 46 - (52 * cardEffect.conditionNumber);
                if (cardEffect.effect == CardEffect.Effect.DICE ||
                cardEffect.effect == CardEffect.Effect.DICE_MOD) return new Vector2(670, yPos - halfHeight);
                break;
            case CardEffect.Condition.FIRST:
                if (cardEffect.effect == CardEffect.Effect.HEALTH) return new Vector2(-450, -410);
                break;
        }
        return Vector2.zero;
    }

    void activateEffect()
    {
        if (card.cardEffect != null)
        {
            Player.nDice -= card.cardEffect.cost;
            if (card.cardEffect.condition == CardEffect.Condition.INSTANT || 
                card.cardEffect.condition == CardEffect.Condition.INSTANT_PERMANENT)
            {
                card.cardEffect.applyInstantEffect();
            }
            else
                Player.addCardEffect(card.cardEffect);
        }
    }

    void updateImage(CardEffect cardEffect)
    {
        CardImageManager.ciInstance.updateImage(gameObject, cardEffect);
    }

}
