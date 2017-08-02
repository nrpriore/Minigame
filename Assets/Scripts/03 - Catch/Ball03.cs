using UnityEngine;

public class Ball03 : MonoBehaviour {

	// Gameplay vars
	public int PositionIndex;
	private bool _checked;
	private float _spawnY;
	private int _round;

	// Object references
	private Rigidbody _ball;
	private Transform _tr;
	private Transform _playerTR;
	private MeshRenderer _playerMesh;

	void Awake () {
		_tr = gameObject.transform;
		_ball = gameObject.GetComponent<Rigidbody>();
		_playerTR = Game03.Main.Player.gameObject.transform;
		_playerMesh = _playerTR.gameObject.GetComponent<MeshRenderer>();
		_spawnY = Camera.main.orthographicSize * 2f;
	}
	
	void Update () {
		if(_tr.localPosition.y <= Game03.Main.CheckY && !_checked) {
			if(Mathf.Abs(_tr.localPosition.x - _playerTR.localPosition.x) > _playerMesh.bounds.size.x / 3f) {
				_tr.localPosition = new Vector3(_tr.localPosition.x,_tr.localPosition.y,-1);
			}
			_checked = true;
		}else if(_tr.localPosition.y <= Game03.Main.PlayerY && _tr.localPosition.z == 0) {
			Game03.Main.Count = (Type == "Good")? Game03.Main.Count + 1 : Mathf.Max(0, Game03.Main.Count - 3);
			Game03.Main.CountText.text = Game03.Main.Count.ToString();
			DestroyBall();
		}else if(_tr.localPosition.y <= -Camera.main.orthographicSize) {
			DestroyBall();
		}
	}

	public void SetBall(int posIndex, int round) {
		PositionIndex = posIndex;
		_round = round;
		_tr.localPosition = new Vector3(PositionIndex * Game03.Main.DiffX,_spawnY,0);
		_ball.AddForce(Vector3.down * Game03.Main.Speed, ForceMode.Impulse);
	}

	private void DestroyBall() {
		if(_round == Game03.MAX_ROUNDS) {
			Game03.Main.EndGame();
		}
		Destroy(gameObject);
	}

	private string Type {
		get{return name.Substring(0, name.Length-7);}
	}
}
