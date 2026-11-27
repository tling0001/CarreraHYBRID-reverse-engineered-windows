using System;
using UnityEngine;
using CarreraHybrid.UI;

namespace CarreraHybrid.Bootstrap
{
    /// <summary>
    /// Windows player entry point. This installs the reconstructed screen flow while original scene assets are being recovered.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class StartupBootstrap : MonoBehaviour
    {
        private static StartupBootstrap instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (instance != null) return;
            var root = new GameObject("CarreraHybrid.Bootstrap");
            DontDestroyOnLoad(root);
            instance = root.AddComponent<StartupBootstrap>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Debug.Log($"[CarreraHybrid] Windows bootstrap started. Unity {Application.unityVersion}; " +
                      $"platform {Application.platform}; app version {Application.version}.");
            gameObject.AddComponent<CarreraScreenFlow>();
            ValidateRequiredRuntime();
        }

        private static void ValidateRequiredRuntime()
        {
            // These checks are diagnostic only until the original scene export is available.
            // They never redirect users into invented or incomplete replacement screens.
            foreach (AppScene scene in Enum.GetValues(typeof(AppScene)))
            {
                if (!SceneCatalog.IsIncluded(scene))
                    Debug.LogWarning($"[CarreraHybrid] Original serialized scene not yet imported (UI approximation is active): {SceneCatalog.GetName(scene)}");
            }
        }
    }
}
