using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickJump : ObjectBase {

	// Update is called once per frame
	protected override void Update_ () {
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.InputJump();
	}

	void OnTriggerStay2D(Collider2D hit2D) {
	}

	void OnGUI() {
	}
}
