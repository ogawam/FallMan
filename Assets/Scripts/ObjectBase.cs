using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour {

	[SerializeField] UIObject prefabUIObject;

	protected UIObject uiObject;
	public UIObject GetUIObject() {
		return uiObject;
	}

	public void SetVisible(bool on) {
		uiObject.gameObject.SetActive(on);
	}

	public Type GetCollider2D<Type>() where Type : Collider2D {
		return collider2D as Type;
	}

	public BoxCollider2D box2D { get{ return collider2D as BoxCollider2D; } }

	void Start() {
		uiObject = Instantiate(prefabUIObject) as UIObject;
		uiObject.transform.parent = UIRoot2D.Get().mainPanel.transform;
		uiObject.transform.localScale = Vector3.one;
		Start_();
	}

	protected virtual void Start_() {}

	void Update() {
		uiObject.transform.localPosition = transform.position;
		uiObject.transform.localRotation = transform.rotation;
		Update_();
	}

	protected virtual void Update_() {}

	void OnDrawGizmos() {
		BoxCollider2D box2D = GetCollider2D<BoxCollider2D>();
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

