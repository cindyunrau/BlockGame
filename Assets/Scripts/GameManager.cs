using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int score;

    public Block blockPrefab;

    void Start()
    {
        AddBlock(-2, -2);
        AddBlock(0, -2);
        AddBlock(2, -2);
        AddBlock(-1, -3);
        AddBlock(1, -3);
    }


    public void AddBlock(int x, int y)
    {
        Instantiate(blockPrefab, new Vector3(x, y, 0), transform.rotation);
    }
}
