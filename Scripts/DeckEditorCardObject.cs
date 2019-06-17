using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckEditorCardObject : MonoBehaviour, ISelectHandler {
    
    public enum Origin { COLLECTION, DECK };
    [HideInInspector] public Origin origin;
    [HideInInspector] public Card card;
    [HideInInspector] public Vector2 nextDestination;

    public void initialize(Card card, Origin origin)
    {
        this.card = card;
        this.origin = origin;
        updateImage(card.cardEffect);
    }

    public void OnSelect(BaseEventData baseEventData)
    {
        showCardInfo();
    }

    void showCardInfo()
    {
        DeckEditor.deInstance.changeHelpText(card.getDescription());
    }

    public void moveCardBetweenCollectionAndDeck() // Gets called when the card is clicked/touched
    {
        if (!Player.isAndroid())
            useCard(); // If not on Android -> Click = Move card
        else
            showCardInfo(); // If on Android -> Click = Show card info
    }

    public void useCard()
    {
        // If on lock mode, not on deck view mode and clicking/tapping a card from collection, (un)lock it
        if (DeckEditor.deInstance.lockModeOn && !DeckEditor.deInstance.deckView && origin == Origin.COLLECTION)
            DeckEditor.deInstance.lockCard(this);
        else
            DeckEditor.deInstance.moveCardBetweenCollectionAndDeck(this);
    }

    void updateImage(CardEffect cardEffect)
    {
        CardImageManager.ciInstance.updateImage(gameObject, cardEffect);
    }

}
