using UnityEngine;
using System.Collections;

//Interface for killing and destroying an actor (excluding the player)
public interface IKillable {
    void destroyOnDeath();
}