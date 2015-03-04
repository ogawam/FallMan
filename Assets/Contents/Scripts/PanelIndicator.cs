using UnityEngine;
using System.Collections;

public class PanelIndicator : MonoBehaviour {

	Common.Button touchedButton = Common.Button.None;

	[SerializeField] GameObject title;
	[SerializeField] GameObject information;
	[SerializeField] UILabel labelInfo;

	public void PressStart() {
		touchedButton = Common.Button.Start;
	}

	public bool IsTouchedButton(Common.Button button) {
		bool result = button == touchedButton;
		touchedButton = Common.Button.None;
		return result;
	}

	public void DispTitle() {
		title.SetActive(true);
	}

	public void HideTitle() {
		title.SetActive(false);
	}

	public void DispInformation(string info) {
		information.SetActive(true);
		labelInfo.text = info;
		labelInfo.MakePixelPerfect();
	}

	public void HideInformation() {
		information.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
