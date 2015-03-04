using UnityEngine;
using System.Collections;

// 2Dスプライトのアニメーション管理
public class UISpriteController : MonoBehaviour {

	UISprite uiSprite;
	void Awake() {
		uiSprite = GetComponent<UISprite>();
	}

	public UISprite sprite { get { return uiSprite; } }
}