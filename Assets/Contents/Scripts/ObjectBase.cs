using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour {

	[SerializeField] UIObject prefabUIObject;

	protected UIObject uiObject = null;
	public UIObject GetUIObject() {
		return uiObject;
	}

	public Type GetUIObject<Type>() where Type : UIObject {
		return uiObject as Type;
	}

	public void SetVisible(bool on) {
		uiObject.gameObject.SetActive(on);
	}

	public Type GetCollider2D<Type>() where Type : Collider2D {
		return collider2D as Type;
	}

	public BoxCollider2D box2D { get{ return collider2D as BoxCollider2D; } }

	void Start() {
		if(prefabUIObject != null) {
			uiObject = Instantiate(prefabUIObject) as UIObject;
			uiObject.transform.parent = UIRoot2D.Get().main.transform;
	 		uiObject.transform.localPosition = transform.position;
			uiObject.transform.localRotation = transform.rotation;
			uiObject.transform.localScale = Vector3.one;
		}
		Start_();
	}

	protected virtual void Start_() {}

	void Update() {
		if(Environment.Get().isPause)
			return;

		if(uiObject) {
	 		uiObject.transform.localPosition = transform.position;
			uiObject.transform.localRotation = transform.rotation;
		}
		Update_();
	}

	void OnDestroy() {
		if(uiObject != null)
			Destroy(uiObject.gameObject);
	}

	protected virtual void Update_() {}

	protected virtual void OnDrawGizmos_() {
		Gizmos.color = Color.yellow;		
	}

	void OnDrawGizmos() {
		OnDrawGizmos_();

		BoxCollider2D box2D = GetCollider2D<BoxCollider2D>();
		if(box2D != null) {

			Matrix4x4 tempMat = Gizmos.matrix;
	        Gizmos.matrix = Matrix4x4.TRS(
	        	transform.position, 
	        	transform.rotation, 
	        	new Vector3(1f, 1.0f, 1.0f)
	        );

			Gizmos.DrawWireCube(box2D.center, box2D.size);

			Gizmos.matrix = tempMat;
			return;
		}

		CircleCollider2D sphere2D = GetCollider2D<CircleCollider2D>();
		if(sphere2D != null) {

			Matrix4x4 tempMat = Gizmos.matrix;
	        Gizmos.matrix = Matrix4x4.TRS(
	        	transform.position, 
	        	transform.rotation, 
	        	new Vector3(1f, 1.0f, 1.0f)
	        );

			Gizmos.DrawWireSphere(sphere2D.center, sphere2D.radius);

			Gizmos.matrix = tempMat;
			return;
		}
	}

	public int gridSize = 32;
	public void ArrangeOnGrid() {
		Vector3 pos = transform.localPosition;

		float xDiff = pos.x % gridSize;
		pos.x -= xDiff;

		if(Mathf.Abs(xDiff) > gridSize / 2)
			pos.x += pos.x > 0 ? gridSize : -gridSize;

		float yDiff = pos.y % gridSize;
		pos.y -= yDiff;

		if(Mathf.Abs(yDiff) > gridSize / 2) 
			pos.y += pos.y > 0 ? gridSize : -gridSize;

		transform.localPosition = pos;
	}
}

