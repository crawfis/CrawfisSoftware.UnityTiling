using CrawfisSoftware.Collections.Maze;
//using CrawfisSoftware.PCG;
using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public class MazeProvider : MonoBehaviour
    {
        [SerializeField]
        private MazeBuilderProviderScriptableAbstract mazeBuilderProvider;
        [SerializeField]
        private int startColumn = 5;
        [SerializeField]
        private int endColumn = 5;
        [Space(32f)]
        [SerializeField]
        private RandomProviderFromList randomProvider;

        public MazeBuilderProviderScriptableAbstract Builder { get { return mazeBuilderProvider; } }

        public Maze<int, int> Maze { get; private set; }
        public void Build()
        {
            randomProvider.Initialize();
            var random = randomProvider.NewGenerator();
            this.mazeBuilderProvider.CreateMaze(random);
            this.mazeBuilderProvider.MazeBuilder.RemoveUndefines();
            Maze = mazeBuilderProvider.MazeBuilder.GetMaze();
            Debug.Log(Maze.ToString());
        }
    }
}