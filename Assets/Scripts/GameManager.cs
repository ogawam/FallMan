using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[SerializeField] Camera camera2D;
	[SerializeField] UnitBase playerUnit;
	[SerializeField] UIPanel mainPanel;
	float cameraHeight = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(playerUnit.transform.localPosition.y < cameraHeight - (Common.viewHeight / 2)) {
			cameraHeight -= Common.viewHeight;
			mainPanel.transform.localPosition = Vector3.down * cameraHeight;
		}

		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(2) || Input.touchCount > 2) {
			playerUnit.transform.position = Vector3.zero;
			cameraHeight = 0;
			mainPanel.transform.localPosition = Vector3.zero;
		}
	}

	void OnDrawGizmos() {
		for(int i = 0; i < 64; ++i) {
			Gizmos.color = (i % 2) != 0 ? 
				new Color(0.75f,1,0.5f, 0.25f): 
				new Color(0.5f,0.75f,1f, 0.25f);
			Gizmos.DrawCube(
				new Vector3(0, -Common.viewHeight * i, 0), 
				new Vector3(Common.viewWidth, Common.viewHeight, 0)
			);
		}
	}
}
