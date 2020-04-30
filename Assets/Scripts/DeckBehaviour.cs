using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBehaviour : MonoBehaviour
{
    public Transform[] cards;

    // Start is called before the first frame update
    void Start()
    {
        Shuffle();

        // Note cards are instantiated such that the last item is on top
        for (int iCard = 0; iCard < cards.Length; iCard++)
        {
            var card = cards[iCard];
            var cardInstance = Instantiate(card, transform);
            var cardBehaviour = cardInstance.GetComponent<CardBehaviour>();
            if (cardBehaviour)
            {
                cardBehaviour.SetOrderInLayer(iCard);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        for (int pass = 0; pass < 3; pass++)
        {
            for (int iCard = 0; iCard < cards.Length; iCard++)
            {
                int iSwapCard = Random.Range(0, cards.Length);
                if (iSwapCard != iCard)
                {
                    var savedCard = cards[iCard];
                    cards[iCard] = cards[iSwapCard];
                    cards[iSwapCard] = savedCard;
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
}
