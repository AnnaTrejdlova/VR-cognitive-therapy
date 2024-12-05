using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacing : MonoBehaviour, ITileInteractionStrategy {
    public void OnTileClick(Tile tile) {
        PlaceObjectOnTile(tile);
    }

    public void OnTileHover(Tile tile) {
        tile.ToggleHighlightMaterial(true);
    }

    public void OnTileUnhover(Tile tile) {
        tile.ToggleHighlightMaterial(false);
    }

    void PlaceObjectOnTile(Tile tile) {
        if (EditorObjectManager.Instance.GetSelectedObject() == null) {
            return;
        }
        tile.AddObjectToTile(EditorObjectManager.Instance.GetSelectedObject());
        TileManager.Instance.SetActiveTile(tile); 
    }
}
