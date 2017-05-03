using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public bool isMined = false;
    public TextMesh displayText;
    public GameObject displayFlag;

    public Material materialIdle;
    public Material materialLightup;
    public Material materialUncovered;
    public Material materialDetonated;

    public int ID;
    public int tilesPerRow;

    public Tile tileUpper;
    public Tile tileLower;
    public Tile tileLeft;
    public Tile tileRight;
    public Tile tileUpperRight;
    public Tile tileUpperLeft;
    public Tile tileLowerRight;
    public Tile tileLowerLeft;

    public ArrayList adjacentTiles = new ArrayList();
    public int adjacentMines = 0;
    public string state = "idle";

    // Use this for initialization
    void Start()
    {
        displayFlag.GetComponent<Renderer>().enabled = false;
        displayText.GetComponent<Renderer>().enabled = false;

        if (inBounds(Grid.tilesAll, ID + tilesPerRow)) tileUpper = Grid.tilesAll[ID + tilesPerRow] as Tile;
        if (inBounds(Grid.tilesAll, ID - tilesPerRow)) tileLower = Grid.tilesAll[ID - tilesPerRow] as Tile;
        if (inBounds(Grid.tilesAll, ID - 1) && ID % tilesPerRow != 0) tileLeft = Grid.tilesAll[ID - 1] as Tile;
        //Debug.Log("ID:" + ID);
        if (inBounds(Grid.tilesAll, ID + 1) && (ID + 1) % tilesPerRow != 0) tileRight = Grid.tilesAll[ID + 1] as Tile;

        if (inBounds(Grid.tilesAll, ID + tilesPerRow + 1) && (ID + 1) % tilesPerRow != 0) tileUpperRight = Grid.tilesAll[ID + tilesPerRow + 1] as Tile;
        if (inBounds(Grid.tilesAll, ID + tilesPerRow - 1) && ID % tilesPerRow != 0) tileUpperLeft = Grid.tilesAll[ID + tilesPerRow - 1] as Tile;
        if (inBounds(Grid.tilesAll, ID - tilesPerRow + 1) && (ID + 1) % tilesPerRow != 0) tileLowerRight = Grid.tilesAll[ID - tilesPerRow + 1] as Tile;
        if (inBounds(Grid.tilesAll, ID - tilesPerRow - 1) && ID % tilesPerRow != 0) tileLowerLeft = Grid.tilesAll[ID - tilesPerRow - 1] as Tile;

        if (tileUpper) adjacentTiles.Add(tileUpper);
        if (tileLower) adjacentTiles.Add(tileLower);
        if (tileLeft) adjacentTiles.Add(tileLeft);
        if (tileRight) adjacentTiles.Add(tileRight);
        if (tileUpperRight) adjacentTiles.Add(tileUpperRight);
        if (tileUpperLeft) adjacentTiles.Add(tileUpperLeft);
        if (tileLowerRight) adjacentTiles.Add(tileLowerRight);
        if (tileLowerLeft) adjacentTiles.Add(tileLowerLeft);

        CountMines();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnMouseOver()
    //{
    //    if (state == "idle")
    //    {
    //        GetComponent<Renderer>().material = materialLightup;

    //        if (Input.GetMouseButtonDown(0))
    //            UncoverTile();

    //        if (Input.GetMouseButtonDown(1))
    //            SetFlag();
    //    }
    //    else if (state == "flagged")
    //    {
    //        GetComponent<Renderer>().material = materialLightup;

    //        if (Input.GetMouseButtonDown(1))
    //            SetFlag();
    //    }
    //}

    //void OnMouseExit()
    //{
    //    //GetComponent<Renderer>().material = materialIdle;
    //    if (state == "idle" || state == "flagged")
    //        GetComponent<Renderer>().material = materialIdle;
    //}

    void OnMouseOver()
    {
        if (Grid.state == "inGame")
        {
            if (state == "idle")
            {
                GetComponent<Renderer>().material = materialLightup;

                if (Input.GetMouseButtonDown(0))
                    UncoverTile();

                if (Input.GetMouseButtonDown(1))
                    SetFlag();
            }
            else if (state == "flagged")
            {
                GetComponent<Renderer>().material = materialLightup;

                if (Input.GetMouseButtonDown(1))
                    SetFlag();
            }
        }
    }
    void OnMouseExit()
    {
        if (Grid.state == "inGame")
        {
            if (state == "idle" || state == "flagged")
                GetComponent<Renderer>().material = materialIdle;
        }
    }

    private bool inBounds(ArrayList inputArray, int targetID)
    {
        if (targetID < 0 || targetID >= inputArray.Count)
            return false;
        else
            return true;
    }

    void CountMines()
    {
        adjacentMines = 0;

        foreach(Tile currentTile in adjacentTiles)
            if (currentTile.isMined)
                adjacentMines += 1;

        displayText.text = adjacentMines.ToString();

        if (adjacentMines <= 0)
            displayText.text = "";
    }

    public void SetFlag()
    {
        if (state == "idle")
        {
            state = "flagged";
            displayFlag.GetComponent<Renderer>().enabled = true;
            Grid.minesRemaining -= 1;
            if (isMined)
                Grid.minesMarkedCorrectly += 1;
        }
        else if (state == "flagged")
        {
            state = "idle";
            displayFlag.GetComponent<Renderer>().enabled = false;
            Grid.minesRemaining += 1;
            if (isMined)
                Grid.minesMarkedCorrectly -= 1;
        }
    }

    void UncoverTile()
    {
        if (!isMined)
        {
            state = "uncovered";
            displayText.GetComponent<Renderer>().enabled = true;
            GetComponent<Renderer>().material = materialUncovered;

            Grid.tilesUncovered += 1;

            if (adjacentMines == 0)
                UncoverAdjacentTiles();
        }
        else
            Explode();
    }

    private void UncoverAdjacentTiles()
    {
        foreach (Tile currentTile in adjacentTiles)
        {
            //uncover all adjacent nodes with 0 adjacent mines
            if (!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines == 0)
                currentTile.UncoverTile();

            //uncover all adjacent nodes with more than 1 adjacent mine, then stop uncovering
            else if (!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines > 0)
                currentTile.UncoverTileExternal();
        }
    }

    public void UncoverTileExternal()
    {
        state = "uncovered";
        displayText.GetComponent<Renderer>().enabled = true;
        GetComponent<Renderer>().material = materialUncovered;
        Grid.tilesUncovered += 1;
    }

    void Explode()
    {
        state = "detonated";
        GetComponent<Renderer>().material = materialDetonated;

        foreach (Tile currentTile in Grid.tilesMined)
            currentTile.ExplodeExternal();

        Grid.state = "gameOver";
    }
    void ExplodeExternal()
    {
        state = "detonated";
        GetComponent<Renderer>().material = materialDetonated;
    }
}
