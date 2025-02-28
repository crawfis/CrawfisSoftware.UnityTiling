using CrawfisSoftware.PCG;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "sidewinder", menuName = "CrawfisSoftware/Sidewinder", order = 3)]
    internal class SidewinderMazeBuilder : MazeBuilderProviderScriptableAbstract
    {
        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            var mazeBuilder = new PathGeneratorSideWinder<int, int>(mazeBuilderProvider.MazeBuilder);
            mazeBuilder.CarvePath(mazeBuilderProvider.MazeBuilder, true);
        }
    }
}