using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private GameManager gm;
    public int cost = 0;

    private Block activeBlock;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>(true);
    }

    private void FixedUpdate()
    {
        //if (activeBlock)
        //{
        //    // causing unplaceable blocks to be seen as placeable
        //    if (activeBlock.placeable && gm.streak < cost)
        //    {
        //        activeBlock.SetPlaceable(false);
        //    }
        //}
        
    }

    public Block GetActiveBlock()
    {
        return activeBlock;
    }

    public void SetActiveBlock(Block block)
    {
        activeBlock = block;
        if(block) block.cost = cost;
    }
}
