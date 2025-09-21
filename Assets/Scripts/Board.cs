using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Tile tilePrefab;
    private readonly Tile[,] tiles = new Tile[4, 3];

    private readonly List<Tile> _activeGhost = new();


    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Tile tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation, transform);
                tileClone.name = "(" + i + "," + j + ")";
                tileClone.row = j;
                tileClone.col = i;
                tiles[i, j] = tileClone;
            }
        }
    }

    public void Hover(List<Transform> blocks)
    {
        ClearGhost();
        List<Tile> projectedBlock = new();
        foreach (Transform block in blocks)
        {
            Tile projectedTile = GetClosestTile(block.position);
            projectedBlock.Add(projectedTile);
        }

        projectedBlock.RemoveAll(item => item == null);
        if (blocks.Count == projectedBlock.Distinct().Count())
        {
            print("STSRT");
            foreach (Tile ghostTile in projectedBlock)
            {
                print(ghostTile);
                ShowGhost(ghostTile);
            }
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


    // Given a point, returns closest valid Anchor or null if none
    public Tile GetClosestTile(Vector3 point)
    {
        float distance = 100;
        Collider2D closest = null;
        Collider2D[] gridCollisions = Physics2D.OverlapCircleAll(new Vector2(point.x, point.y), (1/2), LayerMask.GetMask("ActiveGrid"));
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

    public bool AttemptPlaceBlock(List<Transform> blocks)
    {
        List<Tile> projectedBlock = new();
        foreach (Transform block in blocks)
        {
            Tile projectedTile = GetClosestTile(block.position);
            projectedBlock.Add(projectedTile);
        }

        projectedBlock.RemoveAll(item => item == null);
        if (blocks.Count == projectedBlock.Distinct().Count())
        {
            for(int i = 0; i< blocks.Count; i++)
            {
                blocks[i].position = projectedBlock[i].transform.position;
            }
            return true;
        }
        return false;

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
        print("CHECK");
        for(int i = 0; i < 3; i++)
        {
            print(index + ":" + i + " ->" + tiles[index, i].IsEmpty());
            if (tiles[index, i].IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

}

