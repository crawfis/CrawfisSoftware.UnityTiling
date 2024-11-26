using CrawfisSoftware.Collections.Maze;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "MazeGen1", menuName = "CrawfisSoftware/Maze RecursiveBacktracker", order = 3)]
    internal class RecursiveBacktrackerMazeBuilder : MazeBuilderProviderScriptableAbstract
    {
        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            int width = mazeBuilderProvider.MazeBuilder.Width;
            int height = mazeBuilderProvider.MazeBuilder.Height;
            var backTracker = new MazeBuilderRecursiveBacktracker<int, int>(mazeBuilderProvider.MazeBuilder);
            backTracker.CreateMaze(true);
            //mazeBuilder.MergeDeadEnds();
            mazeBuilderProvider.MazeBuilder.RemoveDeadEnds();
            mazeBuilderProvider.MazeBuilder.OpenRegion(width * 3 + 3, width * 5 + 5);
            mazeBuilderProvider.MazeBuilder.BlockRegion(width * 8 + 3, width * 12 + 5);
            mazeBuilderProvider.MazeBuilder.MakeBidirectionallyConsistent();
            this.MazeBuilder = mazeBuilderProvider.MazeBuilder;
        }
    }
}