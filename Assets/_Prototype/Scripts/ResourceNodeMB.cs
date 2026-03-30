// ============================================================
//  ResourceNodeMB.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour (world-space)
//
//  MonoBehaviour wrapper around the ResourceNode model.
//  Handles the physical presence in the scene: visuals,
//  collider, and the respawn coroutine.
//
//  Flow:
//    LocationResourceSpawner.SpawnNodes() → Instantiate prefab
//      → ResourceNodeMB.Initialise(type, amount)
//    PlayerController.TryInteract() → finds this via OverlapCircle
//      → calls ResourceNodeMB.Collect()
//      → returns ResourceDrop to PlayerController
//
//  Attach to: resource node prefabs (one per ResourceType)
//  Inspector : assign SpriteRenderer and optional overlays
// ============================================================

using System.Collections;
using UnityEngine;

namespace ContractorCo
{
    public class ResourceNodeMB : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _sprite;
        [Tooltip("Optional child GameObject shown when node is collected (e.g. greyed-out overlay)")]
        [SerializeField] private GameObject _collectedOverlay;

        [Header("Respawn")]
        [Tooltip("Seconds before this node resets. Set to 0 for no respawn (v1 default).")]
        [SerializeField] private float _respawnTime = 0f;

        private ResourceNode _node;

        // -------------------------------------------------------
        //  Initialisation  (called by LocationResourceSpawner)
        // -------------------------------------------------------

        public void Initialise(ResourceType type, int amount)
        {
            _node = new ResourceNode(type, amount, _respawnTime);

            if (_collectedOverlay != null)
                _collectedOverlay.SetActive(false);

            if (_sprite != null)
                _sprite.color = Color.white;
        }

        // -------------------------------------------------------
        //  Collection  (called by PlayerController)
        // -------------------------------------------------------

        public bool IsAvailable() => _node != null && _node.IsAvailable();

        /// Returns a ResourceDrop on success, null if already collected.
        public ResourceDrop Collect()
        {
            ResourceDrop drop = _node?.Collect();
            if (drop == null) return null;

            // Visual feedback
            if (_collectedOverlay != null)
                _collectedOverlay.SetActive(true);

            if (_sprite != null)
                _sprite.color = Color.gray;

            // Respawn if configured
            if (_respawnTime > 0f)
                StartCoroutine(RespawnAfter(_respawnTime));

            return drop;
        }

        // -------------------------------------------------------
        //  Respawn
        // -------------------------------------------------------

        private IEnumerator RespawnAfter(float delay)
        {
            yield return new WaitForSeconds(delay);

            _node.Respawn();

            if (_collectedOverlay != null)
                _collectedOverlay.SetActive(false);

            if (_sprite != null)
                _sprite.color = Color.white;
        }
    }
}
