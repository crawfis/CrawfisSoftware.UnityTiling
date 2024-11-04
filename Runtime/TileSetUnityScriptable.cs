using CrawfisSoftware.Tiling;
using CrawfisSoftware.Tiling.TileSets;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    /// <summary>
    /// ScriptableObject that encapsulates a CrawfisSoftware.Tiling.TileSets.TileSet and "is-a" ITileSet.
    /// </summary>
    [CreateAssetMenu(fileName = "TileSet", menuName = "CrawfisSoftware/TileSets", order = 2)]
    public class TileSetUnityScriptable : ScriptableObject, ITileSet
    {
        // User-input from Unity Editor.
        [SerializeField] private string _name, _description;
        [SerializeField] private float _width, _height;
        [SerializeField] private UnityTileScriptableBase _defaultTile;
        [SerializeField] private List<string> _keywords = new List<string>();
        [SerializeField] private List<UnityTileScriptableBase> _tiles = new List<UnityTileScriptableBase>();

        protected TileSet _tileSet;

        public string Name { get { return _tileSet.Name; } }
        public string Description
        {
            get { return _tileSet.Description; }
            set
            {
                _tileSet.Description = value;
                _description = value;
            }
        }
        public float Width { get { return _tileSet.Width; } }
        public float Height { get { return _tileSet.Height; } }
        public IEnumerable<string> Keywords { get { return _tileSet.Keywords; } }
        public ITile2D DefaultTile { get { return _tileSet.DefaultTile; } }
        //public IEnumerable<ITile2D> Tiles { get { return _tileSet.; } }

        public int Count { get { return _tileSet.Count; } }
        public bool IsComplete { get { return _tileSet.IsComplete; } }

        public void SetName(string name) { _tileSet.Name = name; _name = name; }
        public void SetWidth(float width) { _tileSet.Width = width; _width = width; }
        public void SetHeight(float height) { _tileSet.Height = height; _height = height; }
        public void SetDescription(string description)
        {
            _tileSet.Description = description;
            _description = description;
        }
        public void AddKeyword(string keyword)
        {
            _tileSet.AddKeyword(keyword);
            _keywords.Add(keyword);
        }
        public void SetDefaultTile(UnityTileScriptableBase tile)
        {
            _tileSet.SetDefaultTile(tile);
            _defaultTile = tile;
        }
        public void AddTiles(List<UnityTileScriptableBase> tiles)
        {
            foreach (var tile in tiles)
            {
                _tileSet.AddTile(tile);
                _tiles.Add(tile);
            }
        }
        public void AddTile(UnityTileScriptableBase tile) { _tileSet.AddTile(tile); _tiles.Add(tile); }

        public async Task Awake()
        {
            // Create a TileSet and add it.
            await Initialize();
        }

        public async Task Initialize()
        {
            _tileSet = new(_name, _description, _width, _height);
            List<Task> tasks = new List<Task>();
            foreach (var tile in _tiles)
            {
                _tileSet.AddTile(tile);
                tasks.Add(tile.Initialize());
            }
            if (_defaultTile != null)
            {
                _tileSet.SetDefaultTile(_defaultTile);
                //_tileSet.AddTile(_defaultTile);
                tasks.Add(_defaultTile.Initialize());
            }
            await Task.WhenAll(tasks);
        }

        public ITile2D GetTile(int tileID)
        {
            return _tileSet.GetTile(tileID);
        }

        public IList<ITile2D> GetMatchingTiles(int leftChoice, int topChoice, int rightChoice, int bottomChoice)
        {
            return _tileSet.GetMatchingTiles(leftChoice, topChoice, rightChoice, bottomChoice);
        }

        public bool TryGetMatchingTiles(int leftChoice, int topChoice, int rightChoice, int bottomChoice, out IList<ITile2D> tiles)
        {
            return _tileSet.TryGetMatchingTiles(leftChoice, topChoice, rightChoice, bottomChoice, out tiles);
        }

        public IEnumerator<ITile2D> GetEnumerator()
        {
            return _tileSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}