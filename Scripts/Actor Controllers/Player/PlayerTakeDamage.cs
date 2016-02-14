using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerStats))]
public class PlayerTakeDamage : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    [Header("Knockback Values")]
    [SerializeField]
    [Tooltip("The amount of time the player is being knocked back. Controls are disabled during this time.")]
    private float defaultTimer;
    private float timer;

    /*[SerializeField]
    [Tooltip("The amount the player will be knocked back per frame over the duration of the timer on the x axis.")]
    private float knockbackXVal;
    [SerializeField]
    [Tooltip("The amount the player will be knocked back per frame over the duration of the timer on the y axis.")]
    private float knockbackYVal;*/ //TODO: determine usefulness / move to MovementController

    private bool callKnockbackHit = false;
    private bool knockbackRight = false;
    public bool KnockbackRight { get { return knockbackRight; } }
    //Accessors
    private PlayerStats playerStats;
    private MovementController moveControl;

    //-------------------------------------------------Functions-------------------------------------------------//
    void Awake()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        moveControl = gameObject.GetComponent<MovementController>();
    }

    void Start()
    {
        timer = defaultTimer;
    }

    public void playerTakeDamage(float damage)
    {
        if (playerStats.CurrentVulnerableState == PlayerStats.VulnerabilityState.Vulnerable)
        {
            playerStats.CurrentHealth -= damage;
            callKnockbackHit = true;
            StartCoroutine(playerStats.TempInvulnerableOnHit());
        }
        destroyPlayerOnDeath();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "Hazard" || col.gameObject.tag == "Projectile") //TODO: might need to use collisions for enemies, and call Physics.ignoreCollisions()
        {
            if (col.gameObject.GetComponent<IDealDamage<float>>() != null)
            {
                playerTakeDamage(col.gameObject.GetComponent<IDealDamage<float>>().dealDamageOnHit());
            }
        }
    }

    private void destroyPlayerOnDeath()
    {
        if (playerStats.CurrentHealth <= 0)
        {
            //TODO: play death animation / remove destroy and reload level?
            playerStats.IsDead = true;
            Destroy(gameObject);
        }
    }

    private void knockBackOnHit()
    {
        playerStats.KnockBackHit = true;
        if (moveControl.FacingRight)
        {                
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                playerStats.KnockBackHit = false;
                callKnockbackHit = false;
                timer = defaultTimer;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                playerStats.KnockBackHit = false;
                callKnockbackHit = false;
                timer = defaultTimer;
            }
        }
    }
    
    void Update()
    {
        if (callKnockbackHit)
        {
            knockBackOnHit();
        }
        else
        {
            knockbackRight = moveControl.FacingRight;
        }
    }
}
