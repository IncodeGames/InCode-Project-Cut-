using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideySentryController : EnemyBase, IKillable, IDamageable<float>
{

    //Inspector Variables
    [SerializeField]
    private GameObject hideyProjectile; //TODO: move any projectile oriented code into seperate script, and create a base class

    [SerializeField]
    private Transform bulletOffset;

    //Private variables
    private int maxPooledAmount = 8;
    private int fireCounter = 0;
    private List<GameObject> pooledHideyProjectiles;
    //Accessors
    private GameObject player;
    private Animator anim;
	
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        anim = gameObject.GetComponent<Animator>();
    }

	void Start ()
    {
        pooledHideyProjectiles = new List<GameObject>();
        for (int i = 0; i < maxPooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(hideyProjectile);
            obj.transform.parent = transform;
            obj.SetActive(false);
            pooledHideyProjectiles.Add(obj);
        }
        anim.SetBool("IsFiring", true);
	}
	
    
	private void fireContinously()
    {
        if (fireCounter < pooledHideyProjectiles.Count)
        {
            if (!pooledHideyProjectiles[fireCounter].activeInHierarchy)
            {
                pooledHideyProjectiles[fireCounter].transform.position = bulletOffset.position;
                pooledHideyProjectiles[fireCounter].transform.rotation = bulletOffset.rotation;
                pooledHideyProjectiles[fireCounter].SetActive(true);
            }
            fireCounter++;
        }
        if (fireCounter == pooledHideyProjectiles.Count)
        {
            fireCounter = 0;
        }
        
    }

    private void animOnDeath() //TODO: Overwrite death script?
    {
        if (health <= 0)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            anim.SetBool("IsDeadFull", true);
            
        }
    }

    private void aimAtPlayer()
    {
        if (player != null)
        {
            var dir = player.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle += 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }
    }

    void Update()
    {
        checkVulnerability(health);
        animOnDeath();
        aimAtPlayer();
    }
}
