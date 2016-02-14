using UnityEngine;
using System.Collections;

public class ShadowAnim : MonoBehaviour {

    private Animator anim;
    private Animator parentAnim;
	
	void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        parentAnim = transform.parent.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        anim.SetFloat("Run Speed", parentAnim.GetFloat("Run Speed"));
    }
}
