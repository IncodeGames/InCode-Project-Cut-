using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    private Vector3 targetPosition;

	private Vector3 deadZone;
	private Vector3 camOffset;
    private Vector3 camVelocity;

    //-------------------------------------------------Functions-------------------------------------------------//
    void Start () 
	{
		deadZone = new Vector3 (1.0f, 0.0f, 0.0f);
		camOffset = new Vector3 (0.0f, 1.0f, -10.0f);
	}
	
	void FixedUpdate () //Should be in LateUpdate
	{
        if (GameObject.FindWithTag("Player") != null)
        {
            targetPosition = GameObject.FindWithTag("Player").transform.position;
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + camOffset, ref camVelocity, .125f); //+ (Vector2.Distance(transform.position, targetPosition) / 10.0f) quicker pan based on distance
        }

	}
}
