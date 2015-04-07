using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public bool isTest = false;

	enum Sequence {
		Title,
		Main,
		Dead,
		Fall,
		Pause,		
	}
	Sequence sequence = Sequence.Title;

	enum State {
		Start,
		Update,
		End,
	}
	State state = State.Start;

	float seqElapsedSec = 0;

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

	UITexture texLightMulti = null;
	UITexture texLightAdd = null;

	UIRoot2D root2D = null;
	PanelGame panelGame = null;

	// Use this for initialization
	IEnumerator Start () {
		root2D = UIRoot2D.Get();
		panelGame = root2D.game;

		// 初期化
		mainPanel.transform.localPosition = Vector3.zero;

		yield return StartCoroutine(StartSequenceTitle());
		state = State.Update;

		texLightMulti = panelGame.GetLightMulti();
		texLightAdd = panelGame.GetLightAdd();
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
		case State.Start:
			// ステート開始処理
			break;

		case State.Update:
			SendMessage("UpdateSequence"+sequence);
			seqElapsedSec += Time.deltaTime;
			break;

		case State.End:
			// ステート終了処理
			break;
		}
	}

	// シーケンスを遷移する
	IEnumerator TransitSequence(Sequence nextSeq) {
		state = State.End;
		yield return StartCoroutine("EndSequence"+ sequence);

		sequence = nextSeq;

		state = State.Start;
		yield return StartCoroutine("StartSequence"+ nextSeq);

		state = State.Update;
		seqElapsedSec = 0;
	}

	IEnumerator StartSequenceTitle() {
		Environment.Get().Pause();
		root2D.indicator.DispTitle();
		yield return StartCoroutine(fader.CoFadeIn());
	}

	void UpdateSequenceTitle() {
		if(root2D.GetButtonController(Common.Button.Start).IsPress()) {
			indicator.HideTitle();
			StartCoroutine(TransitSequence(Sequence.Main));
		}
	}

	IEnumerator EndSequenceTitle() {
		yield return StartCoroutine(fader.CoFadeIn());
	}

	IEnumerator StartSequenceMain() {
		Vector3 pos = mainPanel.transform.localPosition;

		pos.y = -currentAreaHeight;
		mainPanel.transform.localPosition = pos;

		currentArea.Pause(false);
		Environment.Get().Resume();
		yield break;
	}

	void UpdateSequenceMain() {
		if(playerUnit.IsFlagsOn(UnitBase.Flag.Dead)) {
			StartCoroutine(TransitSequence(Sequence.Dead));
			return;
		}
		else if(playerUnit.transform.localPosition.y < currentAreaHeight - (Common.viewHeight / 2)) {
			StartCoroutine(TransitSequence(Sequence.Fall));			
			return;
		}

		texLightMulti.transform.localPosition = 
		texLightAdd.transform.localPosition = playerUnit.GetUIObject().transform.localPosition;

#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1))
			playerUnit.InputJump();

		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			playerUnit.InputMoveLeft();
		}
		else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			playerUnit.InputMoveRight();
		}
#endif
	
		if(root2D.GetButtonController(Common.Button.MoveLeft).IsHolding())
			playerUnit.InputMoveLeft();
		else if(root2D.GetButtonController(Common.Button.MoveRight).IsHolding())
			playerUnit.InputMoveRight();
		if(root2D.GetButtonController(Common.Button.Jump).IsPress())
			playerUnit.InputJump();
		if(root2D.GetButtonController(Common.Button.Jump).IsRelease())
			playerUnit.InputDown();
	}

	IEnumerator EndSequenceMain() {
		yield break;
	}

	IEnumerator StartSequenceDead() {
		life--;
		yield break;
	}

	void UpdateSequenceDead() {
		if(seqElapsedSec > 2) {
			if(life == 0) {
				life = 3;
				currentStageNo = 0;
				currentAreaNo = 0;
			}
	
			playerUnit.transform.position = respawnPosition;
			playerUnit.Respawn();
			StartCoroutine(TransitSequence(Sequence.Main));
		}
	}

	IEnumerator EndSequenceDead() {
		yield break;
	}

	IEnumerator StartSequenceFall() {
		currentArea.Pause(true);

		currentAreaNo++;
		if(currentAreaNo >= currentStage.areaCount) {
			currentAreaNo = 0;

			currentStageNo++;
			if(currentStageNo >= stages.Length) {
				currentStageNo = 0;
			}
		}

		Environment.Get().Pause();
		yield break;
	}

	void UpdateSequenceFall() {
		Vector3 pos = mainPanel.transform.localPosition;
		if(pos.y < -currentAreaHeight) {
			Debug.Log("pos.y "+ pos.y+ " currentAreaHeight "+ currentAreaHeight);
			pos.y += scrollSpeed * Time.deltaTime;
		}
		else {
			pos.y = -currentAreaHeight;
			Environment.Get().Resume();
			StartCoroutine(TransitSequence(Sequence.Main));
		}
		mainPanel.transform.localPosition = pos;
	}

	IEnumerator EndSequenceFall() {
		currentArea.Begin();
		yield break;
	}

	IEnumerator StartSequencePause() {
		yield break;
	}

	void UpdateSequencePause() {

	}

	IEnumerator EndSequencePause() {
		yield break;
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

	void KillPlayer() {
		if(playerUnit.IsFlagsOff(UnitBase.Flag.Dead)) {
			playerUnit.Kill();
		}
	}

	bool debugDisp = false;
	int debugStageNo = 0;
	int debugAreaNo = 0;
	void OnGUI() {
		if(isTest) {
			GUILayoutOption guiHeight = GUILayout.Height(80 / Common.scrn2View);
			GUILayout.BeginHorizontal();
				debugDisp ^= GUILayout.Button("デバッグ", guiHeight);
				if(debugDisp) {

					if(GUILayout.Button("ダメージ", guiHeight)) {
						KillPlayer();
					}
					if(GUILayout.Button("やり直し", guiHeight)) {
						life = 3;
						currentStageNo = 0;
						currentAreaNo = 0;
						playerUnit.transform.position = respawnPosition;				
					}
				}

			GUILayout.EndHorizontal();

			if(debugDisp) {

				GUILayout.BeginHorizontal();

					for(int i = 0; i < stages.Length; ++i) {
						GUI.color = i != debugStageNo ? Color.white : Color.yellow;
						if(GUILayout.Button(stages[i].name, guiHeight))
							debugStageNo = i;
					}

				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();

					StageManager debugStage = stages[debugStageNo];
					for(int i = 0; i < debugStage.areaCount; ++i) {
						GUI.color = i != debugAreaNo ? Color.white : Color.yellow;
						if(GUILayout.Button(debugStage[i].name, guiHeight))
							debugAreaNo = i;
					}

				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();

					if(GUILayout.Button("ステージを移動する", guiHeight)) {
						currentArea.Pause(true);
						currentStageNo = debugStageNo;
						currentAreaNo = debugAreaNo;
						playerUnit.transform.position = respawnPosition;				
						currentArea.Pause(false);
					}

				GUILayout.EndHorizontal();

				GUILayout.BeginVertical("", "box");

					GUILayout.Label("fps "+ 1f / Time.deltaTime);
					GUILayout.Label("sequence "+ sequence);
					GUILayout.Label("state "+ state);
					GUILayout.Label("life "+ life);
				GUILayout.EndVertical();
			}
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
