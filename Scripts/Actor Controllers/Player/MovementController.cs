/***************************************************************************************
 *  Movement Controller by Benjamin Lane
 *  Last Modified: 2016-Feb-12
 *
 *  Desc: The controller for player movement. Includes collisions, jumping, walking,
 *        running, and any other basic movement related tasks
 *
 *  Usage: Player movement should be entirely contained within this class, or related
 *         classes. This is not intended to be a generic class, and should create a
 *         highly specific movment feel for this character.
 *
 *         Collision is handled via raycasts. Gravity is handled for the player in
 *         this class, allowing the player to jump without Unity2D physics.
 ***************************************************************************************/

using UnityEngine;
using TeamUtility.IO;
using System.Collections;

public class MovementController : RaycastController {

    //-------------------------------------------------Variables-------------------------------------------------//

    //Inspector variables 
    [Header("Collision Layers")]
    public LayerMask collisionMask;

    [Header("Base Movement")]
    [SerializeField]
    private float walkSpeed;
    public float WalkSpeed { get { return walkSpeed; } }
    [SerializeField]
    private float runSpeed;
    public float RunSpeed { get { return runSpeed; } }
    [SerializeField]
    private float jumpHeight = 6.0f;
    [SerializeField]
    private float timeToJumpApex = 0.4f;
    [SerializeField]
    private float jumpTimer = 0.0f;
    [SerializeField]
    private float maxFallSpeed = 0.0f;
    [SerializeField]
    private float knockbackVelocityX = 15.0f;

    [Header("Wall Movement")]
    [SerializeField]
    private float wallSlideSpeedMax;

    //Private variables that may or may not have properties
    private int facingDirection;
    private bool facingRight;
	public bool FacingRight { get { return facingRight; }}
    private bool isRunning;
    public bool IsRunning { get { return isRunning; } set { isRunning = value; } }
    private bool wallSliding;
    public bool WallSliding { get { return wallSliding; } }

    private Vector2 feetPositionA;
    private Vector2 feetPositionB;
    private Vector2 sideLeftA;
    private Vector2 sideLeftB;
    private Vector2 sideRightA;
    private Vector2 sideRightB;
    private Vector2 headPositionA;
    private Vector2 headPositionB;


    private float skinWidth = 0.025f;
    private float maxClimbAngle = 65.0f;
    private float maxDescendAngle = 65.0f;

    private float accelerationTimeAirborne = 0f;
    private float velocityXSmoothing = 0;

    private float gravity;
    private float jumpVelocity;
    private Vector3 playerVelocity;
    public Vector3 PlayerVelocity
    {
        get { return playerVelocity; }
        set { playerVelocity = value; }
    }

    //Public variables
    private Vector3 prevVelocity;

    //Variables for InputManager

    //Jump States
    public enum JumpStates
    {
        JumpReady,
        WallJumpReady,
        Jumping,
        WallJumping,
        DoubleJumping,
        JumpFinishing

    }
    private JumpStates jumpState;
    public JumpStates JumpState
    {
        get { return jumpState; }
        set { jumpState = value; }
    }

    //Struct for storing raycast hit information
    public struct CollisionInfo
    {
        public bool hitAbove, hitBelow;
        public bool hitLeft, hitRight;
        public bool climbingSlope;
        public bool descendingSlope;
        public float prevSlopeAngle;
        public float currSlopeAngle;
        public float timeSinceFloorCollision;

        public Collider2D hitCollider;
        public GameObject hitGameObject;

        public void Reset()
        {
            hitAbove = false;
            hitBelow = false;
            hitLeft = false;
            hitRight = false;
            climbingSlope = false;
            descendingSlope = false;
            prevSlopeAngle = currSlopeAngle;
            currSlopeAngle = 0.0f;
        }
    }

    public CollisionInfo collisionInfo;

    //Accessors for gameObject Components
    private PlayerController playerController;
    private Rigidbody2D rigBody;
    private ActionManager actManager;
    private TheCutAction cutAction;
	private PlayerDamageController dmgManager;
    private PlayerTakeDamage playerTakeDmg;
    private PlayerStats playerStats;
    [SerializeField] //TODO: remove this when debugging/drawing gizmos is no longer necessary
    private BoxCollider2D playerCol;
    

    //-------------------------------------------------Functions-------------------------------------------------//
    new void Awake()
    {
        //Assign Accessors
        playerController = gameObject.GetComponent<PlayerController>();
        rigBody = gameObject.GetComponent<Rigidbody2D>();
        actManager = gameObject.GetComponent<ActionManager>();
        cutAction = gameObject.GetComponent<TheCutAction>();
        dmgManager = gameObject.GetComponent<PlayerDamageController>();
        playerTakeDmg = gameObject.GetComponent<PlayerTakeDamage>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerCol = gameObject.GetComponent<BoxCollider2D>();
        col = playerCol;
    }

    new void Start () 
	{
        //Assign the actionDelegate a method
        //TODO: This should be assigned elsewhere, when the player gains access to the ability

        //Set initial movement variables
        facingDirection = 1;
        facingRight = true;
        isRunning = false;
        wallSliding = false;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //Call methods once on Start
        CalculateRaySpacing();
    }

