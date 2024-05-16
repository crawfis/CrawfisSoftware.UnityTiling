using System.Threading.Tasks;

using CrawfisSoftware.Tiling;
using CrawfisSoftware.Tiling.TileSets;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public class UnityTileSetBuilderScriptables : MonoBehaviour
    {
        private TileSet _tileSet;
        [SerializeField] private TileSetUnityScriptable _tileSetScriptable;
        public ITileSet UnityTileSet { get; private set; }
        public ITile2D DefaultTile { get; private set; }

        public void CreateTileSet()
        {
            int index = 0;
            _tileSet = new TileSet(_tileSetScriptable.Name, _tileSetScriptable.Description, _tileSetScriptable.Width, _tileSetScriptable.Height);
            foreach (var keyword in _tileSetScriptable.Keywords)
            {
                _tileSet.AddKeyword(keyword);
            }
            foreach (var tileScriptable in _tileSetScriptable.Tiles)
            {
                var tile = new UnityTile(tileScriptable.Name, _tileSetScriptable.Name, index++, tileScriptable.Prefab, tileScriptable.LeftColor, tileScriptable.TopColor, tileScriptable.RightColor, tileScriptable.BottomColor);
                _tileSet.AddTile(tile);
            }
            var defaultTileScriptable = _tileSetScriptable.DefaultTile;
            DefaultTile = null;
            if (defaultTileScriptable != null)
            {
                var defaultTile = new UnityTile(defaultTileScriptable.Name, _tileSetScriptable.Name, index++, defaultTileScriptable.Prefab, defaultTileScriptable.LeftColor, defaultTileScriptable.TopColor, defaultTileScriptable.RightColor, defaultTileScriptable.BottomColor);
                _tileSet.AddTile(defaultTile);
                _tileSet.SetDefaultTile(defaultTile);
                DefaultTile = defaultTile;
            }
            Debug.Log("Working on loading the tilesets within UnityTileSetBuilderScriptables");
            Debug.Log("UnityTileSetBuilderScriptables is finishing up.");
            UnityTileSet = _tileSet;
        }

        public async Task Initialize()
        {
            await _tileSetScriptable.Initialize();
        }
    }
}