using UnityEngine;
using System.Collections;

public class particleDeathCleanup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 5.0f);
	}
}
