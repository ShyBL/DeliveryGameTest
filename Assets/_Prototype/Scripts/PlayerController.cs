// ============================================================
//  PlayerController.cs
//  Place in: Assets/_Prototype/Scripts/
//  Layer   : Controller - MonoBehaviour
//
//  Handles player movement and resource collection.
//  Owns the ResourceInventory.
//  References UIManager for HUD updates.
//
//  Attach to: Player GameObject
//  Requires : Rigidbody2D
// ============================================================

using UnityEngine;

namespace ContractorCo
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 5f;

        [Header("Interaction")]
        [SerializeField] private float     _interactRange = 1.5f;
        [SerializeField] private LayerMask _interactLayer;

        [Header("Inventory")]
        [SerializeField] private int _maxCarry = 20;

        [Header("UI")]
        [SerializeField] private UIManager _uiManager;

        private Rigidbody2D       _rb;
        private ResourceInventory _inventory;
        private Contract          _activeContract;

        private void Awake()
        {
            _rb        = GetComponent<Rigidbody2D>();
            _inventory = new ResourceInventory(_maxCarry);

            // Refresh HUD automatically on every inventory change
            _inventory.OnChanged += () =>
            {
                _uiManager.HUD.SetContract(_activeContract, _inventory);
            };
        }

        private void Update()
        {
            HandleMovement();
            if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        }

        private void HandleMovement()
        {
            float h   = Input.GetAxisRaw("Horizontal");
            float v   = Input.GetAxisRaw("Vertical");
            var   dir = new Vector2(h, v).normalized;
            _rb.linearVelocity = dir * _speed;
        }

        public void TryInteract()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, _interactRange, _interactLayer);

            foreach (var hit in hits)
            {
                var nodeMB = hit.GetComponent<ResourceNodeMB>();
                if (nodeMB != null && nodeMB.IsAvailable())
                {
                    CollectResource(nodeMB);
                    return;
                }
                // TODO v2: check for ClientNPC component here
            }
        }

        private void CollectResource(ResourceNodeMB nodeMB)
        {
            if (_inventory.IsAtCapacity())
            {
                Debug.Log("[PlayerController] Inventory full.");
                return;
            }

            ResourceDrop drop = nodeMB.Collect();
            if (drop == null) return;

            _inventory.Add(drop.resourceType, drop.amount);
        }

        public void SetActiveContract(Contract contract)
        {
            _activeContract = contract;
            _uiManager.HUD.SetContract(_activeContract, _inventory);
        }

        public ResourceInventory GetInventory() => _inventory;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactRange);
        }
    }
}
