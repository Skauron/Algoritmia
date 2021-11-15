using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Target))]
public class TargetsType : Editor
{
    Target mTargets;
    void OnEnable()
    {
        mTargets = (Target)target;
    }
    public override void OnInspectorGUI()
    {
        mTargets.type = (TargetType)EditorGUILayout.EnumPopup("TargetType", mTargets.type);
        switch (mTargets.type)
        {
            case TargetType.Light:
                {
                    mTargets.objName = EditorGUILayout.TextField("Name", mTargets.objName);
                    mTargets.Object = (GameObject)EditorGUILayout.ObjectField("Object", mTargets.Object, typeof(GameObject), true);
                    break;
                }
            case TargetType.Enemy:
                {
                    mTargets.objName = EditorGUILayout.TextField("Name", mTargets.objName);
                    mTargets.Object = (GameObject)EditorGUILayout.ObjectField("Object", mTargets.Object, typeof(GameObject), true);
                    break;
                }
            case TargetType.Door:
                {
                    mTargets.objName = EditorGUILayout.TextField("Name", mTargets.objName);
                    break;
                }
            case TargetType.Object:
                {
                    mTargets.objName = EditorGUILayout.TextField("Name", mTargets.objName);
                    mTargets.force = EditorGUILayout.FloatField("Force", mTargets.force);
                    break;
                }
        }
    }
}