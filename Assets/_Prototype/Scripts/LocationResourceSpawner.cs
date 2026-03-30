// ============================================================
//  LocationResourceSpawner.cs
//  Place in: Assets/Scripts/Controllers/
//  Layer   : Controller — MonoBehaviour (service)
//
//  Instantiates ResourceNodeMB prefabs into the scene based
//  on a LocationData's resource availability profile.
//
//  Abundance → node count mapping:
//    0 (none)     → 0 nodes
//    1 (scarce)   → 2 nodes
//    2 (moderate) → 4 nodes
//    3 (abundant) → 6 nodes
//
//  Attach to: a "Spawner" GameObject in the level scene
//  Inspector : assign one prefab per ResourceType +
//              place SpawnPoint Transforms in the scene
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    public class LocationResourceSpawner : MonoBehaviour
    {
        [Header("Prefabs — one per ResourceType")]
        [SerializeField] private ResourceNodeMB _scrapsPrefab;
        [SerializeField] private ResourceNodeMB _rawMaterialsPrefab;
        [SerializeField] private ResourceNodeMB _hiEndPrefab;

        [Header("Spawn Points")]
        [Tooltip("Place empty GameObjects in the scene and assign them here.")]
        [SerializeField] private List<Transform> _spawnPoints;

        private readonly List<ResourceNodeMB> _activeNodes = new List<ResourceNodeMB>();

        // -------------------------------------------------------
        //  Spawn / Clear
        // -------------------------------------------------------

        /// Clears existing nodes then spawns fresh ones for the given location.
        public void SpawnNodes(LocationData location)
        {
            ClearNodes();

            var availablePoints = new List<Transform>(_spawnPoints);
            Shuffle(availablePoints);

            int pointIdx = 0;

            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                int abundance = location.GetAvailability(type);
                int count     = abundance * 2; // 0→0  1→2  2→4  3→6

                ResourceNodeMB prefab = GetPrefab(type);
                if (prefab == null)
                {
                    Debug.LogWarning($"[Spawner] No prefab assigned for {type}.");
                    continue;
                }

                for (int i = 0; i < count && pointIdx < availablePoints.Count; i++, pointIdx++)
                {
                    ResourceNodeMB node = Instantiate(
                        prefab,
                        availablePoints[pointIdx].position,
                        Quaternion.identity
                    );
                    node.Initialise(type, amount: 1);
                    _activeNodes.Add(node);
                }
            }

            Debug.Log($"[Spawner] Spawned {_activeNodes.Count} nodes for '{location.locationName}'.");
        }

        /// Destroys all currently spawned nodes.
        public void ClearNodes()
        {
            foreach (var node in _activeNodes)
                if (node != null) Destroy(node.gameObject);

            _activeNodes.Clear();
        }

        public List<ResourceNodeMB> GetActiveNodes() =>
            new List<ResourceNodeMB>(_activeNodes);

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private ResourceNodeMB GetPrefab(ResourceType type) => type switch
        {
            ResourceType.Scraps            => _scrapsPrefab,
            ResourceType.RawMaterials      => _rawMaterialsPrefab,
            ResourceType.HighEndComponents => _hiEndPrefab,
            _                              => null
        };

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
