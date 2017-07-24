using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.SceneManagement;	// To change scenes

public class MenuController : MonoBehaviour {

	public static bool IsFreePlay;

	void Awake() {
		IsFreePlay = false;
	}

	public void QuickPlay() {
		float numGames = 2f;
		int gameID = (int)(1f + Random.value * (numGames - 0.001f)); // Subtract 0.001 since Random.value can equal 1
		string strID = (gameID < 10)? "0" + gameID : gameID.ToString();

		SceneManager.LoadSceneAsync("Game" + strID, LoadSceneMode.Single);
	}
	
}
