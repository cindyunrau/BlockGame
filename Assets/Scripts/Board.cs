using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<int> OnBlockPlaced;
    public event Action<int> OnLinesCleared;

    public GameObject tilePrefab;

    public int numCols = 4;
    public int numRows = 3;

    public Color activeColor = Color.white;
    public Color ghostColor = Color.gray;
    public Sprite tileSprite;
    public Sprite blockSprite;

    private int[,] cellIndicies;
    private bool[,] occupied;

    private readonly List<int> _activeGhosts = new();

    private struct Cell
    {
        public int c; public int r;

        public override string ToString()
        {
            return $"Cell: (c={c}, r={r})";
        }
    }

    private void Start()
    {
        cellIndicies = new int[numCols, numRows];
        occupied = new bool[numCols, numRows];

        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Instantiate(tilePrefab, new Vector3(i, j, 0) + transform.position, transform.rotation, transform);
                cellIndicies[i, j] = (i * numRows) + j;
                occupied[i, j] = false;
            }
        }
    }

    private void Update()
    {
        foreach(int i in _activeGhosts)
        {
            ShowGhost(i);
        }
    }

    // Converts World Coordinates (float) to Board Coordinates (int)
    private Cell WorldToBoard(Vector3 world)
    {
        Cell result = new();
        result.c = (int)Math.Round(world.x - transform.position.x);
        result.r = (int)Math.Round(world.y - transform.position.y);
        return result;
    }

    // Converts Board Coordinates (int) to World Coordinates (float)
    private Vector3 BoardToWorld(Cell board)
    {
        Vector3 result = new();
        result.x = transform.position.x + board.c;
        result.y = transform.position.y + board.r;
        result.z = transform.position.z;
        return result;
    }

    private bool InBounds(Vector3 coords)
    {
        Cell cell = WorldToBoard(coords);
        if(cell.c < numCols && cell.c >=0 && cell.r < numRows && cell.r >= 0)
        {
            return true;
        }
        return false;
    }

    private bool IsOccupied(Vector3 coords)
    {
        Cell cell = WorldToBoard(coords);
        return occupied[cell.c, cell.r];
    }

    private bool CanPlace(List<Transform> minos)
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

    public void Hover(List<Transform> minos)
    {
        ClearGhosts();
        if (CanPlace(minos))
        {
            foreach (Transform mino in minos)
            {
                Cell cell = WorldToBoard(mino.position);
                _activeGhosts.Add(cellIndicies[cell.c,cell.r]);
            }
        }
    }

    private void ShowGhost(int childIndex)
    {
        SetCellColor(childIndex, ghostColor); 
    }

    private void ClearGhosts()
    {
        foreach(int childIndex in _activeGhosts)
        {
            SetCellColor(childIndex, activeColor); 
        }
        _activeGhosts.Clear();
    }

    public bool TryPlaceBlock(Block block)
    {
        ClearGhosts();
        if (CanPlace(block.minos))
        {
            PlaceBlock(block);

            return true;
        }
        return false;
    }

    private void PlaceBlock(Block block)
    {
        List<int> colsToClear = new();
        List<int> rowsToClear = new();

        foreach (Transform mino in block.minos)
        {
            Cell cell = WorldToBoard(mino.transform.position);
            occupied[cell.c, cell.r] = true;

            SetCellSprite(cell, blockSprite);

            if (CheckCol(cell.c)) colsToClear.Add(cell.c);
            if (CheckRow(cell.r)) rowsToClear.Add(cell.r);
        }

        foreach (int index in colsToClear) ClearCol(index);
        foreach (int index in rowsToClear) ClearRow(index);

        OnBlockPlaced.Invoke(block.value);
        OnLinesCleared.Invoke(colsToClear.Count + rowsToClear.Count);

        Destroy(block.gameObject);
    }

    //Returns true if the column at index is all filled in, otherwise false
    private bool CheckCol(int index)
    {
        for (int r = 0; r < numRows; r++)
        {
            if (!occupied[index, r])
            {
                return false;
            }
        }
        return true;
    }

    //Returns true if the row at index is all filled in, otherwise false
    private bool CheckRow(int index)
    {
        for (int c = 0; c < numCols; c++)
        {
            if (!occupied[c, index])
            {
                return false;
            }
        }
        return true;
    }

    private void ClearCol(int index)
    {
        Debug.Log($"column {index} clear");
        Cell cell = new();
        for (int r = 0; r < numRows; r++)
        {
            cell.c = index; cell.r = r;
            ClearCell(cell);
        }
    }

    private void ClearRow(int index)
    {
        Debug.Log($"row {index} clear");
        Cell cell = new();
        for (int c = 0; c < numCols; c++)
        {
            cell.c = c; cell.r = index;
            ClearCell(cell);
        }
    }

    private void ClearCell(Cell coords)
    {
        occupied[coords.c, coords.r] = false;
        SetCellSprite(coords, tileSprite);
    }

    private void SetCellSprite(Cell coords, Sprite sprite)
    {
        transform.GetChild(cellIndicies[coords.c, coords.r]).GetComponent<SpriteRenderer>().sprite = sprite;
    }
   

    private void SetCellColor(int index, Color color)
    {
        transform.GetChild(index).GetComponent<SpriteRenderer>().color = color;
    }
}

