using UnityEngine;
using System.Collections;

public class AreaManager : MonoBehaviour {

	[SerializeField] GimmickRespawn respawn;
	public GimmickRespawn GetRespawn() {
		return respawn;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnValidate() {
		hideFlags = HideFlags.HideInHierarchy;
	}
}
