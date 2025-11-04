using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int c; public int r; 
    public int maxRows;
    public bool isOccupied;
    public bool isShadow;

    public Sprite cellSprite;
    public Sprite blockSprite;

    public void Init(int col, int row, int maxRows)
    {
        c = col;
        r = row;
    }
    void Start()
    {
        isOccupied = false;
        isShadow = false;
        GetComponent<SpriteRenderer>().sprite = cellSprite;
    }

    void Update()
    {
        if (isOccupied)
        {
            GetComponent<SpriteRenderer>().sprite = blockSprite;
        }

        if (isShadow)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        isShadow = false;

    }

    public int GetIndex()
    {
        return (c * maxRows) + r;
    }

    public void Clear()
    {
        isOccupied = false;
        GetComponent<SpriteRenderer>().sprite = cellSprite;
    }

    public override string ToString()
    {
        return $"Cell: (c={c}, r={r})";
    }
}
