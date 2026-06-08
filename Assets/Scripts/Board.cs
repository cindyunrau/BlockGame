using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<Block, int, int> OnBlockPlaced;

    public GameObject cellPrefab;
    public GameObject staticSprite;

    public static int numCols = 10;
    public static int numRows = 8;

    private Cell[,] cells;
    private Cell[,] border;
    private List<Cell> cellsInShadow;
    private List<Mino> minosInShadow;
    private List<Cell> occupiedCells;
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



    private void Start()
    {
        transform.position += new Vector3(-7f, -4f, 0.0f);
        cells = new Cell[numCols, numRows];
        //border = new Cell[]
        
        cellsInShadow = new List<Cell>();
        minosInShadow = new List<Mino>();
        occupiedCells = new List<Cell>();

        for (int i = -1; i < numCols+1; i++)
        {
            for (int j = -1; j < numRows+1; j++)
            {
                if(i>=0 && i<numCols && j>=0 && j < numRows)
                {
                    
                    cells[i, j] = Instantiate(cellPrefab, new Vector3(i, j, 0) + transform.position, transform.rotation, transform).GetComponent<Cell>();
                    cells[i, j].Init(i, j, GSDefault, GSFire);
                } else
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
        GameObject handle = Instantiate(staticSprite, new Vector3(-1.35f, 3.5f, 0f) + transform.position, transform.rotation, transform);
        handle.GetComponent<SpriteRenderer>().sprite = GSHandle;
        handle.GetComponent<SpriteRenderer>().sortingOrder = 1;

        handle = Instantiate(staticSprite, new Vector3(10.2f, 3.5f, 0f) + transform.position, transform.rotation, transform);
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
    private Vector3 BoardToWorld(int x, int y)
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

    private void AddGrillShadow(Cell cell)
    {
        cellsInShadow.Add(cell);
        cell.SetGrillInShadow(true);
    }

    private void AddBlockShadow(Cell cell)
    {
        cellsInShadow.Add(cell);
        if(cell.mino) cell.mino.SetInShadow(true);
    }

    private void AddMinoShadow(Mino mino)
    {
        minosInShadow.Add(mino);
        mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
    }

    private void ClearShadows()
    {
        foreach (Cell cell in cellsInShadow)
        {
            cell.SetInShadow(false);
        }
        cellsInShadow.Clear();

        foreach (Mino mino in minosInShadow)
        {
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
        minosInShadow.Clear();
    }

    private void ClearMinoShadows()
    {
        foreach (Mino mino in minosInShadow)
        {
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
        minosInShadow.Clear();
    }

    public void Hover(List<Mino> minos)
    {
        ClearShadows();

        if (CanPlace(minos))
        {
            foreach (Mino mino in minos)
            {
                Vector3 coords = WorldToBoard(mino.transform.position);
                Cell cell = cells[(int)coords.x, (int)coords.y];
                AddGrillShadow(cell);

                if (CheckCol(cell.c))
                {
                    for(int row = 0; row < numRows; row++)
                    {
                        AddBlockShadow(cells[cell.c, row]);
                    }
                    foreach (Mino m in minos)
                    {
                        if(WorldToBoard(m.transform.position).x == cell.c)
                        {
                            AddMinoShadow(m);
                        }
                    }
                    
                }
                if (CheckRow(cell.r))
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        AddBlockShadow(cells[col, cell.r]);
                    }
                    foreach (Mino m in minos)
                    {
                        if(WorldToBoard(m.transform.position).y == cell.r)
                        {
                            AddMinoShadow(m);
                        }
                    }
                }
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
        foreach(Cell cell in occupiedCells)
        {
            cell.mino.IncreaseAge();
        }

        ClearShadows();
        List<Cell> colsToClear = new();
        List<Cell> rowsToClear = new();

        foreach (Mino mino in block.minos)
        {
            Vector3 coords = WorldToBoard(mino.transform.position);
            Cell cell = cells[(int)coords.x, (int)coords.y];

            
            cell.SetOccupied(true, mino);
            cell.mino.transform.localPosition = new Vector3(-0.08f, 0.1f, 0.0f);
            occupiedCells.Add(cell);

            if (CheckCol(cell.c) && !colsToClear.Contains(cell)) colsToClear.Add(cell);
            if (CheckRow(cell.r) && !rowsToClear.Contains(cell)) rowsToClear.Add(cell);
        }

        foreach (Cell cell in colsToClear) ClearCol(cell.c);
        foreach (Cell cell in rowsToClear) ClearRow(cell.r);

        OnBlockPlaced.Invoke(block, colsToClear.Count + rowsToClear.Count, colsToClear.Count * numRows + rowsToClear.Count * numCols);
    }

    private bool CheckCol(int col)
    {
        for (int row = 0; row < numRows; row++)
        {
            if (!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied()) return false;
        }
        return true;
    }

    private bool CheckRow(int row)
    {
        for (int col = 0; col < numCols; col++)
        {
            if (!cells[col, row].GrillInShadow() && !cells[col, row].IsOccupied()) return false;
        }
        return true;
    }

    private void ClearCol(int col)
    {
        Debug.Log($"column {col} clear");
        for (int row = 0; row < numRows; row++)
        {
            occupiedCells.Remove(cells[col, row]);
            cells[col, row].Clear();
        }
    }

    private void ClearRow(int row)
    {
        Debug.Log($"row {row} clear");
        for (int col = 0; col < numCols; col++)
        {
            occupiedCells.Remove(cells[col, row]);
            cells[col, row].Clear();
        }
    }
}