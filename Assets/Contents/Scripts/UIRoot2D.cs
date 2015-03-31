using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIRoot2D : Singleton<UIRoot2D> {

	[SerializeField] PanelGame panelGame;
	public PanelGame game {
		get{ return panelGame; }
	}

	List<UIButtonController> buttonControllers = new List<UIButtonController>();
	public bool AddButtonController(UIButtonController buttonController) {
		if(buttonControllers.Contains(buttonController))
			return false;

		buttonControllers.Add(buttonController);
		return true;
	}

	public bool RemoveButtonController(UIButtonController buttonController) {
		if(buttonControllers.Contains(buttonController)) {
			buttonControllers.Remove(buttonController);
			return true;
		}
		return false;
	}

	public UIButtonController GetButtonController(Common.Button button) {
		return buttonControllers.FirstOrDefault(elem => elem.GetButtonType() == button);
	}

	[SerializeField] PanelIndicator panelIndicator;
	public PanelIndicator indicator {
		get{ return panelIndicator; }
	}

	void Update() {
		RaycastHit hit;
		UIButtonController buttonController = null;
#if UNITY_EDITOR
		for(int i = 0; i < 3; ++i) {
			if(Input.GetMouseButtonDown(i)) {
				if(UICamera.Raycast (Input.mousePosition, out hit)) {
					buttonController = hit.collider.GetComponent<UIButtonController>();
					if(buttonController != null)
						buttonController.Press();
				}
			}
			else if(Input.GetMouseButton(i)) {
				if(UICamera.Raycast (Input.mousePosition, out hit)) {
					buttonController = hit.collider.GetComponent<UIButtonController>();
					if(buttonController != null)
						buttonController.Holding();
				}
			}
		}
#else
		foreach(Touch touch in Input.touches) {
			if(UICamera.Raycast (touch.position, out hit)) {				
				buttonController = hit.collider.GetComponent<UIButtonController>();
				if(buttonController != null) {
					switch(touch.phase) {
					case TouchPhase.Began:
						buttonController.Press();
						break;
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						buttonController.Holding();
						break;
					}
				}
			}
		}
#endif		
	}
}
