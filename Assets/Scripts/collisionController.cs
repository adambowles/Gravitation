using UnityEngine;
using System.Collections;

public class collisionController : MonoBehaviour {
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject == GameObject.Find ("Player")) {
			Main.Instance.deactivateSpecificCollectible(gameObject);
		}
	}
}
