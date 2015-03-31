using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitBase : ObjectBase {

	[SerializeField] EffectExplode prefabExplode;

	public enum Flag {
		Land = (1<<0),
		Move = (1<<1),
		Dead = (1<<2),
	}

	Flag flags = 0;
	void OnFlags(Flag flags_) {
		flags |= flags_;
	}

	void OffFlags(Flag flags_) {
		flags &= ~flags_;
	}

	public bool IsFlagsOn(Flag flags_) {
		return (flags & flags_) != 0;
	}

	public bool IsFlagsOff(Flag flags_) {
		return (flags & flags_) == 0;
	}

	public float accel_move;
	public float accel_stop;
	public float speed_jump;

	public float bodySize;
	public Vector2 standAxis;

	public Vector2 speed_max;


	// Use this for initialization
	Vector2 speed = Vector2.zero;

	public void InputMoveLeft() {
		Vector3 move = transform.rotation * (Vector3.left * accel_move * Time.deltaTime);
		speed.x += move.x;
		speed.y += move.y;
		OnFlags(Flag.Move);
	}

	public void InputMoveRight() {
		Vector3 move = transform.rotation * (Vector3.right * accel_move * Time.deltaTime);
		speed.x += move.x;
		speed.y += move.y;
		OnFlags(Flag.Move);
	}

	public void InputJump(float rate = 1) {
		if(IsFlagsOn(Flag.Land)) {
			Vector3 move = transform.rotation * (Vector3.up * speed_jump * rate);
			speed.x += move.x;
			speed.y += move.y;
			OffFlags(Flag.Land);
		}
	}

	public void InputDown() {
		Quaternion inverse = Quaternion.Inverse(transform.rotation);
		Vector3 localSpeed = inverse * speed;
		localSpeed.y = Mathf.Min(localSpeed.y, 0);
		speed = transform.rotation * localSpeed;		
	}

	// Update is called once per frame
	protected override void Update_ () {
		if(IsFlagsOn(Flag.Dead))
			return;

		foreach(Collider2D landCollider in landColliders) {
			GimmickJump gmkJump = landCollider.GetComponent<GimmickJump>();
			if(gmkJump != null && gmkJump.IsOpen()) {
				InputJump(1.5f);
			}
		}

		Quaternion inverse = Quaternion.Inverse(transform.rotation);
		{
			Vector3 localSpeed = inverse * speed;
			localSpeed.x = Mathf.Clamp(localSpeed.x, -speed_max.x, speed_max.x);
			localSpeed.y = Mathf.Clamp(localSpeed.y, -speed_max.y, speed_max.y);

			if(IsFlagsOff(Flag.Move))
				localSpeed.x += (-localSpeed.x * 10f) * Time.deltaTime;
			OffFlags(Flag.Move);

			speed = transform.rotation * localSpeed;
			uiObject.transform.localScale = new Vector3(localSpeed.x < 0 ? -1 : 1,1,1);
		}

		Vector2 grvVec = Physics2D.gravity.normalized;

		// 重力方向に足を向ける
		Vector3 eulerAngles = transform.localEulerAngles;
		float zAngle = Common.Rad2Deg(Mathf.PI - Mathf.Atan2(grvVec.x, grvVec.y));
		zAngle = Common.ClipDeg(zAngle);
		eulerAngles.z += Common.ClipDegMin(zAngle - eulerAngles.z) * 0.1f;
		transform.localEulerAngles = eulerAngles;

		Vector3 prevPos = transform.position;

		speed += Physics2D.gravity * Time.deltaTime;

		if(speed.magnitude > 1) {
			Vector3 nextVec = speed * 60 * Time.deltaTime;

			if(IsFlagsOn(Flag.Land)) {
				Debug.DrawRay(transform.position, standAxis * 100);
				float angle = Common.Rad2Deg(Mathf.Atan2(standAxis.x, standAxis.y));
				Quaternion rotate = new Quaternion();
				rotate.eulerAngles = Vector3.forward * angle;

				Vector3 localNextVec = inverse * nextVec;
				Vector3 offset = inverse * (rotate * (Vector3.right * localNextVec.x));
				offset.x = 0;
				offset = transform.rotation * offset;
				nextVec -= offset;
//				Debug.Log("offset "+ offset);
			}

			CollideOnMove(CalcCollidedPosition(prevPos, nextVec, 3));
		}
	}

	void OnTriggerEnter2D(Collider2D hit2D) {
		if(hit2D.isTrigger)
			return;

		OnCollider2D(hit2D);
	}

	void OnTriggerStay2D(Collider2D hit2D) {
		if(hit2D.isTrigger)
			return;

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

			CollideOnMove(CalcCollidedPosition(pos, vec, 3));
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

		Vector3 bodySize = (GetComponent<Collider2D>() as BoxCollider2D).size;
		Vector3 prevBodyPos = prevRootPos + GetBodyOffset();
		Vector3 nextBodyPos = prevBodyPos + nextVec;

		for(int i = 0; i < repeat; ++i) {
			RaycastHit2D result = Physics2D.BoxCast(
				prevBodyPos, bodySize, 
				transform.localEulerAngles.z, 
				nextVec, nextVec.magnitude + 2, 
				1 << LayerMask.NameToLayer("Default")
			);
			if(result.collider != null) {
				colliderResult.hits.Add(result);
				Vector3 bodyPos = nextBodyPos;

				Quaternion inverse = Quaternion.Inverse(transform.rotation);
				Vector3 localNormal = inverse * result.normal;
				Vector3 localPoint = inverse * result.point;
				nextBodyPos = inverse * nextBodyPos;

				if(Mathf.Abs(localNormal.x) > Mathf.Abs(localNormal.y))
					nextBodyPos.x = localPoint.x + (localNormal.x > 0 ? 1:-1) * (bodySize.x / 2 + 2);
				else nextBodyPos.y = localPoint.y + (localNormal.y > 0 ? 1:-1) * (bodySize.y / 2 + 2);

				nextBodyPos = transform.rotation * nextBodyPos;

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

	List<Collider2D> landColliders = new List<Collider2D>();

	void CollideOnMove(ColliderResult result) {
		landColliders.Clear();

		transform.position = result.pos;
		Quaternion inverse = Quaternion.Inverse(transform.rotation);
		Vector3 localSpeed = inverse * speed;
		foreach(RaycastHit2D hit in result.hits) {
			Vector3 localNormal = inverse * hit.normal;
			if(localNormal.y > 0.75f) {
				standAxis = new Vector2(hit.normal.x, hit.normal.y);
				OnFlags(Flag.Land);
				localSpeed.y = 0;

				landColliders.Add(hit.collider);
			}
			else if(localNormal.y < -0.5f) {
				localSpeed.y = Mathf.Min(localSpeed.y, 0);
			}
		}
		speed = transform.rotation * localSpeed;
	}

	EffectExplode explode = null;
	public void Kill() {
		OnFlags(Flag.Dead);
		SetVisible(false);
		if(GetComponent<Collider2D>() != null)
			GetComponent<Collider2D>().enabled = false;
		explode = Instantiate(prefabExplode) as EffectExplode;
		explode.transform.parent = transform.parent;
		explode.transform.position = transform.position;
//		transform.localEulerAngles = Vector3.forward * 90;
	}

	public void Respawn() {
		explode.Shrink(transform.position);
		OffFlags(Flag.Dead | Flag.Land);
		if(GetComponent<Collider2D>() != null)
			GetComponent<Collider2D>().enabled = true;
		SetVisible(true);
	}

	public Vector3 GetBodyOffset() {
		return transform.rotation * box2D.offset;
	}

	public Vector3 GetBodyPos() {
		return transform.position + GetBodyOffset();
	}

	protected override void OnDrawGizmos_() {
/*
		Vector3 pos = GetBodyPos();
		pos.x += speed.x;
		pos.y += speed.y;
		Gizmos.color = new Color(1,1,1,0.5f);
		Gizmos.DrawWireCube(pos, new Vector3(box2D.size.x, box2D.size.y, 0));
*/
	}
}
