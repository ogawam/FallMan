using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickStone: ObjectBase {

	public float delay = 10;

	protected override void Update_() {
		delay -= Time.deltaTime;
		if(delay < 0)
			(GetComponent<Rigidbody2D>() as Rigidbody2D).isKinematic = false;
		if(transform.localPosition.x < -Common.vanishWidth
		|| transform.localPosition.x >  Common.vanishWidth
		|| transform.localPosition.y < -Common.vanishHeight
		|| transform.localPosition.y >  Common.vanishHeight)
		{
			Vanish();
		}
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit saw");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}
}
