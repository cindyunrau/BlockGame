using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<int, int, int, int> OnBlockPlaced;

    public GameObject cellPrefab;

    public static int numCols = 6;
    public static int numRows = 5;

    private Cell[,] cells;
    private List<Cell> cellsInShadow;

    private void Start()
    {
        transform.position += new Vector3(-(numCols / 2.0f) + 0.5f, 0, 0);
        cells = new Cell[numCols, numRows];
        cellsInShadow = new List<Cell>();

        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(i, j, 0) + transform.position, transform.rotation, transform);
                cells[i, j] = cell.GetComponent<Cell>();
                cells[i, j].Init(i, j, numRows);
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

    private bool CanPlace(Block block, Vector3 pos)
    {
        foreach (Transform mino in block.minos)
        {
            if (!InBounds(pos + mino.localPosition) || IsOccupied(pos + mino.localPosition))
            {
                return false;
            }
        }
        return true;
    }

    private void AddShadow(Cell cell)
    {
        cellsInShadow.Add(cell);
        cell.SetInShadow(true);
    }

    private void ClearShadows()
    {
        foreach (Cell cell in cellsInShadow)
        {
            cell.SetInShadow(false);
        }
        cellsInShadow.Clear();
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
                AddShadow(cell);

                if (CheckCol(cell.c))
                {
                    for(int row = 0; row < numRows; row++)
                    {
                        AddShadow(cells[cell.c, row]);
                    }
                }
                if (CheckRow(cell.r))
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        AddShadow(cells[col, cell.r]);
                    }
                }
            }
        }
    }

    public bool CheckPlaceable(Block block)
    {
        bool isPlaceable = false;
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                if (CanPlace(block, BoardToWorld(i, j)))
                {
                    isPlaceable = true;
                }
            }
        }
        return isPlaceable;
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
            cell.SetOccupied(true);

            if (CheckCol(cell.c) && !colsToClear.Contains(cell)) colsToClear.Add(cell);
            if (CheckRow(cell.r) && !rowsToClear.Contains(cell)) rowsToClear.Add(cell);
        }

        foreach (Cell cell in colsToClear) ClearCol(cell.c);
        foreach (Cell cell in rowsToClear) ClearRow(cell.r);

        OnBlockPlaced.Invoke(block.value, block.spawnLocation, colsToClear.Count + rowsToClear.Count, colsToClear.Count * numRows + rowsToClear.Count * numCols);

        Destroy(block.gameObject);
    }

    private bool CheckCol(int col)
    {
        for (int row = 0; row < numRows; row++)
        {
            if (!cells[col, row].InShadow() && !cells[col, row].IsOccupied()) return false;
        }
        return true;
    }

    private bool CheckRow(int row)
    {
        for (int col = 0; col < numCols; col++)
        {
            if (!cells[col, row].InShadow() && !cells[col, row].IsOccupied()) return false;
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