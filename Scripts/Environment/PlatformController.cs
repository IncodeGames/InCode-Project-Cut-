using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {
    //-------------------------------------------------Variables-------------------------------------------------//
    [SerializeField]
    private LayerMask playerMask;
    [SerializeField]
    private LayerMask enemyMask;

    [SerializeField]
    private Vector2 move;

    [SerializeField]
    private Vector2 boxRaySize;

    //-------------------------------------------------Functions-------------------------------------------------//
    public override void Start()
    {
        base.Start();
    }

    private void MoveOtherObjects(Vector3 velocity)
    {
        HashSet<Transform> movedObject = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + extraWidth;

            Vector2 rayOrigin = transform.position;
            RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, boxRaySize, 0, Vector2.up * directionY, rayLength, playerMask | enemyMask); //TODO: Determine whether to use boxcast or more raycasts

            if (hit)
            {
                if (!movedObject.Contains(hit.transform))
                {
                    movedObject.Add(hit.transform);

                    float pushX = (directionY == 1) ? velocity.x : 0;
                    float pushY = velocity.y - (hit.distance - extraWidth) * directionY;

                    hit.transform.Translate(new Vector2(pushX, pushY));
                }
            }
        }
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + extraWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin;
                if (directionX < 0)
                {
                    rayOrigin = raycastOrigins.bottomLeft;
                }
                else
                {
                    rayOrigin = raycastOrigins.bottomRight;
                }
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, playerMask | enemyMask);

                if (hit)
                {
                    if (!movedObject.Contains(hit.transform))
                    {
                        movedObject.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - extraWidth) * directionX;
                        float pushY = 0;

                        hit.transform.Translate(new Vector2(pushX, pushY));
                    }
                }
            }
        }
        if (directionY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength = extraWidth * 2;

            
            Vector2 rayOrigin = transform.position;
            RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, boxRaySize, 0, Vector2.up, rayLength, playerMask | enemyMask);

            if (hit)
            {
                if (!movedObject.Contains(hit.transform))
                {
                    movedObject.Add(hit.transform);

                    float pushX = (directionY == 1) ? velocity.x : 0;
                    float pushY = velocity.y - (hit.distance - extraWidth) * directionY;

                    hit.transform.Translate(new Vector2(pushX, pushY));
                }
            }
        }
    }

    void FixedUpdate()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = move * Time.deltaTime;
        MoveOtherObjects(velocity);
        transform.Translate(velocity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        

        Gizmos.DrawWireCube(transform.position, boxRaySize);
    }
}
