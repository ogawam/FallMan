using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitBase : MonoBehaviour {

	public float accel_move;
	public float accel_stop;
	public float speed_jump;

	public float bodySize;

	public Vector2 speed_max;

	ObjectBase objBase;

	[SerializeField] ObjectBase objGuide;


	// Use this for initialization
	void Start () {
		objBase = GetComponent<ObjectBase>();	
	}
	
	Vector2 speed = Vector2.zero;

	public void InputMoveLeft() {
		speed.x -= accel_move * Time.deltaTime;
	}

	public void InputMoveRight() {
		speed.x += accel_move * Time.deltaTime;		
	}

	public void InputJump() {
		if(stand) {
			speed.y = speed_jump;
			stand = false;
		}
	}

	bool stand = false;

	// Update is called once per frame
	void Update () {
		speed.x = Mathf.Clamp(speed.x, -speed_max.x, speed_max.x);
		speed.y = Mathf.Clamp(speed.y, -speed_max.y, speed_max.y);

		objBase.GetUIObject().transform.localScale = new Vector3(speed.x < 0 ? -1 : 1,1,1);

		float touchVec = 0;
#if UNITY_EDITOR
		if(Input.GetMouseButton(0)) {
			Vector3 touchPos = Vector3.right * ((Input.mousePosition.x - Screen.width / 2) / (Screen.width / 2) * 320);
			objGuide.transform.position = touchPos;
			touchVec = touchPos.x - transform.position.x;
			objGuide.SetVisible(true);
		}
#else
		if(Input.touchCount > 0) {
			foreach(Touch touch in Input.touches) {
				if(touch.fingerId == 0) {

					Vector3 touchPos = Vector3.right * ((touch.position.x - Screen.width / 2) / (Screen.width / 2) * 320);
					objGuide.transform.position = touchPos;

					touchVec = touchPos.x - transform.position.x;
					objGuide.SetVisible(true);
					break;
				}
			}
		}
#endif
		else {
			objGuide.SetVisible(false);
		}

		if(Input.GetKey(KeyCode.LeftArrow) || touchVec < -accel_stop)
			InputMoveLeft();
		else if(Input.GetKey(KeyCode.RightArrow) || touchVec > accel_stop)
			InputMoveRight();
		else speed.x += (-speed.x * 10f) * Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1) || Input.touchCount > 1)
			InputJump();

		Vector2 grvVec = Physics2D.gravity.normalized;

		Vector2 prevPos = transform.position;
		Vector2 nextPos = prevPos;

		if(false) { // stand) {
			RaycastHit2D result = Physics2D.Raycast(prevPos, grvVec, 64);
			stand = (result.collider != null);
		}
		else {
			speed += Physics2D.gravity * Time.deltaTime;
		}

		if(speed.magnitude > 1) {
			Vector3 nextVec = speed * 60 * Time.deltaTime;
			ColliderResult result = CalcCollidedPosition(prevPos, nextVec, 3);
			transform.position = result.pos;
			foreach(RaycastHit2D hit in result.hits) {
				if(hit.normal.y > 0.75f) {
					stand = true;
					speed.y = 0;
				}
				else if(hit.normal.y < -0.5f) {
					speed.y = Mathf.Min(speed.y, 0);
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(2) || Input.touchCount > 2)
			transform.position = Vector3.zero;
	}

	void CheckCollider(Collider2D other) {

		BoxCollider2D box = other as BoxCollider2D;
		Vector3 boxPos = box.bounds.center;
		Vector3 bodyPos = GetBodyPos();
		Vector3 vec = bodyPos - boxPos;
		Debug.DrawRay(boxPos, vec, Color.green);

		vec = vec.normalized * (((box.size.magnitude + objBase.Box2D.size.magnitude) * 0.5f) - vec.magnitude);
		Vector3 pos = bodyPos + vec;
		vec = transform.position - pos;

		Debug.DrawRay(pos, vec, Color.yellow);

		Debug.Log("hit "+ other.name + " vec "+ vec+ " dist "+ vec.magnitude);

		ColliderResult result = CalcCollidedPosition(pos, vec, 3);
		transform.position = result.pos;
		foreach(RaycastHit2D hit in result.hits) {
			if(hit.normal.y > 0.75f) {
				stand = true;
				speed.y = 0;
			}
			else if(hit.normal.y < -0.5f) {
				speed.y = Mathf.Min(speed.y, 0);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		CheckCollider(other);
	}

	void OnTriggerStay2D(Collider2D other) {
		CheckCollider(other);
	}

	class ColliderResult {
		public Vector3 pos;
		public List<RaycastHit2D> hits = new List<RaycastHit2D>();
	}

	// 衝突後の座標を算出する
	ColliderResult CalcCollidedPosition(
		Vector3 prevRootPos, Vector3 nextVec, int repeat = 1) 
	{
		ColliderResult colliderResult = new ColliderResult();
		Vector3 nextRootPos = prevRootPos + nextVec;
		Debug.DrawRay(prevRootPos, nextVec, Color.yellow);

		Vector3 bodySize = objBase.Box2D.size;
		Vector3 prevBodyPos = prevRootPos + GetBodyOffset();
		Vector3 nextBodyPos = prevBodyPos + nextVec;

		for(int i = 0; i < repeat; ++i) {
			RaycastHit2D result = Physics2D.BoxCast(
				prevBodyPos, bodySize, 0, nextVec, nextVec.magnitude + 8, 
				Physics2D.AllLayers//LayerMask.NameToLayer("Default")
			);
			if(result.collider != null) {
				colliderResult.hits.Add(result);

				Vector3 bodyPos = nextBodyPos;

				if(Mathf.Abs(result.normal.x) > Mathf.Abs(result.normal.y))
					nextBodyPos.x = result.point.x + (result.normal.x > 0 ? 1:-1) * (bodySize.x / 2 + 8);
				else nextBodyPos.y = result.point.y + (result.normal.y > 0 ? 1:-1) * (bodySize.y / 2 + 8);

				Debug.DrawLine(bodyPos, nextBodyPos, Color.gray);
				Debug.DrawLine(prevRootPos, result.point, Color.cyan);

				nextVec = nextBodyPos - prevBodyPos;
				nextBodyPos = prevBodyPos + nextVec;
			}
			else break;
		}

		colliderResult.pos = nextBodyPos - GetBodyOffset();
		return colliderResult;
	}



	public Vector3 GetBodyOffset() {
		if(objBase != null && objBase.Box2D != null)
			return transform.rotation * objBase.Box2D.center;
		return Vector3.zero;
	}

	public Vector3 GetBodyPos() {
		return transform.position + GetBodyOffset();
	}

	void OnGUI() {
		GUILayout.BeginVertical("", "box");
			GUILayout.Label("fps "+ 1f / Time.deltaTime);
			GUILayout.Label("speed x "+ speed.x);
			GUILayout.Label("speed y "+ speed.y);
			GUILayout.Label("stand "+ stand);
		GUILayout.EndVertical();
	}

	void OnDrawGizmos() {

		if(objBase != null && objBase.Box2D != null) {
			Vector3 pos = GetBodyPos();
			pos.x += speed.x;
			pos.y += speed.y;
			Gizmos.color = new Color(1,1,1,0.5f);
			Gizmos.DrawWireCube(pos, new Vector3(bodySize * 0.5f, bodySize, 0));
		}
	}
}
