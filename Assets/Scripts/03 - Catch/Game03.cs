using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Game03 : MonoBehaviour {

	// Static reference to self
	public static Game03 Main;

	// Constants
	public const float INDEX_SCREEN_PERCENT 	= 0.2f;
	public const int MAX_ROUNDS					= 100;
	private const float BASE_SPEED 				= 6f;
	private const float MAX_DIFF				= 3f;
	private const float BASE_INTERVAL			= 0.5f;
	private const float MIN_INTERVAL			= 0.2f;
	private const float PERCENT_TO_MAX			= 0.6f;	

	// Gameplay
	public int Count;
	public float CheckY;
	public float PlayerY;
	public float DiffX;
	private float _timer;
	private int _round;
	private List<GameObject> _balls;
	private List<int> _indexes;
	private GameObject _goodBall;
	private GameObject _badBall;

	// Object references
	public Player03 Player;
	public Text CountText;
	public GameObject Menu;

	void Start () {
		Main = this;
		PlayerY = Player.transform.localPosition.y;
		CheckY = PlayerY + (Player.GetComponent<MeshRenderer>().bounds.size.y / 3f);
		DiffX = (2f * Camera.main.orthographicSize * Camera.main.aspect) * INDEX_SCREEN_PERCENT;
		_balls = new List<GameObject>();
		_indexes = new List<int>();
		_goodBall = Resources.Load<GameObject>("03/Prefabs/Good");
		_badBall = Resources.Load<GameObject>("03/Prefabs/Bad");
		Menu.SetActive(false);

		Count = 0;
		_timer = 0f;
		_round = 0;
	}
	
	void Update () {
		if(_timer >= Interval && _round < MAX_ROUNDS) {
			CreateRound();
			_timer = 0;
		}
		_timer += Time.deltaTime;
	}

	private void CreateRound() {
		_balls.Clear();
		_indexes.Clear();
		float randNum = Random.value;
		float randCol = Random.value;
		if(randNum <= 0.5f) { 		// One ball
			if(randCol <= 0.5f) {		// Green
				_balls.Add(_goodBall);
				_round++;
			}else{						// Red
				_balls.Add(_badBall);
			}
		}else if(randNum <= 0.8f) {	// Two balls
			if(randCol <= 0.7f) {		// Green & Red
				_balls.Add(_goodBall);
				_balls.Add(_badBall);
				_round++;
			}else{						// Red & Red
				_balls.Add(_badBall);
				_balls.Add(_badBall);
			}
		}else {						// No balls
			return;
		}
		CreateBalls();
	}

	private void CreateBalls() {
		for(int i = 0; i < _balls.Count; i++) {
			Ball03 ball = (Instantiate(_balls[i]) as GameObject).GetComponent<Ball03>();
			int index = GetIndex();
			while(_indexes.Contains(index)) {
				index = GetIndex();
			}
			_indexes.Add(index);
			ball.SetBall(index, _round);
		}
	}
	private int GetIndex() {
		return -1 + (int)(Random.value * 2.999f);
	}

	public void LeanLeft() {
		Player.Lean("left");
	}
	public void StopLean(string direction) {
		Player.Lean("stop" + direction);
	}
	public void LeanRight() {
		Player.Lean("right");
	}

	public void EndGame() {
		Menu.SetActive(true);
	}
	public void BackToMenu() {
		SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
	}

	public float Speed {
		get{return BASE_SPEED + Difficulty;}
	}
	private float Difficulty {
		get{return MAX_DIFF * RoundRatio;}
	}
	private float Interval {
		get{float temp = BASE_INTERVAL - ((BASE_INTERVAL - MIN_INTERVAL) * RoundRatio);
			Debug.Log(temp);
			return temp;}
	}
	private float RoundRatio {
		get{return Mathf.Min(1,(float)_round / (MAX_ROUNDS * PERCENT_TO_MAX));}
	}
}
