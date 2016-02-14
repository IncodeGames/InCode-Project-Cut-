/***************************************************************************************
 *  Dialogue Base by Benjamin Lane
 *  Last Modified: 2016-Jan-17
 *
 *  Desc: Base class for dialogue with Non-player characters. Has variables
 *        and functions for handling text output to a UI.
 *
 *  Usage: Inherit from this class and handle any nuances related to specific
 *         dialogue in a child class.
 ***************************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueBase : MonoBehaviour, IChattable<string> {

    //-------------------------------------------------Variables-------------------------------------------------//
    public bool readNextLine = false;

    protected bool dialogueAllowed = false;

    [Header("Text File")]
    [SerializeField]
    protected TextAsset dialogueFile;
    
    protected string[] textLines;
    protected string textLine;

    protected int currentLine;

    [Header("Text Modifiers")]
    [Tooltip("The line that this instanced character should begin at in the text file.")]
    [SerializeField]
    protected int lineToStart = 0;
    [Tooltip("Max number of lines to display at once.")]
    [SerializeField]
    protected int maxLines = 8;

    [SerializeField]
    [Tooltip("The speed at which the text is displayed letter by letter.")]
    [Range(0f, 10f)]
    private float textSpeed; //TODO: make this framerate independent and do cancelTyping

    private int endAtLine = 0;

    //Accessors
    [SerializeField]
    protected GameObject player;
    [SerializeField]
    protected Text dialogueTextUI;

    //-------------------------------------------------Functions-------------------------------------------------//

	protected void Start ()
    {
	    if (dialogueFile != null)
        {
            textLines = (dialogueFile.text.Split('\n'));
        }
        currentLine = lineToStart;
	}

    protected void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            dialogueAllowed = true;
        }

    }

    protected void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            dialogueAllowed = false;
            currentLine = lineToStart;
            dialogueTextUI.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Enables dialogue box if a string of any length is specified. If no end line is set in the inspector, the end line is set to the maximum number of lines.
    /// </summary>
    /// <param name="str"></param>
    public void enableDialogue(string str)
    {
        if (str.Length <= 0 || str == null)
        {
            return;
        }
        else
        {
            dialogueTextUI.transform.parent.gameObject.SetActive(true);
        }

        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }
    }

    protected void ChangeLineToStart(int newLine)
    {
        lineToStart = newLine;
    }

    /// <summary>
    /// Prints the string it receives as a parameter character by character.
    /// </summary>
    /// <param name="strComplete"></param>
    /// <returns></returns>
    protected IEnumerator AnimText(string strComplete)
    {
        string str = "";
        int i = 0;

        while (i < strComplete.Length)
        {
            str += strComplete[i++];
            if (dialogueTextUI.text != null)
            {
                dialogueTextUI.text = str;
            }
            yield return new WaitForSeconds(textSpeed/100);
        }
    }
}

//TODO: Set Enables/disables, cap lines, make parsing more efficient, stop trying to read another line if one is currently being written.