using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    Rigidbody2D rb2d;
    BoxCollider2D boxCollider;
    float inverseMoveTime;


    protected virtual void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        inverseMoveTime = 1f / moveTime;
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
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while(sqrRemainingDistance > float.Epsilon) {
            
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
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
