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

    // Až se to bude dìlat jinak než pøes tlaèítka v UI tak se to otáèení bude øešit mimo
    public void OnRotateObjectInTileRight() {
        TileManager.Instance.GetActiveTile().RotateAddedObject(turnRight: true);
    }

    public void OnRotateObjectInTileLeft() {
        TileManager.Instance.GetActiveTile().RotateAddedObject(turnRight: false);
    }
}
