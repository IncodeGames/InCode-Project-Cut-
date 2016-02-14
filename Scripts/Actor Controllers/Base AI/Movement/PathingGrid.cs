using UnityEngine;
using System.Collections;

namespace AIPathing
{
    public class PathingGrid : MonoBehaviour {
        public Transform PlayerTransform;

        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        private Node[ , ] grid;

        private float nodeDiameter;
        private int gridSizeX;
        private int gridSizeY;

        private Vector3 gridOffset;

        void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            gridOffset = new Vector3(transform.position.x, transform.position.y);

            CreateGrid();
        }

        void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];

            Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.up * gridWorldSize.y / 2);
            Debug.Log(worldBottomLeft);

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));

                    grid[x, y] = new Node(walkable, worldPoint);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            //Offset the position of the target world position, by the starting position of the grid
            worldPosition -= gridOffset;
            float partialX = (worldPosition.x / gridWorldSize.x + 0.5f);
            float partialY = (worldPosition.y/gridWorldSize.y + 0.5f);

            //Clamp partials between 0 and 1
            partialX = Mathf.Clamp01(partialX);
            partialY = Mathf.Clamp01(partialY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * partialX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * partialY);

            return grid[x, y];


        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(PlayerTransform.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.gray : Color.red;
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}
