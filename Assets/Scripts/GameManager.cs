using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private Board board;

    public Block singleBlock;
    public Block tBlock;

    public int score = 0;
    public int streak = 0;

    private List<Transform> spawnPoints = new();
    private List<Block> _active = new();

    public TextMeshProUGUI scoreUI;

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
        score = 0; streak = 0;
        UpdateUI();
        SpawnAll();

    }

    private void HandleBlockPlaced(int blockValue, int spawnLocation)
    {
        score += blockValue;
        _active[spawnLocation] = null;

        if (IsTrayEmpty()) SpawnAll();

        UpdateUI();
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



    private void HandleLinesCleared(int numLines, int blocksRemoved)
    {
        int basePoints = 2 * blocksRemoved;
        int combo = numLines * 10;
        streak = (numLines > 0) ? streak + 1 : 0;
        score += (basePoints + combo) * streak ;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreUI.text = $"{score}";
    }

    private void SpawnBlockAt(int i)
    {
        _active[i] = Instantiate(ChooseBlock(), spawnPoints[i].position, spawnPoints[i].rotation);
        _active[i].spawnLocation = i;
    }

    private void SpawnAll()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            SpawnBlockAt(i);
        }
    }

    private bool CheckNoMoves()
    {
        bool movesLeft = false;
        foreach (Block block in _active)
        {
            if (block)
            {
                if (board.CheckPlaceable(block))
                {
                    movesLeft = true;
                } 
            }
        }
        return movesLeft;
    }

    private Block ChooseBlock()
    {
        int rng = Random.Range(1, 3);
        if (rng == 1) return singleBlock;
        if (rng == 2) return tBlock;
        else return singleBlock;
    }
}
