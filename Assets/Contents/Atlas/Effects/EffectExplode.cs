using UnityEngine;
using System.Collections;

public class EffectExplode : EffectBase {

	enum State {
		Expand,
		Shrink
	}

	State state = State.Expand;
	float stateSec = 0;

	Vector3 beginShrinkPos;
	public void Shrink(Vector3 pos) {
		state = State.Shrink;
		stateSec = 0;
		uiObject.GetComponent<UIExplode>().Shrink();
		TweenPosition.Begin(uiObject.gameObject, 0.5f, pos);
	}

	// Use this for initialization
	protected override void Start_ () {
	}
	
	// Update is called once per frame
	protected override void Update_ () {

		switch(state) {
		case State.Expand:
			if(uiObject) {
		 		uiObject.transform.localPosition = transform.position;
				uiObject.transform.localRotation = transform.rotation;
			}
			break;

		case State.Shrink:
			if(stateSec > 0.5f) {
				Destroy(gameObject);
			}
			break;
		}

		stateSec += Time.deltaTime;
	}
}
