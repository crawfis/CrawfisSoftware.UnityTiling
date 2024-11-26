using CrawfisSoftware.PCG;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "Loop1", menuName = "CrawfisSoftware/Loop Sidewinder", order = 3)]
    internal class LoopSidewinderMazeBuilder : MazeBuilderProviderScriptableAbstract
    {
        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            var loop = new LoopGeneratorSideWinder<int, int>(mazeBuilderProvider.MazeBuilder);
            loop.CreateMaze();
            this.MazeBuilder = mazeBuilderProvider.MazeBuilder;
        }
    }
}