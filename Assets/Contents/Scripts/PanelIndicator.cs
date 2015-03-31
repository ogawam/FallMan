using UnityEngine;
using System.Collections;

public class PanelIndicator : MonoBehaviour {

	[SerializeField] GameObject title;
	[SerializeField] GameObject information;
	[SerializeField] UILabel labelInfo;

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
