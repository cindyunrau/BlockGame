using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int c; public int r; 
    public int maxRows;
    private bool isOccupied;
    private bool inShadow;

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
        inShadow = false;
        GetComponent<SpriteRenderer>().sprite = cellSprite;
    }

    void FixedUpdate()
    {
        SetInShadow(false);
    }

    public void SetOccupied(bool value)
    {
        isOccupied = value;
        if (isOccupied)
        {
            GetComponent<SpriteRenderer>().sprite = blockSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = cellSprite;
        }
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetInShadow(bool value)
    {
        inShadow = value;
        if (inShadow)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public bool InShadow()
    {
        return inShadow;
    }

    public int GetIndex()
    {
        return (c * maxRows) + r;
    }

    public void Clear()
    {
        SetOccupied(false);
    }

    public override string ToString()
    {
        return $"Cell: (c={c}, r={r})";
    }
}
