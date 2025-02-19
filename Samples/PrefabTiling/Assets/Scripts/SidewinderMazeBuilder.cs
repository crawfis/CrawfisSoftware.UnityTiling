﻿using CrawfisSoftware.PCG;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "sidewinder", menuName = "CrawfisSoftware/Sidewinder", order = 3)]
    internal class SidewinderMazeBuilder : MazeBuilderProviderScriptableAbstract
    {
        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            int width = mazeBuilderProvider.MazeBuilder.Width;
            int height = mazeBuilderProvider.MazeBuilder.Height;
            // Todo: Add a constructor to LoopGeneratorSideWinder that takes a mazeBuilder
            var mazeBuilder = new PathGeneratorSideWinder<int, int>(width, height);
            mazeBuilder.StartCell = startColumn;
            mazeBuilder.EndCell = endColumn + width * (height - 1);
            mazeBuilder.RandomGenerator = randomGenerator;
            mazeBuilder.CreateMaze(true);

            this.MazeBuilder = mazeBuilder;

        }
    }
}