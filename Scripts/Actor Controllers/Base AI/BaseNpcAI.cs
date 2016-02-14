/***************************************************************************************
 *  Base Npc AI by Benjamin Lane
 *  Last Modified: 2016-Jan-18
 *
 *  Desc: Base Npc (non-combatant) AI behavior script. Should be the base for all
 *        Npc AI scripts.
 *
 *  Usage: Any Npc class should inherit from this script and extend functionality. This
 *         script implemnets underlying timing mechanics for calling functions, and
 *         handles events / calls delegates.
 ***************************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class BaseNpcAI : MonoBehaviour {

    [Header("Init. Variables")]
    [SerializeField]
    protected bool behaveOnStart = false;
    [SerializeField]
    protected bool loopBehavior = false;

    [Header("Npc Behaviors")]
    [Tooltip("An array for housing a number of iterative behaviors for the Npc to perform.")]
    [SerializeField]
    protected List<NpcBehavior> npcBehaviors = new List<NpcBehavior>();
    protected int currentBehavior = 0;

    private Queue behaviorQueue = new Queue();
    
    protected virtual void Start()
    {
        InitBehaviorQueue();
        if (behaveOnStart)
        {
            Behave();
        }
    }

    protected virtual IEnumerator Move()
    {
        while (Vector3.Distance(transform.position, npcBehaviors[currentBehavior].movePos) > .5f)
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, npcBehaviors[currentBehavior].movePos, 5 * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(npcBehaviors[currentBehavior].nextEventTimer);
        DequeueBehavior();
        Behave();
    }
    protected virtual IEnumerator WanderAimless()
    {
        Debug.Log("Wander");
        yield return new WaitForSeconds(npcBehaviors[currentBehavior].nextEventTimer);
        DequeueBehavior();
        Behave();
    }
    protected virtual void Vendor() { }

    protected virtual IEnumerator TalkWithPlayer()
    {
        Debug.Log("Talk Player");
        yield return new WaitForSeconds(npcBehaviors[currentBehavior].nextEventTimer);
        DequeueBehavior();
        Behave();
    }

    protected virtual IEnumerator TalkWithNPC()
    {
        Debug.Log("Talk NPC");
        yield return new WaitForSeconds(npcBehaviors[currentBehavior].nextEventTimer);
        DequeueBehavior();
        Behave();
    }

    protected virtual IEnumerator OpenDoor()
    {
        Debug.Log("Open Door");
        yield return new WaitForSeconds(npcBehaviors[currentBehavior].nextEventTimer);
        DequeueBehavior();
        Behave();
    }

    //Enqueue all of the serialized behaviors on Start
    protected void InitBehaviorQueue()
    {   
        foreach (NpcBehavior behavior in npcBehaviors)
        {
            behaviorQueue.Enqueue(behavior.npcBehavior);
        }
    }

    /// <summary>
    /// Adds a behavior to the end of the queue.
    /// </summary>
    /// <param name="newBehavior">The behavior to add.</param>
    protected void AddBehavior(NpcBehavior newBehavior)
    {
        behaviorQueue.Enqueue(newBehavior.npcBehavior);
        Debug.Log(behaviorQueue.Count);
    }

    /// <summary>
    /// Adds a behavior inside the queue by insertion. Does not delete the behavior at the index.
    /// </summary>
    /// <param name="newBehavior">Behavior to add</param>
    /// <param name="positionInQueue">Position at which to insert the Behavior. Shifts all proceedinng behaviors down one index.</param>
    protected void InsertBehaviorInsideQueue(NpcBehavior newBehavior, int positionInQueue)
    {
        //Error checking
        if (positionInQueue < 0)
        {
            Debug.Log("Position in queue cannot be negative.");
            return;
        }
        else if (positionInQueue > npcBehaviors.Count)
        {
            Debug.Log("Position in queue cannot be greater than the list length. Use AddBehavior to add to the end of the queue.");
            return;
        }

        //Clear the queue
        foreach (System.Object behavior in behaviorQueue)
        {
            behaviorQueue.Dequeue();
        }
        //Insert the new behavior and specified index
        npcBehaviors.Insert(positionInQueue, newBehavior);

        //Recreate the queue with added behavior
        foreach (NpcBehavior behavior in npcBehaviors)
        {
            behaviorQueue.Enqueue(behavior.npcBehavior);
            Debug.Log(behaviorQueue.Count);
        }
    }

    protected void DequeueBehavior()
    {

        if (behaviorQueue.Count >= 1)
        {
            behaviorQueue.Dequeue();

            //If the behavior is set to loop, add the most recently completed behavior to the end of the queue.
            if (loopBehavior == true)
            {
                behaviorQueue.Enqueue(npcBehaviors[currentBehavior].npcBehavior);
            }

            if (currentBehavior < npcBehaviors.Count - 1)
            {
                currentBehavior++;
            }
            else
            {
                currentBehavior = 0;
            }
        }
    }

    protected void Behave()
    {
        if (behaviorQueue.Count > 0)
        {
            switch ((NpcBehavior.BaseBehavior)behaviorQueue.Peek())
            {
                case NpcBehavior.BaseBehavior.Move:
                    StartCoroutine(Move());
                    break;
                case NpcBehavior.BaseBehavior.Wander_Aimless:
                    StartCoroutine(WanderAimless());
                    break;
                case NpcBehavior.BaseBehavior.Talk_Player:
                    StartCoroutine(TalkWithPlayer());
                    break;
                case NpcBehavior.BaseBehavior.Talk_NPC:
                    StartCoroutine(TalkWithNPC());
                    break;
                case NpcBehavior.BaseBehavior.Open_Door:
                    StartCoroutine(OpenDoor());
                    break;
                default:
                    Debug.Log("hello");
                    break;
            }
        }
    }

    protected virtual void Update()
    {
        //DequeueBehavior();
    }
}

[System.Serializable]
public class NpcBehavior { //TODO: write custom inspector to hide/show fields based on the selected behavior
    [Header("Behavior")]
    [Tooltip("(Optional) Name for organizing particular events. Does not affect the event.")]
    [SerializeField]
    private string instanceName;


    [Header("Behavior Attributes")]
    [Tooltip("Time until the next event occurs. Only runs once the current event is complete.")]
    public float nextEventTimer = 0.0f;

    [Tooltip("Position to move to if the move behavior is called")]
    public Vector3 movePos = new Vector3(1, 0, 0);

    public enum BaseBehavior
    {
        Move,
        Wander_Aimless,
        Talk_Player,
        Talk_NPC,
        Open_Door,
        Vendor,
        ChangeBehavior
    }
    [Header("Instance Behavior")]
    public BaseBehavior npcBehavior = new BaseBehavior();

    public void ChangeNpcBehavior(BaseBehavior state)
    {
        npcBehavior = state;
    }
}