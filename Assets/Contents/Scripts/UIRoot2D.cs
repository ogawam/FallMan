using UnityEngine;
using System.Collections;

public class UIRoot2D : Singleton<UIRoot2D> {

	[SerializeField] UIPanel panelMain;
	public UIPanel main {
		get{ return main; }
	}

	[SerializeField] PanelIndicator panelIndicator;
	public PanelIndicator indicator {
		get{ return indicator; }
	}
}
