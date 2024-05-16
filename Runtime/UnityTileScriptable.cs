using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "Tile", menuName = "CrawfisSoftware/Tile", order = 1)]
    public class UnityTileScriptable : ScriptableObject, IUnityTile
    {
        [SerializeField] private string _name;
        [SerializeField] private int leftEdgeColor;
        [SerializeField] private int rightEdgeColor;
        [SerializeField] private int topEdgeColor;
        [SerializeField] private int bottomEdgeColor;
        [SerializeField] private CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> prefab;
        [SerializeField] private bool isDefaultTile;

        public bool IsDefaultTile { get { return isDefaultTile; } }

        public string TileSetName { get; set; }

        public int ID { get; set; }
        public string Name { get { return _name; } set { _name = value; } }

        public int LeftColor { get { return leftEdgeColor; } set { leftEdgeColor = value; } }

        public int RightColor { get { return rightEdgeColor; } set { rightEdgeColor = value; } }

        public int TopColor { get { return topEdgeColor; } set { topEdgeColor = value; } }

        public int BottomColor { get { return bottomEdgeColor; } set { bottomEdgeColor = value; } }

        public CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> Prefab { get { return prefab; } set { prefab = value; } }

        // Would love to use interfaces, but Unity does not serialize interfaces.
        //public void SetPrefab(CrawfisSoftware.AssetManagement.IAssetManagerAsync<GameObject> prefab)
        public void SetPrefab(CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> prefab)
        {
            this.prefab = prefab;
        }
        public void SetDefaultTile() { isDefaultTile = true; }

        public async Task Initialize()
        {
            await prefab.Initialize();
        }
    }
}