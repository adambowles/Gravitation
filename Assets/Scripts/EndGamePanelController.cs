using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGamePanelController : MonoBehaviour {
	
	// Singleton Instance
	public static EndGamePanelController Instance { get; private set; }

	public Text scoreText;
	public GameObject panel;
	
	void Awake()
	{
		// singleton locking code
		if (Instance == null) Instance = this;
		else Destroy( gameObject );
	}
	
	void Update()
	{
		if ( Input.GetKeyDown ("return")) {
			Application.LoadLevel("Menu");
		}
	}
	
	public void updateScore(int new_score)
	{
		scoreText.text = "Game over!\n\nYou scored:\n" + new_score;

		if (new_score < 50) {
			scoreText.text += "\n\nBetter luck next time";
		} else if (new_score < 100) {
			scoreText.text += "\n\nPretty good";
		} else if (new_score < 150) {
			scoreText.text += "\n\nNice!";
		} else {
			scoreText.text += "\n\nAmazing!";
		}

		scoreText.text += "\n\nPress enter to return to the menu";
	}
}
