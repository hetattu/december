using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public class Manager : MonoBehaviour {

	public GameObject BgObj;
	public GameObject ListObj;
	public GameObject BtnAObj;
	public GameObject BtnBObj;
	public GameObject BtnCObj;
	public GameObject OverlayObj;
	public GameObject LifeObj;

	public int PlayerLife;
	public int CurrentNumber;
	public LitJson.JsonData stageJsonData;
	public float TextSpeed;

	// いろいろ準備
	public void Start() {
		PlayerLife = 20;
		CurrentNumber = 1;
		TextSpeed = 0f;

		DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
		BgObj = GameObject.Find ("Background");
		ListObj = GameObject.Find ("ButtonList").gameObject;
		BtnAObj = ListObj.transform.Find ("ButtonA").gameObject;
		BtnBObj = ListObj.transform.Find ("ButtonB").gameObject;
		BtnCObj = ListObj.transform.Find ("ButtonC").gameObject;
		OverlayObj = GameObject.Find ("Overlay");
		LifeObj = GameObject.Find ("Life");

		ReadTextData();
		Placement ();
		ListObj.SetActive (false);
	}

	// テキスト読了後の処理
	// ・次の場面を選択するボタンリストを表示
	// ・次の場面に移動する
	public void OnButton() {
		GameObject button = OverlayObj.transform.Find ("Button").gameObject;
		button.SetActive (false);

		string number = CurrentNumber.ToString ();
		if (stageJsonData [number] ["button_list"] == null) {
			NextStory ();
		} else {
			PlacementButtonText ();
			ShowButtonList ();
		}
	}

	// 次のストーリー（選択肢）へ移動
	public void NextStory() {
		GameObject text = OverlayObj.transform.Find ("Text").gameObject;
		text.GetComponent<Text> ().text = "";

		string number = CurrentNumber.ToString ();
		CurrentNumber = (int)stageJsonData [number] ["button"] ["go"];

		Placement ();
	}

	// ボタンリストを表示
	public void ShowButtonList() {
		ListObj.transform.localPosition = new Vector3 (0, 500, 0);
		ListObj.SetActive (true);
		ListObj.transform.DOLocalMoveY (0, 0.5f);
	}

	// ボタンリストを非表示
	public void HideButtonList() {
		ListObj.transform.DOLocalMoveY (500, 0.5f);
	}

	// ボタンリストから選択してボタン押下
	public void OnSelectButton(GameObject obj) {
		HideButtonList ();

		GameObject text = OverlayObj.transform.Find ("Text").gameObject;
		text.GetComponent<Text> ().text = "";

		string number = CurrentNumber.ToString ();
		CurrentNumber = (int)stageJsonData [number] ["button_list"] ["b"] ["go"];

		UpdateLife ();
		Placement ();
	}

	// 全ての設定をセット
	public void Placement() {
//		PlacementBackground ();
		PlacementText ();
		StartCoroutine ("PlacementText");
	}

	// ボタンリストの各ボタン上コメントをセット
	public void PlacementButtonText() {
		string number = CurrentNumber.ToString();

		if (stageJsonData [number] ["button_list"] != null) {
			GameObject btnAText = BtnAObj.transform.FindChild ("Text").gameObject;
			GameObject btnBText = BtnBObj.transform.FindChild ("Text").gameObject;
			GameObject btnCText = BtnCObj.transform.FindChild ("Text").gameObject;

			LitJson.JsonData json = stageJsonData [number] ["button_list"];

			btnAText.GetComponent<Text> ().text = (string)json ["a"] ["text"];
			btnBText.GetComponent<Text> ().text = (string)json ["b"] ["text"];
			btnCText.GetComponent<Text> ().text = (string)json ["c"] ["text"];

			BtnAObj.GetComponent<SelectButton> ().goToNumber = (int)json ["a"] ["go"];
			BtnBObj.GetComponent<SelectButton> ().goToNumber = (int)json ["b"] ["go"];
			BtnCObj.GetComponent<SelectButton> ().goToNumber = (int)json ["c"] ["go"];
		}
	}

	// 背景をセット
	public void PlacementBackground() {
		string number = CurrentNumber.ToString();
		string textureName = (string)stageJsonData [number] ["bg"];
		Texture2D texture = Resources.Load(textureName) as Texture2D;
		Image img = GameObject.Find("Canvas/Background").GetComponent<Image>();
		img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
	}

	// エフェクト管理
	public void StoryEffect(string effect_name) {
		if (effect_name == "<blood>") {
			StartCoroutine ("effectBlood");
		} else if (effect_name == "<lose>") {
			StartCoroutine ("LoseLife");
		}
	}

	// 現在の場面の減少値からライフを更新
	public void UpdateLife() {
		string number = CurrentNumber.ToString();
		int value = (int)stageJsonData [number] ["lose_life"];
		PlayerLife = PlayerLife - value;
	}

	// ライフの表記更新
	private void PlacementLife() {
		LifeObj.transform.FindChild ("Text").gameObject.GetComponent<Text>().text = PlayerLife.ToString();
	}

	// テキストをjsonファイルからロードしてJsonDataへ格納
	private void ReadTextData() {
		TextAsset stageTextAsset = Resources.Load("story") as TextAsset;
		LitJson.JsonData json = LitJson.JsonMapper.ToObject(stageTextAsset.text);
		stageJsonData = json ["story"];
	}

	// 長文を表示
	private IEnumerator PlacementText() {
		GameObject button = OverlayObj.transform.Find ("Button").gameObject;
		button.SetActive (false);

		string number = CurrentNumber.ToString();
		string longText = (string)stageJsonData [number] ["long_text"];

		yield return new WaitForSeconds (0.5f);

		bool effect = false;
		int size = longText.Length;
		for (int i = 0; i < size; i++) {
			if (longText [i] == '<') {
				int index = longText.IndexOf ('>', i);
				string effect_name = longText.Substring (i, index - i + 1);
				StoryEffect (effect_name);
				effect = true;
			}
			if (!effect) {
				OverlayObj.transform.Find ("Text").GetComponent<Text> ().text += longText [i];
				if (TextSpeed > 0f) {
					yield return new WaitForSeconds (TextSpeed);
				}
			}
			if (longText [i] == '>') {
				effect = false;
			}
		}

		yield return new WaitForSeconds (0.1f);

		if (PlayerLife > 0 && stageJsonData[number]["button"] != null) {
			GameObject buttonText = button.transform.Find ("Text").gameObject;
			buttonText.GetComponent<Text> ().text = (string)stageJsonData [number] ["button"] ["text"];
			button.SetActive (true);
		}
		yield return null;
	}

	// 減少ライフ
	private IEnumerator LoseLife() {
		string number = CurrentNumber.ToString();
		if ((int)stageJsonData [number] ["lose_life"] > 0) {

			// 減少ライフ表示
			GameObject loseLife = Instantiate (LifeObj.transform.Find ("Text").gameObject);
			loseLife.transform.parent = LifeObj.transform;
			loseLife.transform.localPosition = new Vector3 (60, 20, 0);

			string lose_text = stageJsonData [number] ["lose_life"].ToString ();
			loseLife.GetComponent<Text> ().text = "-"+lose_text;
			loseLife.GetComponent<Text> ().color = new Color (0.7f, 0.1f, 0.1f);

			// スコアの減少
			PlacementLife ();

			// フェードアウト
			float alpha = 1f;
			for (int i = 0; i < 20; i++) {
				alpha -= 0.05f;
				loseLife.GetComponent<Text> ().color = new Color (0.7f, 0.1f, 0.1f, alpha);
				yield return new WaitForSeconds (0.05f);
			}
		}
		yield return null;
	}

	// 血痕エフェクト
	private IEnumerator effectBlood() {
		GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/Blood"));
		obj.transform.parent = BgObj.transform;

		int width = UnityEngine.Random.Range (0, 500);
		int height = UnityEngine.Random.Range (0, 200);
		obj.transform.localPosition = new Vector3 (width-250,height-100,0);

		// フェードアウト
		float alpha = 1f;
		for (int i = 0; i < 20; i++) {
			alpha -= 0.05f;
			obj.GetComponent<Image>().color = new Color (1f, 1f, 1f, alpha);
			yield return new WaitForSeconds (0.05f);
		}
		yield return null;
	}
}
