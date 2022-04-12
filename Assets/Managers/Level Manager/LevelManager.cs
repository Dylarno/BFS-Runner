using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Grid grid;
    public Tilemap l1Tilemap;

    private GameObject portal;
    public List<GameObject> coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public TileBase GetTileAtPosition(int x, int y)
    {
        return l1Tilemap.GetTile(grid.WorldToCell(new Vector2(x, y)));
    }

    void Start()
    {
        portal = GameObject.Find("Portal");

        var coinsArray = GameObject.FindGameObjectsWithTag("Coin");
        coins = coinsArray.ToList();
    }

    void Update()
    {
        foreach (var coin in coins.ToList())
        {
            if (coin == null)
            {
                coins.Remove(coin);

                if (coins.Count == 0)
                    OpenPortal();
            }
        }
    }

    private void OpenPortal()
    {
        portal.GetComponent<CircleCollider2D>().enabled = true;
        portal.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }
}
