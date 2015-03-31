using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickJump : ObjectBase {

	UIButtonController buttonController;
	protected override void Start_ () {
		buttonController = uiObject.GetComponent<UIButtonController>();
	}

	// Update is called once per frame
	protected override void Update_ () {
		isOpen = false;
		if(buttonController.IsPress())
			isOpen = true;
	}

	bool isOpen = false;
	public bool IsOpen() {
		return isOpen;
	}

	void OnGUI() {
	}
}
