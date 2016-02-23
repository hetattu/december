using UnityEngine;
using System.Collections;

public class TextManager : SingletonMonoBehaviour<TextManager> {

	public LitJson.JsonData storyJsonData;

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
		ReadTextJson();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// テキストをjsonファイルからロードしてJsonDataへ格納
	private void ReadTextJson() {
		TextAsset stageTextAsset = (TextAsset)Resources.Load("Text/story");
		LitJson.JsonData json = LitJson.JsonMapper.ToObject(stageTextAsset.text);
		storyJsonData = json["story"];
	}
}
