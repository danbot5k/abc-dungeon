using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MovingObject {

    public int healthPerPotion, attack;
    public Slider healthSlider;
    public AudioClip move1, pickupSound1, ladderSound, ladderFindSound, playerHurtSound1, playerAttackSound1; 

    private Animator animator;
    private int health, tabletCount;
    private RaycastHit2D hit;
    private Vector2 touchOrigin = -Vector2.one;
    private bool facingRight = true;
 
	protected override void Start () {
        animator = GetComponent<Animator>();
        health = GameController.instance.playerHealth;
        base.Start();	
	}
	
    void OnLevelWasLoaded() {
        healthSlider.value = GameController.instance.playerHealth;
    }

	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKey(KeyCode.Q)) {
            Application.Quit();
        }
        if (!GameController.instance.playersTurn || GameController.instance.movingCam) return; // Do nothing if not player's turn!
        int horizontal = 0; 
        int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) {
            vertical = 0;
        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
            {
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];
                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began)
                {
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }
                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
                {
                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;
                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;
                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;
                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;
                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        horizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        vertical = y > 0 ? 1 : -1;
                }
            }
            
#endif //End of mobile platform dependendent compilation section started above with #elif

        if (horizontal > 0 && !facingRight) Flip();
        if (horizontal < 0 && facingRight) Flip();
        if (horizontal != 0 || vertical != 0) {
            AttemptMove<EnemyController>(horizontal, vertical);
        }
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected override void AttemptMove<T> (int x, int y) {
        base.AttemptMove<T>(x,y);
       if(Move(x, y, out hit)) {
            AudioController.instance.RandomizeSfx(move1);
        }
        CheckIfGameOver();
        GameController.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component) {
        EnemyController hitEnemy = component as EnemyController;
        hitEnemy.TakeDamage(attack);
        AudioController.instance.RandomizeSfx(playerAttackSound1);
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(int damage) {
        health -= damage;
        AudioController.instance.RandomizeSfx(playerHurtSound1);
        healthSlider.value = health;
        CheckIfGameOver();
        animator.SetTrigger("Hurt");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Ladder") {
            AudioController.instance.PlaySingle(ladderSound);
            GameController.instance.Restart();
        }
        if (other.tag == "Tablet") {
            tabletCount++;
            AudioController.instance.PlaySingle(pickupSound1);
            Destroy(other.gameObject);
            if(tabletCount == 3) {
                AudioController.instance.PlaySingle(ladderFindSound);
                GameController.instance.ActivateLadder();
            }
        }
        if (other.tag == "Potion") {
            //ladder.SetActive(false);
            AudioController.instance.PlaySingle(pickupSound1);
            health += healthPerPotion;
            if (health > 100) health = 100;
            Destroy(other.gameObject);
            healthSlider.value = health;
        }
    }

    void OnDisable() {
        GameController.instance.playerHealth = health;
    }

    void CheckIfGameOver() {
        if (health <= 0) {
            GameController.instance.GameOver();
        }
    }

    


}
