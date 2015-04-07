using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour {

	bool isPause = false;
	public virtual void Pause(bool pause) {
		Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
		if(rigidbody2D != null)
 			rigidbody2D.isKinematic = pause;
		isPause = pause;
	}

	public void Vanish() {
		Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
		rigidbody2D.isKinematic = false;
		gameObject.SetActive(false);
		SetVisible(false);
	}

	[SerializeField] UIObject prefabUIObject;

	protected UIObject uiObject = null;
	public UIObject GetUIObject() {
		return uiObject;
	}

	protected UIButtonController buttonController = null;

	public Type GetUIObject<Type>() where Type : UIObject {
		return uiObject as Type;
	}

	public void SetVisible(bool visible) {
		uiObject.SetVisible(visible);
	}

	public Type GetCollider2D<Type>() where Type : Collider2D {
		return GetComponent<Collider2D>() as Type;
	}

	public BoxCollider2D box2D { get{ return GetComponent<BoxCollider2D>(); } }
	public CircleCollider2D circle2D { get{ return GetComponent<CircleCollider2D>(); } }

	protected Vector3 firstPosition;
	protected Quaternion firstRotation;
	public virtual void Reset() {
		transform.position = firstPosition;
		transform.rotation = firstRotation;
	}

	void Awake() {
		firstPosition = transform.position;
		firstRotation = transform.rotation;
	}

	void Start() {
		if(prefabUIObject != null) {
			uiObject = Instantiate(prefabUIObject) as UIObject;
			uiObject.transform.parent = UIRoot2D.Get().game.transform;
	 		uiObject.transform.localPosition = transform.position;
			uiObject.transform.localRotation = transform.rotation;
			uiObject.transform.localScale = Vector3.one;
			buttonController = uiObject.GetComponent<UIButtonController>();
		}
		Start_();
	}

	protected virtual void Start_() {}

	void Update() {
		if(Environment.Get().isPause || isPause)
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

		if(box2D != null) {

			Matrix4x4 tempMat = Gizmos.matrix;
	        Gizmos.matrix = Matrix4x4.TRS(
	        	transform.position, 
	        	transform.rotation, 
	        	new Vector3(1f, 1.0f, 1.0f)
	        );

			Gizmos.DrawWireCube(box2D.offset, box2D.size);

			Gizmos.matrix = tempMat;
			return;
		}

		if(circle2D != null) {

			Matrix4x4 tempMat = Gizmos.matrix;
	        Gizmos.matrix = Matrix4x4.TRS(
	        	transform.position, 
	        	transform.rotation, 
	        	new Vector3(1f, 1.0f, 1.0f)
	        );

			Gizmos.DrawWireSphere(circle2D.offset, circle2D.radius);

			Gizmos.matrix = tempMat;
			return;
		}

		if(uiObject != null) {
			
		}
	}

	protected bool inVanishRange {	get {
			return transform.localPosition.x < -Common.vanishWidth
				|| transform.localPosition.x >  Common.vanishWidth
				|| transform.localPosition.y < -Common.vanishHeight
				|| transform.localPosition.y >  Common.vanishHeight;
		}
	}

	public int gridSize = 8;
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

