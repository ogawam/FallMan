using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {

	[SerializeField] AreaManager[] areas; 
	public AreaManager this[int index] {
		get { return areas[index]; }
	}

	public int areaCount {
		get { return areas.Length; }
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
