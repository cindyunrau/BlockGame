using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private Board board;

    public List<Block> blockShapes;
    public Block singleBlock;
    public Block tBlock;

    public int score = 0;
    public int streak = 0;

    private List<SpawnPoint> traySpawnPoints = new();
    public List<SpawnPoint> shopSpawnPoints = new();

    public Canvas canvas;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI streakUI;
    public TextMeshProUGUI popupText;
    public GameObject button;

    public Camera mainCamera;

    public ResourceManager rm;

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

    private void HandleBlockPlaced(Block block, List<int> colsCleared, List<int> rowsCleared)
    {
        int numLines = colsCleared.Count + rowsCleared.Count;
        int blocksRemoved = colsCleared.Count * Board.numRows + rowsCleared.Count * Board.numCols;
        score += block.value;
        // ??? diff value and cost?
        streak -= block.cost;
        block.GetComponentInParent<SpawnPoint>().SetActiveBlock(null);
        
        int scoreAdded = HandleLinesCleared(numLines, blocksRemoved);

        if (scoreAdded > 0)
        {
            int xpos = colsCleared.Count > 0 ? (int)colsCleared.Average() : 0;
            int ypos = rowsCleared.Count > 0 ? (int)rowsCleared.Average() : 0;
            Vector3 averageClearPosition = board.BoardToWorld(xpos,ypos);//new Vector3(xpos,ypos, block.transform.position.z);
            TriggerTextFade(scoreAdded, averageClearPosition);
        }

        if (IsTrayEmpty()) SpawnAll();
        SpawnShopItems();
        UpdateUI();

        Destroy(block.gameObject);

        if (CheckNoMoves()) GameOver();
    }

    private int CalculatePoints()
    {
        int points = 0;

        return points;
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

    private int HandleLinesCleared(int numLines, int blocksRemoved)
    {
        int basePoints = 2 * blocksRemoved;
        int combo = numLines * 10;
        streak = (numLines > 0) ? streak + 1 : 0;
        int scoreToAdd = (basePoints + combo) * streak;
        score += scoreToAdd;

        return scoreToAdd;
    }

    public void TriggerTextFade(int score, Vector3 pos)
    {
        TextMeshProUGUI text = Instantiate<TextMeshProUGUI>(popupText,canvas.transform);
        text.transform.position = mainCamera.WorldToScreenPoint(pos);
        text.text = $"{score}";
        GetComponent<TextFade>().TriggerFadeSequence(text);
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
            Block block = Instantiate(ChooseBlock(), sp.transform.position, ChooseRotation(), sp.transform);
            ResourceManager.Tile tile = rm.foods[Random.Range(0, rm.foods.Count)];
            block.Init(tile.name, tile.rawSprite);
            sp.SetActiveBlock(block);

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
        Block block = blockShapes[Random.Range(0, blockShapes.Count)];

        return block;
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
