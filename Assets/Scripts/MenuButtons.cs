using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {

	public void StartGame() {
		SceneManager.LoadScene("NewFirstLevel");
	}

	public void MainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	public void Credits() {
		SceneManager.LoadScene("Credits");
	}
}
