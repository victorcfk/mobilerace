using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {


    public class fiGenericPropertyDrawerPropertyEditor<TContainer, T> : PropertyEditor<T>
        where TContainer : fiPropertyDrawerMonoBehaviorContainer<T> {

        public override bool DisplaysStandardLabel {
            get {
                // We have no way of knowing what type of label will be rendered
                return false;
            }
        }

        public class ItemMetadata : IGraphMetadataItemNotPersistent {
            [NonSerialized]
            public fiPropertyDrawerMonoBehaviorContainer<T> Container;
            [NonSerialized]
            public SerializedObject SerializedObject;
            [NonSerialized]
            public SerializedProperty SerializedProperty;
        }

        // see http://answers.unity3d.com/questions/436295/how-to-have-a-gradient-editor-in-an-editor-script.html
        // for the inspiration behind this approach

        T DoPropertyField(ItemMetadata metadata, Rect region, GUIContent label, T item) {
            metadata.Container.Item = item;
            metadata.SerializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(region, metadata.SerializedProperty, label);
            if (EditorGUI.EndChangeCheck()) {
                metadata.SerializedObject.ApplyModifiedProperties();
            }

            return metadata.Container.Item;
        }

        float DoPropertyFieldHeight(ItemMetadata metadata, GUIContent label, T item) {
            metadata.Container.Item = item;
            metadata.SerializedObject.Update();

            return EditorGUI.GetPropertyHeight(metadata.SerializedProperty, label);
        }

        private ItemMetadata GetContainer(fiGraphMetadata metadata) {
            var container = metadata.GetMetadata<ItemMetadata>();

            // note: we delay construction of the GameObject because when the metadata is
            //       deserialized, we don't want to construct a new GameObject in another thread
            //       via the GradientContainer constructor
            if (container.Container == null) {
                // note: using HideFlags.HideAndDontSave includes HideFlags.NotEditable
                var obj = EditorUtility.CreateGameObjectWithHideFlags("Proxy editor for " + typeof(T).CSharpName(),
                    HideFlags.HideInHierarchy | HideFlags.DontSave);

                container.Container = obj.AddComponent<TContainer>();

                container.SerializedObject = new SerializedObject(container.Container);
                container.SerializedProperty = container.SerializedObject.FindProperty("Item");
            }

            return container;
        }

        public override T Edit(Rect region, GUIContent label, T element, fiGraphMetadata metadata) {
            var container = GetContainer(metadata);
            return DoPropertyField(container, region, label, element);
        }

        public override float GetElementHeight(GUIContent label, T element, fiGraphMetadata metadata) {
            var container = GetContainer(metadata);
            return DoPropertyFieldHeight(container, label, element);
        }
    }

    public class fiGenericPropertyDrawerPropertyEditorManager {
        /// <summary>
        /// An editor that is displayed while we are generating the PropertyDrawer bindings.
        /// </summary>
        private class TemporaryEditor : IPropertyEditor, IPropertyEditorEditAPI {
            public PropertyEditorChain EditorChain {
                get;
                set;
            }

            public bool CanEdit(Type dataType) {
                return true;
            }

            public TemporaryEditor(Type type) {
                _message = "The PropertyDrawer bindings for type " + type.CSharpName() +
                    " are being generated. The editor will be displayed when Unity has finished recompiling.";
            }

            private readonly string _message;

            public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
                EditorGUI.HelpBox(region, _message, MessageType.Info);
                return element;
            }

            public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
                return fiCommentUtility.GetCommentHeight(_message, CommentType.Info);
            }

            public GUIContent GetFoldoutHeader(GUIContent label, object element) {
                return label;
            }

            public object OnSceneGUI(object element) {
                return element;
            }

            public bool DisplaysStandardLabel {
                get { return true; }
            }
        }


        public struct PropertyDrawerContainer {
            public bool IsInherited;
            public Type PropertyType;

            public bool IsApplicableTo(Type propertyType) {
                if (IsInherited) return PropertyType.IsAssignableFrom(propertyType);
                return propertyType == PropertyType;
            }

            public override string ToString() {
                return string.Format("IsInherited: {0}, PropertyType: {1}", IsInherited, PropertyType.CSharpName());
            }
        }

        private static readonly PropertyDrawerContainer[] _propertyDrawers;

        static fiGenericPropertyDrawerPropertyEditorManager() {
            _propertyDrawers =
                (from assembly in fiRuntimeReflectionUtility.GetAllEditorAssemblies()
                 from type in assembly.GetTypes()

                 let attrs = type.GetCustomAttributes(typeof(CustomPropertyDrawer), true)
                 where attrs != null

                 // Do not generate bindings for various the PropertyDrawer binders
                 where type != typeof(fiInspectorOnly_PropertyDrawer)
                 where type != typeof(fiValuePropertyDrawer)

                 from CustomPropertyDrawer attr in attrs

                 select new PropertyDrawerContainer
                 {
                     // Unity renamed these fields after 4.3
                     IsInherited = fiRuntimeReflectionUtility.ReadFields<CustomPropertyDrawer, bool>(attr, "m_UseForChildren", "useForChildren"),
                     PropertyType = fiRuntimeReflectionUtility.ReadFields<CustomPropertyDrawer, Type>(attr, "m_Type", "Type")
                 }).ToArray();
        }

        private static bool HasPropertyDrawer(Type type) {
            return _propertyDrawers.Any(drawer => drawer.IsApplicableTo(type));
        }

        private static void GenerateScripts(Type type) {
            string rawTypeName = type.CSharpName(/*namespace*/true).Replace('<', '_').Replace('>', '_').Replace(',', '_');

            string typeClassName = rawTypeName;
            if (type.IsGenericType) typeClassName = "Generated_" + typeClassName.Replace('.', '_') + "_NoGenerics";
            string behaviorClassName = "Generated_" + rawTypeName.Replace('.', '_') + "_MonoBehaviourStorage";
            string editorClassName = "Generated_" + rawTypeName.Replace('.', '_') + "_PropertyEditor";

            var runtimeOutput = "";
            runtimeOutput += "using System;" + Environment.NewLine;
            runtimeOutput += "using FullInspector.Internal;" + Environment.NewLine;
            runtimeOutput += "using UnityEngine;" + Environment.NewLine;
            runtimeOutput += Environment.NewLine;
            runtimeOutput += "namespace FullInspector.Generated {" + Environment.NewLine;
            if (type.IsGenericType) {
                runtimeOutput += "    [Serializable]" + Environment.NewLine;
                runtimeOutput += "    public class " + typeClassName + " : " + type.CSharpName(/*namespace*/true) + " {} " + Environment.NewLine;
            }
            runtimeOutput += "    [AddComponentMenu(\"\")]" + Environment.NewLine;
            runtimeOutput += "    public class " + behaviorClassName + " : fiPropertyDrawerMonoBehaviorContainer<" + typeClassName + "> {} " + Environment.NewLine;
            runtimeOutput += "}" + Environment.NewLine;

            var editorOutput = "";
            editorOutput += "using System;" + Environment.NewLine;
            editorOutput += "using FullInspector.Internal;" + Environment.NewLine;
            editorOutput += Environment.NewLine;
            editorOutput += "namespace FullInspector.Generated {" + Environment.NewLine;
            editorOutput += "    [CustomPropertyEditor(typeof(" + type.CSharpName(/*namespace*/true) + "))]" + Environment.NewLine;
            editorOutput += "    public class " + editorClassName + " : fiGenericPropertyDrawerPropertyEditor<" + behaviorClassName + ", " + typeClassName + "> {" + Environment.NewLine;
            editorOutput += "        public override bool CanEdit(Type type) {" + Environment.NewLine;
            editorOutput += "            return typeof(" + type.CSharpName(/*namespace*/true) + ").IsAssignableFrom(type);" + Environment.NewLine;
            editorOutput += "        }" + Environment.NewLine;
            editorOutput += "    }" + Environment.NewLine;
            editorOutput += "}" + Environment.NewLine;


            Directory.CreateDirectory(fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "PropertyDrawerIntegration"));
            Directory.CreateDirectory(fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "PropertyDrawerIntegration", "Editor"));

            string runtimePath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "PropertyDrawerIntegration", behaviorClassName + ".cs");
            string editorPath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "PropertyDrawerIntegration", "Editor", editorClassName + ".cs");

            File.WriteAllText(runtimePath, runtimeOutput);
            File.WriteAllText(editorPath, editorOutput);

            AssetDatabase.Refresh();
        }

        public static IPropertyEditor TryCreate(Type type) {
            if (HasPropertyDrawer(type)) {
                // Don't recreate the same editor multiple times if we're called twice in a row.
                if (_created.Add(type) == false) return new TemporaryEditor(type);

                Debug.Log("Generating PropertyDrawer integration bindings for " + type.CSharpName() + ". Please wait for the recompile to finish.");
                GenerateScripts(type);
                return new TemporaryEditor(type);
            }

            return null;
        }

        // A set of types that we are currently generating editors for. This is used so that
        // we do not accidently recreate the same editor twice.
        private static readonly HashSet<Type> _created = new HashSet<Type>();
    }
}