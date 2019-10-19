using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class AnimationClipRecoveryEditor : EditorWindow
{
    [MenuItem("Window/AnimationClip Recover")]
    static void Init ()
    {
        var w = EditorWindow.GetWindow<AnimationClipRecoveryEditor>();
        w.title = "Clipリカバリー";
        w.Show();
    }

    private Transform _tr;
    private AnimationClip _clip;
    private List<RecoveryData> _props = new List<RecoveryData>(); 

    private string[] _curveNames = new string[]{
        "m_PositionCurves",
        "m_ScaleCurves",
        "m_FloatCurves",
        "m_PPtrCurves",
        "m_EditorCurves",
        "m_EulerEditorCurves"
    };

    void OnGUI ()
    {
        _clip = (AnimationClip)EditorGUILayout.ObjectField((AnimationClip)_clip, typeof(AnimationClip));
        _tr = (Transform)EditorGUILayout.ObjectField((Transform)_tr, typeof(Transform), true);
        if (_tr == null || _clip == null) {
            return;
        }
        if (GUILayout.Button("Check")) {
            var soClip = new SerializedObject(_clip);
            _props = FindCurveProps(soClip);
        }

        if (_props.Count > 0) {
            GUILayout.Label("AnimationClip is INVALID");
            for (int i = 0; i < _props.Count; i++)
            {
                var p = _props[i];
                GUILayout.Space(3f);
                p.name = GUILayout.TextField(p.name);
                if (GUILayout.Button("Recover")) {
                    p.prop.stringValue = p.name;
                    p.so.ApplyModifiedProperties();
                }
            }
        } else {
            GUILayout.Label("AnimationClip is ALL GREEN");
        }
    }

    private List<RecoveryData> FindCurveProps (SerializedObject so)
    {
        List<RecoveryData> resultList = new List<RecoveryData>();
        foreach (var curveName in _curveNames) {
            var posCurves = so.FindProperty(curveName);
            int len = posCurves.arraySize;
            for (int i = 0; i < len; i++)
            {
                var curve = posCurves.GetArrayElementAtIndex(i);
                var pathProp = curve.FindPropertyRelative("path");
                var path = pathProp.stringValue;
                var result = _tr.Find(path);
                Debug.LogFormat("{0} : {1}", path, result);
                if (result == null) {
                    resultList.Add(new RecoveryData(){ so = so, prop = pathProp, name = path });
                    Debug.LogError(path);
                }
            }
        }
        return resultList;
    }

}

class RecoveryData {
    public SerializedObject so;
    public SerializedProperty prop;
    public string name;
}
