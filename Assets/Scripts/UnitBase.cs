using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitBase : ObjectBase {

	public float accel_move;
	public float accel_stop;
	public float speed_jump;

	public float bodySize;

	public Vector2 speed_max;

	[SerializeField] ObjectBase objGuide;


	// Use this for initialization
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
	protected override void Update_ () {
		speed.x = Mathf.Clamp(speed.x, -speed_max.x, speed_max.x);
		speed.y = Mathf.Clamp(speed.y, -speed_max.y, speed_max.y);

		uiObject.transform.localScale = new Vector3(speed.x < 0 ? -1 : 1,1,1);

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

		Vector3 prevPos = transform.position;

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
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		OnCollider2D(hit2D);
	}

	void OnTriggerStay2D(Collider2D hit2D) {
		OnCollider2D(hit2D);
	}

	void OnCollider2D(Collider2D hit2D) {
		BoxCollider2D hitBox2D = hit2D as BoxCollider2D;
		if(hitBox2D != null) {
			Vector3 boxPos = hitBox2D.bounds.center;
			Vector3 bodyPos = GetBodyPos();
			Vector3 vec = bodyPos - boxPos;
			Debug.DrawRay(boxPos, vec, Color.green);

			vec = vec.normalized * (((hitBox2D.size.magnitude + box2D.size.magnitude) * 0.5f) - vec.magnitude);
			Vector3 pos = bodyPos + vec;
			vec = transform.position - pos;

			Debug.DrawRay(pos, vec, Color.yellow);

			Debug.Log("hit "+ hit2D.name + " vec "+ vec+ " dist "+ vec.magnitude);

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
		Debug.DrawRay(prevRootPos, nextVec, Color.yellow);

		Vector3 bodySize = (collider2D as BoxCollider2D).size;
		Vector3 prevBodyPos = prevRootPos + GetBodyOffset();
		Vector3 nextBodyPos = prevBodyPos + nextVec;

		for(int i = 0; i < repeat; ++i) {
			RaycastHit2D result = Physics2D.BoxCast(
				prevBodyPos, bodySize, 0, nextVec, nextVec.magnitude + 2, 
				1 << LayerMask.NameToLayer("Default")
			);
			if(result.collider != null) {
				colliderResult.hits.Add(result);

				Vector3 bodyPos = nextBodyPos;

				if(Mathf.Abs(result.normal.x) > Mathf.Abs(result.normal.y))
					nextBodyPos.x = result.point.x + (result.normal.x > 0 ? 1:-1) * (bodySize.x / 2 + 2);
				else nextBodyPos.y = result.point.y + (result.normal.y > 0 ? 1:-1) * (bodySize.y / 2 + 2);

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
		return transform.rotation * box2D.center;
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

		Vector3 pos = GetBodyPos();
		pos.x += speed.x;
		pos.y += speed.y;
		Gizmos.color = new Color(1,1,1,0.5f);
		Gizmos.DrawWireCube(pos, new Vector3(box2D.size.x, box2D.size.y, 0));
	}
}
