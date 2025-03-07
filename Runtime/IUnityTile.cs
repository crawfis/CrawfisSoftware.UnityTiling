﻿using CrawfisSoftware.Tiling;

using System.Threading.Tasks;

using UnityEngine;

namespace CrawfisSoftware.UnityTiling
{
    public interface IUnityTile : ITile2D
    {
        string Name { get; }
        // Would love to use interfaces, but Unity does not serialize interfaces.
        //CrawfisSoftware.AssetManagement.ScriptableAssetProviderBase<GameObject> Prefab { get; }
        Task<GameObject> SpawnInstanceAsync(bool createPrefabs);
        Task Initialize();
    }
}