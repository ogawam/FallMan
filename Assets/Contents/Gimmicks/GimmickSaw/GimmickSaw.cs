using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickSaw: ObjectBase {

	[SerializeField] Vector3 posStart;
	[SerializeField] Vector3 posGoal;
	[SerializeField] float loopTime;
	[SerializeField] float stopTime;

	float loopCount = 0;
	float stopCount = 0;

	Vector3 posFirst = Vector3.zero;
	UIButtonController buttonController;
	protected override void Start_ () {
		buttonController = uiObject.GetComponent<UIButtonController>();
		posFirst = transform.position;
	}

	// Update is called once per frame
	protected override void Update_ () {

		if(stopCount > 0) {
			stopCount -= Time.deltaTime;
		}
		else {
			Vector3 posCenter = (posGoal - posStart) * 0.5f;
			float rate = Mathf.Sin(Mathf.PI * loopCount / loopTime);
			transform.position = posFirst + (posCenter * rate);
			loopCount += Time.deltaTime;

			transform.localEulerAngles = Vector3.forward * (360 * 10 * Time.time);
		}

		if(buttonController.IsPress())
			stopCount = stopTime;
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit saw");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}

	protected override void OnDrawGizmos_() {
		Gizmos.color = Color.green;		
		Gizmos.DrawWireSphere(transform.position + posStart, circle2D.radius);
		Gizmos.DrawWireSphere(transform.position + posGoal, circle2D.radius);
		Gizmos.DrawLine(transform.position + posStart, transform.position + posGoal);

		Gizmos.color = Color.red;		
	}

	void OnGUI() {
	}
}
