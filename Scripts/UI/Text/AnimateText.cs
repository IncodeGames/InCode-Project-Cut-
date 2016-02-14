using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimateText : MonoBehaviour {

    private string str;

    [SerializeField]
    [Range(.05f, .5f)]
    private float textSpeed;
    [SerializeField]
    private Text dialogue;

    protected IEnumerator AnimText(string strComplete)
    {
        int i = 0;
        str = "";

        while (i < strComplete.Length)
        {
            str += strComplete[i++];
            yield return new WaitForSeconds(textSpeed);
            dialogue.text = str;
        }
    }
	
}
