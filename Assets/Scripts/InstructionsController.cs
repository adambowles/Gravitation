using UnityEngine;
using System.Collections;

public class InstructionsController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKey) {
			Application.LoadLevel("Menu");
		}
	}
}
