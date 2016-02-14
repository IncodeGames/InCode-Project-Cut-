using UnityEngine;
using System.Collections;

namespace AIPathing
{
    public class Node
    {

        public bool walkable;
        public Vector3 worldPosition;

        public Node(bool ctorWalkable, Vector3 ctorWorldPos)
        {
            walkable = ctorWalkable;
            worldPosition = ctorWorldPos;
        }

    }
}