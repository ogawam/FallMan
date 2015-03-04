using UnityEngine;
using System.Collections;

public class UIFaderController : MonoBehaviour {

	[SerializeField] UITexture fadeTexture;
	[SerializeField] float defaultFadeTime;
	float fadeCount = 0;
	float fadeTime = 0;
	Color prevColor;
	Color fadeColor;

	enum State {
		FadeOut,
		FadeOutOver,
		FadeIn,
		FadeInOver,
	}
	State state = State.FadeOutOver;

	void Fade(float time, Color color) {		
		prevColor = fadeTexture.color;
		fadeColor = color;
		fadeCount = 0;
		fadeTime = time;
	}

	// フェードアウト
	public void FadeOut(float time, Color color) {
		state = State.FadeOut;
		Fade(time, color);
	}

	public void FadeOut(float time) {
		FadeOut(time, Color.black);
	}

	public void FadeOut() {
		FadeOut(defaultFadeTime);		
	}

	// コルーチン版
	public IEnumerator CoFadeOut(float time, Color color) {
		FadeOut(time, color);
		while(state != State.FadeOutOver)		
			yield return null;
	}

	public IEnumerator CoFadeOut(float time) {
		yield return StartCoroutine(CoFadeOut(time, Color.black));
	}

	public IEnumerator CoFadeOut() {
		yield return StartCoroutine(CoFadeOut(defaultFadeTime));
	}

	// 実用フェードアウト
	public bool SmartFadeOut(float time, Color color) {
		if(state == State.FadeInOver) {
			FadeOut(time, color);
			return true;
		}
		return false;
	}

	public bool SmartFadeOut(float time) {
		return SmartFadeOut(time, Color.black);
	}

	public bool SmartFadeOut() {
		return SmartFadeOut(defaultFadeTime);
	}

	// コルーチン版
	public IEnumerator CoSmartFadeOut(float time, Color color) {
		while(isFading)
			yield return null;

		if(SmartFadeOut(time, color))
			while(state != State.FadeOutOver)		
				yield return null;
	}

	public IEnumerator CoSmartFadeOut(float time) {
		yield return StartCoroutine(CoSmartFadeOut(time, Color.black));
	}

	public IEnumerator CoSmartFadeOut() {
		yield return StartCoroutine(CoSmartFadeOut(defaultFadeTime));
	}

	// フェードイン
	public void FadeIn(float time, Color color) {
		state = State.FadeIn;
		fadeTexture.color = color;
		color.a = 0;
		Fade(time, color);
	}

	public void FadeIn(float time) {
		FadeIn(time, Color.black);		
	}

	public void FadeIn() {
		FadeIn(defaultFadeTime);		
	}

	// コルーチン版
	public IEnumerator CoFadeIn(float time, Color color) {
		FadeIn(time, color);
		while(state != State.FadeInOver)		
			yield return null;
	}

	public IEnumerator CoFadeIn(float time) {
		yield return StartCoroutine(CoFadeIn(defaultFadeTime, Color.black));
	}

	public IEnumerator CoFadeIn() {
		yield return StartCoroutine(CoFadeIn(defaultFadeTime));
	}

	// 実用フェードイン
	public bool SmartFadeIn(float time, Color color) {
		if(state == State.FadeOutOver) {
			FadeIn(time, color);
			return true;
		}
		return false;
	}

	public bool SmartFadeIn(float time) {
		return SmartFadeIn(time, Color.black);
	}

	public bool SmartFadeIn() {
		return SmartFadeIn(defaultFadeTime);
	}

	// コルーチン版
	public IEnumerator CoSmartFadeIn(float time, Color color) {
		while(isFading)
			yield return null;

		if(SmartFadeIn(time, color))
			while(state != State.FadeInOver)		
				yield return null;
	}

	public IEnumerator CoSmartFadeIn(float time) {
		yield return StartCoroutine(CoSmartFadeIn(time, Color.black));
	}

	public IEnumerator CoSmartFadeIn() {
		yield return StartCoroutine(CoSmartFadeIn(defaultFadeTime));
	}

	public bool isFading { get { return state == State.FadeOut || state == State.FadeIn; } }

	void Awake () {
		prevColor =
		fadeTexture.color = Color.black;
		state = State.FadeOutOver;
	}

	void Update () {
		switch(state) {
		case State.FadeOut:
		case State.FadeIn:
			FadeUpdate();
			break;
		}
	}

	void FadeUpdate () {
		if(fadeCount < fadeTime) {
			float rate = fadeCount / fadeTime;
			fadeTexture.color = prevColor + (fadeColor - prevColor) * rate;
			fadeCount += Time.deltaTime;
		}
		else {
			fadeTexture.color = fadeColor;
			switch(state) {
			case State.FadeOut:
				state = State.FadeOutOver;
				break;
			case State.FadeIn:
				state = State.FadeInOver;
				break;
			}	
		}
	}

	public bool isTest;
	public float testTime;
	public Color testColor;
	void OnGUI() {
		if(isTest) {
			GUILayout.BeginVertical("", "box");
				GUILayout.Label("state "+ state);
				if(GUILayout.Button("FadeOut(defalut)"))		FadeOut();
				if(GUILayout.Button("FadeOut"))					FadeOut(testTime, testColor);
				if(GUILayout.Button("SmartFadeOut(default)"))	SmartFadeOut();
				if(GUILayout.Button("SmartFadeOut"))			SmartFadeOut(testTime, testColor);
				if(GUILayout.Button("CoFadeOut(defalut)"))		StartCoroutine(CoFadeOut());
				if(GUILayout.Button("CoFadeOut"))				StartCoroutine(CoFadeOut(testTime, testColor));
				if(GUILayout.Button("CoSmartFadeOut(default)"))	StartCoroutine(CoSmartFadeOut());
				if(GUILayout.Button("CoSmartFadeOut"))			StartCoroutine(CoSmartFadeOut(testTime, testColor));			
			GUILayout.EndVertical();
		}	
	}
}
