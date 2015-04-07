using UnityEngine;
using System.Collections;

// 2Dオブジェクトのルート
public class UIObject : MonoBehaviour {

	[SerializeField] protected UISpriteController[] spriteCtrls;

	public void SetVisible(bool visible) {
		foreach(UISpriteController spriteCtrl in spriteCtrls) {
			spriteCtrl.sprite.enabled = visible;
		}
	}
}