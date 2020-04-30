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
    }

    // Update is called once per frame
    void Update()
    {
        // !null == this card is part of a deck
        var deckBehaviour = GetComponentInParent<DeckBehaviour>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics2D.OverlapPoint(mousePos) == myCollider)
            {
                if (deckBehaviour)
                {
                    deckBehaviour.RemoveCardFromDeck(transform);
                    transform.parent = null;
                    deckBehaviour = null; // can now treat as a real card
                }
                //var parent = transform.parent;
                //if (parent != null)
                //{
                //    print(string.Format("Parent: {0}", transform.parent.name));
                //    // This not great as technically we should only remove it from the deck
                //    // if we take it OFF the deck. But it's kind of hard to know that.
                //    var deckBehaviour = GetComponentInParent<DeckBehaviour>();
                //    if (deckBehaviour != null)
                //    {
                //        print("Parent has deck behaviour.");
                //        //deckBehaviour.RemoveCardFromDeck(transform);
                //        deckBehaviour.DealTopCard();
                //        return; // don't treat this as a pickup.
                //    }
                //    transform.parent = null; // remove from hierarchy
                //}
                var xyPos = new Vector2(this.transform.position.x, this.transform.position.y);
                this.mouseOffset = new OptionalVector2(mousePos - xyPos);
                print(string.Format("Mouse down while mouse over card! Mouse offset = {0}", mouseOffset));
                this.orderInLayer = 1000; // above everything
                this.currentLayerID = SortingLayer.NameToID("Moving");
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

        this.front.enabled = flippedOver;
        this.back.enabled = !flippedOver;

        if (deckBehaviour == null)
        {
            this.front.sortingLayerID = this.currentLayerID;
            this.back.sortingLayerID = this.currentLayerID;

            // Determine our order in the display
            if (this.currentLayerID == this.defaultLayerID)
            {
                var overlapResults = new List<Collider2D>();
                var contactFilter = new ContactFilter2D();
                var numHits = this.myCollider.OverlapCollider(contactFilter.NoFilter(), overlapResults);
                // print(string.Format("Overlapping with {0} cards", numHits));
                // TODO: find the highest sort order and set ours to one above that.
                foreach (Collider2D collider in overlapResults)
                {

                }
                // TODO: if no hits, then our sort order is 0.

            }
        }

    }
    public void SetOrderInLayer(int newOrderInLayer)
    {
        this.orderInLayer = newOrderInLayer;
        this.front.sortingOrder = newOrderInLayer;
        this.back.sortingOrder = newOrderInLayer;
    }
    
}
