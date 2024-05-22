using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CustomEditor(typeof(CreateMultiTiling))]
    class MultiTilingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CreateMultiTiling tilingScript = (CreateMultiTiling)target;
            if (GUILayout.Button("Build Tiling"))
            {
                string parentName = tilingScript.tilingName;
                var parent = GameObject.Find(parentName);
                if (parent != null)
                {
                    GameObject.DestroyImmediate(parent);
                }
                parent = new GameObject(parentName);
                var parentTransform = parent.transform;
                var task = tilingScript.Build(parentTransform);
                //await task; // Requires the signature to change. Seems to give the same result as the ContinueWith.
                try
                {
                    task.ContinueWith(t =>
                    {
                        // I think this doesn't work as it is on the wrong thread. Although I would expect
                        // Repaint to marshal over to the correct thread.
                        this.Repaint();
                        SceneView.RepaintAll();
                        EditorWindow view = EditorWindow.GetWindow<SceneView>();
                        view.Repaint();
                        view.Focus();
                    });
                    //task.RunSynchronously(); // Want to avoid this as it will block the UI thread.
                    //RunAsync(task);
                    // These will not affect anything as they will be called right away.
                    SceneView.RepaintAll();
                    EditorWindow view = EditorWindow.GetWindow<SceneView>();
                    view.Repaint();
                    view.Focus();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogException(ex.InnerException);
                }
                //System.Threading.Tasks.Task task = tilingScript.Build(parent.transform);
                //task.RunSynchronously();
            }
        }
        private void RunAsync(Task task)
        {
            task.ContinueWith(t =>
            {
                Debug.LogError($"Unexpected Error {t.Exception}");

            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}