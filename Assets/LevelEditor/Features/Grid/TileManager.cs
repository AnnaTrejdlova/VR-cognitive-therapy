using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager> {

    public GameObject TilePrefab;
    public List<TileInteractionStrategyEntry> tileInteractionStrategyEntries = new List<TileInteractionStrategyEntry>();
    float gridYpos = 0f;
    float tileSize = 1f;
    List<Tile> tiles = new List<Tile>();
    Dictionary<Vector2Int, Tile> tileDictionary = new Dictionary<Vector2Int, Tile>();
    Tile activeTile; 

    protected override void Awake() {
        base.Awake();

        SetUpGrid(10, 10);
    }

    public List<Tile> GetTilesInLine(Vector2Int start, Vector2Int end) {
        List<Tile> tilesToReturn = new List<Tile>();

        Vector2Int startPos = start;
        Vector2Int endPos = end;

        int xDistance = Mathf.Abs(endPos.x - startPos.x);
        int yDistance = Mathf.Abs(endPos.y - startPos.y);

        if (xDistance != 0 && yDistance != 0) {
            // Not in the same row or column; snap to closest valid position
            if (xDistance > yDistance) {
                // Snap to the same row
                endPos = new Vector2Int(endPos.x, startPos.y);
            } else {
                // Snap to the same column
                endPos = new Vector2Int(startPos.x, endPos.y);
            }
        }

        int minX = Mathf.Min(startPos.x, endPos.x);
        int maxX = Mathf.Max(startPos.x, endPos.x);
        int minY = Mathf.Min(startPos.y, endPos.y);
        int maxY = Mathf.Max(startPos.y, endPos.y);

        // Determine line orientation
        bool isHorizontal = startPos.y == endPos.y;
        bool isVertical = startPos.x == endPos.x;

        foreach (Tile tile in tiles) {
            Vector2Int tilePos = tile.GetGridPosition();

            if (isHorizontal && tilePos.y == startPos.y && tilePos.x >= minX && tilePos.x <= maxX) {
                tilesToReturn.Add(tile);
            } else if (isVertical && tilePos.x == startPos.x && tilePos.y >= minY && tilePos.y <= maxY) {
                tilesToReturn.Add(tile);
            }
        }

        return tilesToReturn;
    }

    public List<Tile> GetTilesInLine(Tile start, Tile end) {
        List<Tile> tilesToReturn = new List<Tile>();

        Vector2Int startPos = start.GetGridPosition();
        Vector2Int endPos = end.GetGridPosition();

        return GetTilesInLine(startPos, endPos);
    }

    # region Tile interaction strategy

    public void TileClickHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileClick(tile);
    }

    public void TileHoverEnterHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileHover(tile);
    }

    public void TileHoverExitHandle(Tile tile) {
        FindStrategy(LevelEditorManager.Instance.GetState()).OnTileUnhover(tile);
    }

    #endregion

    #region Grid setup

    public void SetUpGrid(int width, int height) {
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j <= height; j++) {
                Vector3 position = new Vector3(i * tileSize, gridYpos, j * tileSize);
                SpawnTile(i, j, position);
            }
        }
    }

    void SpawnTile(int x, int y, Vector3 position) {
        Tile tile = Instantiate(TilePrefab, position, Quaternion.identity).GetComponent<Tile>();
        tile.SetGridPosition(new Vector2Int(x, y));
        tileDictionary[new Vector2Int(x, y)] = tile;
        tiles.Add(tile);
    }

    #endregion

    # region Support functions

    ITileInteractionStrategy FindStrategy(EditorState state) {
        foreach (var strat in tileInteractionStrategyEntries) {
            if (strat.state == state) {
                return strat.GetStrategy();
            }
        }
        print("no strategy found for this editor state!");
        return null;
    }

    #endregion

    public void SetActiveTile(Tile tile) {
        activeTile = tile;
    }

    public Tile GetActiveTile() {
        return activeTile;
    }

}

// Class for being able to assign strategies in editor

[Serializable]
public class TileInteractionStrategyEntry {
    public EditorState state;
    public MonoBehaviour strategy;

    public ITileInteractionStrategy GetStrategy() {
        return strategy as ITileInteractionStrategy;
    }
}