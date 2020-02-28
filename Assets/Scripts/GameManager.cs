using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	void Start () {
		GetComponent<AudioSource> ().Play ();
		DontDestroyOnLoad (this.gameObject);
		SceneManager.LoadScene ("Menu");
	}
}
