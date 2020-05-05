//  Copyright © 2020 TR Solutions Pte. Ltd.
//  Licensed under Apache 2.0 and MIT
//  See appropriate LICENCE files for details.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBehaviour : MonoBehaviour
{
    public Transform[] cards;

    private Transform[] cardInstances;

    // Start is called before the first frame update
    void Start()
    {
        // Note cards are instantiated such that the last item is on top
        this.cardInstances = new Transform[cards.Length];
        for (int iCard = 0; iCard < cards.Length; iCard++)
        {
            var card = cards[iCard];
            var cardInstance = Instantiate(card, transform);
            //var cardBehaviour = cardInstance.GetComponent<CardBehaviour>();
            //if (cardBehaviour)
            //{
            //    cardBehaviour.SetOrderInLayer(iCard);
            //}
            this.cardInstances[iCard] = cardInstance;
        }
        Shuffle();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (cards.Length > 0 && Input.GetMouseButtonDown(0))
        {
            // print("mouse down detected!");
            Collider2D myCollider = null;
            for (int iCard = 0; iCard < cards.Length; iCard++)
            {
                var cardCollider = cardInstances[iCard].GetComponent<Collider2D>();
                if (cardCollider != null && Physics2D.OverlapPoint(mousePos) == cardCollider)
                {
                    //print(string.Format("Checking {0} against mouse point {1}", cardCollider.bounds.size, mousePos));
                    //var cardBounds = new Bounds(mousePosV3, new Vector3(0.1f, 0.1f, 0.1f));
                    //if (cardCollider.bounds.Contains(mousePosV3))
                    //{
                        myCollider = cardCollider;
                        break;
                    //}
                }
            }
            if (myCollider != null)
            {
                print("Deck has been touched.");
                // Pull the last card
                var dealtCard = cardInstances[cardInstances.Length - 1];
                // draw down both the card protos and the card instances
                // We only do this because the card protos are public, and someone might be watching
                Transform[] newCards = new Transform[cards.Length - 1];
                Transform[] newCardInstances = new Transform[cards.Length - 1];
                for (int iCard = 0; iCard < newCards.Length; iCard++)
                {
                    newCards[iCard] = cards[iCard];
                    newCardInstances[iCard] = cardInstances[iCard];
                }
                this.cards = newCards;
                this.cardInstances = newCardInstances;

                // separate the card from the deck object
                dealtCard.parent = null;
                // Tell the card the mouse has been pressed over it
                var cardBehaviour = dealtCard.GetComponent<CardBehaviour>();
                if (cardBehaviour != null)
                {
                    cardBehaviour.StartDrag(mousePos);
                }
            }
        }
    }

    public void Shuffle()
    {
        for (int pass = 0; pass < 15; pass++)
        {
            for (int iCard = 0; iCard < cards.Length; iCard++)
            {
                int iSwapCard = Random.Range(0, cards.Length);
                if (iSwapCard != iCard)
                {
                    var savedCard = cards[iCard];
                    cards[iCard] = cards[iSwapCard];
                    cards[iSwapCard] = savedCard;

                    var savedCardInstance = cardInstances[iCard];
                    cardInstances[iCard] = cardInstances[iSwapCard];
                    cardInstances[iSwapCard] = savedCardInstance;
                }
            }
        }
    }

    public void DealTopCard()
    {
        var dealtCard = cards[cards.Length - 1];
        Transform[] newCards = new Transform[cards.Length - 1];
        for (int iCard = 0; iCard < cards.Length - 1; iCard++)
        {
            newCards[iCard] = cards[iCard];
        }
        cards = newCards;
        dealtCard.parent = null; // so now available to be moved.

    }

    public void RemoveCardFromDeck(Transform cardToRemove)
    {
        int foundIndex = -1;
        for (int iCard = 0; iCard < cards.Length && foundIndex < 0; iCard++)
        {
            if (cards[iCard] == cardToRemove)
            {
                foundIndex = iCard;
            }
        }
        if (foundIndex >= 0)
        {
            print(string.Format("Removing card at index {0}", foundIndex));
            Transform[] newCards = new Transform[cards.Length - 1];
            for (int iCard = 0; iCard < foundIndex; iCard++)
            {
                newCards[iCard] = cards[iCard];
            }
            for (int iCard = foundIndex + 1; iCard < cards.Length; iCard++)
            {
                newCards[iCard - 1] = cards[iCard];
            }
            this.cards = newCards;
        } else
        {
            print(string.Format("Didn't find card {0} in deck!", cardToRemove));
        }
    }

    public void RestoreCard(Transform card)
    {
        print(string.Format("Replacing card at index {0}", cards.Length));

        Transform[] newCards = new Transform[cards.Length + 1];
        Transform[] newCardInstances = new Transform[cards.Length + 1];
        for(int iCard=0; iCard < cards.Length; iCard++)
        {
            newCards[iCard] = cards[iCard];
            newCardInstances[iCard] = cardInstances[iCard];
        }
        // since we don't have the prefab, we'll just use the instantiated object
        // as a prefab.
        newCards[cards.Length] = card;
        newCardInstances[cards.Length] = card;

        cards = newCards;
        cardInstances = newCardInstances;
    }

    public int GetOrderInLayer(int sortingLayerID)
    {
        var topCard = cardInstances.Length > 0 ? cardInstances[cardInstances.Length - 1] : null;
        var topCardBehaviour = topCard != null ? topCard.GetComponent<CardBehaviour>() : null;
        return topCardBehaviour != null ? topCardBehaviour.GetOrderInLayer(sortingLayerID) : int.MinValue;
    }
}
