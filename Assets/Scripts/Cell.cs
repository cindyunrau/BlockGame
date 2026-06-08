using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int c; public int r; 
    public int maxRows;
    private bool isOccupied;
    private bool grillInShadow;


    private Sprite GSDefault;
    private Sprite GSFire;
    private GameObject grillObj;

    public Mino mino;


    public void Init(int col, int row, Sprite gSpr, Sprite gsfire = null)
    {
        c = col;
        r = row;
        GSDefault = gSpr;
        GSFire = gsfire;

        grillObj = transform.GetChild(0).gameObject;
        grillObj.GetComponent<SpriteRenderer>().sprite = GSDefault;
        
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
            grillObj.GetComponent<SpriteRenderer>().sprite = GSFire;
        }
        else
        {
            grillObj.GetComponent<SpriteRenderer>().sprite = GSDefault;
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