    /// <summary>
    /// Method for moving the player using transform.Translate and internally defined velocity
    /// </summary>
    public void TranslatePos(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();

        if (velocity.x != 0)
        {
            //sets and stores the direction the character is facing if moving
            facingDirection = (int)Mathf.Sign(velocity.x);
            facingRight = (facingDirection == 1) ? true : false;
        }

        prevVelocity = velocity;
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        HorizontalCollisions(ref velocity);
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        
        transform.Translate(velocity);
    }

    /// <summary>
    /// Called when climbing a slope, translates the player based on the angle of the slope while keeping consistent speed
    /// </summary>
    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisionInfo.hitBelow = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.currSlopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;
                        collisionInfo.currSlopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.hitBelow = true;
                    }
                }
            }
        }
    } 

    //Flip the x scale of the sprite if the sprite faces left
    //TODO: Remove the flip function, then rework some movement code that utilizes Vector2.Right
	public void Flip()
	{
        Vector2 initialOffset = playerCol.offset;
        initialOffset.x = initialOffset.x * -1;
        playerCol.offset = initialOffset;
	}

    void HorizontalCollisions(ref Vector3 velocity)
    {
        sideLeftA.Set(playerCol.bounds.min.x - skinWidth, playerCol.bounds.max.y - skinWidth);
        sideLeftB.Set(playerCol.bounds.center.x, playerCol.bounds.min.y + skinWidth);

        sideRightA.Set(playerCol.bounds.max.x + skinWidth, playerCol.bounds.max.y - skinWidth);
        sideRightB.Set(playerCol.bounds.center.x, playerCol.bounds.min.y + skinWidth);

        float directionX = facingDirection;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = skinWidth * 2;
        }

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
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //TODO: Use raycast non-alloc?

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
                        velocity = prevVelocity;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.prevSlopeAngle)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisionInfo.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisionInfo.currSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisionInfo.hitLeft = (directionX == -1);
                    collisionInfo.hitRight = (directionX == 1);
                }

                collisionInfo.hitCollider = hit.collider;
                collisionInfo.hitGameObject = hit.collider.gameObject;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        feetPositionA.Set(playerCol.bounds.min.x, playerCol.bounds.min.y + skinWidth);
        feetPositionB.Set(playerCol.bounds.max.x, playerCol.bounds.min.y - skinWidth);

        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin;
            if (directionY == -1)
            {
                rayOrigin = raycastOrigins.bottomLeft;
            }
            else
            {
                rayOrigin = raycastOrigins.topLeft;
            }
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask); //TODO: Use raycast non-alloc?

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.currSlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionInfo.hitBelow = (directionY == -1);
                collisionInfo.hitAbove = (directionY == 1);

                collisionInfo.hitCollider = hit.collider;
                collisionInfo.hitGameObject = hit.collider.gameObject;
            }
        }

        //Checks whether a change in the slope angle is about to happen, and adjusts the player transform smoothly (TODO: merge this with above code?)
        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 climbRayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            RaycastHit2D hit = Physics2D.Raycast(climbRayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.currSlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.currSlopeAngle = slopeAngle;
                }
            }
        }


    }

    private Vector2 modifyJump(float x, float y)
    {
        return new Vector2(x, y);
    }

    private void ManageJump()
    {
        int wallDirectionX = (collisionInfo.hitLeft) ? -1 : 1;
        wallSliding = false; //TODO: check if this causes issues, and address logic
        
        if ((collisionInfo.hitLeft || collisionInfo.hitRight) && !collisionInfo.hitBelow && !cutAction.IsActioning 
            && collisionInfo.hitGameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            wallSliding = true;
            //TODO: maintain velocity from fall but slowly decrease momentum? or maintain velocity from fall, and only adjust after a jump?
            if (playerVelocity.y < -wallSlideSpeedMax)
            {
                playerVelocity.y = -wallSlideSpeedMax;
            }

            if (((playerController.HorzAxis > .5f && collisionInfo.hitRight) || (playerController.HorzAxis < -.5f && collisionInfo.hitLeft)) && collisionInfo.timeSinceFloorCollision > 0.05f && jumpTimer >= 0)
            {
                playerVelocity.y = .5f;
                jumpTimer -= Time.deltaTime;
            }
        }
        if (jumpState == JumpStates.Jumping)
        {
            accelerationTimeAirborne = .15f;
            jumpState = JumpStates.JumpFinishing;
            if (collisionInfo.hitBelow)
            {
                playerVelocity.y = jumpVelocity;
            }
        }
        if (jumpState == JumpStates.WallJumping)
        {
            if (wallSliding)
            {
                //Give airborne acceleration only while wall jumping, it feels floaty otherwise
                accelerationTimeAirborne = .2f;
                //These statements modify the player velocity while wall jumping given certain conditions
                if ((wallDirectionX == -1 && playerController.HorzAxis < 0) || (wallDirectionX == 1 && playerController.HorzAxis > 0))
                {
                    playerVelocity.x = -wallDirectionX * modifyJump(6.5f, 14).x;
                    playerVelocity.y = modifyJump(6.5f, 14).y;
                }
                else if (playerController.HorzAxis == 0)
                {
                    playerVelocity.x = -wallDirectionX * modifyJump(8.5f, 7).x;
                    playerVelocity.y = modifyJump(8.5f, 7).y;
                }
                else
                {
                    playerVelocity.x = -wallDirectionX * modifyJump(16, 14).x;
                    playerVelocity.y = modifyJump(16, 14).y;
                }
            }
            jumpState = JumpStates.JumpFinishing;
        }
        if (jumpState == JumpStates.JumpFinishing)
        {
            if (collisionInfo.hitBelow || wallSliding)
            {
                jumpTimer = 1f;
                jumpState = JumpStates.JumpReady;
            }
        }
    }
	
    void Update() //TODO: Ensure physics are performed in FixedUpdate, while visual representations / steps are performed in Update for smooth cameras
    {
        float targetVelocityX = (isRunning) ? playerController.HorzAxis * runSpeed : playerController.HorzAxis * walkSpeed;
        if (cutAction.IsActioning)
        {
            accelerationTimeAirborne = 0f;
        }
        
        playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, targetVelocityX, ref velocityXSmoothing, (!collisionInfo.hitBelow) ? accelerationTimeAirborne : 0);

        actManager.actionDelegate();
        ManageJump();

        if (playerVelocity.y >= maxFallSpeed)
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        if (playerStats.KnockBackHit)
        {
            if (playerTakeDmg.KnockbackRight)
            {
                playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, -(knockbackVelocityX), ref velocityXSmoothing, 0);
            }
            else
            {
                playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, knockbackVelocityX, ref velocityXSmoothing, 0);
            }
        }
        TranslatePos(playerVelocity * Time.deltaTime);

        if (collisionInfo.hitAbove || collisionInfo.hitBelow)
        {
            playerVelocity.y = 0;
        }

        if (!collisionInfo.hitBelow)
        {
            collisionInfo.timeSinceFloorCollision += Time.deltaTime;
        } else
        {
            collisionInfo.timeSinceFloorCollision = 0.0f;
        }
        //Debug.Log(collisionInfo.timeSinceFloorCollision);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        feetPositionA.Set(playerCol.bounds.min.x + .025f, playerCol.bounds.min.y + skinWidth);
        feetPositionB.Set(playerCol.bounds.max.x - .025f, playerCol.bounds.min.y - skinWidth);

        sideLeftA.Set(playerCol.bounds.min.x - skinWidth, playerCol.bounds.max.y - skinWidth);
        sideLeftB.Set(playerCol.bounds.center.x, playerCol.bounds.min.y + skinWidth);

        sideRightA.Set(playerCol.bounds.max.x + skinWidth, playerCol.bounds.max.y - skinWidth);
        sideRightB.Set(playerCol.bounds.center.x, playerCol.bounds.min.y + skinWidth);

        headPositionA.Set(playerCol.bounds.min.x + .025f, playerCol.bounds.max.y + skinWidth);
        headPositionB.Set(playerCol.bounds.max.x - .025f, playerCol.bounds.max.y - skinWidth);

        //Debug.DrawLine(feetPositionA, feetPositionB, Color.red);
        Vector2 drawBoxFeetCenter = new Vector2((feetPositionA.x + feetPositionB.x) / 2, (feetPositionA.y + feetPositionB.y) / 2);
        Vector2 drawBoxLeftCenter = new Vector2((sideLeftA.x + sideLeftB.x) / 2, (sideLeftA.y + sideLeftB.y) / 2);
        Vector2 drawBoxRightCenter = new Vector2((sideRightA.x + sideRightB.x) / 2, (sideRightA.y + sideRightB.y) / 2);
        Vector2 drawBoxHeadCenter = new Vector2((headPositionA.x + headPositionB.x) / 2, (headPositionA.y + headPositionB.y) / 2);

        Vector2 drawBoxFeetSize = new Vector2(feetPositionA.x - feetPositionB.x, feetPositionA.y - feetPositionB.y);
        Vector2 drawBoxLeftSize = new Vector2(sideLeftA.x - sideLeftB.x, sideLeftA.y - sideLeftB.y);
        Vector2 drawBoxRightSize = new Vector2(sideRightA.x - sideRightB.x, sideRightA.y - sideRightB.y);
        Vector2 drawBoxHeadSize = new Vector2(headPositionA.x - headPositionB.x, headPositionA.y - headPositionB.y);

        Gizmos.DrawWireCube(drawBoxFeetCenter, drawBoxFeetSize);
        Gizmos.DrawWireCube(drawBoxLeftCenter, drawBoxLeftSize);
        Gizmos.DrawWireCube(drawBoxRightCenter, drawBoxRightSize);
        Gizmos.DrawWireCube(drawBoxHeadCenter, drawBoxHeadSize);
    }

}
