#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CarreraHybrid.EditorTools
{
    /// <summary>
    /// Read-only report of recovered Unity assets and scene hierarchies.
    /// </summary>
    public static class CarreraAssetRecoveryAudit
    {
        private static readonly string[] Keywords =
        {
            "car", "vehicle", "racer", "track", "circuit", "race", "garage",
            "logo", "brand", "font", "ui", "menu", "login", "button",
            "background", "hud", "dashboard", "wheel", "tire", "engine"
        };

        [MenuItem("Carrera Hybrid/Recovery/Generate Asset Inventory")]
        public static void GenerateInventory()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outputDirectory = Path.Combine(projectRoot, "Documentation");
            Directory.CreateDirectory(outputDirectory);
            string outputPath = Path.Combine(outputDirectory, "Asset-Recovery-Inventory.md");

            string[] paths = AssetDatabase.GetAllAssetPaths()
                .Where(p => p.StartsWith("Assets/", StringComparison.Ordinal))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var counts = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var keywordMatches = new List<string>();
            var typeDetails = new Dictionary<string, List<string>>();
            var scenes = paths.Where(p => p.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string path in paths)
            {
                string extension = Path.GetExtension(path);
                if (!counts.ContainsKey(extension)) counts[extension] = 0;
                counts[extension]++;

                if (Keywords.Any(k => path.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                    keywordMatches.Add(path);

                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null) continue;

                string typeName = asset.GetType().Name;
                if (!typeDetails.TryGetValue(typeName, out List<string> list))
                {
                    list = new List<string>();
                    typeDetails[typeName] = list;
                }
                list.Add(path);
            }

            var sb = new StringBuilder();
            sb.AppendLine("# Carrera Hybrid imported asset inventory");
            sb.AppendLine();
            sb.AppendLine("Generated: " + DateTime.UtcNow.ToString("u"));
            sb.AppendLine();
            sb.AppendLine("Read-only inventory of the imported Unity project. Review this report before replacing reconstructed screens.");
            sb.AppendLine();
            sb.AppendLine("## Asset totals");
            sb.AppendLine();
            sb.AppendLine("| Extension | Count |");
            sb.AppendLine("|---|---:|");
            foreach (var pair in counts)
                sb.AppendLine("| " + (string.IsNullOrEmpty(pair.Key) ? "(no extension)" : pair.Key) + " | " + pair.Value + " |");

            sb.AppendLine();
            sb.AppendLine("## Important assets");
            sb.AppendLine();
            string[] importantTypes =
            {
                "Texture2D", "Sprite", "Mesh", "Material", "Font", "TMP_FontAsset",
                "AnimationClip", "AnimatorController", "Prefab", "SceneAsset",
                "AudioClip", "Shader", "ScriptableObject"
            };
            foreach (string type in importantTypes)
            {
                if (!typeDetails.TryGetValue(type, out List<string> list)) continue;
                sb.AppendLine("### " + type + " (" + list.Count + ")");
                foreach (string path in list.Take(500)) sb.AppendLine("- " + path);
                if (list.Count > 500) sb.AppendLine("- " + (list.Count - 500) + " more omitted");
                sb.AppendLine();
            }

            sb.AppendLine("## Likely car, track, and UI assets");
            sb.AppendLine();
            foreach (string path in keywordMatches.Take(2000)) sb.AppendLine("- " + path);
            if (keywordMatches.Count > 2000) sb.AppendLine("- " + (keywordMatches.Count - 2000) + " more omitted");

            sb.AppendLine();
            sb.AppendLine("## Recovered scene hierarchy and UI references");
            sb.AppendLine();
            if (scenes.Length == 0)
            {
                sb.AppendLine("No Unity scene assets were imported. Confirm that the AssetRipper export included its scene files.");
            }
            else
            {
                foreach (string path in scenes)
                {
                    Scene scene = default;
                    bool opened = false;
                    try
                    {
                        scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                        opened = true;
                        sb.AppendLine("### " + path);
                        foreach (GameObject root in scene.GetRootGameObjects())
                            AppendHierarchy(sb, root, 0);
                        sb.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("- Could not inspect scene: " + ex.Message);
                    }
                    finally
                    {
                        if (opened && scene.IsValid())
                            EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }

            sb.AppendLine("## Next checks");
            sb.AppendLine();
            sb.AppendLine("- Identify the original Startup and Login canvases, their panels, buttons, text, fonts, sprites, animations, and navigation.");
            sb.AppendLine("- Verify the Menu and Garage car prefabs, meshes, materials, textures, and camera setup.");
            sb.AppendLine("- Trace Lobby track/car selection references and the Race scene's vehicle, track, camera, HUD, and physics components.");
            sb.AppendLine("- Check missing scripts and shaders before attempting to run an exported scene.");
            sb.AppendLine("- Preserve imported source assets. Apply Windows-specific fixes to duplicated assets.");

            File.WriteAllText(outputPath, sb.ToString(), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            EditorUtility.RevealInFinder(outputPath);
            Debug.Log("Carrera Hybrid asset inventory written to: " + outputPath);
        }

        private static void AppendHierarchy(StringBuilder sb, GameObject gameObject, int depth)
        {
            string indent = new string(' ', depth * 2);
            int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
            sb.Append(indent).Append("- **").Append(gameObject.name).Append("**");
            if (missing > 0) sb.Append(" [missing scripts: ").Append(missing).Append("]");
            sb.AppendLine();

            foreach (Component component in gameObject.GetComponents<Component>())
            {
                if (component == null) continue;

                if (component is Canvas canvas)
                    sb.Append(indent).Append("  - Canvas: ").Append(canvas.renderMode).AppendLine();
                else if (component is Image image)
                    sb.Append(indent).Append("  - Image sprite: ").Append(AssetPath(image.sprite)).AppendLine();
                else if (component is SpriteRenderer spriteRenderer)
                    sb.Append(indent).Append("  - SpriteRenderer: ").Append(AssetPath(spriteRenderer.sprite)).AppendLine();
                else if (component is TMP_Text tmp)
                    sb.Append(indent).Append("  - TMP text: ").Append(SingleLine(tmp.text))
                        .Append(" | font: ").Append(AssetPath(tmp.font)).AppendLine();
                else if (component is Text text)
                    sb.Append(indent).Append("  - UI Text: ").Append(SingleLine(text.text))
                        .Append(" | font: ").Append(AssetPath(text.font)).AppendLine();
                else if (component is Animator animator)
                    sb.Append(indent).Append("  - Animator: ").Append(AssetPath(animator.runtimeAnimatorController)).AppendLine();
                else if (component is Button button)
                    sb.Append(indent).Append("  - Button interactable: ").Append(button.interactable).AppendLine();
                else if (component is TMP_InputField)
                    sb.Append(indent).AppendLine("  - TMP Input Field");
                else if (component is InputField)
                    sb.Append(indent).AppendLine("  - Input Field");
                else if (!(component is Transform))
                    sb.Append(indent).Append("  - Component: ").Append(component.GetType().FullName).AppendLine();
            }

            foreach (Transform child in gameObject.transform)
                AppendHierarchy(sb, child.gameObject, depth + 1);
        }

        private static string AssetPath(UnityEngine.Object asset)
        {
            return asset == null ? "(none)" : AssetDatabase.GetAssetPath(asset);
        }

        private static string SingleLine(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("\r", " ").Replace("\n", " ").Replace("|", "/");
        }
    }
}
#endif
