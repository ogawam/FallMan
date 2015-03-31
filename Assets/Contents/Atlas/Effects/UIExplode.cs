using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIExplode : UIObject {

	enum State {
		Expand,
		Shrink
	}

	State state = State.Expand;	
	public void Shrink() {
		state = State.Shrink;
		foreach(UIObject element in elements)
			TweenPosition.Begin(element.gameObject, 0.5f, Vector3.zero);
	}

	[SerializeField] UIObject prefabElement;
	[SerializeField] int elementsNum;
	[SerializeField] float elementOffset;
	[SerializeField] float elementSpeed;

	List<UIObject> elements = new List<UIObject>();

	// Use this for initialization
	void Start () {
		float angle = 360 / elementsNum;
		for(int i = 0; i < elementsNum; ++i) {
			UIObject element = Instantiate(prefabElement) as UIObject; 
			element.transform.parent = transform;
			element.transform.localEulerAngles = Vector3.forward * (angle * i);
			element.transform.localPosition = element.transform.rotation * (Vector3.up * elementOffset);
			element.transform.localScale = Vector3.one;
			elements.Add(element);
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(state) {
		case State.Expand:
			foreach(UIObject element in elements) {
				element.transform.localPosition += element.transform.rotation * (Vector3.up * elementSpeed * Time.deltaTime);
			}
			break;
		case State.Shrink:
			break;
		}
	}
}
