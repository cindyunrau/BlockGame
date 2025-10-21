using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Board : MonoBehaviour
{
    public event Action<int, int, int, int> OnBlockPlaced;

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

    public void Reset()
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = tileSprite;
            occupied[(int) child.localPosition.x, (int)child.localPosition.y] = false;
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
        Cell cell = WorldToBoard(coords);
        if (cell.c < numCols && cell.c >= 0 && cell.r < numRows && cell.r >= 0)
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

    //private bool InBounds(Vector3 coords)
    //{
    //    if ((int) coords.x < numCols && (int)coords.x >= 0 && (int)coords.y < numRows && (int)coords.y >= 0)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //private bool IsOccupied(Vector3 coords)
    //{
    //    return occupied[(int) coords.x, (int)coords.y];
    //}

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

    private bool CanPlace(Block block, Vector3 pos)
    {
        foreach (Transform mino in block.minos)
        {
            if (!InBounds(pos + mino.localPosition) || IsOccupied(pos + mino.localPosition))
            {
                print("Cant place mino at: " + (pos + mino.localPosition));
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

    public bool CheckPlaceable(Block block)
    {
        bool isPlaceable = false;
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                print("Trying to place "+ block + " at: " + i + ":" + j);
                if (CanPlace(block, BoardToWorld(i,j)))
                {
                    isPlaceable = true;
                }
            }
        }
        if (isPlaceable)
        {
            block.SetColor(activeColor);
        }
        else
        {
            print("block not placeable" + block);
            block.SetColor(ghostColor);
        }
        return isPlaceable;
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

            SetCellSprite(cell.c, cell.r, blockSprite);

            if (CheckCol(cell.c) && !colsToClear.Contains(cell.c)) colsToClear.Add(cell.c);
            if (CheckRow(cell.r) && !rowsToClear.Contains(cell.r)) rowsToClear.Add(cell.r);
        }

        foreach (int index in colsToClear) ClearCol(index);
        foreach (int index in rowsToClear) ClearRow(index);

        OnBlockPlaced.Invoke(block.value, block.spawnLocation, colsToClear.Count + rowsToClear.Count, colsToClear.Count * numRows + rowsToClear.Count * numCols);

        Destroy(block.gameObject);
    }

    //Returns true if the column at index is all filled in, otherwise false
    private bool CheckCol(int col)
    {
        for (int r = 0; r < numRows; r++)
        {
            if (!CheckCell(col, r)) return false;
        }
        return true;
    }

    //Returns true if the row at index is all filled in, otherwise false
    private bool CheckRow(int row)
    {
        for (int c = 0; c < numCols; c++)
        {
            if (!CheckCell(c, row)) return false;
        }
        return true;
    }

    //Returns true if the cell at [col,row] is all filled in, otherwise false
    private bool CheckCell(int col, int row)
    {
        if (occupied[col, row])
        {
            return true;
        }
        return false;
    }

    private void ClearCol(int col)
    {
        Debug.Log($"column {col} clear");
        for (int r = 0; r < numRows; r++)
        {
            ClearCell(col, r);
        }
    }

    private void ClearRow(int row)
    {
        Debug.Log($"row {row} clear");
        for (int c = 0; c < numCols; c++)
        {
            ClearCell(c,row);
        }
    }

    private void ClearCell(int c, int r)
    {
        occupied[c, r] = false;
        SetCellSprite(c, r, tileSprite);
    }

    private void SetCellSprite(int c, int r, Sprite sprite)
    {
        transform.GetChild(cellIndicies[c, r]).GetComponent<SpriteRenderer>().sprite = sprite;
    }
   

    private void SetCellColor(int index, Color color)
    {
        transform.GetChild(index).GetComponent<SpriteRenderer>().color = color;
    }
}

