using UnityEngine;

public class Player03 : MonoBehaviour {

	// Constanrs
	private const float POS_LERP_THRESHOLD 		= 0.05f;

	// Gameplay
	// Player leaning -1: Left, 0: Center, 1: Right
	public int TargetPositionIndex;		// Intended movement
	public int PositionIndex;			// Set when player completes movement
	private bool _leaningLeft;
	private bool _leaningRight;

	// Object references
	private Transform _tr;				

	void Start () {
		_tr = gameObject.transform;

		TargetPositionIndex = 0;
		PositionIndex = 0;

		_leaningLeft = false;
		_leaningRight = false;
	}
	
	void Update () {
		if(PositionIndex != TargetPositionIndex || (TargetPositionIndex == 0 && Mathf.Abs(_tr.localPosition.x) > POS_LERP_THRESHOLD)) {
			_tr.localPosition = Vector2.Lerp(_tr.localPosition, new Vector2(TargetPosition,_tr.localPosition.y), Time.deltaTime * 40f);
			if(Mathf.Abs(TargetPosition - _tr.localPosition.x) <= POS_LERP_THRESHOLD) {
				PositionIndex = TargetPositionIndex;
			}
		}
	}

	public void Lean(string pos) {
		switch(pos) {
			case "left":
				_leaningLeft = true;
				TargetPositionIndex = -1;
				break;
			case "stopleft":
				_leaningLeft = false;
				TargetPositionIndex = (_leaningRight)? 1 : 0;
				break;
			case "right":
				_leaningRight = true;
				TargetPositionIndex = 1;
				break;
			case "stopright":
				_leaningRight = false;
				TargetPositionIndex = (_leaningLeft)? -1 : 0;
				break;
		}
	}

	private float TargetPosition {
		get{return TargetPositionIndex * Game03.Main.DiffX;}
	}
}
