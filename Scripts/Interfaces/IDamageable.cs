using UnityEngine;
using System.Collections;

//Interface for dealing damage to an actor given a damage parameter (excluding the player)
public interface IDamageable<T>
{
	void takeDamage (T damageTaken);

}