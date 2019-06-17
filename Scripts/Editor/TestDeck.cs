using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestDeck {

	[Test]
	public void TestGetCardsLeft() {
        List<Card> emptyCardSet = new List<Card>();
        Deck deck = new Deck( emptyCardSet );
        Assert.AreEqual(0, deck.getCardsLeft());

        deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 5)));
        Assert.AreEqual(1, deck.getCardsLeft());

        for (int i=0; i < 20; i++)
            deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 5)));
        Assert.AreEqual(21, deck.getCardsLeft());
    }

    [Test]
    public void TestDrawFirstCard()
    {
        List<Card> emptyCardSet = new List<Card>();
        Deck deck = new Deck(emptyCardSet);
        deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 5)));
        deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 2)));
        deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, 11)));

        Card firstCard = deck.drawFirstCard();
        Assert.AreEqual(2, deck.getCardsLeft());
        Assert.AreEqual(5, firstCard.cardEffect.effectNumber);

        firstCard = deck.drawFirstCard();
        Assert.AreEqual(1, deck.getCardsLeft());
        Assert.AreEqual(2, firstCard.cardEffect.effectNumber);

        firstCard = deck.drawFirstCard();
        Assert.AreEqual(0, deck.getCardsLeft());
        Assert.AreEqual(11, firstCard.cardEffect.effectNumber);
    }

    [Test]
    public void TestShuffle()
    {
        List<Card> emptyCardSet = new List<Card>();
        int nCards = 15;
        Deck deck = new Deck(emptyCardSet);
        for (int i=0; i < nCards; i++)
            deck.cards.Add(new Card(new CardEffect(0, CardEffect.Condition.INSTANT, 0, CardEffect.Effect.DICE, i)));

        // First dimension is the cardId, second dimension is its position in the deck
        double[][] nOfIncidences = new double[nCards][];
        for (int i = 0; i < nCards; i++)
        {
            nOfIncidences[i] = new double[nCards];
        }

        // Shuffle multiple times and note the positions of each card after each shuffle
        int nShuffles = 10000;//10000;
        for (int i=0; i < nShuffles; i++)
        {
            deck.shuffle();
            for (int j=0; j < nCards; j++)
            {
                int cardId = (int)deck.cards[j].cardEffect.effectNumber;
                nOfIncidences[cardId][j]++;
            }
        }

        // Statistically, each card should have occuped each position roughly ~1/nCards of the time
        float expectedDistribution = 1f / nCards;
        double errorMargin = 0.01;
        for (int i = 0; i < nCards; i++)
        {
            for (int j = 0; j < nCards; j++)
            {
                float distribution = (float)nOfIncidences[i][j] / nShuffles;
                Assert.AreEqual(expectedDistribution, distribution, errorMargin);
            }
        }
    }
    
}
