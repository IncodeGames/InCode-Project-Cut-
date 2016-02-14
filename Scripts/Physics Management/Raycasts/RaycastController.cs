using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [HideInInspector]
    public float horizontalRaySpacing = 0.0f;
    [HideInInspector]
    public float verticalRaySpacing = 0.0f;
    public float extraWidth = 0.015f;

    //Accessors
    [HideInInspector]
    public BoxCollider2D col;

    //Struct for storing position of raycast origins at corners
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        //Assign accessors
        col = gameObject.GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(extraWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(extraWidth * -2);

        if (horizontalRayCount > 50 || horizontalRayCount < 2)
        {
            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, 50);
        }
        if (verticalRayCount > 50 || verticalRayCount < 2)
        {
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, 50);
        }

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}
