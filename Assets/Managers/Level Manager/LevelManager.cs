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

    public TileBase GetTileAtPosition(float x, float y)
    {
        return l1Tilemap.GetTile(grid.WorldToCell(new Vector2(x, y)));
    }

    public TileBase GetTileAtPosition(Vector3 position)
    {
        return l1Tilemap.GetTile(grid.WorldToCell(position));
    }

    public Vector2 GetLocalPosition(Vector3 worldPosition)
    {
        Debug.Log(l1Tilemap.WorldToLocal(worldPosition));
        return l1Tilemap.WorldToLocal(worldPosition);
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
