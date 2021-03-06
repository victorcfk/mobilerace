using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FullInspector;
using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using tk = FullInspector.tk<FullInspector.Internal.fiEditorImage>;

namespace FullInspector.Internal {
    public static class fiImageUtility {
        /// <summary>
        /// Read width and height of PNG file in pixels.
        /// </summary>
        private static void GetImageSize(byte[] imageData, out int width, out int height) {
            width = ReadInt(imageData, 3 + 15);
            height = ReadInt(imageData, 3 + 15 + 2 + 2);
        }

        private static int ReadInt(byte[] imageData, int offset) {
            return (imageData[offset] << 8) | imageData[offset + 1];
        }

        public static string Encode(Texture2D texture) {
            if (texture.format != TextureFormat.RGBA32) {
                var updated = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
                updated.SetPixels(texture.GetPixels());
                texture = updated;
            }

            byte[] data = texture.EncodeToPNG();
            return Convert.ToBase64String(data);
        }

        public static Texture2D Decode(string encoded) {
            // Get image data (PNG) from base64 encoded strings.
            byte[] imageData = Convert.FromBase64String(encoded);

            // Gather image size from image data.
            int texWidth, texHeight;
            GetImageSize(imageData, out texWidth, out texHeight);

            // Generate texture asset.
            var tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, /*linear:*/ true);
            tex.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            tex.name = "(Generated)";
            tex.filterMode = FilterMode.Point;
            tex.LoadImage(imageData);

            return tex;
        }
    }

    public class fiEditorImage : fiInspectorOnly {
        public readonly string EncodedLightSkin;
        public readonly string EncodedDarkSkin;

        public fiEditorImage(string lightSkin, string darkSkin) {
            EncodedLightSkin = lightSkin;
            EncodedDarkSkin = darkSkin;
        }

        public Texture2D Decoded {
            get {
                if (_decoded == null) {
                    string encodedTexture = EditorGUIUtility.isProSkin ? EncodedDarkSkin : EncodedLightSkin;
                    _decoded = fiImageUtility.Decode(encodedTexture);
                }
                return _decoded;
            }
        }
        private Texture2D _decoded;
    }

    public class fiEditorImagesManagerWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Developer/Editor Images Manager")]
        public static void ShowWindow() {
            var window = GetWindow<fiEditorImagesManagerWindow>();
            window.title = "Editor Images";
        }

        private class DiscoveredResource {
            public string Name;
            public Texture2D LightSkin;
            public Texture2D DarkSkin;
        }

        public void OnEnable() {
            _resources = new HashSet<DiscoveredResource>(GetResources());
            fiEditorUtility.RepaintableEditorWindows.Add(this);
        }

        private static IEnumerable<DiscoveredResource> GetResources() {
            foreach (var field in typeof(fiEditorImages).GetDeclaredFields()) {
                if (typeof(fiEditorImage).IsAssignableFrom(field.FieldType) == false) continue;

                yield return new DiscoveredResource
                {
                    Name = field.Name,
                    LightSkin = fiImageUtility.Decode(((fiEditorImage)field.GetValue(null)).EncodedLightSkin),
                    DarkSkin = fiImageUtility.Decode(((fiEditorImage)field.GetValue(null)).EncodedDarkSkin),
                };
            }
        }

        private readonly fiGraphMetadata _metadata = new fiGraphMetadata();
        private HashSet<DiscoveredResource> _resources = new HashSet<DiscoveredResource>();

        public void OnGUI() {
            PropertyEditor.Get(_resources.GetType(), null)
                .FirstEditor
                .EditWithGUILayout(new GUIContent("Resources"), _resources, _metadata.Enter("Resources"));

            if (GUILayout.Button("Write To Disk")) {
                const string outputPath = "Assets/FullInspector2/Modules/EditorImagesManager/Editor/fiEditorImages.cs";
                File.WriteAllText(outputPath, BuildFile(_resources));
                AssetDatabase.Refresh();
            }
        }

        private static StringBuilder Indent(StringBuilder o, int count) {
            while (count > 0) {
                --count;
                o.Append("    ");
            }
            return o;
        }

        private static string BuildFile(HashSet<DiscoveredResource> resources) {
            var o = new StringBuilder();

            Indent(o, 0).Append("// WARNING: THIS FILE IS AUTOMATICALLY GENERATED").AppendLine();
            Indent(o, 0).Append("// Please use the Editor Images Manager to edit it").AppendLine();
            Indent(o, 0).AppendLine();
            Indent(o, 0).Append("using FullInspector.Internal;").AppendLine();
            Indent(o, 0).Append("").AppendLine();
            Indent(o, 0).Append("public static class fiEditorImages {").AppendLine();

            var indentLine = false;
            foreach (var resource in resources) {
                if (indentLine) o.AppendLine();
                indentLine = true;

                Indent(o, 1).Append("public static fiEditorImage " + resource.Name + " = new fiEditorImage(").AppendLine();
                Indent(o, 2).Append("/* light */ \"" + fiImageUtility.Encode(resource.LightSkin) + "\",").AppendLine();
                Indent(o, 2).Append("/* dark  */ \"" + fiImageUtility.Encode(resource.DarkSkin) + "\");").AppendLine();
            }
            Indent(o, 0).Append("}").AppendLine();

            return o.ToString();
        }
    }
}