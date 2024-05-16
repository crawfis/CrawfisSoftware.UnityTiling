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
        [SerializeField]
        private List<UnityTileSetBuilderScriptables> tileSetBuilders = new List<UnityTileSetBuilderScriptables>(); // set in the Unity Editor
        [SerializeField]
        public string tilingsName = "Tilings";
        [SerializeField] MazeProvider _mazeProvider;

        private ITiling2D tiling;
        private float tileWidth;
        private float tileHeight;

        //public GTMY.Graph.MultilanePath Path { get; private set; }

        public async Task Build(Transform topLevelParent)
        {
            tileSetBuilders[0].CreateTileSet();
            tileWidth = tileSetBuilders[0].UnityTileSet.Width;
            tileHeight = tileSetBuilders[0].UnityTileSet.Height;
            for (int i = 1; i < tileSetBuilders.Count; i++)
            {
                var tileSetBuilder = tileSetBuilders[i];
                tileSetBuilder.CreateTileSet();
                if (tileWidth != tileSetBuilder.UnityTileSet.Width || tileHeight != tileSetBuilder.UnityTileSet.Height)
                    throw new ArgumentException("Tilesets must having matching width and heights.");
            }
            //var maze = BuildPath();
            _mazeProvider.Build();
            var maze = _mazeProvider.Maze;
            int tileSetIndex = 1;
            Debug.Log("Waiting for tiles to load.");
            foreach (var tileSet in tileSetBuilders)
            {
                await tileSet.Initialize();
            }
            Debug.Log("All tilesets are loaded. Creating the tiling.");
            foreach (var tileSet in tileSetBuilders)
            {
                GameObject emptyGO = new GameObject("Tiling" + tileSetIndex.ToString());
                emptyGO.transform.SetParent(topLevelParent, false);
                tiling = CreateTilingFromMaze(tileSet.UnityTileSet, maze, tileSet.DefaultTile);
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
                    var assetReference = unityTile.Prefab;
                    //var prefab = assetReference.Asset as GameObject;
                    var task = CreateTileAsync(tileWidth, tileHeight, parent, sortOrder--, j, i, unityTile, assetReference);
                    if (task != null || !task.IsCompleted)
                    {
                        tasks.Add(task);
                    }
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task CreateTileAsync(float tileWidth, float tileHeight, GameObject parent, int sortOrder, int j, int i, IUnityTile unityTile, IAssetManagerAsync<GameObject> assetReference)
        {
            var task = assetReference.GetAsync(unityTile.Name);
            var prefab = await task;
            if (prefab == null) await Task.CompletedTask;

            // This code seems to be delayed in executing until mouse focus or something else happens.
            float x = i * tileWidth;
            float z = j * tileHeight;
            Vector3 position = new Vector3(x, 0, z);
            // Hack: Specific 0.35 scale factor for an tileSet. Needed, since width and height of tiles are in integers.
            Vector3 isometricPosition = new Vector3(0.5f * (x - z), 0.35f * (x + z), 0);
            //Addressables.InstantiateAsync(prefab, position, Quaternion.identity, parent.transform).Completed += InstantiateTile_Completed;
            //var tileGO = GameObject.Instantiate(prefab, position, Quaternion.identity, parent.transform);
            //var tileGO = GameObject.Instantiate(prefab, position, Quaternion.identity, parent.transform);
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

        //private void InstantiateTile_Completed(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
        //{
        //    GameObject tile = obj.Result;
        //    int ix = (int)(tile.transform.localPosition.x / tileWidth + 0.5f);
        //    int iy = (int)(tile.transform.localPosition.z / tileWidth + 0.5f);
        //    tile.name = "Tile_" + ix.ToString("D3") + "_" + iy.ToString("D3");
        //    //tile.transform.parent = this.transform;

        //}

        //public Maze<int, int> BuildPath()
        //{
        //    Maze<int, int> maze = GenerateGraph();
        //    Debug.Log(maze.ToString());

        //    //GetPath(maze);
        //    return maze;
        //}

        //private ITiling2D CreateRandomTiling(ITileSet unityTileSet, ITile2D defaultTile)
        //{
        //    // This does not use the maze or path (one was generated for camera flights)
        //    ITilingBuilder tilingBuilder = new RandomTilingBuilder(width, height);
        //    ITilingEnumerator tilingEnumerator = new RandomTilingEnumerator();
        //    tilingEnumerator.Width = width;
        //    tilingEnumerator.Height = height;
        //    tilingBuilder.TilingEnumerator = tilingEnumerator;
        //    tilingBuilder.TileSelector.DefaultTile = defaultTile;
        //    tilingBuilder.UpdateTiling(unityTileSet);
        //    ITiling2D tiling = tilingBuilder.GetTiling();
        //    //BoundaryTilingEnumerator boundary = new BoundaryTilingEnumerator(3);
        //    //tiling.TilingEnumerator = boundary;
        //    return tiling;
        //}

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

        //private Maze<int, int> GenerateGraph()
        //{
        //    int seed = randomSeed;
        //    if(seed == 0)
        //        seed = System.DateTime.Now.Millisecond; // 13 is a good number
        //    var randomGenerator = new System.Random(seed);
        //    Maze<int, int> maze;
        //    switch (testTiling)
        //    {
        //        case TestTilingCase.Random:
        //            maze = CreateSidewinderPathGraph(randomGenerator);
        //            break;
        //        case TestTilingCase.RandomPath:
        //            maze = CreatePathGraph(randomGenerator);
        //            break;
        //        case TestTilingCase.Maze:
        //            maze = CreateMaze1Graph(randomGenerator);
        //            break;
        //        case TestTilingCase.MazeThinned:
        //            maze = CreateMaze2Graph(randomGenerator);
        //            break;
        //        case TestTilingCase.Dungeonish:
        //            maze = CreateDungeonGraph(randomGenerator);
        //            break;
        //        default:
        //            throw new InvalidProgramException("The enum TestTilingCase was updated, but not this code.");
        //    }
        //    return maze;
        //}

        //private void GetPath(Maze<int, int> maze)
        //{
        //    // Todo: Fix PathTileSetBuilder to take in floats rather than ints.
        //    var pathTileSetBuilder = new GTMY.Tiling.PathTileSetBuilder((int)tileWidth, (int)tileHeight);
        //    var pathTileSet = pathTileSetBuilder.GenerateArcTileSet();
        //    //var pathTileSet = pathTileSetBuilder.GenerateManhattanTileSet();
        //    pathTiling = CreateTilingFromMaze(pathTileSet, maze, null);
        //    List<Vector3> positions = new List<Vector3>();
        //    // Bug: The start and end are probably not in the Path tiling as they are dead ends.
        //    var mazePath = PathQuery<int, int>.FindPath(maze, startColumn, endColumn + (height - 1) * width).GetEnumerator();
        //    // For debugging
        //    var mazeSolution = new List<IIndexedEdge<int>>();
        //    mazePath.MoveNext(); // Skip over start cell (the path tile does not exist since it is a dead-end (for now).
        //    while (mazePath.MoveNext())
        //        mazeSolution.Add(mazePath.Current);
        //    // Since this was a non-re-usable IEnumerator we need a new one.
        //    mazePath = PathQuery<int, int>.FindPath(maze, startColumn, endColumn + (height - 1) * width).GetEnumerator();
        //    // Todo: Remove this once dead-end PathTiles are added.
        //    mazePath.MoveNext(); // Skip over start cell (the path tile does not exist since it is a dead-end (for now).
        //    while (mazePath.MoveNext())
        //    {
        //        int fromIndex = mazePath.Current.From;
        //        int toIndex = mazePath.Current.To;
        //        Direction edgeDirection = DirectionExtensions.GetEdgeDirection(fromIndex, toIndex, width);
        //        //Direction previousDirection = DirectionExtensions.Reverse(edgeDirection); // DirectionExtensions.GetEdgeDirection(toIndex, fromIndex, width); // reversed
        //        int i = fromIndex % width;
        //        int j = fromIndex / width;
        //        IPathTile tile = pathTiling.Tile(i, j) as IPathTile;
        //        IEnumerable<Vector3> tilePositions = tile.PathCenterPositions;
        //        if (edgeDirection != tile.ToEdge)
        //        {
        //            tilePositions = Enumerable.Reverse<Vector3>(tile.PathCenterPositions);
        //        }
        //        Vector3 tileOrigin = new Vector3(i * tileWidth, 0, j * tileHeight);
        //        // Merge tile's path with the overall path.
        //        foreach (var position in tilePositions)
        //        {
        //            positions.Add(position + tileOrigin);
        //        }
        //    }
        //    var path = new GTMY.Graph.MultilanePath(positions);
        //    // Assign path
        //    this.Path = path;
        //}

        #region Sample tiling creation
        //private Maze<int, int> CreateDungeonGraph(System.Random randomGenerator)
        //{
        //    var randomWalk = new RandomWalkMazeBuilder<int, int>(width, height, null, null);
        //    //{
        //    //    MaxWalkingDistance = 20, // 10 * columns * rows,
        //    //    PercentToCarve = 0.08f,
        //    //    //favorForwardCarving = false,
        //    //    InitialNumberOfWalkers = 4, //1,
        //    //    //NumberOfWalkers = 3,
        //    //    //RandomGenerator = new Random()
        //    //};
        //    randomWalk.EndCell = (width * height) - 1;
        //    randomWalk.InitialNumberOfWalkers = 4;
        //    randomWalk.MaxWalkingDistance = 20;
        //    randomWalk.PercentToCarve = (float)0.8;
        //    randomWalk.StartCell = 0;
        //    MazeBuilderAbstract<int,int> mazeBuilder = randomWalk;
        //    mazeBuilder.CreateMaze(false);
        //    var maze = mazeBuilder.GetMaze();
        //    mazeBuilder.CreateMaze(true);
        //    maze = mazeBuilder.GetMaze();

        //    //mazeBuilder.FreezeCells();
        //    mazeBuilder = new MazeBuilderBinaryTree<int, int>(mazeBuilder as RandomWalkMazeBuilder<int, int>);
        //    mazeBuilder.CreateMaze(false);
        //    maze = mazeBuilder.GetMaze();
        //    Direction startDir = maze.GetDirection(0, 0) & ~Direction.Undefined;
        //    startDir |= Direction.S;
        //    Direction endDir = maze.GetDirection(width - 1, height - 1) & ~Direction.Undefined;
        //    endDir |= Direction.E;
        //    var mazeBuilder2 = new MazeBuilderExplicit<int, int>(mazeBuilder);
        //    mazeBuilder2.SetCell(0, 0, startDir);
        //    mazeBuilder2.SetCell(width - 1, height - 1, endDir);

        //    mazeBuilder2.RemoveDeadEnds(true);
        //    mazeBuilder2.RemoveDeadEnds(true);
        //    mazeBuilder2.RemoveDeadEnds(true);
        //    maze = mazeBuilder2.GetMaze();
        //    return maze;
        //}

        //private Maze<int, int> CreatePathGraph(System.Random randomGenerator)
        //{
        //    PathGenerator randomPathsGenerator = new PathGenerator(width, height, randomGenerator);
        //    IList<int> grid;
        //    IList<int> horizontalGrid;
        //    randomPathsGenerator.GenerateRandomPath(startColumn, endColumn, out grid, out horizontalGrid);
        //    var mazeBuilder = new MazeBuilderExplicit<int, int>(width, height, MazeBuilderUtility<int, int>.DummyNodeValues, MazeBuilderUtility<int, int>.DummyEdgeValues);
        //    mazeBuilder.StartCell = startColumn;
        //    mazeBuilder.EndCell = (height - 1) * width + endColumn;
        //    MazeWrapperFromGridBitArrays.CarvePath(mazeBuilder, grid, horizontalGrid);
        //    var maze = mazeBuilder.GetMaze();
        //    //maze.AddOpening(startColumn, 0, Direction.S);
        //    //maze.AddOpening(endColumn, height - 1, Direction.N);
        //    return maze;
        //}

        //private Maze<int, int> CreateSidewinderPathGraph(System.Random randomGenerator)
        //{
        //    var mazeBuilder = new PathGeneratorSideWinder(width, height);
        //    mazeBuilder.StartCell = startColumn;
        //    mazeBuilder.EndCell = (height-1)*width + endColumn;
        //    mazeBuilder.CreateMaze();
        //    Maze<int, int> maze = mazeBuilder.GetMaze();
        //    //maze.AddOpening(startColumn, 0, Direction.S);
        //    //maze.AddOpening(endColumn, height - 1, Direction.N);
        //    return maze;
        //}

        //private Maze<int, int> CreateMaze1Graph(System.Random randomGenerator)
        //{
        //    var mazeBuilder = new MazeBuilderRecursiveBacktracker<int, int>(width, height);
        //    mazeBuilder.RandomGenerator = randomGenerator;
        //    mazeBuilder.StartCell = startColumn;
        //    mazeBuilder.EndCell = (height - 1) * width + endColumn;
        //    mazeBuilder.CreateMaze();
        //    var maze = mazeBuilder.GetMaze();
        //    //maze.AddOpening(startColumn, 0, Direction.S);
        //    //maze.AddOpening(endColumn, height - 1, Direction.N);
        //    return maze;
        //}

        //private Maze<int, int> CreateMaze2Graph(System.Random randomGenerator)
        //{
        //    var mazeBuilder = new MazeBuilderRecursiveBacktracker<int, int>(width, height);
        //    //var mazeBuilder = new MazeBuilderBinaryTree<int, int>(width, height, nodeFunc, edgeFunc);
        //    //var mazeBuilder = new CrawfisSoftware.PCG.PathGeneratorSideWinder(width, height, nodeFunc, edgeFunc);
        //    //var mazeBuilder = new RandomWalkMazeBuilder<int,int>(width, height, nodeFunc, edgeFunc);
        //    //mazeBuilder.MaxWalkingDistance = 2 * width * height;
        //    //mazeBuilder.RandomGenerator = randomGenerator;
        //    //mazeBuilder.BlockRegion(3 + 3 * width, 13 + 7 * width);
        //    //mazeBuilder.BlockRegion(14 + 9 * width, 22 + 13 * width);
        //    mazeBuilder.CreateMaze(false);
        //    mazeBuilder.CarveDirectionally(0, 0, Direction.E);
        //    mazeBuilder.CarveDirectionally(width - 1, height - 1, Direction.W);
        //    Maze<int, int> maze = mazeBuilder.GetMaze();
        //    Direction startDir = maze.GetDirection(0, 0) & ~Direction.Undefined;
        //    startDir |= Direction.S;
        //    Direction endDir = maze.GetDirection(width - 1, height - 1) & ~Direction.Undefined;
        //    endDir |= Direction.E;
        //    var mazeBuilder4 = new MazeBuilderExplicit<int, int>(mazeBuilder);
        //    mazeBuilder4.SetCell(0, 0, startDir);
        //    mazeBuilder4.SetCell(width - 1, height - 1, endDir);

        //    mazeBuilder4.RemoveDeadEnds(true);
        //    mazeBuilder4.RemoveDeadEnds(true);
        //    return maze;
        //}
        #endregion
    }
}