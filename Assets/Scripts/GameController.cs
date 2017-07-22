using UnityEngine;			// To inherit from MonoBehaviour 
using UnityEngine.UI;		// To set UI properties
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// Gameplay constants
	private const float MAX_TIME 			= 45f;
	private const float MIN_MULT 			= 10f;
	private const float MULT_RANGE 			= 18f;
	private const float PERCENT_TO_MAX_MULT	= 2f/3f;
	private const float GRAVITY 			= -60f;

	// Gameplay variables
	private static float _timer;		// Current duration of game
	public static GameObject Player;	// The player's gameobject

	// UI variables
	private float _platformStartX;
	private Transform _lastPlatform;

	// Public UI variables
	public Text GOTimeRem;


#region // Functions
	// Alters speed variable into a value usable by the game
	public static float UISpeedMult {
		get{return Mathf.Min(MIN_MULT + (TimeRatio * MULT_RANGE), MIN_MULT + MULT_RANGE);}
	}
	public static float SpeedMult {
		get{return UISpeedMult / 100f;}
	}
	// Returns the size of the gap between platforms based on game speed
	private float GapSize {
		//get{return 0.5f * (float)_speedMult - 0.5f;}
		get{return 0.25f * UISpeedMult - 0.5f;}
	}
	private int TimeRemaining {
		get{return (int)(MAX_TIME - _timer);}
	}
	private static float TimeRatio {
		get{return Mathf.Min(_timer / (PERCENT_TO_MAX_MULT * MAX_TIME),1f);}
	}
	public static bool Lost {
		get{return (Player.transform.localPosition.y <= -20f) || !Player.activeSelf;}
	}
#endregion


	// Runs on game start
	void Start() {
		_platformStartX = 10f * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * (Camera.main.aspect * 1.5f);

		_timer = 0f;
		Physics.gravity = new Vector3(0,GRAVITY,0);
		CreatePlatform();
		Player = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
	}

	// Creates platform based on game variables
	private void CreatePlatform() {
		PlatformController _pc = (Instantiate(Resources.Load("Prefabs/Platform")) as GameObject).GetComponent<PlatformController>();
		float spawnX;
		if(_lastPlatform == null) {
			_pc.CreatePlatform(12);
			spawnX = _platformStartX;
		}else {
			int size = (int)(Random.value * (1 - TimeRatio) * 8f) + (int)(Random.value * 8f);
			_pc.CreatePlatform(size);
			spawnX = _platformStartX + _pc.PWidth;
		}
		_pc.gameObject.transform.localPosition = new Vector3(spawnX,-1f,0);
		_lastPlatform = _pc.gameObject.transform;
	}

	// Runs every frame
	void Update() {
		if(Lost) {
			if(Input.GetKey("space") || Input.touchCount > 0) {
				SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
			}
		}else{
			if(_lastPlatform.localPosition.x <= _platformStartX - GapSize) {
				CreatePlatform();
			}
			_timer += Time.deltaTime;
			// Set UI variables
			GOTimeRem.text = TimeRemaining.ToString();
		}
	}

}
