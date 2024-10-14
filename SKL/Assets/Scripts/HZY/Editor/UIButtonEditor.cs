using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Button;
[CustomEditor(typeof(UIButton), true)]
public class UIButtonEditor : Editor
{
    UIButton btn;
    SerializedProperty m_OnClickProperty;
    public override void OnInspectorGUI()
    {
        if (btn == null)
        {
            btn = target as UIButton;
        }
        // base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        bool scaleEnable = EditorGUILayout.Toggle("是否允许缩放动画", btn.scaleEnable);
        if (btn.scaleEnable != scaleEnable)
        {
            EditorUtility.SetDirty(btn);
            btn.scaleEnable = scaleEnable;
        }

        bool interactable = EditorGUILayout.Toggle("是否可用", btn.interactable);
        if (interactable != btn.interactable)
        {
            EditorUtility.SetDirty(btn);
            btn.interactable = interactable;
        }
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OnClickProperty);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
        // btn.onClick = EditorGUILayout.ObjectField(btn.onClick,UnityEvent);
        //base.OnInspectorGUI();
    }
}