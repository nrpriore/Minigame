using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.SceneManagement;	// To change scenes

public class FreePlayController : MonoBehaviour {

	public int AvailableGames;

	public void FreePlay(string gameID) {
		MenuController.IsFreePlay = true;
     	SceneManager.LoadSceneAsync("Game" + gameID, LoadSceneMode.Single);
     }
}
