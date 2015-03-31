using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {

	[SerializeField] float rotateSpeed = 45;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = Vector3.forward * rotateSpeed * Time.time;
	}
}
