/***************************************************************************************
 *  Simple Door by Benjamin Lane
 *  Last Modified: 2016-Jan-17
 *
 *  Desc: Basic door script, that can be locked and/or opened by Npc's through events.
 *
 *  Usage: Attach to the child of a door with a trigger collider on it. Modify whether
           the door is locked, and whether an Npc can open it.
 ***************************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SimpleDoor : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    private UnityAction NpcOpenDoorListener;

    [SerializeField]
    private bool doorLocked;
    public bool DoorLocked { get { return doorLocked; }  set { doorLocked = value; } }

    [SerializeField]
    private bool NpcEventInterest;

    private Collider2D trigCol;

    private bool inRange = false;
    //Accessors
    private GameObject player;


    //-------------------------------------------------Functions-------------------------------------------------//
    void Awake()
    {
        NpcOpenDoorListener = new UnityAction(NpcOpenDoor);
    }

	void Start ()
    {
        player = GameObject.FindWithTag("Player");
	}

    void OnEnable()
    {
        EventManager.StartListening("NpcOpenDoor", NpcOpenDoorListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("NpcOpenDoor", NpcOpenDoorListener);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        inRange = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        inRange = false;
        if ((col == player.GetComponent<Collider2D>()) || (col == GameObject.FindWithTag("Npc").GetComponent<Collider2D>()))
        {
            //TODO: play door animation
            Debug.Log("Door Closed.");
            transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void PlayerOpenDoor(Collider2D col)
    {
        if (col == player.GetComponent<Collider2D>() && player.GetComponent<PlayerController>().Submit && inRange)
        {
            //TODO: play door animation
            Debug.Log("Door opened.");
            transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void NpcOpenDoor()
    {
        //TODO: play door animation
        if (NpcEventInterest == true)
        {
            transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void Update()
    {
        PlayerOpenDoor(player.GetComponent<Collider2D>());
    }

    void OnDestroy()
    {
        EventManager.StopListening("NpcOpenDoor", NpcOpenDoorListener);
    }

}
