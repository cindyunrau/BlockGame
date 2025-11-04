using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private Board board;

    public List<Block> blockTypes;
    public Block singleBlock;
    public Block tBlock;

    public int score = 0;
    public int streak = 0;

    private List<Transform> spawnPoints = new();
    private List<Block> _active = new();

    public TextMeshProUGUI scoreUI;
    public GameObject button;

    void Start()
    {
        board = FindObjectOfType<Board>(true);

        board.OnBlockPlaced += HandleBlockPlaced;

        foreach(Transform sp in transform)
        {
            spawnPoints.Add(sp);
            _active.Add(null);
        }

        NewGame();
    }

    public void NewGame()
    {
        score = 0; streak = 0;
        UpdateUI();

        SpawnAll();
    }

    public void Reset()
    {
        ResetTray();
        board.Reset();
        button.SetActive(false);

        NewGame();
    }

    private void GameOver()
    {
        print("Game over");
        scoreUI.text = $"No Moves Left ;(\nYour final score was: {score}";
        button.SetActive(true);
    }

    private void HandleBlockPlaced(int blockValue, int spawnLocation, int numLines, int blocksRemoved)
    {
        score += blockValue;
        _active[spawnLocation] = null;

        HandleLinesCleared(numLines, blocksRemoved);

        if (IsTrayEmpty()) SpawnAll();
        UpdateUI();

        if (CheckNoMoves()) GameOver();
    }

    private void ResetTray()
    {
        foreach(Block block in _active)
        {
            if(block) Destroy(block.gameObject);
        }

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
    }

    private void UpdateUI()
    {
        scoreUI.text = $"{score}";
    }

    private void SpawnBlockAt(int i)
    {
        _active[i] = Instantiate(ChooseBlock(), spawnPoints[i].position, ChooseRotation());
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
        bool noMoves = true;
        foreach (Block block in _active)
        {
            if (block)
            {
                if (board.CheckPlaceable(block))
                {
                    block.SetPlaceable(true);
                    noMoves = false;
                }
                else
                {
                    block.SetPlaceable(false);
                }
            }
        }
        return noMoves;
    }

    private Block ChooseBlock()
    {
        int rng = Random.Range(0, blockTypes.Count);
        return blockTypes[rng];
    }

    private Quaternion ChooseRotation()
    {
        int rng = Random.Range(0, 4);
        return Quaternion.Euler(0, 0, (float)(rng * 90));
    }
}
