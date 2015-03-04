using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickFire : ObjectBase {

	Vector3 firstPos = Vector3.zero;
	protected override void Start_ () {
		firstPos = transform.localPosition;
		TweenPosition tp = TweenPosition.Begin(gameObject, 0.75f, firstPos + Vector3.up * 480);
		tp.method = UITweener.Method.EaseOut;
		tp.style = UITweener.Style.PingPong;
		tp.delay = Random.Range(0,2);
	}

	// Update is called once per frame
	protected override void Update_ () {
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit fire");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}

	protected override void OnDrawGizmos_() {
		Gizmos.color = Color.red;		
	}

	void OnGUI() {
	}
}
