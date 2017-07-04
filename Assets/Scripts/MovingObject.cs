using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveSpeed = 30f;
    public LayerMask blockingLayer;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;



    protected virtual void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
	}

    protected bool Move(int horizontal, int vertical, out RaycastHit2D hit) {

        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + new Vector2(horizontal, vertical);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(startPosition, endPosition, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            StartCoroutine(SmoothMovement(endPosition));
            return true;
        }
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end) {
        float remainingDistance = (transform.position - end).magnitude;
        while(remainingDistance > float.Epsilon) {
            
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, (moveSpeed * Time.deltaTime));
            rb2d.MovePosition(newPosition);
            remainingDistance = (transform.position - end).magnitude;
            yield return null;
        }
    }
 
    protected virtual void AttemptMove<T> (int x, int y) 
        where T : Component {
        RaycastHit2D hit;
        bool canMove = Move(x, y, out hit); //true if move was successful

        if (hit.transform == null)
            return; // return if nothing was hit
        T hitComponent = hit.transform.GetComponent<T>(); // get the supplied generic component of the hit
        if(!canMove && hitComponent != null) { // can't move and had the supplied genereic component
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component)
       where T : Component;

}
