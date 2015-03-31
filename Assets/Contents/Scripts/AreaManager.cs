using UnityEngine;
using System.Collections;

public class AreaManager : MonoBehaviour {

	[SerializeField] GimmickRespawn respawn;
	public GimmickRespawn GetRespawn() {
		return respawn;
	}

	// Use this for initialization
	ObjectBase[] objects = null;
	void Start () {
		objects = GetComponentsInChildren<ObjectBase>(true);
		Reset();
	}
	
	// Update is called once per frame
	void Update () {
			
	}

	public void Pause(bool pause) {
		foreach(ObjectBase obj in objects)
			obj.Pause(pause);
	}

	public void Reset() {
		foreach(ObjectBase obj in objects) {
			obj.Pause(true);
			obj.Reset();
		}
	}

	public void Begin() {
		foreach(ObjectBase obj in objects) {
			obj.Pause(false);
			obj.Reset();
		}
	}

	void OnValidate() {
		hideFlags = HideFlags.HideInHierarchy;
	}
}
