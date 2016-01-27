using UnityEngine;
using System.Collections;

public class SelectButton : MonoBehaviour {
	private int number = 1; //ディフォルト値
	public int goToNumber {get {return number;} set { number=value;}}

	public Manager manager;

	public void OnClickSelectButton() {
		manager.OnSelectButton (gameObject);
	}
}
