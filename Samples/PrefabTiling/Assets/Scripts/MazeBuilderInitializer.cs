using CrawfisSoftware.Maze;

using System;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "MazeData", menuName = "CrawfisSoftware/MazeSize", order = 2)]
    internal class MazeBuilderInitializer : MazeBuilderProviderScriptableAbstract
    {
        [Tooltip("Width in columns of the grid")]
        [Range(1, 256)]
        [SerializeField] private int _numberOfColumns;
        [Tooltip("Width in rows of the grid")]
        [Range(1, 256)]
        [SerializeField] private int _numberOfRows;
        [Tooltip("Should the created grid with all walls present (false) or removed (true)")]
        [SerializeField] private bool _openAllCells;


        internal override void CreateMazeGraph(MazeBuilderProviderScriptableAbstract mazeBuilderProvider, System.Random randomGenerator)
        {
            MazeBuilder = new MazeBuilder<int, int>(_numberOfColumns, _numberOfRows);

            if (_openAllCells) MazeBuilder.OpenRegion(0, _numberOfRows * _numberOfColumns - 1);
            MazeBuilder.StartCell = startColumn;
            MazeBuilder.EndCell = _numberOfRows * (_numberOfColumns - 1) + endColumn;
        }
    }
}