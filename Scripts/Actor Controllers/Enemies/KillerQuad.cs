using UnityEngine;
using System.Collections;

public class KillerQuad : EnemyBase, IKillable, IDamageable<float> {

    //-------------------------------------------------Variables-------------------------------------------------//
    //Accessors
    


    //-------------------------------------------------Functions-------------------------------------------------//
    void Start()
	{
		SetBaseStats(100.0f, 1.0f, 10.0f, 0.5f, 5.0f, false);
	}


	void Update () 
	{
        checkVulnerability(health);
		destroyOnDeath();
	}
}
