using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState
{
	Intro,
	Menu,
	Running,
	Paused,
	Game_Over
}

public class GameStateManager : MonoBehaviour {

	protected GameStateManager(){}
	public static GameStateManager instance = null;
	public GameState gameState { get; private set; }

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

	public void setGameState (GameState state) 
	{
		this.gameState = state;
	}

	public void OnApplicationQuit()
	{
		instance = null;
	}
}
