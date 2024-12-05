using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHUDui : MonoBehaviour {

    public void OnDeleteModeClick() {
        LevelEditorManager.Instance.ChangeState(EditorState.RemovingObjects);
    }

    public void OnWallModeClick() {
        LevelEditorManager.Instance.ChangeState(EditorState.PlacingWalls);
    }

    // A� se to bude d�lat jinak ne� p�es tla��tka v UI tak se to ot��en� bude �e�it mimo
    public void OnRotateObjectInTileRight() {
        TileManager.Instance.GetActiveTile().RotateAddedObject(turnRight: true);
    }

    public void OnRotateObjectInTileLeft() {
        TileManager.Instance.GetActiveTile().RotateAddedObject(turnRight: false);
    }
}
