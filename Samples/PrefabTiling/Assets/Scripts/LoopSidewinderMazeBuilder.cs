using CrawfisSoftware.PCG;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "Loop1", menuName = "CrawfisSoftware/Loop Sidewinder", order = 3)]
    internal class LoopSidewinderMazeBuilder : MazeBuilderProviderScriptableAbstract
    {
        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            int width = mazeBuilderProvider.MazeBuilder.Width;
            int height = mazeBuilderProvider.MazeBuilder.Height;
            // Todo: Add a constructor to LoopGeneratorSideWinder that takes a mazeBuilder
            var mazeBuilder = new LoopGeneratorSideWinder<int, int>(width, height);
            mazeBuilder.RandomGenerator = randomGenerator;
            mazeBuilder.CreateMaze(true);

            this.MazeBuilder = mazeBuilder;
        }
    }
}