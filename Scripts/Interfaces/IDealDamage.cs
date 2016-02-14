using UnityEngine;
using System.Collections;

//Interface for dealing damage to the player from an arbitrary actor
public interface IDealDamage<T> {
	float dealDamageOnHit();
	//float dealDamage(T damageDealt);
}
