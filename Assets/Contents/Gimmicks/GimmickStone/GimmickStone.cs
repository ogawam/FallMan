using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickStone: ObjectBase {

	protected override void Update_() {
		if(inVanishRange)
			Vanish();
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit saw");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}
}
