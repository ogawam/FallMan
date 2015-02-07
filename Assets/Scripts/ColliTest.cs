using UnityEngine;
using System.Collections;

public class ColliTest : MonoBehaviour {

	public Vector2 pos;
	public Vector2 vec;
	public Vector2 size;
	public float angle;

	bool isHit = false;
	Vector2[] centroids = null;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit2D[] results = Physics2D.BoxCastAll(pos, size, angle, vec, vec.magnitude);
		Debug.DrawRay(pos, vec);
		if(results.Length > 0)
			centroids = new Vector2[results.Length];

		isHit = false;
		for(int i = 0; i < results.Length; ++i) {
			RaycastHit2D result = results[i];
			Debug.DrawRay(result.point, result.normal * result.distance);
//			Debug.DrawRay(result.centroid, result.normal * result.distance);
			centroids[i] = result.centroid;
			isHit = true;
		}
	}

	void OnDrawGizmos() {
		Quaternion rotation = Quaternion.identity;
		rotation.eulerAngles = Vector3.forward * angle;

		Matrix4x4 tempMat = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(pos, rotation, new Vector3(1f, 1.0f, 1.0f));
		Gizmos.DrawWireCube(Vector2.zero, size);

		Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(pos+vec, rotation, new Vector3(1f, 1.0f, 1.0f));
		Gizmos.DrawWireCube(Vector2.zero, size);

		if(isHit) {
			Gizmos.color = Color.red;

			foreach(Vector2 centroid in centroids) {
		        Gizmos.matrix = Matrix4x4.TRS(centroid, rotation, new Vector3(1f, 1.0f, 1.0f));
				Gizmos.DrawWireCube(Vector2.zero, size);
			}
		}

		Gizmos.matrix = tempMat;
	}
}
