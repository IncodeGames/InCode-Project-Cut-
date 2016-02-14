using UnityEngine;
using System.Collections;

public class OrbitPoint : MonoBehaviour {

    [SerializeField]
    private Vector3 rotatePoint;
	
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5F);
        Gizmos.DrawWireSphere(rotatePoint, .1f);
    }
	// Update is called once per frame
	void Update ()
    {
        transform.RotateAround(rotatePoint, Vector3.forward, 20 * Time.deltaTime);
	}
}
