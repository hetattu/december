using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public void Awake() {
		GameManager.Instance.Init();
		TextManager.Instance.Init();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
