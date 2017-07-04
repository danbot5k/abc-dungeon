using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MovingObject {

    public int health;
    public int attack;
    public AudioClip enemyDieSound;
    public Slider enemyHealthSlider;

    Transform target;
    Animator animator;
    

	// Use this for initialization
	protected override void Start () {
        animator = GetComponent<Animator>();
        GameController.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}
	
    public void MoveEnemy() {
        int x= 0, y= 0;  // declare move variables

        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
            y = target.position.y > transform.position.y ? 1 : -1;  // move y if x not same
        }
        else {
            x = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<PlayerController>(x, y);
    }

    protected override void AttemptMove<T>(int x, int y) {
        base.AttemptMove<T>(x, y);
    }


    public void TakeDamage(int amount) {
        health -= amount;
        enemyHealthSlider.value = health;
        if(health <= 0 ) {
            Debug.Log("Enemy Died!");
            AudioController.instance.RandomizeSfx(enemyDieSound);
            GameController.instance.RemoveEnemyFromList(this);
            animator.SetTrigger("Die");
            Destroy(gameObject, 1.1f);
        }
    }


    protected override void OnCantMove<T>(T component) {
        PlayerController hitPlayer = component as PlayerController;
        hitPlayer.TakeDamage(attack);
        animator.SetTrigger("Attack");
        Debug.Log("Enemy hit player!");
    }

}
