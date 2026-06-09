using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour
{
    public List<Mino> minos = new();

    public int value = 0;
    public int cost = 0;
    public int spawnLocation;
    public bool placeable = true;
    
    private Vector3 offset;
    private Vector3 originalPosition;
    private Board board;

    private string activeSortingLayer = "ActiveBlock";
    private string defaultSortingLayer = "Blocks";

    private void Awake()
    {
        originalPosition = transform.position;
        board = FindObjectOfType<Board>(true);

        foreach (Transform child in transform)
        {
            minos.Add(child.gameObject.GetComponent<Mino>());
            value += 1;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        foreach (Mino mino in minos)
        {
            mino.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    void OnMouseDown()
    {
        if (placeable)
        {
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
        }
        foreach (Mino mino in minos)
        {
            mino.GetComponent<SpriteRenderer>().sortingLayerName = activeSortingLayer;
        }
    }

    void OnMouseDrag()
    {
        if (placeable)
        {
            Vector3 currentScreenPoint = new(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            Vector3 currentWorldPoint = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
            foreach (Mino mino in minos) mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4f);
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
        foreach (Mino mino in minos)
        {
            mino.GetComponent<SpriteRenderer>().sortingLayerName = defaultSortingLayer;
            mino.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
        
    }

    private void SetOriginalPosition()
    {
        transform.position = originalPosition;
    }

    public void SetColor(Color color)
    {
        foreach(Mino mino in minos)
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
