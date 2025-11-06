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

    private List<SpawnPoint> traySpawnPoints = new();
    public List<SpawnPoint> shopSpawnPoints = new();

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI streakUI;
    public GameObject button;


    void Start()
    {
        board = FindObjectOfType<Board>(true);

        board.OnBlockPlaced += HandleBlockPlaced;

        foreach(Transform child in transform)
        {
            traySpawnPoints.Add(child.GetComponent<SpawnPoint>());
        }

        NewGame();
    }

    public void NewGame()
    {
        score = 0; streak = 0;
        UpdateUI();

        SpawnAll();
        SpawnShopItems();

        CheckNoMoves();
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

    private void HandleBlockPlaced(Block block, int numLines, int blocksRemoved)
    {
        score += block.value;
        streak -= block.cost;
        block.GetComponentInParent<SpawnPoint>().SetActiveBlock(null);
        Destroy(block.gameObject);

        HandleLinesCleared(numLines, blocksRemoved);

        if (IsTrayEmpty()) SpawnAll();
        SpawnShopItems();
        UpdateUI();

        if (CheckNoMoves()) GameOver();
    }

    private void ResetTray()
    {
        foreach(SpawnPoint sp in traySpawnPoints)
        {
            if(sp.GetActiveBlock()) Destroy(sp.GetActiveBlock().gameObject);
        }

    }

    private bool IsTrayEmpty()
    {
        foreach (SpawnPoint sp in traySpawnPoints)
        {
            if (sp.GetActiveBlock())
            {
                //print(sp.GetActiveBlock());
                return false;
            }
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
        streakUI.text = $"{streak}";
    }

    private void SpawnAll()
    {
        foreach (SpawnPoint sp in traySpawnPoints)
        {
            sp.SetActiveBlock(Instantiate(ChooseBlock(), sp.transform.position, ChooseRotation(), sp.transform));
        }
    }

    private bool CheckNoMoves()
    {
        bool noMoves = true;
        Block curr;

        // Check Tray Blocks
        foreach (SpawnPoint sp in traySpawnPoints)
        {
            curr = sp.GetActiveBlock();
            if (curr)
            {
                if (board.CheckPlaceable(curr))
                {
                    curr.SetPlaceable(true);
                    noMoves = false;
                }
                else
                {
                    curr.SetPlaceable(false);
                }
            }
        }

        // Check Shop Blocks
        foreach (SpawnPoint sp in shopSpawnPoints)
        {
            curr = sp.GetActiveBlock();
            if (curr)
            {
                if (board.CheckPlaceable(curr) && streak >= curr.cost)
                {
                    curr.SetPlaceable(true);
                    noMoves = false;
                }
                else
                {
                    curr.SetPlaceable(false);
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

    // Shop
    private bool CheckShopEmpty()
    {
        foreach(SpawnPoint sp in shopSpawnPoints)
        {
            if (!sp) return true;
        }
        return false;
    }

    private void SpawnShopItems()
    {
        foreach(SpawnPoint sp in shopSpawnPoints)
        {
            if (!sp.GetActiveBlock())
            {
                sp.SetActiveBlock(Instantiate(singleBlock, sp.transform.position, sp.transform.rotation, sp.transform));
            }
        }
    }
}
