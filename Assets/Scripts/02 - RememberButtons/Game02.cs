using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.UI;				// To access UI elements
using UnityEngine.SceneManagement;	// To change scenes
using System.Collections.Generic;	// For lists
using System;						// For enums

public class Game02 : MonoBehaviour {

	private const int MAX_HP 				= 8;
	private const float MEM_TIME			= 11.999f;
	private const float GUESS_TIME			= 8.999f;
	private const int LENGTH				= 8;

	private List<GameObject> _buttonTypes;
	private List<GameObject> _realList;
	private List<int> _guessList;
	private float _timer;
	private float _maxTimer;
	private bool _countingDown;
	private bool _guessing;
	private int _round;
	private int _difficulty;
	private int HP;

	public GameObject ReadButtons;
	public GameObject InputButtons;
	public GameObject Menu;
	public Text CountdownRem;
	public Text TimeRem;


#region // Functions
	// Time remaining in countdown
	private int CountdownRemaining {
		get{return (int)(_maxTimer - _timer);}
	}
	// Caps the difficulty
	private int Difficulty {
		get{return Mathf.Min(_difficulty, 3);}
	}
#endregion


	void Start() {
		InitGameVars();
	}

	void Update() {
		if(_countingDown) {
			_timer += Time.deltaTime;
			CountdownRem.text = CountdownRemaining.ToString();
			if(CountdownRemaining <= 0) {
				_countingDown = false;
				_guessing = true;
				CountdownRem.gameObject.transform.parent.gameObject.SetActive(false);
				ReadButtons.SetActive(false);
				LockInput(false);
				_maxTimer = GUESS_TIME;
				_timer = 0f;
			}
		}else if(_guessing) {
			_timer += Time.deltaTime;
			TimeRem.text = CountdownRemaining.ToString();
			if(CountdownRemaining <= 0) {
				_guessing = false;
				LockInput(true);
				CheckAnswers();
			}
		}
	}

	private void InitGameVars() {
		_difficulty = 0;
		_round = 0;
		HP = MAX_HP;
		_realList = new List<GameObject>();
		_guessList = new List<int>();
		_buttonTypes = new List<GameObject>();
		foreach(ButtonType button in Enum.GetValues(typeof(ButtonType))) {
			_buttonTypes.Add(Resources.Load<GameObject>("02/Prefabs/" + button));
		}
		Menu.SetActive(false);
		InitRoundVars();
	}

	private void InitRoundVars() {
		_round++;
		_difficulty++;
		_maxTimer = MEM_TIME - Difficulty;
		_realList.Clear();
		_guessList.Clear();
		for(int i = 0; i < LENGTH; i++) {
			_realList.Add(_buttonTypes[Mathf.FloorToInt(UnityEngine.Random.value * (_buttonTypes.Count - 0.001f))]);
			RectTransform _rt = (Instantiate(_realList[i], ReadButtons.transform) as GameObject).GetComponent<RectTransform>();
			_rt.anchoredPosition = new Vector2(-450f + (300f * (i % 4)), 150f - (300f * (int)(i / 4f)));
		}
		_timer = 0f;
		_round++;
		_countingDown = true;
		CountdownRem.gameObject.transform.parent.gameObject.SetActive(true);
		ReadButtons.SetActive(true);
		_guessing = false;
		LockInput(true);
	}

	public void QueueAnswer(string button) {
		int num = (int)Enum.Parse(typeof(ButtonType), button);
		_guessList.Add(num);
		if(_guessList.Count == _realList.Count) {
			_guessing = false;
			LockInput(true);
			CheckAnswers();
		}
	}

	private void LockInput(bool _toLock) {
		Button[] _inputs = InputButtons.GetComponentsInChildren<Button>();
		foreach(Button _input in _inputs) {
			_input.interactable = !_toLock;
		}
	}

	private void CheckAnswers() {
		TimeRem.text = "0";
		for(int i = 0; i < _realList.Count; i++) {
			if(_guessList[i] == (int)Enum.Parse(typeof(ButtonType), _realList[i].name)) {
				CorrectAnswer();
			}else {
				WrongAnswer();
			}
		}
		if(HP > 0) {
			NextRound();
		}else{
			Lose();
		}
	}

	private void CorrectAnswer() {

	}
	private void WrongAnswer() {
		HP--;
		Debug.Log(HP);
	}

	private void NextRound() {
		foreach(Transform child in ReadButtons.transform) {
			Destroy(child.gameObject);
		}
		InitRoundVars();
	}

	private void Lose() {
		Debug.Log("You lost");
		Menu.SetActive(true);

	}

	public void BackToMenu() {
		SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
	}
}

enum ButtonType {
	Up,
	Down,
	Left,
	Right,
	A,
	B,
	C,
	D
}