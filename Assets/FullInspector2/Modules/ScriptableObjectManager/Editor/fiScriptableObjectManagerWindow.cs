using System;
using System.IO;
using UnityEngine;
using System.Linq;
using FullInspector.Internal;
using UnityEditor;
using FullInspector;
using tk = FullInspector.tk<fiScriptableObjectManagerWindow, FullInspector.tkEmptyContext>;

/// <summary>
/// Provides a nice interface for interacting with and managing scriptable object instances
/// </summary>
public class fiScriptableObjectManagerWindow : EditorWindow {

    [MenuItem("Window/Full Inspector/Scriptable Object Manager &o")]
    public static void ShowWindow() {
        var window = EditorWindow.GetWindow<fiScriptableObjectManagerWindow>();

        window.title = "Scriptable Object Manager";
        window.position = new Rect(window.position.x, window.position.y, 600, 315);

        window.minSize = new Vector2(0, 0);
        window.maxSize = new Vector2(10000, 10000);
    }

    private const string PathPreferencesKey = "FullInspector_ScriptableObjectCreatorWindow_Path";

    private static void CreateNewScriptableObject(Type instanceType) {
        string assetPath =
            EditorUtility.SaveFilePanelInProject("Select Path (" + instanceType.CSharpName() + ")",
                Guid.NewGuid().ToString(), "asset", "", EditorPrefs.GetString(PathPreferencesKey, "Assets"));

        if (string.IsNullOrEmpty(assetPath) == false) {
            EditorPrefs.SetString(PathPreferencesKey, Path.GetDirectoryName(assetPath));

            ScriptableObject asset = ScriptableObject.CreateInstance(instanceType);
            AssetDatabase.CreateAsset(asset, assetPath);
        }
    }

    private int _index;
    private Type[] _types;
    private GUIContent[] _labels;

    public void OnEnable() {
        _types =
            (from type in fiRuntimeReflectionUtility.AllSimpleCreatableTypesDerivingFrom(typeof(ScriptableObject))
             where type.Assembly.FullName.Contains("UnityEngine") == false
             where type.Assembly.FullName.Contains("UnityEditor") == false
             select type).ToArray();

        _labels = _types.Select(t => new GUIContent(t.FullName)).ToArray();
        _index = 0;
    }

    private tkControlEditor Editor = new tkControlEditor(
        new tk.VerticalGroup {
            new tk.Empty(5),

            new tk.HorizontalGroup {
                {
                    150,
                    new tk.Label("ScriptableObject Type", FontStyle.Bold)
                },

                15,

                {
                    true,
                    new tk.CenterVertical(new tk.Popup(fiGUIContent.Empty,
                        /* get options */ tk.Val(o => o._labels),                   
                        /* get index */ tk.Val(o => o._index),                   
                        /* set index */
                        (o, c, v) => {
                            o._index = v;
                            return o;
                        }))
                },
                    
                15,

                {
                    65,
                    new tk.Button("Create",
                        (o, c) => CreateNewScriptableObject(o._types[o._index])) {
                            Style = new tk.EnabledIf(o => o._types.Length > 0)
                        }
                }
            },

            tk.PropertyEditor.Create("Instances",
                new fiAttributeProvider(new InspectorCollectionRotorzFlagsAttribute {
                    HideRemoveButtons = true,
                    HideAddButton = true,
                    DisableReordering = true
                }),
                (o, c) => fiEditorUtility.GetAllAssetsOfType(o._types[o._index]),
                (o, c, v) => {}),
        }
    );

    public void OnGUI() {
        EditorGUIUtility.hierarchyMode = true;

        var metadata = fiPersistentMetadata.GetMetadataFor(this);
        float height = fiEditorGUI.tkControlHeight(this, metadata, Editor);

        var rect = EditorGUILayout.GetControlRect(false, height);
        fiEditorGUI.tkControl(rect, this, metadata, Editor);

        if (fiEditorUtility.ShouldInspectorRedraw.Enabled) {
            Repaint();
        }

        EditorGUIUtility.hierarchyMode = false;
    }
}
