using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour {

	[SerializeField] GameObject uiPrefab;
	[SerializeField] float rotateSpeed = 0;
	protected GameObject uiObject;
	public GameObject GetUIObject() {
		return uiObject;
	}

	public void SetVisible(bool on) {
		uiObject.SetActive(on);
	}

	BoxCollider2D box2D = null;
	public BoxCollider2D Box2D { get{ return box2D; } }

	void Start() {
		box2D = GetComponent<BoxCollider2D>();
		uiObject = Instantiate(uiPrefab) as GameObject;
		uiObject.transform.parent = UIRoot2D.Get().mainPanel.transform;
		uiObject.transform.localScale = Vector3.one;
	}

	void Update() {
		transform.localEulerAngles =
		transform.localEulerAngles + Vector3.forward * rotateSpeed * Time.deltaTime;
		uiObject.transform.localPosition = transform.position;
		uiObject.transform.localRotation = transform.rotation;
	}

	void OnDrawGizmos() {
		box2D = GetComponent<BoxCollider2D>();
		if(box2D != null) {
			Gizmos.color = Color.yellow;

			Matrix4x4 tempMat = Gizmos.matrix;
	        Gizmos.matrix = Matrix4x4.TRS(
	        	transform.position, 
	        	transform.rotation, 
	        	new Vector3(1f, 1.0f, 1.0f)
	        );

			Gizmos.DrawWireCube(box2D.center, box2D.size);

			Gizmos.matrix = tempMat;
		}
	}
}

