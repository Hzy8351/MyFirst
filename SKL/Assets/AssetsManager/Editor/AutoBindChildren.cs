
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AutoBindAtlas : System.Attribute
{
    public string Atlas { protected set; get; }
    public AutoBindAtlas(string atlas)
    {
        Atlas = atlas;
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class AutoBindBehaviour : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        {
            //if (GUILayout.Button("Auto Bind Children"))
            //{
            //    serializedObject.Update();
            //    BindValues();
            //    serializedObject.ApplyModifiedProperties();
            //}

            //if (GUILayout.Button("Clear"))
            //{
            //    serializedObject.Update();
            //    ClearAutoBinds();
            //    serializedObject.ApplyModifiedProperties();
            //}
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    public void BindValues()
    {
        var target = serializedObject.targetObject;
        var transform = (target is Component) ? ((Component)target).transform : null;
        if (target != null && transform != null)
        {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<System.NonSerializedAttribute>() != null)
                    continue;

                if (field.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                var property = serializedObject.FindProperty(field.Name);

                if (property != null)
                {
                    if (property.isArray)
                    {
                        int arraySize = property.arraySize;// GetArraySize(field, property);
                        for (int i = 0; i < arraySize; i++)
                        {
                            var tmp = property.GetArrayElementAtIndex(i);
                            //if (tmp == null)
                            //    property.InsertArrayElementAtIndex(i);
                            //tmp = property.GetArrayElementAtIndex(i);

                            var elementType = GetArrayElement(field.FieldType);//.GetGenericArguments()[0];
                            if (elementType == null)
                                break;

                            if (elementType == typeof(GameObject) && tmp.objectReferenceValue == null)
                            {
                                var value = GetValue(field, $"{field.Name}", i, transform);
                                tmp.objectReferenceValue = value != null ? value.gameObject : null;
                                continue;
                            }
                            if (elementType.IsSubclassOf(typeof(Component)) && tmp.objectReferenceValue == null)
                            {
                                var value = GetValue(field, $"{field.Name}", i, transform);
                                tmp.objectReferenceValue = value?.GetComponent(elementType);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (field.FieldType == typeof(GameObject) && property.objectReferenceValue == null)
                        {
                            var value = GetValue(field, field.Name, -1, transform);
                            property.objectReferenceValue = value != null ? value.gameObject : null;
                            continue;
                        }
                        if (field.FieldType.IsSubclassOf(typeof(Component)) && property.objectReferenceValue == null)
                        {
                            var value = GetValue(field, field.Name, -1, transform);
                            property.objectReferenceValue = value?.GetComponent(field.FieldType);
                            continue;
                        }
                    }
                }
            }
        }
    }

    static System.Type GetArrayElement(System.Type type)
    {
        if (!type.IsArray)
            return null;

        var gen = type.GetGenericArguments();
        if (gen.Length > 0)
            return gen[0];

        return type.GetElementType();
    }

    void ClearAutoBinds()
    {
        var target = serializedObject.targetObject;
        if (target != null)
        {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<System.NonSerializedAttribute>() != null)
                    continue;

                if (field.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                var property = serializedObject.FindProperty(field.Name);

                if (property != null)
                {
                    if (property.isArray)
                    {
                        for (int i = 0; i < property.arraySize; i++)
                        {
                            var tmp = property.GetArrayElementAtIndex(i);
                            var elementType = GetArrayElement(field.FieldType);//.GetGenericArguments()[0];
                            if (elementType == null)
                                break;

                            if (elementType == typeof(GameObject))
                            {
                                tmp.objectReferenceValue = null;
                                continue;
                            }
                            if (elementType.IsSubclassOf(typeof(Component)))
                            {
                                tmp.objectReferenceValue = null;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (field.FieldType == typeof(GameObject))
                        {
                            property.objectReferenceValue = null;
                            continue;
                        }
                        if (field.FieldType.IsSubclassOf(typeof(Component)))
                        {
                            property.objectReferenceValue = null;
                            continue;
                        }
                    }
                }
            }
        }
    }

    public Transform FindChild(Transform transform, string child, bool bForceBind)
    {
        Transform _t = transform.Find(child);
        if (_t == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var tmp = transform.GetChild(i);
                //if (bForceBind == false && tmp.GetComponent<BehaviourData>() != null)
                //    continue;
                _t = FindChild(tmp, child, bForceBind);
                if (_t != null)
                    return _t;
            }
        }
        return _t;
    }

    public Transform GetValue(System.Reflection.FieldInfo field, string key, int index, Transform transform)
    {
        var attr = field.GetCustomAttribute<AutoBindAtlas>();
        if (key.StartsWith("_"))
            return transform;

        if (attr != null && !string.IsNullOrEmpty(attr.Atlas))
        {
            var tmp = index < 0 ? $"${attr.Atlas}" : $"${attr.Atlas}_{index}";
            return FindChild(transform, tmp, false);
        }

        var name = index < 0 ? $"${key}" : $"${key}_{index}";
        return FindChild(transform, name, false);
    }
}
