using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<Block, List<int>, List<int>> OnBlockPlaced;

    public GameObject cellPrefab;
    public GameObject staticSprite;

    public static int numCols = 10;
    public static int numRows = 8;

    public static int RAW_BONUS = 0;
    public static int COOKED_BONUS = 10;
    public static int BURNT_BONUS = -2;

    private Cell[,] cells;
    private Cell[,] border;
    private List<Cell> cellsInShadow;
    private List<Cell> cellsOnFire;
    private List<Mino> minosInShadow;
    private List<Cell> occupiedCells;
    private List<Cell> currentHover;
    // float blockScale = 0.5f;

    
    public Sprite GSDefault;
    public Sprite GSFire;

    public Sprite GSTop;
    public Sprite GSBottom;
    public Sprite GSLeft;
    public Sprite GSRight;
    public Sprite GSTopLeftCorner;
    public Sprite GSTopRightCorner;
    public Sprite GSBotLeftCorner;
    public Sprite GSBotRightCorner;
    public Sprite GSHandle;

    public ResourceManager rm;

    private void Start()
    {
        transform.position += new Vector3(-7f, -4f, 0.0f);
        cells = new Cell[numCols, numRows];
        
        cellsInShadow = new List<Cell>();
        minosInShadow = new List<Mino>();
        occupiedCells = new List<Cell>();
        cellsOnFire = new List<Cell>();

        for (int i = -1; i < numCols+1; i++)
        {
            for (int j = -1; j < numRows+1; j++)
            {
                if(i>=0 && i<numCols && j>=0 && j < numRows)
                {
                    cells[i, j] = Instantiate(cellPrefab, new Vector3(i, j, 0) + transform.position, transform.rotation, transform).GetComponent<Cell>();
                    cells[i, j].Init(i, j, GSDefault, GSFire);
                } 
                else
                {
                    GameObject border = Instantiate(staticSprite, new Vector3(i, j, 0) + transform.position, transform.rotation, transform.Find("Border"));
                    if (i == -1 && j == -1) border.GetComponent<SpriteRenderer>().sprite = GSBotLeftCorner;
                    else if (i == -1 && j == numRows) border.GetComponent<SpriteRenderer>().sprite = GSTopLeftCorner;
                    else if (i == numCols && j == -1) border.GetComponent<SpriteRenderer>().sprite = GSBotRightCorner;
                    else if (i == numCols && j == numRows) border.GetComponent<SpriteRenderer>().sprite = GSTopRightCorner;
                    else if (i == -1) border.GetComponent<SpriteRenderer>().sprite = GSLeft;
                    else if (i == numCols) border.GetComponent<SpriteRenderer>().sprite = GSRight;
                    else if (j == -1) border.GetComponent<SpriteRenderer>().sprite = GSBottom;
                    else if (j == numRows) border.GetComponent<SpriteRenderer>().sprite = GSTop;
                }
                
            }
        }
        GameObject handle = Instantiate(staticSprite, new Vector3(-1.35f, 3.5f, 0f) + transform.position, transform.rotation, transform.Find("Border"));
        handle.GetComponent<SpriteRenderer>().sprite = GSHandle;
        handle.GetComponent<SpriteRenderer>().sortingOrder = 1;

        handle = Instantiate(staticSprite, new Vector3(10.2f, 3.5f, 0f) + transform.position, transform.rotation, transform.Find("Border"));
        handle.GetComponent<SpriteRenderer>().sprite = GSHandle;
        handle.GetComponent<SpriteRenderer>().sortingOrder = 1;
        handle.transform.localScale *= -1;
    }


    public void Reset()
    {
        foreach (Transform child in transform)
        {
            if(child.name != "Border") child.GetComponent<Cell>().Clear();
        }
        cellsInShadow = new List<Cell>();
        minosInShadow = new List<Mino>();
        occupiedCells = new List<Cell>();
    }

    // Converts World Coordinates (float) to Board Coordinates (int)
    private Vector3 WorldToBoard(Vector3 world)
    {
        Vector3 result = new();
        result.x = (int)Math.Round(world.x - transform.position.x, MidpointRounding.AwayFromZero);
        result.y = (int)Math.Round(world.y - transform.position.y, MidpointRounding.AwayFromZero);
        result.z = world.z;
  
        return result;
    }

    // Converts Board Coordinates (int) to World Coordinates (float)
    public Vector3 BoardToWorld(int x, int y)
    {
        Vector3 result = new();
        result.x = transform.position.x + x;
        result.y = transform.position.y + y;
        result.z = transform.position.z;
        return result;
    }

    private bool InBounds(Vector3 coords)
    {
        Vector3 boardCoords = WorldToBoard(coords);

        if (boardCoords.x < numCols && boardCoords.x >= 0 && boardCoords.y < numRows && boardCoords.y >= 0)
        {
            return true;
        }

        return false;
    }

    private bool IsOccupied(Vector3 coords)
    {
        Vector3 boardCoords = WorldToBoard(coords);
        return cells[(int)boardCoords.x, (int)boardCoords.y].IsOccupied();
    }

    public bool CanPlace(List<Mino> minos)
    {
        foreach (Mino mino in minos)
        {
            if (!InBounds(mino.transform.position) || IsOccupied(mino.transform.position))
            {
                return false;
            }
        }
        return true;
    }



    private bool CanPlaceAt(Block block, Vector3 pos)
    {
        foreach (Mino mino in block.minos)
        {
            Vector3 minoTransformed = pos + block.transform.rotation * mino.transform.localPosition;
            if (!InBounds(minoTransformed) || IsOccupied(minoTransformed))
            {
                return false;
            }
        }
        return true;
    }

    private void OnGrillHover(Cell cell)
    {
        cellsInShadow.Add(cell);
        cell.SetGrillInShadow(true);
    }

    private void AddBlockShadow(Cell cell)
    {
        cellsOnFire.Add(cell);
        cell.SetInShadow(true);
    }

    private void ClearShadows()
    {
        foreach (Cell cell in cellsInShadow)
        {
            //cell.SetInShadow(false);
            cell.SetGrillInShadow(false);
        }
        cellsInShadow.Clear();

        foreach (Mino mino in minosInShadow)
        {
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
        minosInShadow.Clear();
    }

    private void ClearFire()
    {
        print("Fire Cleared");
        foreach (Cell cell in cellsOnFire)
        {
            cell.SetInShadow(false);
        }
        cellsOnFire.Clear();
    }

    public void Hover(List<Mino> minos)
    {
        bool moved = false;

        foreach (Mino mino in minos)
        {
            Vector3 coords = WorldToBoard(mino.transform.position);
            if (!cellsInShadow.Exists(c => (c.c == (int)coords.x) && (c.r == (int)coords.y))) moved = true;
        }


        if (moved)
        {
            ClearShadows();
            if (CanPlace(minos))
            {
                foreach (Mino mino in minos)
                {
                    Vector3 coords = WorldToBoard(mino.transform.position);
                    Cell cell = cells[(int)coords.x, (int)coords.y];

                    OnGrillHover(cell);

                }
                //// Check all cols for matches
                //for (int c = 0; c < numCols; c++)
                //{
                //    if (CheckCol(c))
                //    {
                //        for (int row = 0; row < numRows; row++)
                //        {
                //            AddBlockShadow(cells[c, row]);
                //        }
                //    }
                //}
                //// Check all rows for matches
                //for (int r = 0; r < numRows; r++)
                //{
                //    if (CheckRow(r))
                //    {
                //        for (int col = 0; col < numCols; col++)
                //        {
                //            AddBlockShadow(cells[col, r]);
                //        }
                //    }
                //}
                //dep
                //if (CheckCol(cell.c))
                //    {
                //        for (int row = 0; row < numRows; row++)
                //        {
                //            AddBlockShadow(cells[cell.c, row]);
                //        }
                //    }
                //    if (CheckRow(cell.r))
                //    {
                //        for (int col = 0; col < numCols; col++)
                //        {
                //            AddBlockShadow(cells[col, cell.r]);
                //        }
                //    }
                
            }
        }
    }

    public bool CheckPlaceable(Block block)
    {
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                if (CanPlaceAt(block, BoardToWorld(i, j)))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryPlaceBlock(Block block)
    {
        if (CanPlace(block.minos))
        {
            PlaceBlock(block);

            return true;
        }
        return false;
    }

    private void PlaceBlock(Block block)
    {
        ClearShadows();
        ClearFire();

        foreach (Cell cell in occupiedCells)
        {
            if (cell.mino.IncreaseAge(1))
            {
                if(cell.mino.status == "cooked")
                {
                    cell.mino.SetSprite(rm.GetCookedSprite(cell.mino.foodName));
                }
                else if(cell.mino.status == "burnt")
                {
                    cell.mino.SetSprite(rm.GetBurntSprite(cell.mino.foodName));
                }
            }
        }


        List<int> colsToClear = new();
        List<int> rowsToClear = new();
        List<int> colsNextTurn = new();
        List<int> rowsNextTurn = new();


        // Check all cols for matches
        for (int c = 0; c < numCols; c++)
        {
            if (CheckCol(c)) colsToClear.Add(c);
        }
        // Check all rows for matches
        for (int r = 0; r < numRows; r++)
        {
            if (CheckRow(r)) rowsToClear.Add(r);
        }


        //    for(int c = 0;c< numCols; c++)
        //    {
        //        if (CheckCol(cell.c) && !colsToClear.Contains(cell)) colsToClear.Add(cell);
        //    }

        //    if (CheckRow(cell.r) && !rowsToClear.Contains(cell)) rowsToClear.Add(cell);
        //}

        int bonusPoints = 0;
        foreach (int i in colsToClear) bonusPoints += ClearCol(i);
        foreach (int i in rowsToClear) bonusPoints += ClearRow(i);

        foreach (Mino mino in block.minos)
        {
            Vector3 coords = WorldToBoard(mino.transform.position);
            Cell cell = cells[(int)coords.x, (int)coords.y];


            cell.SetOccupied(true, mino);
            cell.mino.transform.localPosition = new Vector3(-0.08f, 0.1f, 0.0f);
            occupiedCells.Add(cell);
        }

        // Check all cols for matches
        for (int c = 0; c < numCols; c++)
        {
            if (CheckColNextTurn(c)) colsNextTurn.Add(c);
        }
        // Check all rows for matches
        for (int r = 0; r < numRows; r++)
        {
            if (CheckRowNextTurn(r)) rowsNextTurn.Add(r);
        }

        foreach (int i in colsNextTurn) SetColFire(i);
        foreach (int i in rowsNextTurn) SetRowFire(i);

        OnBlockPlaced.Invoke(block, colsToClear, rowsToClear);
    }

    private bool CheckCol(int col)
    {
        for (int row = 0; row < numRows; row++)
        {
            //if ((!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied()) || (cells[col, row].mino != null && cells[col, row].mino.status == "raw"))
            if ((!cells[col, row].IsOccupied()) || (cells[col, row].mino != null && cells[col, row].mino.status == "raw"))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckRow(int row)
    {
        for (int col = 0; col < numCols; col++)
        {
            if (!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied() || (cells[col, row].mino != null && cells[col, row].mino.status == "raw"))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckColNextTurn(int col)
    {
        for (int row = 0; row < numRows; row++)
        {
            if (!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied() || (cells[col, row].mino != null && !cells[col, row].mino.IsCookedNextTurn()))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckRowNextTurn(int row)
    {
        for (int col = 0; col < numCols; col++)
        {
            if (!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied() || (cells[col, row].mino != null && !cells[col, row].mino.IsCookedNextTurn()))
            {
                return false;
            }
        }
        return true;
    }

    // returns number of points received for optimal mino 'status'
    private int ClearCol(int col)
    {
        Debug.Log($"column {col} clear");
        int bonusPoints = 0;
        for (int row = 0; row < numRows; row++)
        {
            occupiedCells.Remove(cells[col, row]);
            if (cells[col, row].mino.status == "raw") bonusPoints += RAW_BONUS;
            else if (cells[col, row].mino.status == "cooked") bonusPoints += COOKED_BONUS;
            else if (cells[col, row].mino.status == "burnt") bonusPoints += BURNT_BONUS;
            cells[col, row].Clear();
        }
        return bonusPoints;
    }

    private int ClearRow(int row)
    {
        Debug.Log($"row {row} clear");
        int bonusPoints = 0;
        for (int col = 0; col < numCols; col++)
        {
            occupiedCells.Remove(cells[col, row]);
            if (cells[col, row].mino.status == "raw") bonusPoints += RAW_BONUS;
            else if (cells[col, row].mino.status == "cooked") bonusPoints += COOKED_BONUS;
            else if (cells[col, row].mino.status == "burnt") bonusPoints += BURNT_BONUS;
            cells[col, row].Clear();
        }
        return bonusPoints;
    }

    private void SetRowFire(int r)
    {
        print("SetRowFire");
        for (int col = 0; col < numCols; col++)
        {
            AddBlockShadow(cells[col, r]);
        }
    }

    private void SetColFire(int c)
    {
        print("SetColFire");
        for (int row = 0; row < numRows; row++)
        {
            AddBlockShadow(cells[c, row]);
        }
    }
}