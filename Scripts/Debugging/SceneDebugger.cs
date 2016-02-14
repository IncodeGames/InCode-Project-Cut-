using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneDebugger : MonoBehaviour {

    public bool dontdestroyOnLoad;

    public int sceneToLoad;

    protected SceneDebugger() { }
    public static SceneDebugger instance = null;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (this.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.transform.root.gameObject);
            }
        }
        else if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown("q") && SceneManager.sceneCountInBuildSettings >= sceneToLoad)
        {
                SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("="))
        {
            Debug.Log(SceneManager.sceneCountInBuildSettings);
            sceneToLoad++;
            if (sceneToLoad > SceneManager.sceneCountInBuildSettings - 1)
            {
                sceneToLoad = SceneManager.sceneCountInBuildSettings - 1;
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }
        if (Input.GetKeyDown("-"))
        {
            if (sceneToLoad >= 1 && SceneManager.sceneCountInBuildSettings >= sceneToLoad)
            {
                sceneToLoad--;
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
