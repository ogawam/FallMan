using UnityEngine;
using System.Collections;

public class UIRoot2D : Singleton<UIRoot2D> {

	[SerializeField] UIPanel mainPanel_;
	public UIPanel mainPanel {
		get{ return mainPanel_; }
	}
}
