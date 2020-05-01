﻿using System.Collections;
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
    private int orderInLayer;
    private int currentLayerID;
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
        this.orderInLayer = front.sortingOrder;
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
        } else if (Input.GetMouseButtonUp(0))
        {
            this.orderInLayer = 0;
            this.currentLayerID = this.defaultLayerID;

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

        //if (deckBehaviour == null)
        //{
        //    this.front.sortingLayerID = this.currentLayerID;
        //    this.back.sortingLayerID = this.currentLayerID;

        //    // Determine our order in the display
        //    if (this.currentLayerID == this.defaultLayerID)
        //    {
        //        var overlapResults = new List<Collider2D>();
        //        var contactFilter = new ContactFilter2D();
        //        var numHits = this.myCollider.OverlapCollider(contactFilter.NoFilter(), overlapResults);
        //        // print(string.Format("Overlapping with {0} cards", numHits));
        //        // TODO: find the highest sort order and set ours to one above that.
        //        foreach (Collider2D collider in overlapResults)
        //        {

        //        }
        //        // TODO: if no hits, then our sort order is 0.

        //    }
        //}

    }
    public void SetOrderInLayer(int newOrderInLayer)
    {
        this.orderInLayer = newOrderInLayer;
        this.front.sortingOrder = newOrderInLayer;
        this.back.sortingOrder = newOrderInLayer;
    }
    public void StartDrag(Vector2 mousePos)
    {
        var xyPos = new Vector2(this.transform.position.x, this.transform.position.y);
        this.mouseOffset = new OptionalVector2(mousePos - xyPos);
        print(string.Format("Mouse down while mouse over card! Mouse offset = {0}", mouseOffset));
        this.orderInLayer = 1000; // above everything
        this.currentLayerID = SortingLayer.NameToID("Moving");

    }

}
