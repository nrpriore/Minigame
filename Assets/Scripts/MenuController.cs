using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public const float WIDTH_RATIO = 0.5f;

	private static List<RectTransform> _menuTabs = new List<RectTransform>();
	private static RectTransform _currentTab;
	private static List<RectTransform> _skippedTabs = new List<RectTransform>();
	private static Vector2[] _lerpListTo;
	private static bool _lerp;

	public static float ScreenWidth;
	private static float _baseWidth;
	private static float _tabWidthDiff;
	private static float _pivotDiff;
	private ScrollSnapRect _scrollSnap;

	void Awake() {
		InitVars();
	}

	void Update() {
		if(_lerp) {
			for(int i = 0; i < _menuTabs.Count; i++) {
				RectTransform tab = _menuTabs[i];
				if(_skippedTabs.Contains(tab)) {
					tab.pivot = Vector2.Lerp(tab.pivot,_lerpListTo[i],Time.deltaTime * 10f);
				}else {
					tab.sizeDelta = Vector2.Lerp(tab.sizeDelta,_lerpListTo[i],Time.deltaTime * 10f);
				}
			}
			if(Vector2.SqrMagnitude(_currentTab.sizeDelta - _lerpListTo[_menuTabs.IndexOf(_currentTab)]) < 0.25f) {
				_lerp = false;
				_skippedTabs.Clear();
			}
		}
	}

	private void InitVars() {
		_scrollSnap = GameObject.Find("ScrollSnap").GetComponent<ScrollSnapRect>();
		GameObject tabs = GameObject.Find("MenuTabs");
		ScreenWidth = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution.x;
		_baseWidth = ScreenWidth/(tabs.transform.childCount + WIDTH_RATIO);
		_tabWidthDiff = _baseWidth * WIDTH_RATIO;
		_pivotDiff = _tabWidthDiff / _baseWidth;
		_lerpListTo = new Vector2[tabs.transform.childCount];

		float _currX = 0;
		float _x = 0;
		for(int i = 0; i < tabs.transform.childCount; i++) {
			RectTransform child = tabs.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
			_menuTabs.Add(child);
			if(_scrollSnap.startingPage == i) {
				_x = _baseWidth + _tabWidthDiff;
				child.sizeDelta = new Vector2(_x, child.rect.size.y);
			}else{
				_x = _baseWidth;
				child.sizeDelta = new Vector2(_x, child.rect.size.y);
			}
			child.anchoredPosition = new Vector2(_currX, 0);
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
		_currentTab = _menuTabs[tabIndex];
		if(tabIndex <= currIndex) {
			SetPivot(_currentTab, Vector2.zero);
		}else {
			SetPivot(_currentTab, Vector2.right);
		}

		for(int i = 0; i < _menuTabs.Count; i++) {
			RectTransform tab = _menuTabs[i];
			_lerpListTo[i] = (tab == _currentTab)? new Vector2(_baseWidth + _tabWidthDiff,tab.rect.size.y) : new Vector2(_baseWidth,tab.rect.size.y);
			if(_skippedTabs.Contains(tab)) {
				_lerpListTo[i] = (_menuTabs.IndexOf(tab) > _menuTabs.IndexOf(_currentTab))? new Vector2(tab.pivot.x - _pivotDiff,tab.pivot.y) : new Vector2(tab.pivot.x + _pivotDiff,tab.pivot.y);
			}
		}
	}

	private static void SetPivot(RectTransform rectTransform, Vector2 pivot) {
         if (rectTransform == null) return;
         Vector2 size = rectTransform.rect.size;
         Vector2 deltaPivot = rectTransform.pivot - pivot;
         Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
         rectTransform.pivot = pivot;
         rectTransform.localPosition -= deltaPosition;
     }

     public void QuickPlay() {
     	SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
     }
	
}
