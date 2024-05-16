using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "CrawfisSoftware/TileSets", order = 2)]
    public class TileSetUnityScriptable : ScriptableObject
    {
        [SerializeField] private string _name, _description;
        [SerializeField] private float _width, _height;
        [SerializeField] private UnityTileScriptable _defaultTile;
        [SerializeField] private List<string> _keywords = new List<string>();
        [SerializeField] private List<UnityTileScriptable> tiles = new List<UnityTileScriptable>();

        public string Name { get { return _name; } }
        public string Description { get { return _description; } set { _description = value; } }
        public float Width { get { return _width; } }
        public float Height { get { return _height; } }
        public IEnumerable<string> Keywords { get { return _keywords; } }
        public UnityTileScriptable DefaultTile { get { return _defaultTile; } }
        public IEnumerable<UnityTileScriptable> Tiles { get { return tiles; } }

        public void SetName(string name) { _name = name; }
        public void AddTiles(List<UnityTileScriptable> tiles) { this.tiles.AddRange(tiles); }
        public void AddTile(UnityTileScriptable tile) { this.tiles.Add(tile); }
        public void SetHeight(float height) { _height = height; }
        public void SetWidth(float width) { _width = width; }
        public void SetDefaultTile(UnityTileScriptable defaultTile) { this._defaultTile = defaultTile; }
        public void AddKeyword(string keyword) { _keywords.Add(keyword); }
        public async Task Initialize()
        {
            foreach (var tile in tiles)
            {
                await tile.Initialize();
            }
            if (_defaultTile != null)
            {
                await _defaultTile.Initialize();
            }
        }
    }
}
