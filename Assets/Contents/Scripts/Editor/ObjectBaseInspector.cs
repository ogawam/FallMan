using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#if false
[CustomEditor(typeof(ObjectBase))]
public class ObjectBaseInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		ObjectBase instance = (target as ObjectBase);
		if(GUILayout.Button("Arrange On Grid")) {
			Undo.RecordObject(instance, "Arrange On Grid");
			instance.ArrangeOnGrid();
		}
	}
}
#endif