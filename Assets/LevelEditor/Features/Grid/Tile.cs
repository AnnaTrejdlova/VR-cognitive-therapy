using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable {

    [Header("Misc")]
    public float sizeMultiplicator = 0.1f;
    public Material HighlightMaterial;
    public Material PreviewMaterial;
    [Header("Walls")]
    public GameObject wallColliders;
    public List<TileOrientationPosition> tileOrientationPositions = new List<TileOrientationPosition>();

    Material BaseMaterial;
    MeshRenderer meshRenderer;
    BoxCollider classicCollider;
    float tileSize = 1f;
    Outline outline;
    GameObject addedGameObject; // what you put into it
    Vector2Int gridPosition;
    Dictionary<TileWallOrientation, GameObject> AddedWallsDictionary = new Dictionary<TileWallOrientation, GameObject>();
    Dictionary<TileWallOrientation, GameObject> PreviewWallsDictionary = new Dictionary<TileWallOrientation, GameObject>();

    void Awake() {
        classicCollider = GetComponent<BoxCollider>();  
        outline = GetComponent<Outline>();
        meshRenderer = GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.material;
        SetTileSize(tileSize);
        ToggleOutline(false);   
    }

    public void ToggleOutline(bool toggleOn) {
        outline.enabled = toggleOn;
    }

    public void ToggleHighlightMaterial(bool toggleOn) {
        if (toggleOn) {
            meshRenderer.material = HighlightMaterial;
        } else {
            meshRenderer.material = BaseMaterial;
        }
    }

    #region Wall management

    
    public void CreateWallsBasedOnPreview(Material wallMaterial) {
        AddedWallsDictionary = PreviewWallsDictionary;
        foreach (KeyValuePair<TileWallOrientation, GameObject> entry in AddedWallsDictionary) {
            GameObject wall = entry.Value;
            wall.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        PreviewWallsDictionary = new Dictionary<TileWallOrientation, GameObject>();
    }

    public void AddWallJointPreview(GameObject wallJointPrefab, TileWallOrientation orientation) {
        if(!PreviewWallsDictionary.ContainsKey(orientation)) {
            GameObject go = Instantiate(wallJointPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            go.transform.position += new Vector3(0, transform.localScale.y, 0);
            go.GetComponent<MeshRenderer>().material = PreviewMaterial;
            MoveWallByOrientation(go, orientation);
            PreviewWallsDictionary.Add(orientation, go);
        } else {
            PreviewWallsDictionary[orientation].SetActive(true);
        }
    }

    public void AddWallFillPreview(GameObject wallFillPrefab, TileWallOrientation orientation) {
        if (!PreviewWallsDictionary.ContainsKey(orientation)) {
            GameObject go = Instantiate(wallFillPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            go.transform.position += new Vector3(0, transform.localScale.y, 0);
            go.GetComponent<MeshRenderer>().material = PreviewMaterial;
            MoveWallByOrientation(go, orientation);
            PreviewWallsDictionary.Add(orientation, go);
        } else {
            PreviewWallsDictionary[orientation].SetActive(true);
        }
    }

    public void ClearWallPreviews() {
        foreach (KeyValuePair<TileWallOrientation, GameObject> entry in PreviewWallsDictionary) {
            entry.Value.SetActive(false);
        }
    }



    void MoveWallByOrientation(GameObject wall, TileWallOrientation orientation) {
        switch (orientation) {
            case TileWallOrientation.Left:
                wall.transform.position += new Vector3(-0.5f,0,0f);
                wall.transform.rotation = Quaternion.Euler(0,90,0);
                break;
            case TileWallOrientation.TopLeft:
                wall.transform.position += new Vector3(-0.5f, 0, 0.5f);
                break;
            case TileWallOrientation.TopRight:
                wall.transform.position += new Vector3(0.5f, 0, 0.5f);
                break;
            case TileWallOrientation.BottomLeft:
                wall.transform.position += new Vector3(-0.5f, 0, -0.5f);
                break;
            case TileWallOrientation.BottomRight:
                wall.transform.position += new Vector3(0.5f, 0, -0.5f);
                break;
            case TileWallOrientation.Top:
                wall.transform.position += new Vector3(0f, 0, 0.5f);
                break;
            case TileWallOrientation.Right:
                wall.transform.position += new Vector3(0.5f, 0, 0f);
                wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case TileWallOrientation.Bottom:
                wall.transform.position += new Vector3(0f, 0, -0.5f);
                break;
        }
    }


    #endregion

    #region Added object manipulation

    public void RotateAddedObject(bool turnRight) {
        float rotationVALUE = addedGameObject.transform.rotation.y;
        if (turnRight) {
            rotationVALUE += 90f;
        } else {
            rotationVALUE += -90f;
        }
        addedGameObject.transform.Rotate(0, rotationVALUE, 0);
    }

    #endregion

    #region Adding/Removing objects

    public void AddObjectToTile(GameObject obj) {
        InstantiateAddedObject(obj);
    }

    public void RemoveObjectFromTile() {
        Destroy(addedGameObject);
        addedGameObject = null;
    }

    void InstantiateAddedObject(GameObject obj) {
        if (addedGameObject != null) {
            Destroy(addedGameObject);
        }
        // must not be a child of the tile because of the scale shenanigans
        // I am adding 0.5f because the pivot is in the center of the model, if the pivot would be at the bottom of the model there wouldnt be need to make it go up
        addedGameObject = Instantiate(obj, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        addedGameObject.transform.position += new Vector3(0, transform.localScale.y, 0);
    }

    #endregion

    #region IClickable interface

    public void OnClick() {
        TileManager.Instance.TileClickHandle(this);
    }

    public void OnHoverEnter() {
        TileManager.Instance.TileHoverEnterHandle(this);
    }

    public void OnHoverExit() {
        TileManager.Instance.TileHoverExitHandle(this);
    }

    #endregion

    #region SetUp related

    public void SetTileSize(float size) {
        tileSize = size;
        transform.localScale = new Vector3(tileSize * sizeMultiplicator, transform.localScale.y, tileSize * sizeMultiplicator);
    }

    public void SetGridPosition(Vector2Int pos) {
        gridPosition = pos;
    }

    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    public bool isTileOccupied() {
        return addedGameObject != null;
    }

    #endregion

    #region Different collider management

    void SwitchToWallsColliders() {
        classicCollider.enabled = false;
        wallColliders.SetActive(true);
    }

    void SwitchToClassicCollider() {
        classicCollider.enabled = true;
        wallColliders.SetActive(false);
    }

    #endregion

    #region EventListener (editor state)

    void HandleStateChange(EditorState newState) {
        switch (newState) {
            case EditorState.PlacingObjects:
                SwitchToClassicCollider();
                outline.OutlineColor = Color.black;
                break;
            case EditorState.RemovingObjects:
                outline.OutlineColor = Color.red;
                SwitchToClassicCollider();
                break;
            case EditorState.PlacingWalls:
                outline.OutlineColor = Color.black;
                SwitchToWallsColliders();
                break;

            default:
                outline.OutlineColor = Color.black;
                break;
        }
    }
    void OnEnable() {
        LevelEditorManager.Instance.OnStateChanged.AddListener(HandleStateChange);
    }

    void OnDisable() {
        LevelEditorManager.Instance.OnStateChanged.RemoveListener(HandleStateChange);
    }

    #endregion
}
// Class for being able to assign Orientation positions

[Serializable]
public class TileOrientationPosition {
    public TileWallOrientation state;
    public Vector3 position;
}