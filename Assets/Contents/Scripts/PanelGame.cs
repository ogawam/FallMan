using UnityEngine;
using System.Collections;

public class PanelGame : MonoBehaviour {

	[SerializeField] UITexture textureLightMulti;
	public UITexture GetLightMulti() {
		return textureLightMulti;
	}

	[SerializeField] UITexture textureLightAdd;
	public UITexture GetLightAdd() {
		return textureLightAdd;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
