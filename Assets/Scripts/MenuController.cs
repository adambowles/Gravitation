using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class MenuController : MonoBehaviour {

	// Singleton Instance
	public static MenuController Instance { get; private set; }

	public Text menu;
	private List<string> menuItems;
	private int selectedOption;

	public GameObject indicator;
	// stores where the indicator is currently supposed to be
	// as it may not have been tweened to that point yet (still on it's way over)
	public int indicatorAt;
	
	void Awake()
	{
		// singleton locking code
		if (Instance == null) Instance = this;
		else Destroy( gameObject );
	}

	// Use this for initialization
	void Start () {
		menuItems = new List<string>();
		menuItems.Add ("New game");
		menuItems.Add ("How to play");

		selectedOption = 0;
		positionIndicator (true);

		string menuText = string.Join ("\n\n", menuItems.ToArray ());
		menu.text = menuText;

		indicator.transform.DORotate (new Vector3 (0, 0, -360), 1f, (RotateMode)RotateMode.LocalAxisAdd)
						   .SetEase (Ease.Linear)
						   .SetLoops (-1);
	}
	
	// Update is called once per frame
	void Update () {
		positionIndicator ();
//		animateIndicator ();
	}
	
	public void moveUp()
	{
		if (selectedOption == 0) {
			selectedOption = menuItems.Count - 1;
		} else {
			selectedOption--;
		}
	}
	
	public void moveDown()
	{
		if (selectedOption == menuItems.Count - 1) {
			selectedOption = 0;
		} else {
			selectedOption++;
		}
	}
	
	public void activate()
	{
		if (menuItems[selectedOption] == "New game") {
			Application.LoadLevel("Main");
		}
		if (menuItems[selectedOption] == "How to play") {
			Application.LoadLevel("Instructions");
		}
	}
	
	private void positionIndicator(bool force = false)
	{
		// Only tween the indicator once per user input, but allow overriding
		bool move = force || selectedOption != indicatorAt;
		if (move) {
			int count = menuItems.Count;
			int index = selectedOption;
			float gap = 0.8f; // drawn gap between menu options

			float X = indicator.transform.position.x;
			float Y = (gap * (count - 1)) - ((gap * 2) * index);
			float Z = indicator.transform.position.z;
			Vector3 newPosition = new Vector3(X, Y, Z);
			indicator.transform.DOMove (newPosition, 0.1f);
			indicatorAt = selectedOption;
		}
	}
	
//	private void animateIndicator()
//	{
//		float currentRotationX = indicator.transform.rotation.x;
//		float currentRotationY = indicator.transform.rotation.y;
//		float currentRotationZ = indicator.transform.rotation.z;
//		Vector3 newRotation = new Vector3(currentRotationX, currentRotationY, currentRotationZ - 10);
//		indicator.transform.DORotate (newRotation, 0.1f, (RotateMode)RotateMode.LocalAxisAdd);
//	}
}
