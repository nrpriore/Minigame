using UnityEngine;					// To inherit from MonoBehaviour 
using UnityEngine.SceneManagement;	// To change scenes

public class Game02 : MonoBehaviour {


	public void BackToMenu() {
		SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
	}
}
