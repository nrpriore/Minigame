using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.UI;				// To set UI properties
using System.Collections.Generic;	// For lists

public class MenuTabController : MonoBehaviour {

	public const float WIDTH_RATIO = 0.5f;
	private const float LERP_MULT = 25f;

	private static List<RectTransform> _menuTabs;
	private static RectTransform _currentTab;
	private static List<RectTransform> _skippedTabs;
	private static Vector2[] _lerpWidthTo;
	private static Vector2[] _lerpPosTo;
	private static bool _lerp;

	public static float ScreenWidth;
	private static float _baseWidth;
	private static float _tabWidthDiff;
	private ScrollSnapRect _scrollSnap;

	public static int StartingPage;

	void Awake() {
		InitVars();
	}

	void Update() {
		if(_lerp) {
			for(int i = 0; i < _menuTabs.Count; i++) {
				RectTransform tab = _menuTabs[i];
				tab.anchoredPosition = Vector2.Lerp(tab.anchoredPosition,_lerpPosTo[i],Time.deltaTime * LERP_MULT);
				tab.sizeDelta = Vector2.Lerp(tab.sizeDelta,_lerpWidthTo[i],Time.deltaTime * LERP_MULT);
			}
			if(Vector2.SqrMagnitude(_currentTab.sizeDelta - _lerpWidthTo[_menuTabs.IndexOf(_currentTab)]) < 0.25f) {
				_lerp = false;
			}
		}
	}

	private void InitVars() {
		_scrollSnap = GameObject.Find("ScrollSnap").GetComponent<ScrollSnapRect>();
		GameObject tabs = GameObject.Find("MenuTabs");
		ScreenWidth = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution.x;
		_baseWidth = ScreenWidth/(tabs.transform.childCount + WIDTH_RATIO);
		_tabWidthDiff = _baseWidth * WIDTH_RATIO;
		_lerpWidthTo = new Vector2[tabs.transform.childCount];
		_lerpPosTo = new Vector2[tabs.transform.childCount];

		float _currX = 0;
		float _x = 0;
		_menuTabs = new List<RectTransform>();
		_skippedTabs = new List<RectTransform>();
		for(int i = 0; i < tabs.transform.childCount; i++) {
			RectTransform child = tabs.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
			_menuTabs.Add(child);
			_x = (_scrollSnap.startingPage == i)? _baseWidth + _tabWidthDiff : _baseWidth;
			child.sizeDelta = new Vector2(_x, child.rect.size.y);
			child.anchoredPosition = new Vector2(_currX, 0);
			_lerpWidthTo[i] = new Vector2(_x, 0);
			_lerpPosTo[i] = new Vector2(_currX, 0);
			_currX += _x;
		}
	}

	public static void SetTab(int tabIndex) {
		_lerp = true;
		int currIndex = (_currentTab != null)? _menuTabs.IndexOf(_currentTab) : tabIndex;
		int diff = tabIndex - currIndex;
		if(Mathf.Abs(diff) > 1) {
			for(int i = 1; i < Mathf.Abs(diff); i++) {
				_skippedTabs.Add(_menuTabs[currIndex + (i * (diff/Mathf.Abs(diff)))]);
			}
		}
		if(_currentTab != null ) {
			if(tabIndex < currIndex) {
				SetPivot(_currentTab, Vector2.right);
			}else {
				SetPivot(_currentTab, Vector2.zero);
			}
		}
		if(tabIndex <= currIndex) {
			SetPivot(_menuTabs[tabIndex], Vector2.zero);
		}else {
			SetPivot(_menuTabs[tabIndex], Vector2.right);
		}
		_currentTab = _menuTabs[tabIndex];

		for(int i = 0; i < _menuTabs.Count; i++) {
			RectTransform tab = _menuTabs[i];
			_lerpWidthTo[i] = (tab == _currentTab)? new Vector2(_baseWidth + _tabWidthDiff,tab.rect.size.y) : new Vector2(_baseWidth,tab.rect.size.y);
			if(_skippedTabs.Contains(tab)) {
				_lerpPosTo[i] = (_menuTabs.IndexOf(tab) > _menuTabs.IndexOf(_currentTab))? new Vector2(_lerpPosTo[i].x + _tabWidthDiff,_lerpPosTo[i].y) : new Vector2(_lerpPosTo[i].x - _tabWidthDiff,_lerpPosTo[i].y);
			}
		}
		_skippedTabs.Clear();
	}

	private static void SetPivot(RectTransform rectTransform, Vector2 pivot) {
         if (rectTransform == null) return;
         float _x = (_currentTab == rectTransform)? _baseWidth + _tabWidthDiff : _baseWidth;
         Vector2 deltaPivot = rectTransform.pivot - pivot;
         Vector3 deltaPosition = new Vector3(deltaPivot.x * _x, 0);
         rectTransform.pivot = pivot;
         rectTransform.localPosition -= deltaPosition;
         for(int i = 0; i < _menuTabs.Count; i++) {
         	RectTransform rect = _menuTabs[i];
         	if(rect == rectTransform) {
         		_lerpPosTo[i] -= (Vector2)deltaPosition;
         	}
         }
     }

}
