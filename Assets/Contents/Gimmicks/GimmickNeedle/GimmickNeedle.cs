using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickNeedle : ObjectBase {

	// Update is called once per frame
	protected override void Update_ () {
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}

	void OnTriggerStay2D(Collider2D hit2D) {
	}

	void OnGUI() {
	}
}
