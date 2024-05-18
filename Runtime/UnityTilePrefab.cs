using CrawfisSoftware.AssetManagement;

using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "TilePrefab", menuName = "CrawfisSoftware/Tile Prefab", order = 1)]
    public class UnityTilePrefab : UnityTileScriptableBase
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab { get { return prefab; } set { prefab = value; } }

#if UNITY_EDITOR && SPAWN_PREFABS
        public override Task<GameObject> SpawnPrefabAsync()
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            return Task.FromResult(instance);
        }
#endif
        public override Task<GameObject> SpawnInstanceAsync()
        {
            var instance = GameObject.Instantiate(prefab);
            return Task.FromResult(instance);
        }

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}