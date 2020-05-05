//  Copyright © 2020 TR Solutions Pte. Ltd.
//  Licensed under Apache 2.0 and MIT
//  See appropriate LICENCE files for details.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class TokenBehaviour : MonoBehaviour
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

        public Vector2 Vector
        {
            get
            {
                if (!this.IsValid) { throw new UnassignedReferenceException(); } else return _vector2;
            }
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

    private Collider2D myCollider;
    private SpriteRenderer mySpriteRenderer;
    private int currentLayerID;
    private int defaultLayerID;

    private OptionalVector2 mouseOffset;


    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        if (myCollider == null || mySpriteRenderer == null)
        {
            Debug.LogError("Token object MUST have collider and sprite renderer!");
            return;
        }
        this.mouseOffset = new OptionalVector2(); // i.e. not valid
        this.currentLayerID = mySpriteRenderer.sortingLayerID;
        this.defaultLayerID = this.currentLayerID;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics2D.OverlapPoint(mousePos) == myCollider)
            {
                this.StartDrag(mousePos);
            }
        }
        else if (Input.GetMouseButtonUp(0) && this.mouseOffset.IsValid)
        {
            // Set our sorting order based on the sorting orders
            // of anything beneath us.
            setOrderInLayer();

            this.mouseOffset = new OptionalVector2(); // i.e. no longer valid
        }
        else if (Input.GetMouseButton(0) && this.mouseOffset.IsValid)
        {
            var adjustedPositon = mousePos - this.mouseOffset.Vector;
            this.transform.position = new Vector3(
                adjustedPositon.x,
                adjustedPositon.y,
                transform.position.z
                );
        }
    }

    public void StartDrag(Vector2 mousePos)
    {
        var xyPos = new Vector2(this.transform.position.x, this.transform.position.y);
        this.mouseOffset = new OptionalVector2(mousePos - xyPos);
        // print(string.Format("Mouse down while mouse over token! Mouse offset = {0}", mouseOffset));
        this.currentLayerID = SortingLayer.NameToID("Moving");
        this.mySpriteRenderer.sortingLayerID = SortingLayer.NameToID("Moving");

    }

    public int GetOrderInLayer(int sortingLayerID)
    {
        if (mySpriteRenderer.sortingLayerID == sortingLayerID)
        {
            return mySpriteRenderer.sortingOrder;
        }
        return int.MinValue;
    }

    private void setOrderInLayer()
    {
        // action is not valid if the card is not in collision with a deck
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
        mySpriteRenderer.sortingLayerID = this.defaultLayerID;
        mySpriteRenderer.sortingOrder = newOrderInLayer;
    }
}
