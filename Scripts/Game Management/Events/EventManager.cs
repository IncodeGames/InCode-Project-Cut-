/***************************************************************************************
 *  Event Manager by Benjamin Lane
 *  Last Modified: 2016-Jan-17
 *
 *  Desc: General Event manager for handling game world events, and callbacks.
 *        Automatically cleans up whenever a level is loaded.
 *
 *  Usage: Event manager should always be attached to a GameObject in the scene.
 ***************************************************************************************/

 //#define LOG_ALL_MESSAGES

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    private static MessengerHelper messengerHelper;

    private Dictionary<int, UnityEvent> eventDictionary;

    //Message handlers that should never be removed, regardless of calling Cleanup
    public static List<int> permanentMessages = new List<int>();

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in the scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    //-------------------------------------------------Functions-------------------------------------------------//
    private void Init()
    {
        if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<int, UnityEvent>();
            messengerHelper = (new GameObject("MessengerHelper")).AddComponent<MessengerHelper>();
            messengerHelper.transform.parent = gameObject.transform;
        }
    }


    static public void MarkAsPermanent(string eventName)
    {
#if LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
#endif

        permanentMessages.Add(eventName.GetHashCode());
    }

    static public void Cleanup()
    {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
#endif

        List<int> messagesToRemove = new List<int>();

        foreach (KeyValuePair<int, UnityEvent> pair in instance.eventDictionary)
        {
            bool wasFound = false;

            foreach (int message in permanentMessages)
            {
                if (pair.Key == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key);
        }

        foreach (int message in messagesToRemove)
        {
            instance.eventDictionary.Remove(message);
        }
    }

    /// <summary>
    /// Adds a listener to the event dictionary of a given event. If the event does not exist, it creates a new event, before adding the listener and pushing to the dictionary.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening (string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName.GetHashCode(), out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName.GetHashCode(), thisEvent);
        }
    }

    /// <summary>
    /// Removes the listener of specified event. Returns if no event manager is found.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(string eventName, UnityAction listener)
    {
        //If the dictionary is removed for some reason, return.
        if (eventManager == null) { return; }

        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName.GetHashCode(), out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Invokes the event, which calls all functions that are listening on the event
    /// </summary>
    /// <param name="eventName"></param>
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName.GetHashCode(), out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}

//This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //Clean up eventTable every time a new level loads.
    public void OnLevelWasLoaded(int unused)
    {
        EventManager.Cleanup();
    }
}
