/* See Unity tutorials Roguelike and Procedural Caves for more on this code */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class BoardController : MonoBehaviour {

    public GameObject[] grounds, groundTrash, walls, innerWalls, tablets, ladders, potions, enemies;
    public GameObject fog;
    public int width = 50, height = 50, minInnerWallsCoefficient = 10, maxInnerWallsCoefficient = 20;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int randomFillPercent;

    List<Vector3> gridPositions = new List<Vector3>();
    Transform boardHolder;
    int[,] map;
    GameObject player;
    CameraController cameraController;

    public void Awake() {
        
    }
    public void SetupScene(int level) {
        GenerateMap();
      //  BoardSetup();
//        InitializeList();
        PopulateBoard(level);
    }

    void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < 3; i++) {
           SmoothMap();
       }
        ProcessMap();
        DrawMap();
        InitializeList();
    }

    void RandomFillMap() {
        if(useRandomSeed) {
            seed = System.DateTime.Now.ToString();
            Debug.Log(seed);
        }
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                }
                else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if (IsInMapRange(neighbourX,  neighbourY)) {
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void DrawMap() {
        if (map != null) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GameObject toInstantiate = grounds[UnityEngine.Random.Range(0, grounds.Length)];
                    if (map[x, y] == 1) {
                        toInstantiate = walls[UnityEngine.Random.Range(0, walls.Length)];
                    }
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);

                    //Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    //Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    //Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    void InitializeList() {
        gridPositions.Clear();
        for (int x = 0; x < width; x++) {
            for (int y = 1; y < height; y++) {
        //        gridPositions.Add(new Vector3(x, y, 0f));
                if(map[x,y] == 0) {
                    gridPositions.Add(new Vector3(x, y, 0f));
                }

            }
        }
    }

    void BoardSetup() {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < height+1; x++) {
            for (int y = -1; y < width+1; y++) {
                GameObject toInstantiate = grounds[UnityEngine.Random.Range(0, grounds.Length)];
                if(x==-1 || x==height || y==-1 || y==width) {
                    toInstantiate = walls[UnityEngine.Random.Range(0, walls.Length)];
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
        int randomDeco = UnityEngine.Random.Range(0, groundTrash.Length);
        int randomTrashChance = UnityEngine.Random.Range(0, 5);
        if (randomTrashChance == 1) {
            GameObject trashInstance = Instantiate(groundTrash[randomDeco], new Vector3(i, j, 0f), Quaternion.identity);
            trashInstance.transform.SetParent(boardHolder);
        }
    }

    void PopulateBoard(int level) {
     //   LayoutObjectAtRandom(innerWalls, minInnerWallsCoefficient, maxInnerWallsCoefficient);
        LayoutObjectAtRandom(tablets, 3, 3);
        LayoutObjectAtRandom(ladders, 1, 1);
        LayoutObjectAtRandom(potions, 1, 2);
        LayoutObjectAtRandom(enemies, 1, 2);
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("Ladder").GetComponent<BoxCollider2D>().enabled = false;
        Vector3 randomPosition = GetRandomBoardPosition();
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = randomPosition;
        cameraController = FindObjectOfType<CameraController>();
        cameraController.SetCamToPlayer();
        
    }

    void LayoutObjectAtRandom(GameObject[] objects, int min, int max) {
        int quantity = UnityEngine.Random.Range(min, max);
        for (int i = 0; i < quantity; i++) {
            Vector3 randomPosition = GetRandomBoardPosition();
            GameObject randomObject = objects[UnityEngine.Random.Range(0, objects.Length)];
            Instantiate(randomObject, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomBoardPosition() {
        int rand = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[rand];
        gridPositions.RemoveAt(rand);
        return randomPosition;
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


    void ProcessMap() {
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallThresholdSize = 10;
        foreach (List<Coord> wallRegion in wallRegions) {
            if(wallRegion.Count < wallThresholdSize) {
                foreach (Coord tile in wallRegion) {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        int roomThresholdSize = 10;
        List<Room> SurvivingRooms = new List<Room>();  //rooms that aren't culled
        foreach (List<Coord> roomRegion in roomRegions) {
            if (roomRegion.Count < roomThresholdSize) {
                foreach (Coord tile in roomRegion) {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else {
                SurvivingRooms.Add(new Room(roomRegion, map));
            }
        }

        SurvivingRooms.Sort();
        SurvivingRooms[0].isMainRoom = true;
        SurvivingRooms[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(SurvivingRooms);
    }


    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom) {
            foreach (Room room in allRooms) {
                if (room.isAccessibleFromMainRoom) {
                    roomListB.Add(room);
                }
                else {
                    roomListA.Add(room);
                }
            }
        }
        else {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;
        foreach (Room roomA in roomListA) {
            if (!forceAccessibilityFromMainRoom) {
                possibleConnectionFound = false;
                if(roomA.connectedRooms.Count > 0) {
                    continue;
                }
            }
            foreach (Room roomB in roomListB) {
                if(roomA == roomB || roomA.IsConnected(roomB)) {
                    continue; // checking same room
                }
          
                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));
                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if(possibleConnectionFound && !forceAccessibilityFromMainRoom) {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }
        if (!forceAccessibilityFromMainRoom) {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);
        Debug.DrawLine(new Vector3(tileA.tileX, tileA.tileY, 0f), new Vector3(tileB.tileX, tileB.tileY, 0f), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line) {
            DrawCircle(c, 2);
        }

    }

    void DrawCircle(Coord c, int r) {
        for (int x = -r; x <= r; x++) {
            for (int y = -r; y <= r; y++) {
                if(x*x + y*y <= r*r) {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if(IsInMapRange(drawX, drawY)) {
                        map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }


    List<Coord> GetLine(Coord from, Coord to) {
        List<Coord> line = new List<Coord>();
        int x = from.tileX;
        int y = from.tileY;
        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;
        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);
        if(longest < shortest) {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);

        }
        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++) {
            line.Add(new Coord(x, y));
            if(inverted) {
                y += step;
            }
            else {
                x += step;

            }
            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest) {
                if(inverted) {
                    x += gradientStep;
                }
                else {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }
        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile) {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(mapFlags[x,y] == 0 && map[x,y]== tileType) {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach(Coord tile in newRegion) {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;

    }

    // Flood-fill method to determine regions...
    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];  // temp map
        int tileType = map[startX, startY]; // starting tile type
        Queue<Coord> queue = new Queue<Coord>();  // queue for..
        queue.Enqueue(new Coord(startX, startY));  
        mapFlags[startX, startY] = 1; // add start coords to show we looked at it
        while (queue.Count>0) {
            Coord tile = queue.Dequeue();  // return and remove first tile from list
            tiles.Add(tile);
            for (int x = tile.tileX-1; x <= tile.tileX +1; x++) {
                for (int y = tile.tileY-1; y <= tile.tileY + 1; y++) {
                    if(IsInMapRange(x, y) &&(x == tile.tileX || y == tile.tileY)) {
                        if(mapFlags[x,y] == 0 && map[x,y] == tileType) {   // Hasn't been checked yet AND is the right tile type
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;

    }

    bool IsInMapRange(int x, int y) {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    struct Coord {
        public int tileX;
        public int tileY;
        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }

    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;  // only search edges, not insides
        public List<Room> connectedRooms;  // rooms this room connects to
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room() {  // empty  constructor

        }

        public Room(List<Coord> roomTiles, int[,] map) {  //Constructor
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles) {   // look through room tiles to find edges
                for (int x = tile.tileX -1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY -1; y <= tile.tileY + 1; y++) {
                        if (x == tile.tileX || y == tile.tileY) {   // exclude diagonals
                            if (map[x, y] == 1) {
                                edgeTiles.Add(tile);   // add to edges if borders a wall
                            }
                        }
                    }
                }
            }

        }

        public void SetAccessibleFromMainRoom() {
            if(!isAccessibleFromMainRoom) {
                isAccessibleFromMainRoom = true;
                foreach(Room connectedRoom in connectedRooms) {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public int CompareTo(Room otherRoom) {
            return otherRoom.roomSize.CompareTo(roomSize);
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            if(roomA.isAccessibleFromMainRoom) {
                roomB.SetAccessibleFromMainRoom();
            }
            else if(roomB.isAccessibleFromMainRoom) {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

    }


}
