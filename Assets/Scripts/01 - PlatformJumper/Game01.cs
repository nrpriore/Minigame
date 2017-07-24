using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.UI;				// To set UI properties
using UnityEngine.SceneManagement;	// To change scenes

public class Game01 : MonoBehaviour {

	// Gameplay constants
	private const float MAX_TIME 			= 45.999f;
	private const float MIN_MULT 			= 10f;
	private const float MULT_RANGE 			= 18f;
	private const float PERCENT_TO_MAX_MULT	= 2f/3f;
	private const float GRAVITY 			= -60f;
	private const float COUNTDOWN			= 3.999f;

	// Gameplay variables
	public static GameObject Player;			// The player's gameobject
	public static bool Ended;					// If the game has ended
	public static bool CountingDown;			// If the game is counting down
	private static float _timer;				// Current duration of game
	private static Platform01 _finalPlatform;	// Platform that spawns when no time left

	// UI variables
	public static float PlatformStartX;
	private Transform _prevPlatform;

	// Canvas UI variables
	public static Text GOTimeRem;
	public static Text GONumPlatforms;
	public static GameObject GOMenu;
	public static GameObject GORestart;
	public static Text GOCountdown;

	// Stats
	public static int NumPlatforms;


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
	// Time remaining in match
	private int TimeRemaining {
		get{return (int)(MAX_TIME - _timer);}
	}
	// Time remaining in countdown
	private int CountdownRemaining {
		get{return (int)(COUNTDOWN - _timer);}
	}
	// Returns % of max game speed (0 - 1)
	private static float TimeRatio {
		get{return Mathf.Min(_timer / (PERCENT_TO_MAX_MULT * MAX_TIME),1f);}
	}
	// Returns when player has lost
	public static bool Lost {
		get{return (Player.transform.localPosition.y <= -20f) || !Player.activeSelf;}
	}
#endregion


	// Runs on game start
	void Start() {
		// Init vars
		GOTimeRem = GameObject.Find("TimeNum").GetComponent<Text>();
		GONumPlatforms = GameObject.Find("PlatformNum").GetComponent<Text>();
		GOMenu = GameObject.Find("Menu");
		GOMenu.SetActive(false);
		GORestart = GameObject.Find("Restart");
		GORestart.SetActive(false);
		GOCountdown = GameObject.Find("CountdownTime").GetComponent<Text>();
		_timer = 0f;
		NumPlatforms = 0;
		PlatformStartX = 10f * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * (Camera.main.aspect * 1.5f);
		Physics.gravity = new Vector3(0,GRAVITY,0);
		Ended = true;
		CountingDown = true;
		// Init objects
		CreatePlatform();
		Player = Instantiate(Resources.Load("Player")) as GameObject;
	}

	// Creates platform based on game variables
	private Platform01 CreatePlatform(float diff = 0, bool final = false) {
		Platform01 _pc = (Instantiate(Resources.Load("01/Prefabs/Platform")) as GameObject).GetComponent<Platform01>();
		if(_prevPlatform == null || final) {
			_pc.CreatePlatform(12);
		}else {
			int size = (int)(Random.value * (1 - TimeRatio) * 8f) + (int)(Random.value * 8f);
			_pc.CreatePlatform(size);
		}
		float spawnX = (_prevPlatform == null)? PlatformStartX : PlatformStartX + _pc.PWidth - diff;
		_pc.gameObject.transform.localPosition = new Vector3(spawnX,-1f,0);
		_prevPlatform = _pc.gameObject.transform;
		NumPlatforms++;
		return _pc;
	}

	// Runs every frame
	void Update() {
		if(CountingDown) {
			if(CountdownRemaining > 0) {
				_timer += Time.deltaTime;
				GOCountdown.text = CountdownRemaining.ToString();
			}else{
				_timer = 0f;
				CountingDown = false;
				Ended = false;
				GOCountdown.gameObject.transform.parent.gameObject.SetActive(false);
			}
		}else if(Lost) {
			if(!Ended) {
				EndGame();
			}
		}else{
			if(TimeRemaining > 0f && !Ended) {
				// Create platform after gapsize
				if(_prevPlatform.localPosition.x <= PlatformStartX - GapSize) {
					CreatePlatform(PlatformStartX - GapSize - _prevPlatform.localPosition.x);
				}
				// Set UI variables
				_timer += Time.deltaTime;
				GOTimeRem.text = TimeRemaining.ToString();
			}else if(_finalPlatform == null) {
				if(_prevPlatform.localPosition.x <= PlatformStartX - GapSize) {
					_finalPlatform = CreatePlatform(PlatformStartX - GapSize - _prevPlatform.localPosition.x, true);
				}
			}
		}
	}

	// Runs when collision with platforms
	public static void PlayerLand(Platform01 _platform) {
		if(_platform.Landed || _platform.ID == 0) {
			return;
		}
		_platform.Landed = true;
		GONumPlatforms.text = _platform.ID.ToString();
		if(_finalPlatform != null) {
			if(_platform.ID == _finalPlatform.ID) {
				EndGame();
			}
		}
	}

	private static void EndGame() {
		Ended = true;
		GOMenu.SetActive(true);
		if(MenuController.IsFreePlay) {
			GORestart.SetActive(true);
		}
	}

	public void BackToMenu() {
		SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
	}
	public void Restart() {
		SceneManager.LoadSceneAsync("Game01", LoadSceneMode.Single);
	}

}
