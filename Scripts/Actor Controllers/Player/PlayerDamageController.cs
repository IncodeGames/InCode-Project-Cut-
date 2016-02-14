﻿using UnityEngine;
using System.Linq;
using System.Collections;

public class PlayerDamageController : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    public Vector2 raySize;

	RaycastHit2D[] actionHit = new RaycastHit2D[20];

    private Vector2 distanceBufferToCollision;

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
        distanceBufferToCollision = new Vector2(cutAction.DashStartPosition.x + 1f, cutAction.DashStartPosition.y + 1f);
        //The added vector is a small buffer that prevents collision with the environment while on the ground
		Ray2D dashRay = new Ray2D(cutAction.DashStartPosition + new Vector2(0, 0.075f), vec);
        
		actionHit = Physics2D.BoxCastAll(dashRay.origin, raySize, 0.0f, dashRay.direction, cutAction.MaxDashDistance, LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment")); //TODO: make length the distance of last completed dash
        foreach (RaycastHit2D hit in actionHit.OrderBy(x => x.distance))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    hit.collider.gameObject.GetComponent<IDamageable<float>>().takeDamage(playerStats.ActionDamage);
                }
                if (hit.collider.gameObject.tag == "Environment" && Vector2.Distance(cutAction.DashStartPosition, hit.collider.bounds.ClosestPoint(cutAction.DashStartPosition)) > .02f)
                {
                    return;
                }
            }
        }

    }
	

}
