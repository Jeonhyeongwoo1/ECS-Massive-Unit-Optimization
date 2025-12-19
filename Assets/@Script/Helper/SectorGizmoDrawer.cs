using System;
using UnityEngine;

public class SectorGizmoDrawer : MonoBehaviour
{
    [Header("2D 부채꼴 설정")]
    [Range(0f, 360f)] public float angle = 90f;
    public float radius = 5f;
    public int segments = 40;
    public Color gizmoColor = Color.green;
    public Vector3 forwardDirection;
    public bool isReverse = true;
    
    public void SetData(float angle, float radius, Vector3 forwardDirection, bool isReverse = true)
    {
        this.radius = radius;
        this.angle = angle;
        this.forwardDirection = forwardDirection;
        this.isReverse = isReverse;
    }

    private void OnDrawGizmos()
    {
        Draw2DSectorFacingDirection(transform.position, forwardDirection, radius, angle, segments, gizmoColor);
    }

    private void Draw2DSectorFacingDirection(Vector3 center, Vector3 direction, float radius, float angle, int segments, Color color)
    {
        Gizmos.color = color;

        float dirAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, dirAngle);

        float startAngle = -angle / 2f;
        float angleStep = angle / segments;

        Vector3 prevPoint = center + rotation * AngleToDirection(startAngle) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            Vector3 nextPoint = center + rotation * AngleToDirection(currentAngle) * radius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            Gizmos.DrawLine(center, nextPoint);

            prevPoint = nextPoint;
        }

        if (angle < 360f)
        {
            Vector3 startPoint = center + rotation * AngleToDirection(startAngle) * radius;
            Vector3 endPoint = center + rotation * AngleToDirection(startAngle + segments * angleStep) * radius;

            Gizmos.DrawLine(center, startPoint);
            Gizmos.DrawLine(center, endPoint);
        }
    }

    private Vector3 AngleToDirection(float angleInDegrees)
    {
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * (isReverse ? -1 : 1);
    }
}