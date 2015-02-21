using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickRespawn : ObjectBase {

	// Update is called once per frame
	protected override void Update_ () {
	}

	void OnGUI() {

	}

	protected override void OnDrawGizmos_() {
		Gizmos.color = Color.green;		
		Gizmos.DrawWireSphere(transform.position, 64);
	}
}
