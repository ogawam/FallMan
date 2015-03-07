using UnityEngine;
using System.Collections;

public class UIRoot2D : Singleton<UIRoot2D> {

	[SerializeField] UIPanel panelGame;
	public UIPanel game {
		get{ return panelGame; }
	}

	[SerializeField] PanelIndicator panelIndicator;
	public PanelIndicator indicator {
		get{ return panelIndicator; }
	}
}
