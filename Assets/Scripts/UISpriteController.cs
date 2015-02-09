using UnityEngine;
using System.Collections;

// 2Dスプライトのアニメーション管理
public class UISpriteController : MonoBehaviour {

	UISprite sprite;
	void Awake() {
		sprite = GetComponent<UISprite>();
	}
}