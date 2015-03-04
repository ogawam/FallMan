using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GimmickInfo : ObjectBase {

	// Update is called once per frame
	protected override void Update_ () {
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		UIRoot2D.Get().indicator.DispInformation("テスト\nだよ\nあしだ\nまなだよ");
	}

	void OnTriggerExit2D(Collider2D hit2D) {
		UIRoot2D.Get().indicator.HideInformation();
	}

	void OnGUI() {
	}
}
