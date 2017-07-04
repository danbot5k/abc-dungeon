using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    public GameObject[] grounds, groundTrash, walls, innerWalls, tablets, ladders, potions, enemies;
    public GameObject fog;
    public int width = 8, height = 8, minInnerWallsCoefficient = 10, maxInnerWallsCoefficient = 20;

    List<Vector3> gridPositions = new List<Vector3>();
    Transform boardHolder;


    public void SetupScene(int level) {
        BoardSetup();
        InitializeList();
   //     ladderSprite = null;
        
        PopulateBoard(level);

        // AddFog();
    }


    void InitializeList() {
        gridPositions.Clear();
        for (int x = 1; x < height; x++) {
            for (int y = 1; y < width; y++) {
                gridPositions.Add(new Vector3(x, y, 0f));
                
            }
        }
    }

    void BoardSetup() {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < height+1; x++) {
            for (int y = -1; y < width+1; y++) {
                GameObject toInstantiate = grounds[Random.Range(0, grounds.Length)];
                if(x==-1 || x==height || y==-1 || y==width) {
                    toInstantiate = walls[Random.Range(0, walls.Length)];
                }
                else {
                    AddBoardDeco(x, y);
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x,y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
 
            }
        }
    }

    void AddBoardDeco(int i, int j) {
        int randomDeco = Random.Range(0, groundTrash.Length);
        int randomTrashChance = Random.Range(0, 5);
        if (randomTrashChance == 1) {
            GameObject trashInstance = Instantiate(groundTrash[randomDeco], new Vector3(i, j, 0f), Quaternion.identity);
            trashInstance.transform.SetParent(boardHolder);
        }
    }

    Vector3 GetRandomBoardPosition() {
        int rand = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[rand];
        gridPositions.RemoveAt(rand);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] objects, int min, int max) {
        int quantity = Random.Range(min, max);
        for (int i = 0; i < quantity; i++) {
            Vector3 randomPosition = GetRandomBoardPosition();
            GameObject randomObject = objects[Random.Range(0, objects.Length)];
            Instantiate(randomObject, randomPosition, Quaternion.identity);
        }
    }

    void PopulateBoard(int level) {
        LayoutObjectAtRandom(innerWalls, minInnerWallsCoefficient, maxInnerWallsCoefficient);
        LayoutObjectAtRandom(tablets, 3, 3);
        LayoutObjectAtRandom(ladders, 1, 1);
        LayoutObjectAtRandom(potions, 1, 2);
        LayoutObjectAtRandom(enemies, 1, 2);
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<BoxCollider2D>().enabled = false;
    }

    public void ActivateLadder() {
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<SpriteRenderer>().enabled=true;
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<BoxCollider2D>().enabled = true;

    }



    void AddFog() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(x>2) {
                    Instantiate(fog, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (y >2) {
                    Instantiate(fog, new Vector3(x, y, 0f), Quaternion.identity);
                }
            }
        }
    }

    void OnDisable() {
      //  ladderSprite = null;
      //  ladderCollider = null;
    }
}
