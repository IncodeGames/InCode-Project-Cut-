using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;



public class PlayerStats : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    private float maxHealth = 100.0f;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    private float currentHealth;


	private float defaultActionDamage = 100.0f;
	private float actionDamage = 0.0f;
    public float ActionDamage { get { return actionDamage; } set { actionDamage = value; } }

    private bool isDead = false;
	private bool knockbackHit = false;
    public bool KnockBackHit { get { return knockbackHit; } set { knockbackHit = value; } }

	//Properties with functional setters
	public float CurrentHealth
	{
		get{ return currentHealth; }
		set{ if (currentHealth <= maxHealth) currentHealth = value; }
	}

    public bool IsDead
    {
        get { return isDead; } 
        set { isDead = value; }
    }

    //Accessors
    private GameObject player;
	private Rigidbody2D rigBody;
    private TheCutAction cutAction;

    //Enums
    //Set vulnerability state in code wherever the vulnerability should change, not in Update
    public enum VulnerabilityState
    {
        Vulnerable,
        TempInvulnerable,
        Invincible
    }
    private VulnerabilityState currentVulnerableState;
    public VulnerabilityState CurrentVulnerableState { get { return currentVulnerableState; } set { currentVulnerableState = value; }  }

    //-------------------------------------------------Functions-------------------------------------------------//

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rigBody = player.GetComponent<Rigidbody2D>();
        cutAction = player.GetComponent<TheCutAction>();
    }

    void Start()
	{
		currentHealth = maxHealth;
		actionDamage = defaultActionDamage;
	}

	//Become invincible for a given time when hit
    public IEnumerator TempInvulnerableOnHit()
    {
        currentVulnerableState = VulnerabilityState.TempInvulnerable;
        yield return new WaitForSeconds(1.25f);
        currentVulnerableState = VulnerabilityState.Vulnerable;
        StopCoroutine("TempInvulnerableOnHit");
    }
	
}

[System.Serializable]
class PlayerData
{

    private float maxHealth;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { if (currentHealth <= maxHealth) currentHealth = value; }
    }

    private float actionDamage;
    public float ActionDamage { get { return actionDamage; } set { actionDamage = value; } }

    public PlayerData()
    {
        MaxHealth = GameObject.FindWithTag("Player").GetComponent<PlayerStats>().MaxHealth;
        CurrentHealth = GameObject.FindWithTag("Player").GetComponent<PlayerStats>().CurrentHealth;
        ActionDamage = GameObject.FindWithTag("Player").GetComponent<PlayerStats>().ActionDamage;
    }
}