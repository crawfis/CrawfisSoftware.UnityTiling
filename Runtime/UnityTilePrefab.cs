using CrawfisSoftware.AssetManagement;

using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "TilePrefab", menuName = "CrawfisSoftware/Tile Prefab", order = 1)]
    public class UnityTilePrefab : UnityTileScriptableBase
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab { get { return prefab; } set { prefab = value; } }

        public override Task<GameObject> SpawnInstanceAsync(bool createPrefabs = false)
        {
            if (createPrefabs)
            {
#if UNITY_EDITOR
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                return Task.FromResult(instance);
#endif
            }
            var instance = GameObject.Instantiate(prefab);
            return Task.FromResult(instance);
        }

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}