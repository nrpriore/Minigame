using UnityEngine;
using System.Collections.Generic;

public class Player02 : MonoBehaviour {

	private const float SCALE_LERP_THRESHOLD = 0.01f;

	public int ID;
	public int HP;
	public bool Lost;
	public List<int> GuessList;

	public RectTransform HPBar;
	public RectTransform CurrentGuess;

	void Start () {
		HP = MaxHP;

		// Development to init guess lists for computers
		GuessList = new List<int>();
		for(int i = 0; i < Game02.LENGTH; i++) {
			GuessList.Add(0);
		}
		// End Development
	}
	
	void Update () {
		if(LerpHP) {
			HPBar.localScale = Vector2.Lerp(HPBar.localScale, new Vector2(TargetScale,1), Time.deltaTime * 20f);
		}
		if(LerpCurrentGuess) {
			float newScale = CurrentGuess.GetComponent<Button02>().CheckScale;
			CurrentGuess.localScale = Vector2.Lerp(CurrentGuess.localScale, new Vector2(newScale,newScale), Time.deltaTime * 20f);
		}
	}

	public void WrongAnswer() {
		HP = Mathf.Max(HP-1,0);
	}

	public void CorrectAnswer() {

	}

	public void SetCurrentGuess(RectTransform rect = null) {
		if(CurrentGuess != null) {
			Destroy(CurrentGuess.gameObject);
		}
		if(rect != null) {
			rect.localScale = Vector2.zero;
		}
		CurrentGuess = rect;
	}

	private int MaxHP {
		get{return Game02.MAX_HP;}
	}
	private float TargetScale {
		get{return Mathf.Max((float)HP/MaxHP,0);}
	}
	private bool LerpHP {
		get{return Mathf.Abs(TargetScale - HPBar.localScale.x) > SCALE_LERP_THRESHOLD;}
	}
	private bool LerpCurrentGuess {
		get{return (CurrentGuess != null)? ((Mathf.Abs(1 - CurrentGuess.localScale.x) > SCALE_LERP_THRESHOLD)? true : false) : false;}
	}

}
