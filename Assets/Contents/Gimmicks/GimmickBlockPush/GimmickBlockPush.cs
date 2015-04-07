using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickBlockPush: ObjectBase {

	[SerializeField] float pushSec; 
	bool isPush = false;
	float pushCnt = 0;
	IEnumerator coroutine = null;

	UIGmkBlockPush uiPush;
	protected override void Start_() {
		uiPush = uiObject as UIGmkBlockPush;
	}

	protected override void Update_() {
		if(buttonController.IsPress()) {
			if(coroutine == null) {
				coroutine = isPush ? CoPull(): CoPush();
				StartCoroutine(coroutine);
			}
		}
		if(isPush) {
			if(pushCnt > pushSec) {
				if(coroutine == null) {
					coroutine = CoPull();
					StartCoroutine(coroutine);
				}
			}
			pushCnt += Time.deltaTime;
		}
	}

	IEnumerator CoPush() {
		yield return StartCoroutine(uiPush.CoPush());

		box2D.enabled = true;
		coroutine = null;
		isPush = true;
		pushCnt = 0;
	}

	IEnumerator CoPull() {
		yield return StartCoroutine(uiPush.CoPull());

		box2D.enabled = false;
		coroutine = null;
		isPush = false;
		pushCnt = 0;
	}
}
