using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSet : MonoBehaviour
{
    private List<Transform> blocks = new();
    private Vector3 offset;

    private Board board;

    public Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
        board = GameObject.FindObjectOfType<Board>(true);

        foreach (Transform child in transform)
        {
            blocks.Add(child);
        }
    }

    void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

    void OnMouseDrag()
    { 
        Vector3 currentScreenPoint = new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        Vector3 currentWorldPoint = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;

        transform.position = currentWorldPoint;
        board.Hover(blocks);

    }

    void OnMouseUp()
    {
        if (!board.AttemptPlaceBlock(blocks))
        {
            SetOriginalPosition();
        }

    }

    public void SetOriginalPosition()
    {
        transform.position = originalPosition;
    }
}
