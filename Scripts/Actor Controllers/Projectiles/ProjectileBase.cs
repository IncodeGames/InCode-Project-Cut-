using UnityEngine;
using System.Collections;

public class ProjectileBase : MonoBehaviour, IDealDamage<float> {

    public enum projectileMode
    {
        Simple_Straight = 0,
        Simple_Homing = 1,
    }

    public enum modifyDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Diagonal_UpLeft = 4,
        Diagonal_DownLeft = 5,
        Diagonal_UpRight = 6,
        Diagonal_DownRight = 7,
        Manual = 8
    }

    [Header("Firing Mode")]
    [SerializeField]
    [Tooltip("The type of firing pattern for the projectiles.")]
    protected projectileMode fireMode = projectileMode.Simple_Straight;

    [Header("Bullet Direction")]
    [SerializeField]
    [Tooltip("The direction the bullet / bullet pattern should fire.")]
    protected modifyDirection modDirection = modifyDirection.Manual;

    [Header("Bullet Properties")]
    [SerializeField]
    [Tooltip("The duration before the projectile is disabled if no collisions occur.")]
    protected float projectileLifetime = 5.0f;
    [SerializeField]
    protected float bulletDamage = 5.0f;
    [SerializeField]
    protected float bulletSpeed = 10.0f;

    [SerializeField]
    [Tooltip("Direction that is used only when the Direction Modifier is set to manual.")]
    protected Vector2 uniqueDirection;
    protected Vector2 direction;

    //Private maintenance variables
    private float currentLifetime;

    private Vector2 startPosition;
    private Transform initParent;

    // Use this for initialization
    void Start ()
    {
        startPosition = gameObject.transform.position;
        initParent = gameObject.transform.parent;
        currentLifetime = projectileLifetime;

        initBulletDirection();
    }

    void OnEnable()
    {
        currentLifetime = projectileLifetime;
    }

    protected void initBulletDirection()
    {
        switch (modDirection)
        {
            case modifyDirection.Up:
                direction = new Vector2(0, 1);
                break;
            case modifyDirection.Down:
                direction = new Vector2(0, -1);
                break;
            case modifyDirection.Left:
                direction = new Vector2(-1, 0);
                break;
            case modifyDirection.Right:
                direction = new Vector2(1, 0);
                break;
            case modifyDirection.Diagonal_UpLeft:
                direction = new Vector2(-1, 1);
                break;
            case modifyDirection.Diagonal_DownLeft:
                direction = new Vector2(-1, -1);
                break;
            case modifyDirection.Diagonal_UpRight:
                direction = new Vector2(1, 1);
                break;
            case modifyDirection.Diagonal_DownRight:
                direction = new Vector2(1, -1);
                break;
            case modifyDirection.Manual:
                direction = uniqueDirection;
                if (direction == Vector2.zero)
                {
                    Debug.Log("No bullet direction set on " + gameObject.name);
                }
                break;
            default:
                Debug.Log("Error: Invalid bullet direction selected " + gameObject.name);
                break;
        }
    }

    protected void changeFireMode(projectileMode mode)
    {
        fireMode = mode;
    }

    public float dealDamageOnHit()
    {
        return bulletDamage;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "Enemy" && col.gameObject.tag != "Projectile")
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        transform.parent = null;
        if (fireMode == 0)
        {
            transform.Translate((direction * bulletSpeed) / 60.0f);
        }
    }
}
