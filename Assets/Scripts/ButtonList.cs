using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonList : MonoBehaviour {

	// ボタンオブジェクトを格納する
	Dictionary<int, GameObject> buttons;
	GameObject buttonList;

	public void Init() {
		buttonList = GameObject.Find("ButtonList");

		// 全ての子オブジェクトをディクショナリーに格納する
		foreach (var n in buttonList.GetChildren(this)) {
//			if (n.name.IndexOf('Button')) {
				Debug.Log(n.name);
//			}
		}
	}
}
