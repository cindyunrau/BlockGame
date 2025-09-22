using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private Board board;

    public Block singleBlock;
    public Block tBlock;

    public int score = 0;
    public int combo = 0;

    private List<Transform> spawnPoints = new();
    private List<Block> _active = new();

    void Start()
    {
        board = FindObjectOfType<Board>(true);

        board.OnBlockPlaced += HandleBlockPlaced;
        board.OnLinesCleared += HandleLinesCleared;

        foreach(Transform sp in transform)
        {
            spawnPoints.Add(sp);
            _active.Add(null);
        }

        NewGame();
    }

    private void NewGame()
    {
        score = 0; combo = 0;
        SpawnAll();

    }

    private void HandleBlockPlaced(int blockValue)
    {
        score += blockValue;

        if (IsTrayEmpty()) SpawnAll();

        CheckNoMoves();
    }

    private bool IsTrayEmpty()
    {
        foreach (Transform sp in spawnPoints)
        {
            if (!IsSpawnPointEmpty(sp)) return false;
        }
        return true;
    }

    private bool IsSpawnPointEmpty(Transform sp)
    {
        if (Physics2D.OverlapPoint(sp.position)) return false;
        return true;
    }

    private void HandleLinesCleared(int numLines)
    {
        int basePoints = 100 * numLines;
        combo = (numLines > 0) ? combo + 1 : 0;
        score += basePoints * combo;
    }

    private void SpawnBlockAt(int i)
    {
        _active[i] = ChooseBlock();
        Instantiate(_active[i], spawnPoints[i].position, spawnPoints[i].rotation);
    }

    private void SpawnAll()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            SpawnBlockAt(i);
        }
    }

    private void CheckNoMoves()
    {
        return;
    }

    private Block ChooseBlock()
    {
        int rng = Random.Range(1, 3);
        if (rng == 1) return singleBlock;
        if (rng == 2) return tBlock;
        else return singleBlock;
    }
}
