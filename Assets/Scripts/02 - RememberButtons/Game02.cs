using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.UI;				// To access UI elements
using UnityEngine.SceneManagement;	// To change scenes
using System.Collections.Generic;	// For lists
using System;						// For enums

public class Game02 : MonoBehaviour {

	public const int MAX_HP 				= 5;
	public const int LENGTH					= 8;
	private const float MEM_TIME			= 4.999f;
	private const float GUESS_TIME			= 4.999f;
	private const float CHECK_TIME			= 5.999f;
	private const int MAX_ROUNDS			= 5;
	private const int DIFF_CAP				= 3;
	private const float RATIO_SPAWN_TO_MAX	= 0.85f;
	private const float RATIO_CHECK_TO_MAX	= 0.6f;
	private const string PRE_COLOR			= "FFFFFFFF";
	private const string POST_COLOR			= "000000FF";
	private const string CORRECT_COLOR		= "388B3CFF";
	private const string WRONG_COLOR		= "AE2828FF";

	private List<GameObject> _buttonTypes;
	private List<GameObject> _realList;
	private List<int> _guessList;
	private float _timer;
	private float _maxTimer;
	private float _spawnInterval;
	private int _spawnCount;
	private bool _countingDown;
	private bool _guessing;
	private bool _checking;
	private float _checkInterval;
	private int _round;
	private int _difficulty;

	public GameObject ReadButtons;
	public GameObject InputButtons;
	public Text RoundCount;
	public GameObject Menu;
	public GameObject TimeRem;
	public GameObject PlayerContainer;
	public GameObject GuessCircles;

	public List<Player02> Players;
	private Player02 Me;


#region // Functions
	// Time remaining in countdown
	private float CountdownRemaining {
		get{return _maxTimer - _timer;}
	}
	private float CountdownRatioRemaining {
		get{return 1f - ((_timer + (_spawnCount * _spawnInterval)) / (_maxTimer + (_spawnCount * _spawnInterval)));}
	}
	private float GuessRatioInverted {
		get{return _timer/_maxTimer;}
	}
	// Caps the difficulty
	private int Difficulty {
		get{return Mathf.Max(DIFF_CAP - _difficulty, 0);}
	}
	private bool EndOfGame {
		get{
			int playersRemaining = 0; 
			foreach(Player02 player in Players) {
				if(player.HP > 0) {
					playersRemaining++;
				}
			}
			return (playersRemaining > 1)? false : true;
		}
	}
#endregion


	void Start() {
		ImportPlayers();
		InitGameVars();
	}

	void Update() {
		if(_countingDown) {
			_timer += Time.deltaTime;
			TimeRem.GetComponent<RectTransform>().localScale = new Vector2(CountdownRatioRemaining,1);
			if(_timer >= _spawnInterval && _spawnCount < LENGTH) {
				_spawnCount++;
				_maxTimer -= _timer;
				_timer = 0;
			}else if(CountdownRemaining <= 0) {
				_countingDown = false;
				_guessing = true;
				ReadButtons.SetActive(false);
				if(!Me.Lost) {
					LockInput(false);
				}
				_maxTimer = GUESS_TIME;
				_timer = 0f;
			}
			RectTransform currRect;
			float newScale;
			for(int i = 0; i < _realList.Count; i++) {
				currRect = ReadButtons.transform.GetChild(i).GetComponent<RectTransform>();
				newScale = currRect.gameObject.GetComponent<Button02>().RealScale;
				if(_spawnCount > i && newScale - currRect.localScale.x > 0.01f) {
					currRect.localScale = Vector2.Lerp(currRect.localScale, new Vector2(newScale,newScale), Time.deltaTime * 20f);
				}
			}
		}else if(_guessing) {
			_timer += Time.deltaTime;
			TimeRem.GetComponent<RectTransform>().localScale = new Vector2(GuessRatioInverted,1f);
			if(CountdownRemaining <= 0) {
				_guessing = false;
				_checking = true;
				LockInput(true);
				CheckAnswers();
			}
		}else if(_checking) {
			_timer += Time.deltaTime;
			if(_timer >= _checkInterval && _spawnCount < LENGTH) {
				SetCurrentGuess(_spawnCount);
				CheckGuess(_spawnCount);
				_spawnCount++;
				_maxTimer -= _timer;
				_timer = 0;
			}else if(CountdownRemaining <= 0) {
				_checking = false;
				foreach(Player02 player in Players) {
					player.SetCurrentGuess();
				}
				DetermineNextGameAction();
			}
		}
	}

	private void ImportPlayers() {
		int numPlayers = 4;
		Players = new List<Player02>();
		for(int i = 0; i < numPlayers; i++) {
			Players.Add((Instantiate(Resources.Load("02/Prefabs/Player"), PlayerContainer.transform) as GameObject).GetComponent<Player02>());
			Players[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-450f + (300f * i),0);
			Players[i].gameObject.transform.Find("Model").gameObject.GetComponent<Text>().text = (i + 1).ToString();
		}
		Me = Players[0];
	}

