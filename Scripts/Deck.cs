using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

    public GameObject card;

    public List<Card> cards = new List<Card>();

    public Deck()
    {
        cards.Add(new Card(new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
        cards.Add(new Card(new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
        cards.Add(new Card(new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
        cards.Add(new Card(new CardEffect(3, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 3)));
        cards.Add(new Card(new CardEffect(5, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.HEALTH, 10)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 7, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 8, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.LAST, 9, CardEffect.Effect.DICE, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.FIRST, 1, CardEffect.Effect.HEALTH, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.FIRST, 2, CardEffect.Effect.HEALTH, 2)));
        cards.Add(new Card(new CardEffect(4, CardEffect.Condition.FIRST, 3, CardEffect.Effect.HEALTH, 2)));
    }

    public Deck(List<Card> cards)
    {
        this.cards = cards;
    }

    public Card drawFirstCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public int getCardsLeft()
    {
        return cards.Count;
    }

    public void shuffle()
    {
        List<Card> shuffledCards = new List<Card>();
        int nCards = cards.Count;

        for (int i=0; i < nCards; i++)
        {
            int randomIndex = Random.Range(0, cards.Count);
            shuffledCards.Add(cards[randomIndex]);
            cards.RemoveAt(randomIndex);
        }

        cards = shuffledCards;
    }

    public Deck clone()
    {
        List<Card> listOfCards = new List<Card>();
        foreach (Card originalCard in cards)
        {
            Card cardClone = originalCard.clone();
            listOfCards.Add(cardClone);
        }
        return new Deck(listOfCards);
    }

}
