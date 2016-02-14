using UnityEngine;
using TeamUtility.IO;
using System.Collections;

[RequireComponent(typeof(MovementController))]
public class PlayerController : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    //Private Input variables
    private float horzAxis = 0.0f;
    public float HorzAxis { get { return horzAxis; } }
    private float vertAxis = 0.0f;
    public float VertAxis { get { return vertAxis; } }
    private bool submit = false;
    public bool Submit { get { return submit; } }

    //private accessors
    private GameObject player;
    private PlayerStats playerStats;
    private MovementController moveControl;
    private ActionManager actManager;
    private TheCutAction cutAction;
    private Animator anim;

    //-------------------------------------------------Functions-------------------------------------------------//
    void Awake()
    {
        //Accessors
        player = gameObject;
        playerStats = player.GetComponent<PlayerStats>();
        moveControl = player.GetComponent<MovementController>();
        actManager = player.GetComponent<ActionManager>();
        cutAction = player.GetComponent<TheCutAction>();
        anim = gameObject.GetComponent<Animator>();
    }

    void Start ()
    {
        //Assign action delegate (TODO: assign this when the player actually gets the ability)
        actManager.actionDelegate += cutAction.TheCut;
	}
	
	// Update is called once per frame
	void Update ()
    {
        horzAxis = InputManager.GetAxis("Horizontal");
        vertAxis = InputManager.GetAxis("Vertical");
        if (playerStats.KnockBackHit)
        {
            return;
        }
        
        if (horzAxis > 0)
        {
            //TODO: Flip() currently affects the position of the raycasts that results in glitchy behavior
            /*if (!moveControl.FacingRight)
            {
                moveControl.Flip();
            }*/
            anim.SetFloat("Run Speed", 1.0f);
        }
        else if (horzAxis < 0)
        {
            /*if (moveControl.FacingRight)
            {
                moveControl.Flip();
            }*/
            anim.SetFloat("Run Speed", -1.0f);
        }
        else
        {
            anim.SetFloat("Run Speed", 0.0f);
        }

        if (InputManager.GetButtonDown("Jump")) //TODO: turn jump into a state machine, there should be wall friction or temporary wall stick
        {
            if (moveControl.JumpState == MovementController.JumpStates.JumpReady)
            {
                if (moveControl.collisionInfo.hitBelow)
                {
                    moveControl.JumpState = MovementController.JumpStates.Jumping;
                }
                if (!moveControl.collisionInfo.hitBelow && (moveControl.collisionInfo.hitLeft || moveControl.collisionInfo.hitRight))
                {
                    moveControl.JumpState = MovementController.JumpStates.WallJumping;
                }
            }
        }

        if (InputManager.GetButtonDown("Action"))
        {
            if (cutAction.DashState == ActionState.ActionReady && (horzAxis != 0.0f || vertAxis != 0.0f))
            {
                cutAction.DashState = ActionState.Action;
            }
        }

        if (InputManager.GetAxis("RightTrigger") > 0 || InputManager.GetButton("RightTrigger"))
        {
            moveControl.IsRunning = true;
        }
        else
        {
            moveControl.IsRunning = false;
        }

        if (InputManager.GetButtonDown("Submit"))
        {
            submit = true;
            EventManager.TriggerEvent("submitActivateText");
            EventManager.TriggerEvent("submitNextLine");
        }
        else
        {
            submit = false;
        }

    }

}
