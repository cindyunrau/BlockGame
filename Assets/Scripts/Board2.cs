using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board2 : MonoBehaviour
{


    public Tile tilePrefab;
    public int rows = 3;
    public int cols = 4;

    public readonly Tile[,] grid = new Tile[4, 3];
    private readonly bool[,] occupied = new bool[4, 3];

    private readonly List<Tile> _activeGhost = new();

    private void Start()
    {
        InitGrid(rows, cols);
        //foreach (Tile t in grid)
        //{
        //    print(t);
        //}
    }

    private void InitGrid(int rows, int cols)
    {

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Tile tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation, transform);
                tileClone.name = "(" + i + "," + j + ")";
                tileClone.row = j;
                tileClone.col = i;
                grid[i, j] = tileClone;
            }
        }
        print(grid[0, 0]);
        //foreach(tile t in grid)
        //{
        //    print(t);
        //}


        //for (int i = 0; i < cols; i++)
        //{
        //    for (int j = 0; j < rows; j++)
        //    {
        //        Tile tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation, transform);
        //        tileClone.name = "(" + i + "," + j + ")";
        //        tileClone.row = j;
        //        tileClone.col = i;
        //        occupied[i, j] = false;
        //    }
        //}
    }

    // Given a point, returns closest valid Anchor or null if none
    public Tile GetClosestTile(Vector3 point)
    {
        //foreach (Tile t in grid)
        //{
        //    print(t);
        //}
        print(grid[0, 0]);
        float distance = 100;
        Collider2D closest = null;
        Collider2D[] gridCollisions = Physics2D.OverlapCircleAll(new Vector2(point.x, point.y), 2, LayerMask.GetMask("ActiveGrid"));
        foreach (Collider2D col in gridCollisions)
        {
            if (Vector3.Distance(col.ClosestPoint(point), point) < distance)
            {
                closest = col;
                distance = Vector3.Distance(col.ClosestPoint(point), point);
            }
        }
        if (closest)
        {
            return closest.gameObject.GetComponent<Tile>();
        }
        return null;
    }

    public void Hover(Vector3 blockPosition)
    {
        print(grid[0, 0]);
        ClearGhost();
        Tile closestTile = GetClosestTile(blockPosition);
        if (closestTile)
        {
            ShowGhost(closestTile);
        }

    }

    private void ShowGhost(Tile tile)
    {
        _activeGhost.Add(tile);

        foreach (Tile ghostTile in _activeGhost)
        {
            if (ghostTile)
            {
                ghostTile.ghost = true;
            }
        }
    }

    public void ClearGhost()
    {
        foreach (Tile ghostTile in _activeGhost)
        {
            if (ghostTile)
            {
                ghostTile.ghost = false;
            }
        }
        _activeGhost.Clear();
    }

    public void AttemptPlaceBlock(Block block)
    {
        print(grid[0, 0]);
        //ClearGhost();
        //Tile closestTile = GetClosestTile(block.transform.position);
        //if (closestTile)
        //{
        //    block.SetPosition(closestTile.transform.position);
        //    closestTile.Deactivate();
        //    CheckClear(closestTile.row, closestTile.col);
        //    // send block placed event to gm 
        //}
        //else
        //{
        //    block.SetOriginalPosition();
        //}

    }
    private void CheckClear(int row, int col)
    {
        if (CheckCol(col))
        {
            print("COL CLEAR");
        }
    }

    // Returns true if the column is all filled in, otherwise false
    private bool CheckCol(int index)
    {
        for (int i = 0; i < rows; i++)
        {
            print("grid" + grid);
            print(grid.Length);
            foreach (Tile t in grid)
            {
                print(t);
            }
            if (grid[index, i].IsEmpty())
            {
                print(index + ":" + i + " " + occupied[index, i]);
                return false;
            }
        }
        return true;
    }

}

