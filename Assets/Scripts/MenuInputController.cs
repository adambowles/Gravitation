using UnityEngine;
using System.Collections;

public class MenuInputController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		// Menu navigation
		if (Input.GetKeyDown ("up")) {
			MenuController.Instance.moveUp();
		}
		
		if ( Input.GetKeyDown ("down")) {
			MenuController.Instance.moveDown();
		}
		
		if ( Input.GetKeyDown ("return")) {
			MenuController.Instance.activate();
		}
	}
}
