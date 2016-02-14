using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefaultHUD : MonoBehaviour {

    private Text healthText;
    public Text HealthText { get { return healthText; } set { healthText = value; } }  //TODO: set this only when player health changes to reduce Update calls
    private PlayerStats playerStats;

    [SerializeField]
    private GameObject healthSet;

    private GameObject[] healths = new GameObject[100]; // Update health objects based on current health

    void Awake ()
    {
        healthText = gameObject.GetComponent<Text>();
        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        GameObject clone = Instantiate(healthSet);
        clone.transform.SetParent(gameObject.transform);
	}

	// Update is called once per frame
	void Update ()
    {
        
        healthText.text = "Health: " + playerStats.CurrentHealth.ToString();
        
	}
}
