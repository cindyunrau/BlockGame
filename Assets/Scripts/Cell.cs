using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int c; public int r; 
    public int maxRows;
    private bool isOccupied;
    private bool grillInShadow;


    private Sprite grillSprite;
    private GameObject grillObj;

    public Mino mino;


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
    
    }

    public void SetOccupied(bool value, Mino mino)
    {
        isOccupied = value;
        this.mino = mino;
        if (isOccupied)
        {
            mino.transform.SetParent(this.transform);
            mino.transform.position = this.transform.position;
        }
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetInShadow(bool value)
    {
        SetGrillInShadow(value);
        if(mino) mino.SetInShadow(value);
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



    public bool GrillInShadow()
    {
        return grillInShadow;
    }

    public int GetIndex()
    {
        return (c * maxRows) + r;
    }

    public void Clear()
    {
        if (mino) Destroy(mino.gameObject);
        SetOccupied(false, null);
    }

    public override string ToString()
    {
        return $"Cell: (c={c}, r={r})";
    }
}
