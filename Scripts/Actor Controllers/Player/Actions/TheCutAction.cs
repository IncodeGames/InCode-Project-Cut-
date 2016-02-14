using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ActionManager))]
public class TheCutAction : MonoBehaviour {
    //-------------------------------------------------Variables-------------------------------------------------//

    //Inspector Cut Settings
    [Header("The_Cut Settings")]
    [SerializeField]
    private float maxDashDistance = 8.0f;
    public float MaxDashDistance { get { return maxDashDistance; } }
    [SerializeField]
    private float dashCooldownMultiplier = 1.0f;
    [SerializeField]
    private float dashCooldownOnKill = 0.0f;

    //Dash mechanics
    private bool isActioning;
    public bool IsActioning { get { return isActioning; } }

    private float dashStartHorzAxis = 0.0f;
    private float dashStartVertAxis = 0.0f;
    public float DashStartHorzAxis { get { return dashStartHorzAxis; } }
    public float DashStartVertAxis { get { return dashStartVertAxis; } }
    private float dashTimer = 0.0f;

    private Vector2 dashStartPosition;
    public Vector2 DashStartPosition { get { return dashStartPosition; } }
    private Vector3[] playerPosition = new Vector3[2];

    //Private constructor of the global player ActionState for The Cut
    private ActionState dashState;
    public ActionState DashState
    {
        get { return dashState; }
        set { dashState = value; }
    }

    //Accessors
    private GameObject player;
    private PlayerController playerController;
    private MovementController moveControl;
    private PlayerDamageController dmgManager;
    private PlayerStats playerStats;

    //-------------------------------------------------Functions-------------------------------------------------//
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = gameObject.GetComponent<PlayerController>();
        moveControl = gameObject.GetComponent<MovementController>();
        dmgManager = gameObject.GetComponent<PlayerDamageController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
    }

    private IEnumerator actionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActioning = true;
        playerStats.CurrentVulnerableState = PlayerStats.VulnerabilityState.TempInvulnerable;
        StopCoroutine("actionDelay");
    }


    public void TheCut()
    {
        switch (dashState)
        {
            case ActionState.ActionReady:
                isActioning = false;
                dashStartHorzAxis = playerController.HorzAxis;
                dashStartVertAxis = playerController.VertAxis;
                dashStartPosition = transform.position;
                dashTimer = 1.5f;
                break;
            case ActionState.Action:

                if (isActioning == false)
                {
                    moveControl.PlayerVelocity = Vector2.zero;
                    StartCoroutine("actionDelay", .025f);
                }

                //TODO: axes cannot be equal to zero or the character will not be able to move
                if (isActioning)
                {
                    Vector2 vecX = Quaternion.AngleAxis((Mathf.Atan2(dashStartVertAxis, dashStartHorzAxis) * 180 / Mathf.PI), Vector2.up) * Vector2.right;
                    Vector2 vecY = Quaternion.AngleAxis((Mathf.Atan2(dashStartHorzAxis, dashStartVertAxis) * 180 / Mathf.PI), Vector2.right) * Vector2.up;
                    Vector2 vecXY = new Vector2(vecX.x * 50.0f, vecY.y * 50.0f);
                    moveControl.PlayerVelocity = vecXY;

                    Debug.Log(vecXY);

                    if (playerPosition[1] != null)
                    {
                        playerPosition[0] = playerPosition[1];
                    }
                    playerPosition[1] = transform.position;

                    dmgManager.registerCutDamage();
                    //Set the dash on cooldown if the player has traveled the maximum dash distance or have stopped moving
                    if (Vector2.Distance(transform.position, dashStartPosition) >= maxDashDistance || Vector3.Distance(playerPosition[0], playerPosition[1]) < 0.1f)
                    {
                        moveControl.PlayerVelocity = Vector2.zero;
                        playerStats.CurrentVulnerableState = PlayerStats.VulnerabilityState.Vulnerable;
                        dashState = ActionState.ActionCooldown;
                    }
                }
                break;
            case ActionState.ActionCooldown:
                isActioning = false;

                //Ensure player rotation is zero
                transform.rotation = new Quaternion(0, 0, 0, 0);
                dashTimer -= Time.deltaTime * dashCooldownMultiplier;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = ActionState.ActionReady;
                }
                break;
            default:
                break;
        }
    }

    public void ResetCooldownOnKill(bool enemyKilled)
    {
        if (enemyKilled)
        {
            //TODO: add double jump here?
            dashTimer = dashCooldownOnKill;
        }
    }
}
