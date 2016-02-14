using UnityEngine;
using System.Linq;
using System.Collections;

public class DamageManager : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    public Vector2 raySize;

	RaycastHit2D[] actionHit = new RaycastHit2D[20];

	//Accessors
	private GameObject player;
	private PlayerStats playerStats;
	private TheCutAction cutAction;
	private Rigidbody2D rigBody;


    //-------------------------------------------------Functions-------------------------------------------------//  
    void Awake () 
	{
		player = GameObject.FindWithTag("Player");
		playerStats = player.GetComponent<PlayerStats>();
		cutAction = player.GetComponent<TheCutAction>();
		rigBody = player.GetComponent<Rigidbody2D>();

	}

	public void registerCutDamage()
	{
		Vector2 vec = new Vector2(cutAction.DashStartHorzAxis, cutAction.DashStartVertAxis);
		Ray2D dashRay = new Ray2D((Vector2)cutAction.DashStartPosition, vec);

		Physics2D.BoxCastNonAlloc(dashRay.origin, raySize, 0.0f, dashRay.direction, actionHit, cutAction.MaxDashDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment")); //TODO: make length the distance of last completed dash
		foreach (RaycastHit2D hit in actionHit.OrderBy(x => x.distance))
		{
			if (hit.collider != null)
			{
                if (hit.collider.gameObject.tag == "Environment")
                {
                    return;
                }
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    hit.collider.gameObject.GetComponent<IDamageable<float>>().takeDamage(playerStats.ActionDamage);
                }
            }
		}

	}
	

}
