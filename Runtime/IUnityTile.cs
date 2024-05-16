using CrawfisSoftware.Tiling;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public interface IUnityTile : ITile2D
    {
        //UnityEngine.AddressableAssets.AssetReference Prefab { get; }
        string Name { get; }
        // Would love to use interfaces, but Unity does not serialize interfaces.
        CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> Prefab { get; }
    }
}