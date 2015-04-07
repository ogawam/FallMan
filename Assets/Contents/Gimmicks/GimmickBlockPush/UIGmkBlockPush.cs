using UnityEngine;
using System.Collections;

public class UIGmkBlockPush : UIObject {

	[SerializeField] float pushSec = 0;
	[SerializeField] float pullSec = 0;

	[SerializeField] Color pushColor;
	[SerializeField] Color pullColor;
	[SerializeField] float pushScale;
	[SerializeField] float pullScale;

	void Start() {
		transform.localScale = Vector3.one * pullScale;
		foreach(UISpriteController spriteCtrl in spriteCtrls) {
			spriteCtrl.sprite.color = pullColor;
		}		
	}

	public IEnumerator CoPush() {
		Debug.Log("CoPush");
		TweenScale.Begin(gameObject, pushSec, Vector3.one * pushScale);
		foreach(UISpriteController spriteCtrl in spriteCtrls) {
			TweenColor.Begin(spriteCtrl.gameObject, pushSec, pushColor);
		}

		float sec = 0;
		while(sec < pushSec) {
			sec += Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator CoPull() {
		Debug.Log("CoPull");
		TweenScale.Begin(gameObject, pullSec, Vector3.one * pullScale);
		foreach(UISpriteController spriteCtrl in spriteCtrls) {
			TweenColor.Begin(spriteCtrl.gameObject, pullSec, pullColor);
		}

		float sec = 0;
		while(sec < pullSec) {
			sec += Time.deltaTime;
			yield return null;
		}		
	}
}
