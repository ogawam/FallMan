using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public bool isTest = false;

	enum State {
		Title,
		Main,
		Dead,
		Fall,
		Pause,		
	}

	State state = State.Main;
	float stateElapsedSec = 0;

	[SerializeField] Camera camera2D;
	[SerializeField] UnitBase playerUnit;
	[SerializeField] UIPanel mainPanel;
	[SerializeField] UIFaderController fader;
	[SerializeField] PanelIndicator indicator;

	[SerializeField] ObjectBase objGuide;
	[SerializeField] float scrollSpeed;

	[SerializeField] StageManager[] stages;

	int currentStageNo = 0;
	int currentAreaNo = 0;
	int life = 3;

	// Use this for initialization
	void Start () {
		TransitStateTitle();
		fader.FadeIn();

		// 初期化
		mainPanel.transform.localPosition = Vector3.zero;

		Environment.Get().Pause();
	}

	StageManager currentStage {
		get { return currentStageNo < stages.Length ? stages[currentStageNo] : null; }
	}

	AreaManager currentArea {
		get { return currentAreaNo < currentStage.areaCount ? currentStage[currentAreaNo] : null; }
	}

	float currentAreaHeight {
		get { return currentArea.transform.position.y; }
	}

#if !UNITY_EDITOR
	int latestFingerId = -1;
#endif

	// Update is called once per frame
	void Update () {
		switch(state) {
		case State.Title:
			UpdateStateTitle();
			break;
		case State.Main:
			UpdateStateMain();
			break;
		case State.Dead:
			UpdateStateDead();
			break;
		case State.Fall:
			UpdateStateFall();
			break;
		case State.Pause:
			break;
		}
		stateElapsedSec += Time.deltaTime;
	}

	void TransitStateTitle() {
		UIRoot2D.Get().indicator.DispTitle();
		stateElapsedSec = 0;
		state = State.Title;
	}

	void UpdateStateTitle() {
		if(indicator.IsTouchedButton(Common.Button.Start)) {
			indicator.HideTitle();
			StartCoroutine(TransitStateMain());
		}
	}

	IEnumerator TransitStateMain() {

		Vector3 pos = mainPanel.transform.localPosition;

		pos.y = -currentAreaHeight;
		mainPanel.transform.localPosition = pos;
		yield return StartCoroutine(fader.CoFadeIn());

		Environment.Get().Resume();

		stateElapsedSec = 0;
		state = State.Main;
	}

	void UpdateStateMain() {
		if(playerUnit.IsFlagsOn(UnitBase.Flag.Dead)) {
			TransitStateDead();
			return;
		}
		else if(playerUnit.transform.localPosition.y < currentAreaHeight - (Common.viewHeight / 2)) {
			TransitStateFall();
			return;
		}

		bool isMove = false;
		float touchPosX = 0;
#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1))
			playerUnit.InputJump();

		if(Input.GetMouseButton(0)) {
			touchPosX = Input.mousePosition.x;
			isMove = true;
		}
#else
		if(Input.touchCount > 0) {
			foreach(Touch touch in Input.touches) {
				switch(touch.phase) {
				case TouchPhase.Began:
					latestFingerId = touch.fingerId;
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					if(latestFingerId == touch.fingerId)
						latestFingerId = -1;
					break;
				default:
					if(latestFingerId < 0)
						latestFingerId = touch.fingerId;
					break;
				}

				if(latestFingerId == touch.fingerId) {
					latestFingerId = touch.fingerId;
					touchPosX = touch.position.x;
					isMove = true;
				}
			}
		}
#endif

		if(isMove) {
			touchPosX = (touchPosX - Screen.width / 2) * Common.scrn2View;
			objGuide.transform.position = new Vector3(touchPosX, -mainPanel.transform.localPosition.y, 0);
			if(touchPosX < 0)
				playerUnit.InputMoveLeft();
			else playerUnit.InputMoveRight();
//			objGuide.SetVisible(false);
		}
		else {
//			objGuide.SetVisible(false);
		}
	}

	void TransitStateDead() {
		life--;
		stateElapsedSec = 0;
		state = State.Dead;
	}

	void UpdateStateDead() {
		if(stateElapsedSec > 2) {
			if(life == 0) {
				life = 3;
				currentStageNo = 0;
				currentAreaNo = 0;
			}
	
			playerUnit.transform.position = respawnPosition;

			playerUnit.Respawn();
			TransitStateMain();
		}
	}

	public Vector3 respawnPosition { 
		get { 
			Vector3 result = Vector3.zero;
			mainPanel.transform.localPosition = Vector3.down * currentAreaHeight;
			GimmickRespawn respawn = currentArea.GetRespawn();
			if(respawn != null)
				result = respawn.transform.position;
			else result = Vector3.up * currentAreaHeight;
			return result;
		} 
	}

	void TransitStateFall() {

		currentAreaNo++;
		if(currentAreaNo >= currentStage.areaCount) {
			currentAreaNo = 0;

			currentStageNo++;
			if(currentStageNo >= stages.Length) {
				currentStageNo = 0;
			}
		}

		Environment.Get().Pause();
		stateElapsedSec = 0;
		state = State.Fall;
	}

	void UpdateStateFall() {
		Vector3 pos = mainPanel.transform.localPosition;
		if(pos.y < -currentAreaHeight) {
			pos.y += scrollSpeed * Time.deltaTime;
		}
		else {
			pos.y = -currentAreaHeight;
			Environment.Get().Resume();
			TransitStateMain();
		}
		mainPanel.transform.localPosition = pos;
	}

	void TransitStatePause() {
		
		stateElapsedSec = 0;
		state = State.Pause;
	}

	void UpdateStatePause() {

	}

	void KillPlayer() {
		if(playerUnit.IsFlagsOff(UnitBase.Flag.Dead)) {
			playerUnit.Kill();
		}
	}

	int debugStageNo = 0;
	int debugAreaNo = 0;
	void OnGUI() {
		if(isTest) {
			GUILayout.BeginHorizontal();

				if(GUILayout.Button("ダメージ")) {
					KillPlayer();
				}
				if(GUILayout.Button("やり直し")) {
					life = 3;
					currentStageNo = 0;
					currentAreaNo = 0;
					playerUnit.transform.position = respawnPosition;				
				}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

				for(int i = 0; i < stages.Length; ++i) {
					GUI.color = i != debugStageNo ? Color.white : Color.yellow;
					if(GUILayout.Button(stages[i].name))
						debugStageNo = i;
				}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

				StageManager debugStage = stages[debugStageNo];
				for(int i = 0; i < debugStage.areaCount; ++i) {
					GUI.color = i != debugAreaNo ? Color.white : Color.yellow;
					if(GUILayout.Button(debugStage[i].name))
						debugAreaNo = i;
				}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

				if(GUILayout.Button("ステージを移動する")) {
					currentStageNo = debugStageNo;
					currentAreaNo = debugAreaNo;
					playerUnit.transform.position = respawnPosition;				
				}

			GUILayout.EndHorizontal();

			GUILayout.BeginVertical("", "box");
				GUILayout.Label("fps "+ 1f / Time.deltaTime);
				GUILayout.Label("life "+ life);
			GUILayout.EndVertical();
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
