using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	private GameObject player;
//	private bool inverse_gravity = false;

	private bool moving_left;
	private bool moving_right;
	private bool jumping;

	private float movement_force = 75;
	private float maxSpeed = 7.5f;
	
	private Vector3 global_left;
	private Vector3 global_right;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");

		global_right = GameObject.Find ("Bottom").transform.right;
		global_left = -global_right;
	}
	
	// Update is called once per frame
	void Update () {

		// Jumping
		if (Input.GetKeyDown ("space")) {
			jumping = true;
			moving_left = false;
			moving_right = false;
		}

		// Moving left
		if (Input.GetKey ("left")) {
			moving_left = true;
		}
		if (Input.GetKeyUp ("left")) {
			moving_left = false;
		}

		// Moving right
		if (Input.GetKey ("right")) {
			moving_right = true;
		}
		if (Input.GetKeyUp ("right")) {
			moving_right = false;
		}

		// If player wants to move the avatar
		if (moving_left || moving_right) {
			if (moving_left) {
				if(player.rigidbody.velocity.x > maxSpeed * -1) {
					player.rigidbody.AddForce (global_left * movement_force);
				} else {
					Vector3 new_velocity = new Vector3(maxSpeed * -1,
					                                   player.rigidbody.velocity.y,
					                                   player.rigidbody.velocity.z);
					player.rigidbody.velocity = new_velocity;
				}

				moving_left = false;
			}
			
			if (moving_right) {
				if(player.rigidbody.velocity.x < maxSpeed) {
					player.rigidbody.AddForce (global_right * movement_force);
				} else {
					Vector3 new_velocity = new Vector3(maxSpeed,
					                                   player.rigidbody.velocity.y,
					                                   player.rigidbody.velocity.z);
					player.rigidbody.velocity = new_velocity;
				}

				moving_right = false;
			}
		}
		
		if (jumping) {
			if(!Main.Instance.getGravityDirection()) {
				Vector3 new_velocity = new Vector3(player.rigidbody.velocity.x,
				                                   maxSpeed,
				                                   player.rigidbody.velocity.z);
				player.rigidbody.velocity = new_velocity;
			} else {
				Vector3 new_velocity = new Vector3(player.rigidbody.velocity.x,
				                                   maxSpeed * -1,
				                                   player.rigidbody.velocity.z);
				player.rigidbody.velocity = new_velocity;
			}

			jumping = false;
		}

	}
}
