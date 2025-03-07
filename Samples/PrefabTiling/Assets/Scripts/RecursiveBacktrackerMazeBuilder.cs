﻿using CrawfisSoftware.Maze;
using CrawfisSoftware.Maze.PerfectMazes;

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
            var mazeBuilder = mazeBuilderProvider.MazeBuilder;
            mazeBuilder.RecursiveBacktracking(mazeBuilder.StartCell, false);
            //mazeBuilder.MergeDeadEnds();
            mazeBuilder.RemoveDeadEnds();
            mazeBuilder.OpenRegion(width * 3 + 3, width * 5 + 5);
            mazeBuilder.BlockRegion(width * 8 + 3, width * 12 + 5);
            mazeBuilder.MakeBidirectionallyConsistent();
        }
    }
}