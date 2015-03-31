using UnityEngine;
using System.Collections;

public class GimmickGravity : ObjectBase {

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit gravity "+ name+ hit2D.transform.position);
		Quaternion rotate = new Quaternion();
		rotate.eulerAngles = Vector3.forward * 90;
		Physics2D.gravity = rotate * Physics2D.gravity;
	}
}
