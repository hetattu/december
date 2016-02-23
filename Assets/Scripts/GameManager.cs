using UnityEngine;
using System.Collections;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	public ButtonList buttonList;

	// 自身とインスタンスが一致しない場合破棄する
	public void Awake() {
		if (this != Instance) {
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// 初期化処理
	public void Init() {
		buttonList = new ButtonList();
		buttonList.Init();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
