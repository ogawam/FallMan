using UnityEngine;
using System.Collections;

public class GimmickLaser : ObjectBase {

	[SerializeField] float delay;
	[SerializeField] float speed = 320; 
	float sec = 0;

	protected override void Update_ () {
		if(sec > delay) {
			Vector2 size = box2D.size;
			size.x += speed * Time.deltaTime;
			box2D.offset = new Vector2(size.x / 2, 0);
			box2D.size = size;

			GetUIObject<UILaser>().SetScale(size.x);
		}
		sec += Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		Debug.Log("hit laser");
		UnitBase unitBase = hit2D.GetComponent<UnitBase>();
		if(unitBase != null)
			unitBase.Kill();
	}

	protected override void OnDrawGizmos_() {
		Gizmos.color = Color.red;		
	}	
}
