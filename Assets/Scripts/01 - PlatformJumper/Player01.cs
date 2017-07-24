using UnityEngine;					// To inherit from MonoBehaviour 

public class Player01 : MonoBehaviour {

	private bool _onGround;			// Is the player on the ground?


#region // Functions
	// Determine whether the player is eligible to jump
	private bool CanJump {
		get{return !Game01.Ended && _onGround && gameObject.transform.localPosition.y >= -0.05f;}
	}
	// From the jump height and gravity we deduce the upwards speed for the character to reach at the apex
	public static float JumpSpeed {
		get{return Mathf.Sqrt(-2f * (1.5f) * Physics.gravity.y);}
 	}
#endregion
	

	// Runs when player is added to scene
	void Start() {
		Game01.Player = gameObject;
	}

	// Runs every frame
	void Update () {
		if(CanJump) {
			if(Input.GetKeyDown("space") || Input.touchCount > 0) {
				Jump();
			}
		}
		if(Game01.Lost) {
			gameObject.SetActive(false);
		}
	}

	// Player jumps
	private void Jump() {
		Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
		velocity.y = JumpSpeed;
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
		_onGround = false;
	}

	// Case switch the collision event to determine outcome
	void OnCollisionEnter(Collision col) {
		switch(col.gameObject.name) {
			case "Platform":
				_onGround = true;
				if(CanJump) {
					Game01.PlayerLand(col.transform.parent.gameObject.GetComponent<Platform01>());
				}
				break;
		}
	}

	// Case switch the collision event to determine outcome
	void OnCollisionExit(Collision col) {
		switch(col.gameObject.name) {
			case "Platform":
				if(gameObject.transform.localPosition.y <= -0.05f) {
					_onGround = false;
				}
				break;
		}
	}

}
