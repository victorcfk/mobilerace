#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer: PropertyDrawer
{

    private ReadOnlyAttribute _attributeValue = null;
    private ReadOnlyAttribute attributeValue
    {
        get
        {
            if (_attributeValue == null)
            {
                _attributeValue = (ReadOnlyAttribute) attribute;
            }
            return _attributeValue;
        }
    }


    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    
    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif