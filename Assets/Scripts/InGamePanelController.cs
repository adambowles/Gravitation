using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGamePanelController : MonoBehaviour {
	
	// Singleton Instance
	public static InGamePanelController Instance { get; private set; }

	public Text scoreText;
	public Text timerText;
	public GameObject panel;

	void Awake()
	{
		// singleton locking code
		if (Instance == null) Instance = this;
		else Destroy( gameObject );
	}
	
	public void updateScore(int new_score)
	{
		scoreText.text = "Score: " + new_score;
	}
	
	public void updateTimer(int time)
	{
		timerText.text = "Time: " + time;
	}
}
