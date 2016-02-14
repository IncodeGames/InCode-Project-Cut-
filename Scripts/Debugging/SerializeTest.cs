using UnityEngine;
using System.Collections;
using GenericData;

public class SerializeTest : MonoBehaviour {
    PlayerData p1;

    void Awake()
    {
        p1 = new PlayerData();
        Debug.Log(p1.ActionDamage);
        Debug.Log(p1.MaxHealth);
        Debug.Log(p1.CurrentHealth);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 220, 100, 30), "Save"))
        {
            p1 = new PlayerData();
            Data.SavePDP(p1, "/playerInfo.dat");
        }
        if (GUI.Button(new Rect(10, 260, 100, 30), "Load"))
        {
            p1 = (PlayerData)Data.LoadPDP("/playerInfo.dat"); //TODO move save/load calls to manager class, make sure variables are assigned as below
            GameObject.FindWithTag("Player").GetComponent<PlayerStats>().MaxHealth = p1.MaxHealth;
            GameObject.FindWithTag("Player").GetComponent<PlayerStats>().CurrentHealth = p1.CurrentHealth;
            GameObject.FindWithTag("Player").GetComponent<PlayerStats>().ActionDamage = p1.ActionDamage;
        }
    }
}
