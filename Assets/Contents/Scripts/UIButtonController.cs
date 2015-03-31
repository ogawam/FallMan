using UnityEngine;
using System.Collections;

public class UIButtonController : MonoBehaviour {

	[SerializeField] Common.Button type;
	public Common.Button GetButtonType() {
		return type;
	}

	Common.ButtonState state;
	bool isInputFrame = false;

	void Awake () {
		UIRoot2D.Get().AddButtonController(this);
	}

	void OnDestroy() {
		UIRoot2D.Get().RemoveButtonController(this);
	}

	public void Press () {
		Debug.Log("Press "+ type);
		SetState(Common.ButtonState.OnPress);
	}

	public void Holding () {
		SetState(Common.ButtonState.OnHolding);
	}

	void SetState(Common.ButtonState state_) {
		if(state_ == Common.ButtonState.OnPress) {
			if(state == Common.ButtonState.OnPress
			|| state == Common.ButtonState.OnHolding)
			{
				state_ = Common.ButtonState.OnHolding;
			}
		}
		state = state_;
		isInputFrame = true;
	}

	public bool IsFree() {
		return IsState(Common.ButtonState.OnFree);
	}

	public bool IsPress() {
		return IsState(Common.ButtonState.OnPress);
	}

	public bool IsHolding() {
		return IsState(Common.ButtonState.OnHolding);
	}

	public bool IsRelease() {
		return IsState(Common.ButtonState.OnRelease);
	}

	public bool IsState(Common.ButtonState state_) {
		return state == state_;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isInputFrame) {
			isInputFrame = false;
		}
		else {
			switch(state) {
			case Common.ButtonState.OnPress:
			case Common.ButtonState.OnHolding:
				SetState(Common.ButtonState.OnRelease);
				break;
			case Common.ButtonState.OnRelease:
				SetState(Common.ButtonState.OnFree);
				break;
			}
		}	
	}
}
