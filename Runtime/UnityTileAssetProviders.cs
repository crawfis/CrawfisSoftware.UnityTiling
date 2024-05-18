using CrawfisSoftware.AssetManagement;

using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    [CreateAssetMenu(fileName = "TileAssetProvider", menuName = "CrawfisSoftware/TileAsset", order = 1)]
    public class UnityTileAssetProviders : UnityTileScriptableBase
    {
        [SerializeField] private ScriptableAssetProviderBase<GameObject> prefab;
        public ScriptableAssetProviderBase<GameObject> AssetProvider { get { return prefab; } set { prefab = value; } }

#if UNITY_EDITOR && SPAWN_PREFABS
        public override async Task<GameObject> SpawnPrefabAsync()
        {
            return await AssetProvider.GetAsync(Name);
        }
#endif
        public override async Task<GameObject> SpawnInstanceAsync()
        {
            return await AssetProvider.GetAsync(Name);
        }

        public override async Task Initialize()
        {
            await prefab.Initialize();
        }
    }
}