	private void InitGameVars() {
		_difficulty = -1;
		_round = 0;
		_checkInterval = CHECK_TIME * RATIO_CHECK_TO_MAX / LENGTH;
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
		RoundCount.text = _round + " / " + MAX_ROUNDS;
		_difficulty++;
		_maxTimer = MEM_TIME + Difficulty;
		_realList.Clear();
		_guessList.Clear();
		for(int i = 0; i < LENGTH; i++) {
			_realList.Add(_buttonTypes[Mathf.FloorToInt(UnityEngine.Random.value * (_buttonTypes.Count - 0.001f))]);
			RectTransform _rt = (Instantiate(_realList[i], ReadButtons.transform) as GameObject).GetComponent<RectTransform>();
			_rt.anchoredPosition = new Vector2(-450f + (300f * (i % 4)), 150f - (250f * (int)(i / 4f)));
			_rt.localScale = new Vector2(0,0);
		}
		foreach(Transform child in GuessCircles.transform) {
			child.gameObject.GetComponent<Image>().color = Functions.HexToColor(PRE_COLOR);
		}
		_spawnInterval = _maxTimer * (RATIO_SPAWN_TO_MAX - 0.05f * Difficulty)/ LENGTH;
		_spawnCount = 0;
		_timer = 0f;
		_countingDown = true;
		_guessing = false;
		_checking = false;
		ReadButtons.SetActive(true);
		LockInput(true);
	}

	public void QueueAnswer(string button) {
		GuessCircles.transform.GetChild(_guessList.Count).gameObject.GetComponent<Image>().color = Functions.HexToColor(POST_COLOR);
		int num = (int)Enum.Parse(typeof(ButtonType), button);
		_guessList.Add(num);
		if(_guessList.Count == _realList.Count) {
			LockInput(true);
		}
	}

	private void LockInput(bool _toLock) {
		Button[] _inputs = InputButtons.GetComponentsInChildren<Button>();
		foreach(Button _input in _inputs) {
			_input.interactable = !_toLock;
		}
	}

	private void CheckAnswers() {
		_maxTimer = CHECK_TIME;
		_timer = 0f;
		_spawnCount = 0;
		Me.GuessList = _guessList;

		// Development to set guess lists for computer players
		for(int i = 0; i < _realList.Count; i++) {
			if(!Players[1].Lost) {
				Players[1].GuessList[i] = 4;
			}else{
				Players[1].GuessList.Clear();
			}
			if(!Players[2].Lost) {
				//Players[2].GuessList[i] = (int)Enum.Parse(typeof(ButtonType), _realList[i].name);
				Players[2].GuessList[i] = 5;
			}else{
				Players[2].GuessList.Clear();
			}
			if(!Players[3].Lost) {
				Players[3].GuessList[i] = 6;
			}else{
				Players[3].GuessList.Clear();
			}
		}
		// End development
	}

	private void SetCurrentGuess(int index) {
		RectTransform _rt;
		foreach(Player02 player in Players) {
			if(index < player.GuessList.Count) {
				_rt = (Instantiate(_buttonTypes[player.GuessList[index]], player.gameObject.transform) as GameObject).GetComponent<RectTransform>();
				player.SetCurrentGuess(_rt);
			}else {
				player.SetCurrentGuess();
			}
		}
	}

	private void CheckGuess(int index) {
		bool correct;
		foreach(Player02 player in Players) {
			if(index >= player.GuessList.Count) {
				correct = false;
				player.WrongAnswer();
			}else if(player.GuessList[index] == (int)Enum.Parse(typeof(ButtonType), _realList[index].name)) {
				correct = true;
				player.CorrectAnswer();
			}else {
				correct = false;
				player.WrongAnswer();
			}
			if(player == Me && !player.Lost) {
				GuessCircles.transform.GetChild(_spawnCount).gameObject.GetComponent<Image>().color = (correct)? Functions.HexToColor(CORRECT_COLOR) : Functions.HexToColor(WRONG_COLOR);
			}
		}
	}

	private void DetermineNextGameAction() {
		if(!EndOfGame) {
			if(_round < MAX_ROUNDS) {
				NextRound();
			}else{
				Win();
			}
		}else{
			Lose();
		}
	}

	private void NextRound() {
		foreach(Transform child in ReadButtons.transform) {
			Destroy(child.gameObject);
		}
		foreach(Player02 player in Players) {
			player.SetCurrentGuess();
			if(player.HP <= 0) {
				player.Lost = true;
			}
		}
		InitRoundVars();
	}

	private void Lose() {
		Debug.Log("You lost");
		Menu.SetActive(true);
	}
	private void Win() {
		Debug.Log("You win");
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