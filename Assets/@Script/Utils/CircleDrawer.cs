#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public float radius = 3f;
    public Color gizmoColor = Color.red;

    private void OnDrawGizmos()
    {
        Handles.color = gizmoColor;
        Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
    }
}
#endif