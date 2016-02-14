using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour, IKillable, IDamageable<float>, IDealDamage<float> {

    //-------------------------------------------------Variables-------------------------------------------------//
    [Header("Base Stats")]
    public float health;
	public float moveSpeed;
	public float attackDamage;
	public float attackSpeed;
	public float damageOnPlayerCollision;
	public float deathDelay = .75f;

    public GameObject particleDeath;

    private bool isVulnerable;
    private bool isDead = false;

	private Collision2D col;
    //-------------------------------------------------Functions-------------------------------------------------//
    public void SetBaseStats(float h, float mS, float aD, float aS, float dOPC, bool iV)
	{
		health = h;
		moveSpeed = mS;
		attackDamage = aD;
		attackSpeed = aS;
		damageOnPlayerCollision = dOPC;
		isVulnerable = iV;
	}

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        if (particleDeath != null)
        {
            Instantiate(particleDeath, transform.position, new Quaternion(0, 0, 0, 0));
        }
        Destroy(gameObject);
    }

	public void checkVulnerability(float health)
	{
		if (health > 0) {
			isVulnerable = true;
		}
	}

	public void takeDamage(float damageTaken)
	{
		if (isVulnerable) {
			health -= damageTaken;
		}
	}

	public float dealDamageOnHit()
	{
		return damageOnPlayerCollision;
	}

	public void destroyOnDeath()
	{
        if (health <= 0)
        {
            isDead = true; //TODO: this is not the way to handle this
            GameObject.Find("Player").GetComponent<TheCutAction>().ResetCooldownOnKill(isDead);
		}
	}
}
