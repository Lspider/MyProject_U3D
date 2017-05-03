using UnityEngine;
using System.Collections;
using System;

public class Grid : MonoBehaviour
{

    //public GameObject tilePrefab;
    public Tile tilePrefab;
    public int numberOfTiles = 10;
    public int tilesPerRow = 4;
    public float distanceBetweenTiles = 1.0f;
    public int numberOfMines = 10;

    public static ArrayList tilesAll;
    public static ArrayList tilesMined;
    public static ArrayList tilesUnmined;

    public static string state = "inGame";

    public static int minesMarkedCorrectly = 0;
    public static int tilesUncovered = 0;
    public static int minesRemaining = 0;

    // Use this for initialization
    void Start()
    {
        CreateTiles();

        minesRemaining = numberOfMines;
        minesMarkedCorrectly = 0;
        tilesUncovered = 0;

        state = "inGame";
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "inGame")
        {
            if ((minesRemaining == 0 && minesMarkedCorrectly == numberOfMines) || (tilesUncovered == numberOfTiles - numberOfMines))
                FinishGame();
        }
    }

    void CreateTiles()
    {
        tilesAll = new ArrayList();
        tilesMined = new ArrayList();
        tilesUnmined = new ArrayList();

        var xOffset = 0.0f;
        var zOffset = 0.0f;

        for (var tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated += 1)
        {
            xOffset += distanceBetweenTiles;

            if (tilesCreated % tilesPerRow == 0)
            {
                zOffset += distanceBetweenTiles;
                xOffset = 0;
            }

            var newTile = Instantiate(tilePrefab, new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), transform.rotation) as Tile;
            newTile.ID = tilesCreated;
            newTile.tilesPerRow = tilesPerRow;
            tilesAll.Add(newTile);
            tilesUnmined.Add(newTile);
        }

        AssignMines();
    }

    void AssignMines()
    {
        for (var minesAssigned = 0; minesAssigned < numberOfMines; minesAssigned += 1)
        {
            var index = UnityEngine.Random.Range(0, tilesUnmined.Count-1);
            Tile currentTile = tilesUnmined[index] as Tile;

            tilesMined.Add(currentTile);
            tilesUnmined.Remove(currentTile);

            if(currentTile)
                currentTile.isMined = true;
        }
    }

    void OnGUI()
    {
        if (state == "inGame")
        {
            GUI.Box(new Rect(10, 10, 200, 50), "Mines left: " + minesRemaining);
        }
        else if (state == "gameOver")
        {
            GUI.Box(new Rect(10, 10, 200, 50), "You lose");

            if (GUI.Button(new Rect(10, 70, 200, 50), "Restart"))
                Restart();
        }
        else if (state == "gameWon")
        {
            GUI.Box(new Rect(10, 10, 200, 50), "You rock!");

            if (GUI.Button(new Rect(10, 70, 200, 50), "Restart"))
                Restart();
        }
    }

    void Restart()
    {
        state = "loading";
        Application.LoadLevel(Application.loadedLevel);
    }

    void FinishGame()
    {
        state = "gameWon";

        //uncovers remaining fields if all nodes have been placed
        foreach (Tile currentTile in tilesAll)
            if (currentTile.state == "idle" && !currentTile.isMined)
                currentTile.UncoverTileExternal();

        //marks remaining mines if all nodes except the mines have been uncovered
        foreach (Tile currentTile in Grid.tilesMined)
            if (currentTile.state != "flagged")
                currentTile.SetFlag();
    }
}
