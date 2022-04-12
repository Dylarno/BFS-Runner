using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Grid grid;
    public Tilemap l1Tilemap;

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
    }

    void Update()
    {
        
    }
}
