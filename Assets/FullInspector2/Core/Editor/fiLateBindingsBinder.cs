using UnityEngine;
using UnityEditor;

namespace FullInspector.Internal {

    // note: See the docs on fiLateBindings
    //       This is just the actual injection code which only gets run if we're in an editor
    //
    // note: If there is ever a binding that doesn't occur quickly enough, then we can use
    //       reflection to discover it immediately

    [InitializeOnLoad]
    public class fiLateBindingsBinder {
        static fiLateBindingsBinder() {
            fiLateBindings._Bindings._AssetDatabase_LoadAssetAtPath = AssetDatabase.LoadAssetAtPath;


            fiLateBindings._Bindings._EditorApplication_isPlaying = () => EditorApplication.isPlaying;
            fiLateBindings._Bindings._EditorApplication_AddUpdateAction = a => EditorApplication.update += new EditorApplication.CallbackFunction(a);
            fiLateBindings._Bindings._EditorApplication_RemUpdateAction = a => EditorApplication.update -= new EditorApplication.CallbackFunction(a);
            fiLateBindings._Bindings._EditorApplication_timeSinceStartup = () => EditorApplication.timeSinceStartup;


            fiLateBindings._Bindings._EditorPrefs_GetString = EditorPrefs.GetString;
            fiLateBindings._Bindings._EditorPrefs_SetString = EditorPrefs.SetString;


            fiLateBindings._Bindings._EditorUtility_SetDirty = EditorUtility.SetDirty;
            fiLateBindings._Bindings._EditorUtility_InstanceIdToObject = EditorUtility.InstanceIDToObject;
            fiLateBindings._Bindings._EditorUtility_IsPersistent = EditorUtility.IsPersistent;
            fiLateBindings._Bindings._EditorUtility_CreateGameObjectWithHideFlags = (name, flags) => EditorUtility.CreateGameObjectWithHideFlags(name, flags);


            fiLateBindings._Bindings._EditorGUI_BeginChangeCheck = EditorGUI.BeginChangeCheck;
            fiLateBindings._Bindings._EditorGUI_EndChangeCheck = EditorGUI.EndChangeCheck;
            fiLateBindings._Bindings._EditorGUI_BeginDisabledGroup = EditorGUI.BeginDisabledGroup;
            fiLateBindings._Bindings._EditorGUI_EndDisabledGroup = EditorGUI.EndDisabledGroup;
            fiLateBindings._Bindings._EditorGUI_Foldout = EditorGUI.Foldout;
            fiLateBindings._Bindings._EditorGUI_HelpBox = (rect, message, commentType) => EditorGUI.HelpBox(rect, message, (MessageType)commentType);
            fiLateBindings._Bindings._EditorGUI_IntSlider = EditorGUI.IntSlider;
            fiLateBindings._Bindings._EditorGUI_Popup = EditorGUI.Popup;
            fiLateBindings._Bindings._EditorGUI_Slider = EditorGUI.Slider;


#if UNITY_4_3
            fiLateBindings.EditorGUIUtility.standardVerticalSpacing = 2f;
#else
            fiLateBindings.EditorGUIUtility.standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
#endif
            fiLateBindings.EditorGUIUtility.singleLineHeight = EditorGUIUtility.singleLineHeight;


            fiLateBindings._Bindings._EditorStyles_label = () => EditorStyles.label;
            fiLateBindings._Bindings._EditorStyles_foldout = () => EditorStyles.foldout;


            fiLateBindings._Bindings._fiEditorGUI_PushHierarchyMode = state => fiEditorGUI.PushHierarchyMode(state);
            fiLateBindings._Bindings._fiEditorGUI_PopHierarchyMode = () => fiEditorGUI.PopHierarchyMode();


            fiLateBindings._Bindings._PrefabUtility_CreatePrefab = (string path, GameObject template) => PrefabUtility.CreatePrefab(path, template);
            fiLateBindings._Bindings._PrefabUtility_IsPrefab = unityObj => PrefabUtility.GetPrefabType(unityObj) == PrefabType.Prefab;


            fiLateBindings._Bindings._PropertyEditor_Edit =
                (objType, attrs, rect, label, obj, metadata, skippedEditors) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skippedEditors).Edit(rect, label, obj, metadata);
            fiLateBindings._Bindings._PropertyEditor_GetElementHeight =
                (objType, attrs, label, obj, metadata, skippedEditors) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skippedEditors).GetElementHeight(label, obj, metadata);


            fiLateBindings._Bindings._PropertyEditor_EditSkipUntilNot =
                (skipUntilNot, objType, attrs, rect, label, obj, metadata) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skipUntilNot).Edit(rect, label, obj, metadata);
            fiLateBindings._Bindings._PropertyEditor_GetElementHeightSkipUntilNot =
                (skipUntilNot, objType, attrs, label, obj, metadata) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skipUntilNot).GetElementHeight(label, obj, metadata);


            fiLateBindings._Bindings._Selection_activeObject = () => Selection.activeObject;
        }

        public static void EnsureLoaded() {
            // no-op, but it ensures that the static constructor has executed
        }
    }
}
