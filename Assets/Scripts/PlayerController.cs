using UnityEngine;			// To inherit from MonoBehaviour 

public class PlayerController : MonoBehaviour {

	private bool _onGround;


#region // Functions
	// Determine whether the player is eligible to jump
	private bool CanJump {
		get{return _onGround && gameObject.transform.localPosition.y >= -0.02f;}
	}
	// From the jump height and gravity we deduce the upwards speed for the character to reach at the apex
	public static float JumpSpeed {
		get{return Mathf.Sqrt(-2f * (1.5f) * Physics.gravity.y);}
 	}
#endregion
	

	// Runs when player is added to scene
	void Start() {
		GameController.Player = gameObject;
	}

	// Runs every frame
	void Update () {
		if(CanJump) {
			if(Input.GetKeyDown("space") || Input.touchCount > 0) {
				Jump();
			}
		}
		if(GameController.Lost) {
			gameObject.SetActive(false);
		}
	}

	private void Jump() {
		Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
		velocity.y = JumpSpeed;
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
		_onGround = false;
	}

	void OnCollisionEnter(Collision col) {
		switch(col.gameObject.name) {
			case "Platform":
				_onGround = true;
				break;
			case "Bullet":

				break;
			case "Coin":

				break;
		}
	}

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
