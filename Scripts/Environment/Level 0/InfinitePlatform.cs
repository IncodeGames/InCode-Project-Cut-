using UnityEngine;
using System.Collections;

public class InfinitePlatform : MonoBehaviour {

    //TODO: clean up this fucking trash class, probably use nodes for keeping track of platforms

    [SerializeField]
    private GameObject[] platforms = new GameObject[4];
    private GameObject currentPlatform;
    private GameObject lastPlatform;

    [SerializeField]
    private GameObject player;

    private bool startSetPos;
    private bool callOnce;
	
	void Start()
    {
        currentPlatform = platforms[0];
        lastPlatform = platforms[0];
    }

	void Update () {

        if (Vector3.Distance(player.transform.position, platforms[0].transform.position) < 20)
        {
            if (startSetPos)
            {
                lastPlatform = platforms[2];
            }
            currentPlatform = platforms[0];
        }
        if (Vector3.Distance(player.transform.position, platforms[1].transform.position) < 20)
        {
            startSetPos = true;
            lastPlatform = platforms[0];
            currentPlatform = platforms[1];
        }
        if (Vector3.Distance(player.transform.position, platforms[2].transform.position) < 20)
        {
            lastPlatform = platforms[1];
            currentPlatform = platforms[2];
        }

        if ( Vector3.Distance(player.transform.position, lastPlatform.transform.position) > 30 && Vector3.Distance(player.transform.position, lastPlatform.transform.position) < 31 && !callOnce)
        {
            Debug.Log(Vector3.Distance(player.transform.position, lastPlatform.transform.position));
            lastPlatform.transform.position = new Vector3(lastPlatform.transform.position.x + 90, 0, 0);
            platforms[3].transform.position = new Vector3(currentPlatform.transform.position.x - 30, 0, 0);
            callOnce = true;
        }
        else
        {
            callOnce = false;
        }
	}
}
