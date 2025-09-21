using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    private Board board;
    private bool isDragging = false;

    public float gridSize = 1f;

    private Vector3 offset;
    private Vector3 originalPosition;

    public Vector3[] blocks;


    private void Start()
    {
        originalPosition = transform.position;
        board = GameObject.FindObjectOfType<Board>(true);
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentScreenPoint = new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 currentWorldPoint = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;

            transform.position = currentWorldPoint;

            //board.Hover(transform.position);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        //board.AttemptPlaceBlock(this); 
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public void SetOriginalPosition()
    {
        SetPosition(originalPosition);
    }

    //void PlaceBlock()
    //{
    //    transform.position = currentTile.transform.position;
    //    board.ClearGhost();
        
    //}

    //void SetCurrentTile(Tile newTile)
    //{
    //    if (currentTile)
    //    {
    //        currentTile.InShadow(false);
    //    }
    //    currentTile = newTile;
    //    if (currentTile)
    //    {
    //        currentTile.InShadow(true);
    //    }
    //}

    //Tile ClosestTile(Vector2 center, float radius) 
    //{
    //    float distance = 100;
    //    Tile closest = null;
    //    Collider2D[] gridCollisions = Physics2D.OverlapCircleAll(center, radius, LayerMask.GetMask("ActiveGrid"));
    //    foreach(Collider2D col in gridCollisions)
    //    {
    //        if (Vector3.Distance(col.ClosestPoint(transform.position), transform.position) < distance)
    //        {
    //            closest = col.gameObject.GetComponent<Tile>();
    //            distance = Vector3.Distance(col.ClosestPoint(transform.position), transform.position);
    //        }
    //    }
    //    return closest;
    //}
}
