using System;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// A few APIs to forcibly call SaveState and RestoreState on every type that implements
    /// ISerializedObject. This API should no longer have any use as of Unity 4.5, because of
    /// serialization callbacks.
    /// </summary>
    public static class fiSaveManager {
        /// <summary>
        /// Forcibly save the state of all objects which derive from ISerializedObject.
        /// ISerializedObject saving is managed automatically when you use the editor (and can be
        /// customized in fiSettings). However, if you're playing a game and make a save
        /// level, ensure to call FullInspectorSaveManager.SaveAll()
        /// </summary>
        [MenuItem("Window/Full Inspector/Developer/Save All", priority = 0)]
        public static void SaveAll() {
            foreach (Type serializedObjectType in
                fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(ISerializedObject))) {

                if (typeof(UnityObject).IsAssignableFrom(serializedObjectType) == false) continue;

                UnityObject[] objects = UnityObject.FindObjectsOfType(serializedObjectType);
                for (int i = 0; i < objects.Length; ++i) {
                    var obj = (ISerializedObject)objects[i];
                    obj.SaveState();
                }
            }
        }

        /// <summary>
        /// Forcibly restore the state of all objects which derive from ISerializedObject.
        /// </summary>
        [MenuItem("Window/Full Inspector/Developer/Restore All", priority = 1)]
        public static void RestoreAll() {
            foreach (Type serializedObjectType in
                fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(ISerializedObject))) {

                if (typeof(UnityObject).IsAssignableFrom(serializedObjectType) == false) continue;

                UnityObject[] objects = UnityObject.FindObjectsOfType(serializedObjectType);
                for (int i = 0; i < objects.Length; ++i) {
                    var obj = (ISerializedObject)objects[i];
                    obj.RestoreState();
                }
            }
        }

        [MenuItem("Window/Full Inspector/Developer/Remove Metadata", priority = 2)]
        public static void RemoveMetadata() {
            fiUtility.DestroyObject(fiPersistentEditorStorage.SceneStorage);
            fiUtility.DestroyObject(fiPersistentEditorStorage.PrefabStorage);
        }

        /// <summary>
        /// This will clean all of the FI data from the scene.
        /// </summary>
        [MenuItem("Window/Full Inspector/Developer/Danger/Remove All Serialized Data", priority = 3)]
        public static void RemoveAllSerializedData() {
            foreach (Type serializedObjectType in
                fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(ISerializedObject))) {

                if (typeof (UnityObject).IsAssignableFrom(serializedObjectType) == false) continue;

                UnityObject[] objects = UnityObject.FindObjectsOfType(serializedObjectType);
                for (int i = 0; i < objects.Length; ++i) {
                    var obj = (ISerializedObject)objects[i];
                    obj.SerializedStateKeys = new List<string>();
                    obj.SerializedStateValues = new List<string>();
                    obj.SerializedObjectReferences = new List<UnityObject>();
                    obj.SaveState();
                }
            }

            RemoveMetadata();
        }
    }
}