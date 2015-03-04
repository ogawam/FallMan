using UnityEngine;
using System.Collections;

public class UILaser : UIObject {

	public void SetScale(float scale) {
		Vector3 localScale = spriteCtrls[0].transform.localScale;
		localScale.x = scale;
		spriteCtrls[0].transform.localScale = localScale;
	}
}
