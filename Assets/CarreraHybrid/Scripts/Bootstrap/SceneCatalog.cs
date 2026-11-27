using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace CarreraHybrid.Bootstrap
{
    public static class SceneCatalog
    {
        private static readonly IReadOnlyDictionary<AppScene, string> Names =
            new Dictionary<AppScene, string>
            {
                [AppScene.Startup] = "Startup",
                [AppScene.Login] = "Login",
                [AppScene.Menu] = "Menu",
                [AppScene.Lobby] = "Lobby",
                [AppScene.Race] = "Race",
                [AppScene.RaceResult] = "Race Result"
            };

        public static string GetName(AppScene scene)
        {
            if (!Names.TryGetValue(scene, out var name))
                throw new ArgumentOutOfRangeException(nameof(scene), scene, "Unknown application scene.");
            return name;
        }

        public static bool IsIncluded(AppScene scene)
        {
            var name = GetName(scene);
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                if (path.EndsWith("/" + name + ".unity", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static void RequireIncluded(AppScene scene)
        {
            if (!IsIncluded(scene))
                throw new InvalidOperationException(
                    $"Required Carrera Hybrid scene '{GetName(scene)}' is not present in Build Settings. " +
                    "Do not substitute a placeholder scene; export and integrate the original Android scene first.");
        }
    }
}
