using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZombieEscape.Editor
{
    public static class PrototypeBuilder
    {
        private const string ScenePath = "Assets/Scenes/ZombieEscapePrototype.unity";

        [MenuItem("Zombie Escape/Generate Prototype Scene")]
        public static void GenerateScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject bootstrap = new GameObject("Zombie Escape Prototype");
            bootstrap.AddComponent<PrototypeBootstrap>();

            Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            PlayerSettings.companyName = "ITCS 4236 Team";
            PlayerSettings.productName = "Zombie Escape Prototype";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            AssetDatabase.SaveAssets();
            Debug.Log("Zombie Escape prototype scene generated successfully.");
        }

        public static void BuildProject()
        {
            GenerateScene();
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        public static void BuildMacDemo()
        {
            GenerateScene();
            Directory.CreateDirectory("Builds/Mac");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Builds/Mac/Zombie Escape.app",
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            bool succeeded = report.summary.result == BuildResult.Succeeded;
            Debug.Log($"Mac build result: {report.summary.result}; size: {report.summary.totalSize} bytes.");

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(succeeded ? 0 : 1);
            }
        }
    }
}
