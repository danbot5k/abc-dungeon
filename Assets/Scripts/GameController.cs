using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    public static GameController instance = null;
    public int playerHealth = 100;
    public float turnDelay = .9f, levelStartDelay = 1f;
    [HideInInspector]
    public bool playersTurn = true, movingCam = false;

    private Text levelText;
    private GameObject LevelImage;
    BoardController bc;
    private int level = 1;
    private List<EnemyController> enemies;
    private bool enemiesMoving = false, settingUp = false;

    // Use this for initialization
    void Awake () {
        //singleton code
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<EnemyController>();
        bc = GetComponent<BoardController>();
        InitGame();
        Debug.Log("End of the awake call!"); 	
	}
	
    void OnLevelWasLoaded(int index) {
        level++;
        InitGame();
    }

    void InitGame() {
        settingUp = true;
        LevelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = " "+ level + " ";
        LevelImage.SetActive(true);
        enemies.Clear();
        bc.SetupScene(level);
        Invoke("HideLevelImage", levelStartDelay);
    }

    void HideLevelImage() {
        LevelImage.SetActive(false);
        settingUp = false;
    }

    void Update() {
        if (playersTurn || enemiesMoving || settingUp || movingCam) return;
        StartCoroutine( MoveEnemies());
    }

    public void AddEnemyToList(EnemyController script) {
        enemies.Add(script);
    }

    public void RemoveEnemyFromList(EnemyController script) {
        enemies.Remove(script);
    }


    IEnumerator MoveEnemies() {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0) {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(.2f);
        }
        playersTurn = true;
        enemiesMoving = false;
    }

    void OnLevelWasLoaded() {
        Debug.Log("A new scene loaded!");
     //   InitGame();
    }


    public void ActivateLadder() {
        bc.ActivateLadder();
        Camera.main.GetComponent<CameraController>().MoveCamToLadder();
    }

    public void Restart() {
        Invoke("RealRestart", 1f);
    }

    void RealRestart() {
        SceneManager.LoadScene(0);
    }

    public void GameOver() {
        levelText.text = "Dead";
        LevelImage.SetActive(true);
        enabled = false;
        Debug.Log("Game Over");
    }
    
}
