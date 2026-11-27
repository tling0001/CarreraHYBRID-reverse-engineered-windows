#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarreraHybrid.EditorTools
{
    /// <summary>
    /// Read-only inventory tool for integrating recovered Android assets.
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
            var sceneDetails = new List<string>();
            var typeDetails = new Dictionary<string, List<string>>();

            foreach (string path in paths)
            {
                string extension = Path.GetExtension(path);
                if (!counts.ContainsKey(extension)) counts[extension] = 0;
                counts[extension]++;

                string lower = path.ToLowerInvariant();
                if (Keywords.Any(k => lower.Contains(k))) keywordMatches.Add(path);

                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null) continue;

                string typeName = asset.GetType().Name;
                if (!typeDetails.TryGetValue(typeName, out List<string> list))
                {
                    list = new List<string>();
                    typeDetails[typeName] = list;
                }
                list.Add(path);

                if (asset is SceneAsset)
                {
                    Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    foreach (GameObject root in scene.GetRootGameObjects())
                    {
                        string components = string.Join(", ", root
                            .GetComponentsInChildren<Component>(true)
                            .Where(c => c != null)
                            .Select(c => c.GetType().Name)
                            .Distinct()
                            .OrderBy(n => n, StringComparer.Ordinal));
                        sceneDetails.Add("- " + path + " / " + root.name + ": " + components);
                    }
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("# Carrera Hybrid imported asset inventory");
            sb.AppendLine();
            sb.AppendLine("Generated: " + DateTime.UtcNow.ToString("u"));
            sb.AppendLine();
            sb.AppendLine("Read-only inventory of assets currently imported into this Unity project.");
            sb.AppendLine("This report does not by itself prove that an asset is the original Android asset.");
            sb.AppendLine();
            sb.AppendLine("## Asset totals");
            sb.AppendLine();
            sb.AppendLine("| Extension | Count |");
            sb.AppendLine("|---|---:|");
            foreach (var pair in counts)
                sb.AppendLine("| " + (string.IsNullOrEmpty(pair.Key) ? "(no extension)" : pair.Key) + " | " + pair.Value + " |");

            sb.AppendLine();
            sb.AppendLine("## Key Unity asset types");
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
                foreach (string path in list.Take(250)) sb.AppendLine("- " + path);
                if (list.Count > 250) sb.AppendLine("- " + (list.Count - 250) + " more omitted");
                sb.AppendLine();
            }

            sb.AppendLine("## Filename and path keyword matches");
            sb.AppendLine();
            foreach (string path in keywordMatches.Take(1000)) sb.AppendLine("- " + path);
            if (keywordMatches.Count > 1000) sb.AppendLine("- " + (keywordMatches.Count - 1000) + " more omitted");

            sb.AppendLine();
            sb.AppendLine("## Scene hierarchy summary");
            sb.AppendLine();
            if (sceneDetails.Count == 0) sb.AppendLine("No imported scenes were found.");
            else foreach (string line in sceneDetails) sb.AppendLine(line);

            sb.AppendLine();
            sb.AppendLine("## Next manual checks");
            sb.AppendLine();
            sb.AppendLine("- Map imported scenes to Startup, Login, Menu, Lobby, Race, and Race Result.");
            sb.AppendLine("- Check car and track meshes for materials, textures, shaders, animation controllers, and missing scripts.");
            sb.AppendLine("- Compare UI sprites, fonts, layouts, and text against Android screenshots before replacing approximations.");
            sb.AppendLine("- Preserve imported originals; make Windows shader/material fixes in duplicated assets.");

            File.WriteAllText(outputPath, sb.ToString(), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            EditorUtility.RevealInFinder(outputPath);
            Debug.Log("Carrera Hybrid asset inventory written to: " + outputPath);
        }
    }
}
#endif
