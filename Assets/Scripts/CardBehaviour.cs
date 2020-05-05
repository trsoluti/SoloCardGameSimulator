using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(/*typeof(SpriteRenderer),*/ typeof(Collider2D))]
public class CardBehaviour : MonoBehaviour
{
    private class OptionalVector2
    {
        public bool IsValid { get; private set; }
        private Vector2 _vector2;

        public OptionalVector2()
        {
            this.IsValid = false;
        }
        public OptionalVector2(Vector2 vector2)
        {
            this.IsValid = true;
            this._vector2 = vector2;
        }

        public Vector2 Vector { get {
                if (!this.IsValid) { throw new UnassignedReferenceException(); } else return _vector2; }
        }
        public override string ToString()
        {
            if (this.IsValid)
            {
                return this._vector2.ToString();
            }
            return "(not valid)";
        }
    }

    public SpriteRenderer front;
    public SpriteRenderer back;
    public bool flippedOver;

    private int currentLayerID;
    private int currentSortingOrder;
    private int defaultLayerID;

    // private SpriteRenderer myRenderer;
    private Collider2D myCollider;
    private OptionalVector2 mouseOffset;

    // Start is called before the first frame update
    void Start()
    {
        // this.myRenderer = this.GetComponent<SpriteRenderer>();
        this.myCollider = this.GetComponent<Collider2D>();
        this.mouseOffset = new OptionalVector2(); // i.e. not valid
        this.currentSortingOrder = front.sortingOrder;
        this.currentLayerID = front.sortingLayerID;
        this.defaultLayerID = this.currentLayerID;

        this.front.enabled = flippedOver;
        this.back.enabled = !flippedOver;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't do anything if the card is part of a deck.
        var deckBehaviour = GetComponentInParent<DeckBehaviour>();
        if (deckBehaviour != null)
        {
            return;
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics2D.OverlapPoint(mousePos) == myCollider)
            {
                this.StartDrag(mousePos);
            }
        } else if (Input.GetMouseButtonUp(0) && this.mouseOffset.IsValid)
        {
            SetOrderInLayer();

            this.mouseOffset = new OptionalVector2(); // i.e. no longer valid
        } else if (Input.GetMouseButton(0) && this.mouseOffset.IsValid)
        {
            var adjustedPositon = mousePos - this.mouseOffset.Vector;
            this.transform.position = new Vector3(
                adjustedPositon.x,
                adjustedPositon.y,
                transform.position.z
                );
        }


        if (Input.GetButtonDown("Flip"))
        {
            if (Physics2D.OverlapPoint(mousePos) == myCollider)
            {
                this.flippedOver = !this.flippedOver;
            }
        }

        if (Input.GetButtonDown("Replace"))
        {
            if (Physics2D.OverlapPoint(mousePos) == myCollider)
            {
                print("replace button pressed.");
                // action is not valid if the card is not in collision with a deck
                var overlapResults = new List<Collider2D>();
                var contactFilter = new ContactFilter2D();
                var numOverlaps = this.myCollider.OverlapCollider(contactFilter.NoFilter(), overlapResults);
                if (numOverlaps == 0)
                {
                    print("Not overlapping with anything.");
                } else
                {
                    DeckBehaviour deck = null;
                    foreach (Collider2D collider in overlapResults)
                    {
                        var parent = collider.transform.parent;
                        if (parent != null)
                        {
                            deck = parent.GetComponent<DeckBehaviour>();
                            if (deck != null)
                            {
                                break;
                            }
                        }

                    }
                    if (deck == null)
                    {
                        print("Not overlapping with deck.");
                    }
                    else
                    {
                        print(string.Format("About to reattach to deck {0}", deckBehaviour));
                        // tell the deck to take us
                        deck.RestoreCard(transform);
                        // set our parent to the deck
                        transform.parent = deck.transform;
                        transform.position = deck.transform.position;
                        flippedOver = false;
                        // reshuffle the deck
                        deck.Shuffle();
                    }
                }
            }
        }

        this.front.enabled = flippedOver;
        this.back.enabled = !flippedOver;

        // Ensure front and back faces are consistent with this card's sorting position
        this.front.sortingLayerID = this.currentLayerID;
        this.front.sortingOrder = this.currentSortingOrder;

        this.back.sortingLayerID = this.currentLayerID;
        this.back.sortingOrder = this.currentSortingOrder;
    }

    public int GetOrderInLayer(int sortingLayerID)
    {
        return this.currentLayerID == sortingLayerID ? this.currentSortingOrder : int.MinValue;
    }


    private void SetOrderInLayer()
    {
        // Set the order in the layer to be one higher
        // than the largest item beneath it
        // (min of 0).
        var overlapResults = new List<Collider2D>();
        var contactFilter = new ContactFilter2D();
        var numOverlaps = this.myCollider.OverlapCollider(contactFilter.NoFilter(), overlapResults);
        int newOrderInLayer = 0; // i.e. nothing beneath us
        if (numOverlaps == 0)
        {
            print("Not overlapping with anything.");
        }
        else
        {
            foreach (Collider2D collider in overlapResults)
            {
                // We have three possibilities:
                int overlappingOrderInLayer;
                // (1) overlapping a deck -- order in layer is order in layer of top card
                // (2) overlapping a card -- order in layer is card behaviour's order in layer
                // (3) overlapping a token -- order in layer is token behavior's order in layer
                var parent = collider.transform.parent;
                var deck = parent != null ? parent.GetComponent<DeckBehaviour>() : null;
                if (deck != null)
                {
                    overlappingOrderInLayer = deck.GetOrderInLayer(this.defaultLayerID);
                    print(string.Format("Overlapping a deck with order in layer {0}", overlappingOrderInLayer));
                }
                else
                {
                    var card = collider.transform.GetComponent<CardBehaviour>();
                    if (card != null)
                    {
                        overlappingOrderInLayer = card.GetOrderInLayer(this.defaultLayerID);
                        print(string.Format("Overlapping a card with order in layer {0}", overlappingOrderInLayer));
                    }
                    else
                    {
                        var token = collider.transform.GetComponent<TokenBehaviour>();
                        if (token != null)
                        {
                            overlappingOrderInLayer = token.GetOrderInLayer(this.defaultLayerID);
                            print(string.Format("Overlapping a token with order in layer {0}", overlappingOrderInLayer));
                        }
                        else
                        {
                            print("Not overlapping anything.");
                            overlappingOrderInLayer = int.MinValue;
                        }
                    }
                }
                if (overlappingOrderInLayer >= newOrderInLayer)
                {
                    newOrderInLayer = overlappingOrderInLayer + 1;
                }
            }
        }
        this.currentLayerID = this.defaultLayerID;
        this.currentSortingOrder = newOrderInLayer;
    }

    public void StartDrag(Vector2 mousePos)
    {
        var xyPos = new Vector2(this.transform.position.x, this.transform.position.y);
        this.mouseOffset = new OptionalVector2(mousePos - xyPos);
        print(string.Format("Mouse down while mouse over card! Mouse offset = {0}", mouseOffset));
        this.currentLayerID = SortingLayer.NameToID("Moving");

    }

}
