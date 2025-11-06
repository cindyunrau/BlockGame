using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour
{
    public List<Transform> minos = new();

    public int value = 0;
    public int cost = 0;
    public int spawnLocation;
    public bool placeable = true;
    
    private Vector3 offset;
    private Vector3 originalPosition;
    private Board board;

    private void Awake()
    {
        originalPosition = transform.position;
        board = FindObjectOfType<Board>(true);

        foreach (Transform child in transform)
        {
            minos.Add(child);
            value += 1;
        }
    }

    void OnMouseDown()
    {
        if (placeable)
        {
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
        }
    }

    void OnMouseDrag()
    {
        if (placeable)
        {
            Vector3 currentScreenPoint = new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 currentWorldPoint = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;

            transform.position = currentWorldPoint;
            board.Hover(minos);
        }
    }

    void OnMouseUp()
    {
        if (placeable)
        {
            if (!board.TryPlaceBlock(this))
            {
                SetOriginalPosition();
            }
        }
    }

    private void SetOriginalPosition()
    {
        transform.position = originalPosition;
    }

    public void SetColor(Color color)
    {
        foreach(Transform mino in minos)
        {
            mino.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetPlaceable(bool value)
    {
        placeable = value;
        if (!placeable)
        {
            SetColor(Color.gray);
        }
        else
        {
            SetColor(Color.white);
        }
    }
}
