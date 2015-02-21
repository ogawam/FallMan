using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AreaManager))]
public class AreaManagerInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		AreaManager instance = (target as AreaManager);
		if(GUILayout.Button("Arrange On Grid Children")) {
			ObjectBase[] objectBases = instance.GetComponentsInChildren<ObjectBase>(true);
			foreach(ObjectBase objectBase in objectBases) {
				Undo.RecordObject(objectBase, "Arrange On Grid");
				objectBase.ArrangeOnGrid();
			}
		}
	}
}
