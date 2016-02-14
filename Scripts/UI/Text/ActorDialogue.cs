/***************************************************************************************
 *  Actor Dialogue by Benjamin Lane
 *  Last Modified: 2016-Jan-17
 *
 *  Desc: Derived from dialogue base this class enables basic dialogue with Npc's
 *
 *  Usage: Attach to any actor GameObject. Add an TextAsset file, set text speed, and
 *         specify the player and dialogue box.
 ***************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ActorDialogue : DialogueBase
{
    //-------------------------------------------------Variables-------------------------------------------------//
    private UnityAction SubmitActivateTextListener;
    private UnityAction SubmitNextLineListener;

    //-------------------------------------------------Functions-------------------------------------------------//
    void Awake()
    {
        player = GameObject.FindWithTag("Player");

        SubmitActivateTextListener = new UnityAction(ActivateText);
        SubmitNextLineListener = new UnityAction(ReadNextLine);
    }

    void OnEnable()
    {
        EventManager.StartListening("submitActivateText", SubmitActivateTextListener);
        EventManager.StartListening("submitNextLine", SubmitNextLineListener);
    }

    void OnDisable()
    {
        EventManager.StopListening("submitActivateText", SubmitActivateTextListener);
        EventManager.StopListening("submitNextLine", SubmitNextLineListener);
    }

    private void ActivateText()
    {
        if (dialogueTextUI.IsActive() == false && dialogueAllowed)
        {
            if (currentLine <= textLines.Length - 1)
            {
                enableDialogue(textLines[currentLine]);
            }
        }
    }

    private void ReadNextLine()
    {
        if (dialogueTextUI.IsActive() == true)
        {
            if (dialogueTextUI.text.Length < 100)
            {
                if (currentLine <= textLines.Length - 1)
                {
                    StartCoroutine(AnimText(textLines[currentLine]));
                    currentLine++;
                }
                else
                {
                    currentLine = lineToStart;
                    dialogueTextUI.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnDestroy()
    {
        EventManager.StopListening("submitActivateText", SubmitActivateTextListener);
        EventManager.StopListening("submitNextLine", SubmitNextLineListener);
    }
}
