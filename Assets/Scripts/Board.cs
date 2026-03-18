using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<Block, int, int> OnBlockPlaced;

    public GameObject cellPrefab;

    public static int numCols = 8;
    public static int numRows = 6;

    private Cell[,] cells;
    private List<Cell> cellsInShadow;
    private List<Transform> minosInShadow;
    // float blockScale = 0.5f;

    public Sprite GrillSpriteTop;
    public Sprite GrillSpriteMid;
    public Sprite GrillSpriteBot;


    private void Start()
    {
        transform.position += new Vector3(-(numCols / 2.0f) + 0.5f, -1.5f, 0.0f);
        cells = new Cell[numCols, numRows];
        cellsInShadow = new List<Cell>();
        minosInShadow = new List<Transform>();

        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(i, j, 0) + transform.position, transform.rotation, transform);
                cells[i, j] = cell.GetComponent<Cell>();
                if (j == 0)
                {
                    cells[i, j].Init(i, j, GrillSpriteBot);
                }
                else if(j == numRows - 1)
                {
                    cells[i, j].Init(i, j, GrillSpriteTop);
                }
                else
                {
                    cells[i, j].Init(i, j, GrillSpriteMid);
                }
            }
        }
    }

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Cell>().Clear();
        }
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

    public bool CanPlace(List<Transform> minos)
    {
        foreach (Transform mino in minos)
        {
            if (!InBounds(mino.position) || IsOccupied(mino.position))
            {
                return false;
            }
        }
        return true;
    }

    private bool CanPlaceAt(Block block, Vector3 pos)
    {
        foreach (Transform mino in block.minos)
        {
            Vector3 minoTransformed = pos + block.transform.rotation * mino.localPosition;
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
        cell.SetBlockInShadow(true);
    }

    private void AddMinoShadow(Transform mino)
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

        foreach (Transform mino in minosInShadow)
        {
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
        minosInShadow.Clear();
    }

    private void ClearMinoShadows()
    {
        foreach (Transform mino in minosInShadow)
        {
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
        minosInShadow.Clear();
    }

    public void Hover(List<Transform> minos)
    {
        ClearShadows();

        if (CanPlace(minos))
        {
            foreach (Transform mino in minos)
            {
                Vector3 coords = WorldToBoard(mino.position);
                Cell cell = cells[(int)coords.x, (int)coords.y];
                AddGrillShadow(cell);

                if (CheckCol(cell.c))
                {
                    for(int row = 0; row < numRows; row++)
                    {
                        AddBlockShadow(cells[cell.c, row]);
                    }
                    foreach (Transform m in minos)
                    {
                        if(WorldToBoard(m.position).x == cell.c)
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
                    foreach (Transform m in minos)
                    {
                        if(WorldToBoard(m.position).y == cell.r)
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
        ClearShadows();
        List<Cell> colsToClear = new();
        List<Cell> rowsToClear = new();

        foreach (Transform mino in block.minos)
        {
            Vector3 coords = WorldToBoard(mino.position);
            Cell cell = cells[(int)coords.x, (int)coords.y];
            cell.SetOccupied(true,mino.gameObject.GetComponent<SpriteRenderer>().sprite,block.transform.eulerAngles.z + mino.localEulerAngles.z);

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
            cells[col, row].Clear();
        }
    }

    private void ClearRow(int row)
    {
        Debug.Log($"row {row} clear");
        for (int col = 0; col < numCols; col++)
        {
            cells[col, row].Clear();
        }
    }
}