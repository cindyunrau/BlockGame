using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int c; public int r; 
    public int maxRows;
    private bool isOccupied;
    private bool grillInShadow;
    private bool blockInShadow;

    private Sprite cellSprite;
    public Sprite blockSprite;
    private Sprite grillSprite;
    private GameObject grillObj;


    public void Init(int col, int row, Sprite gSpr)
    {
        c = col;
        r = row;
        grillSprite = gSpr;

        grillObj = transform.GetChild(0).gameObject;
        grillObj.GetComponent<SpriteRenderer>().sprite = grillSprite;
        
    }
    void Start()
    {
        isOccupied = false;
        grillInShadow = false;
        blockInShadow = false;
    
    }

    public void SetOccupied(bool value, Sprite sprite, float rot)
    {
        isOccupied = value;
        if (isOccupied)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            transform.rotation = Quaternion.Euler(0f, 0f, rot);
            grillObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = null;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            grillObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        }
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetInShadow(bool value)
    {
        SetGrillInShadow(value);
        SetBlockInShadow(value);
    }

    public void SetGrillInShadow(bool value)
    {
        grillInShadow = value;
        if (grillInShadow)
        {
            grillObj.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0.3f);
            grillObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            grillObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
            grillObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void SetBlockInShadow(bool value)
    {
        blockInShadow = value;
        if (blockInShadow)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1.0f);
        }
    }

    public bool GrillInShadow()
    {
        return grillInShadow;
    }

    public bool BlockInShadow()
    {
        return blockInShadow;
    }

    public int GetIndex()
    {
        return (c * maxRows) + r;
    }

    public void Clear()
    {
        SetOccupied(false, null, 0.0f);
    }

    public override string ToString()
    {
        return $"Cell: (c={c}, r={r})";
    }
}
