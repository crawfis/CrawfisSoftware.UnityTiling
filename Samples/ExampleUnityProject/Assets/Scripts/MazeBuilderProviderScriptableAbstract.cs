using CrawfisSoftware.Collections.Maze;
using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public abstract class MazeBuilderProviderScriptableAbstract : ScriptableObject
    {
        //[SerializeField] private MazeBuilderInitializer mazeBuilderInitializer;
        [SerializeField] private MazeBuilderProviderScriptableAbstract previousMazeBuilderProvider;
        [SerializeField] protected int startColumn = 1;
        [SerializeField] protected int endColumn = 1;
        public MazeBuilderAbstract<int,int> MazeBuilder { get; protected set; }

        public void CreateMaze(System.Random randomGenerator)
        {
            if(previousMazeBuilderProvider != null)
                previousMazeBuilderProvider?.CreateMaze(randomGenerator);
            CreateMazeGraph(previousMazeBuilderProvider, randomGenerator);
        }

        internal abstract void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator);
    }
}
