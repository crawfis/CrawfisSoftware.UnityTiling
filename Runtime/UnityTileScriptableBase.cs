using CrawfisSoftware.AssetManagement;

using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public abstract class UnityTileScriptableBase : ScriptableObject, IUnityTile
    {
        [SerializeField] protected string _name;
        [SerializeField] protected int leftEdgeColor;
        [SerializeField] protected int rightEdgeColor;
        [SerializeField] protected int topEdgeColor;
        [SerializeField] protected int bottomEdgeColor;
        [SerializeField] protected bool isDefaultTile;

        public bool IsDefaultTile { get { return isDefaultTile; } }

        public string TileSetName { get; set; }

        public int ID { get; set; }
        public string Name { get { return _name; } set { _name = value; } }

        public int LeftColor { get { return leftEdgeColor; } set { leftEdgeColor = value; } }

        public int RightColor { get { return rightEdgeColor; } set { rightEdgeColor = value; } }

        public int TopColor { get { return topEdgeColor; } set { topEdgeColor = value; } }

        public int BottomColor { get { return bottomEdgeColor; } set { bottomEdgeColor = value; } }

        public void SetDefaultTile() { isDefaultTile = true; }

#if UNITY_EDITOR && SPAWN_PREFABS
        public abstract Task<GameObject> SpawnPrefabAsync();
#endif
        public abstract Task<GameObject> SpawnInstanceAsync();

        public abstract Task Initialize();
    }
}