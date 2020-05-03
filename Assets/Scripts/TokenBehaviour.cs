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
    private int orderInLayer;
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
        this.orderInLayer = mySpriteRenderer.sortingOrder;
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
        else if (Input.GetMouseButtonUp(0))
        {
            this.orderInLayer = 0;
            this.currentLayerID = this.defaultLayerID;

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

        mySpriteRenderer.sortingOrder = this.orderInLayer;
        mySpriteRenderer.sortingLayerID = this.currentLayerID;
    }

    public void StartDrag(Vector2 mousePos)
    {
        var xyPos = new Vector2(this.transform.position.x, this.transform.position.y);
        this.mouseOffset = new OptionalVector2(mousePos - xyPos);
        // print(string.Format("Mouse down while mouse over token! Mouse offset = {0}", mouseOffset));
        this.orderInLayer = 1001; // above everything, even cards
        this.currentLayerID = SortingLayer.NameToID("Moving");

    }
}
