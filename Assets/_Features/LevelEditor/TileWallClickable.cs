using Unity.VisualScripting;
using UnityEngine;

public class TileWallClickable : MonoBehaviour, IClickable {

    public TileWallPosition position;
    public Material HighlightMaterial;
    public GameObject JointPrefab;
    public Material JointMaterial;

    private Material BaseMaterial;
    private MeshRenderer meshRenderer;
    private Outline outline;
    public Tile relatedTile;
    public Vector2Int positionInTile;

    private GameObject HighlightJoint;

    void OnEnable() {
        outline = gameObject.GetComponent<Outline>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.material;
        relatedTile = transform.root.GetComponent<Tile>();
        ToggleOutline(false);
    }

    public void OnClick() {
        relatedTile.clickedTile = this;
        WallManager.Instance.WallPointClick(relatedTile, this, position);
    }

    public void OnHoverEnter() {
        relatedTile.hoveredTile = this;
        WallManager.Instance.WallPointEnterHover(relatedTile, this, position);
    }

    public void OnHoverExit() {
        WallManager.Instance.WallPointExitHover(relatedTile, this);
    }

    public void ToggleOutline(bool toggleOn) {
        if (outline != null) outline.enabled = toggleOn;

    }
    public void ToggleHighlightMaterial(bool toggleOn) {
        if (toggleOn) {
            if (HighlightJoint == null) {
                HighlightJoint = Instantiate(
                    relatedTile.WallJointPrefab,
                    relatedTile.transform.position + new Vector3(0, 0.5f, 0),
                    Quaternion.identity
                    );
                meshRenderer.material = HighlightMaterial;
                HighlightJoint.GetComponent<MeshRenderer>().material = relatedTile.PreviewMaterial;
            } else {
                HighlightJoint.SetActive(true);
            }

            HighlightJoint.transform.position += new Vector3(0, relatedTile.transform.localScale.y, 0);
            relatedTile.MoveWallByOrientation(HighlightJoint, position);
        } else {
            if (HighlightJoint != null)
                Destroy(HighlightJoint);
            meshRenderer.material = BaseMaterial;
        }
    }

    public Vector2Int GetGridPosition()
    {
        return positionInTile;
    }
}

public enum TileWallPosition {
    Null,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Top,
    Left,
    Right,
    Bottom,
}
