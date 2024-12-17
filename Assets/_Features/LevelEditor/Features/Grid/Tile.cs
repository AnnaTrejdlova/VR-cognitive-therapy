using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable {

    [Header("Misc")]
    public float sizeMultiplicator = 0.1f;
    public Material HighlightMaterial;
    public Material PreviewMaterial;
    public Material RealMaterial;
    public Material JointPreviewMaterial;
    public GameObject WallFillPrefab;
    public GameObject WallJointPrefab;

    [Header("Walls")]
    public GameObject wallColliders;
    public List<TileOrientationPosition> tileOrientationPositions = new List<TileOrientationPosition>();

    public TileWallClickable clickedTile;
    public TileWallClickable hoveredTile;

    private Material BaseMaterial;
    private MeshRenderer meshRenderer;
    private BoxCollider classicCollider;
    private float tileSize = 1f;
    private Outline outline;
    private GameObject addedGameObject; // what you put into it
    private Vector2Int gridPosition;
    public Dictionary<TileWallPosition, GameObject> AddedWallsDictionary = new Dictionary<TileWallPosition, GameObject>();
    public Dictionary<TileWallPosition, GameObject> PreviewWallsDictionary = new Dictionary<TileWallPosition, GameObject>();

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
        if (PreviewWallsDictionary.Count != 0) {
            AddedWallsDictionary.AddRange(PreviewWallsDictionary);
            foreach (KeyValuePair<TileWallPosition, GameObject> entry in AddedWallsDictionary) {
                GameObject wall = entry.Value;
                wall.GetComponent<MeshRenderer>().material = wallMaterial;
            }

            PreviewWallsDictionary = new Dictionary<TileWallPosition, GameObject>();
        }
    }

    public void AddWallJointPreview(GameObject wallJointPrefab, TileWallPosition orientation) {
        if(!PreviewWallsDictionary.ContainsKey(orientation)) {
            GameObject go = Instantiate(wallJointPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            go.transform.position += new Vector3(0, transform.localScale.y, 0);
            go.GetComponent<MeshRenderer>().material = JointPreviewMaterial;
            MoveWallByOrientation(go, orientation);
            PreviewWallsDictionary.Add(orientation, go);
        } else {
            PreviewWallsDictionary[orientation].SetActive(true);
        }
    }

    public void AddWallFillPreview(GameObject wallFillPrefab, TileWallPosition orientation) {
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
        foreach (KeyValuePair<TileWallPosition, GameObject> entry in PreviewWallsDictionary) {
            entry.Value.SetActive(false);
        }
    }

    public void MoveWallByOrientation(GameObject wall, TileWallPosition orientation) {
        switch (orientation) {
            case TileWallPosition.Left:
                wall.transform.position += new Vector3(-0.5f,0,0f);
                wall.transform.rotation = Quaternion.Euler(0,90,0);
                break;
            case TileWallPosition.TopLeft:
                wall.transform.position += new Vector3(-0.5f, 0, 0.5f);
                break;
            case TileWallPosition.TopRight:
                wall.transform.position += new Vector3(0.5f, 0, 0.5f);
                break;
            case TileWallPosition.BottomLeft:
                wall.transform.position += new Vector3(-0.5f, 0, -0.5f);
                break;
            case TileWallPosition.BottomRight:
                wall.transform.position += new Vector3(0.5f, 0, -0.5f);
                break;
            case TileWallPosition.Top:
                wall.transform.position += new Vector3(0f, 0, 0.5f);
                break;
            case TileWallPosition.Right:
                wall.transform.position += new Vector3(0.5f, 0, 0f);
                wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case TileWallPosition.Bottom:
                wall.transform.position += new Vector3(0f, 0, -0.5f);
                break;
        }
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

    #region Helpers

    public bool ContainsPreview(TileWallPosition orientation)
    {
        return PreviewWallsDictionary.ContainsKey(orientation) == true && PreviewWallsDictionary[orientation].activeInHierarchy;
    }

    public bool ContainsWall(TileWallPosition orientation)
    {
        return AddedWallsDictionary.ContainsKey(orientation) || ContainsPreview(orientation);
    }

    public TileWallPosition GetLastOrientation()
    {
        return PreviewWallsDictionary.Last().Key;
    }

    #endregion
}
// Class for being able to assign Orientation positions

[Serializable]
public class TileOrientationPosition {
    public TileWallPosition state;
    public Vector3 position;
}
