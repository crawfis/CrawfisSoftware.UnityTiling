using UnityEngine;
namespace CrawfisSoftware.UnityTiling
{
    public class UnityTile : IUnityTile
    {
        public string TileSetName { get; private set; }

        public int ID { get; private set; }

        public string Name { get; private set; }
        public int LeftColor { get; private set; }

        public int RightColor { get; private set; }

        public int TopColor { get; private set; }

        public int BottomColor { get; private set; }
        public CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> Prefab { get; private set; }
        public UnityTile(string name, string tileSetName, int id, AssetManagement.ScriptableAssetProviderBase<GameObject> prefab, int left, int top, int right, int bottom)
        {
            TileSetName = tileSetName;
            Name = name;
            ID = id;
            Prefab = prefab;
            LeftColor = left;
            TopColor = top;
            RightColor = right;
            BottomColor = bottom;
        }
    }
}