using UnityEngine;

public class BoxColliderGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;

    [SerializeField]
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null)
            return;

        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
    }
}