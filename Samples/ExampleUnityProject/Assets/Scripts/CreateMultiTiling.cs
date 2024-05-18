using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CrawfisSoftware.AssetManagement;
using CrawfisSoftware.Collections.Maze;
using CrawfisSoftware.Tiling;
using CrawfisSoftware.Tiling.TileSets;
using CrawfisSoftware.Tiling.TilingBuilders;

using UnityEditor;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling

{
    public class CreateMultiTiling : MonoBehaviour
    {
        //[SerializeField]
        //private List<UnityTileSetBuilderScriptables> tileSetBuilders = new List<UnityTileSetBuilderScriptables>(); // set in the Unity Editor
        [SerializeField]
        public string tilingName = "Tiling";
        [SerializeField] MazeProvider _mazeProvider;
        [SerializeField] private List<TileSetUnityScriptable> _tileSetUnityScriptable = new(0);

        private ITiling2D tiling;
        private float tileWidth;
        private float tileHeight;

        //public GTMY.Graph.MultilanePath Path { get; private set; }

        public async Task Build(Transform topLevelParent)
        {
            //tileSetBuilders[0].CreateTileSet();
            Debug.Log("Waiting for tiles to load.");
            await _tileSetUnityScriptable[0].Initialize();
            tileWidth = _tileSetUnityScriptable[0].Width;
            tileHeight = _tileSetUnityScriptable[0].Height;
            for (int i = 1; i < _tileSetUnityScriptable.Count; i++)
            {
                var tileSetBuilder = _tileSetUnityScriptable[i];
                await tileSetBuilder.Initialize();
                if (tileWidth != tileSetBuilder.Width || tileHeight != tileSetBuilder.Height)
                    throw new ArgumentException("TileSets must having matching width and heights.");
            }
            //var maze = BuildPath();
            _mazeProvider.Build();
            var maze = _mazeProvider.Maze;
            int tileSetIndex = 1;
            Debug.Log("All tilesets are loaded. Creating the tiling.");
            foreach (var tileSet in _tileSetUnityScriptable)
            {
                GameObject emptyGO = new GameObject("Tiling" + tileSetIndex.ToString());
                emptyGO.transform.SetParent(topLevelParent, false);
                tiling = CreateTilingFromMaze(tileSet, maze, tileSet.DefaultTile);
                await InstantiateTiles(tileWidth, tileHeight, emptyGO);
                tileSetIndex++;
            }
            AssetDatabase.Refresh();
            SceneView.RepaintAll();
            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
            view.Focus();
        }

        private async Task InstantiateTiles(float tileWidth, float tileHeight, GameObject parent)
        {
            int sortOrder = tiling.Height * tiling.Width;
            List<Task> tasks = new(sortOrder);
            for (int j = 0; j < tiling.Height; j++)
            {
                for (int i = 0; i < tiling.Width; i++)
                {
                    ITile2D tile = tiling.Tile(i, j);
                    IUnityTile unityTile = tile as IUnityTile;
                    if (unityTile == null) continue;
                    var task = CreateTileAsync(tileWidth, tileHeight, parent, sortOrder--, j, i, unityTile);
                    if (task != null || !task.IsCompleted)
                    {
                        tasks.Add(task);
                    }
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task CreateTileAsync(float tileWidth, float tileHeight, GameObject parent, int sortOrder, int j, int i, IUnityTile unityTile)
        {
#if UNITY_EDITOR && SPAWN_PREFABS
            var prefab = await unityTile.SpawnPrefabAsync();
#else
            var prefab = await unityTile.SpawnInstanceAsync();
#endif

            if (prefab == null) await Task.CompletedTask;

            // This code seems to be delayed in executing until mouse focus or something else happens.
            float x = i * tileWidth;
            float z = j * tileHeight;
            Vector3 position = new Vector3(x, 0, z);
            // Hack: Specific 0.35 scale factor for an tileSet. Needed, since width and height of tiles are in integers.
            //Vector3 isometricPosition = new Vector3(0.5f * (x - z), 0.35f * (x + z), 0);
            //Addressables.InstantiateAsync(prefab, position, Quaternion.identity, parent.transform).Completed += InstantiateTile_Completed;
            //var tileGO = GameObject.Instantiate(prefab, position, Quaternion.identity, parent.transform);
            //var tileGO = GameObject.Instantiate(prefab, position, Quaternion.identity, parent.transform);
            //GameObject tileGO = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
            GameObject tileGO = prefab;
            tileGO.transform.SetParent(parent.transform, false);
#if UNITY_EDITOR
            //var tileGO = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
#else
                //var tileGO = Instantiate<GameObject>(prefab, parent.transform);
#endif
            InstantiateTile_Completed2(tileGO, position);
            // for SpriteRenderer
            if (tileGO.TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.sortingOrder = sortOrder;
        }

        private void InstantiateTile_Completed2(GameObject tile, Vector3 position)
        {
            tile.transform.localPosition = position;
            int ix = (int)(position.x / tileWidth + 0.5f);
            int iy = (int)(position.z / tileWidth + 0.5f);
            tile.name = tile.name + "_" + ix.ToString("D3") + "_" + iy.ToString("D3");
        }

        private ITiling2D CreateTilingFromMaze(ITileSet tileSet, Maze<int, int> maze, ITile2D defaultTile)
        {
            MazeWrapperTilingBuilder tilingBuilder = new MazeWrapperTilingBuilder(maze);
            ITileSelector tileSelector;
            tileSelector = new RandomTileSelector();
            //tileSelector = new SequentialTileSelector();
            tileSelector.DefaultTile = tileSet.DefaultTile;
            tileSelector.DefaultTile = defaultTile;
            ITilingEnumerator tilingEnumerator;
            //tilingEnumerator = new RandomTilingEnumerator();
            tilingEnumerator = new SequentialTilingEnumerator();
            tilingBuilder.TileSelector = tileSelector;
            tilingBuilder.TilingEnumerator = tilingEnumerator;

            tilingBuilder.UpdateTiling(tileSet);
            ITiling2D tiling = tilingBuilder.GetTiling();
            return tiling;
        }
    }
}