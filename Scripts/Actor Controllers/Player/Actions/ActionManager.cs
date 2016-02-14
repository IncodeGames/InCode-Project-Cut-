using UnityEngine;
using System.Collections;

public enum ActionState
{
    ActionReady,
    Action,
    ActionCooldown
}
//TODO: determine whether this belongs on the character, as a static class, or as a separate manager
public class ActionManager : MonoBehaviour {

    //-------------------------------------------------Variables-------------------------------------------------//
    public delegate void ActionDelegate();
    public ActionDelegate actionDelegate;

    void FixedUpdate()
    {
        //actionDelegate();
    }
}
