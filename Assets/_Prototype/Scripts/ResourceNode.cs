// ============================================================
//  ResourceNode.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies
//
//  Pure model for a collectable world object.
//  ResourceNodeMB (MonoBehaviour) owns one of these and
//  delegates player interaction to Collect().
// ============================================================

using UnityEngine;

namespace ContractorCo
{
    public class ResourceNode
    {
        public ResourceType resourceType { get; private set; }
        public int          amount       { get; private set; }
        public float        respawnTime  { get; private set; }

        private bool _collected;

        public ResourceNode(ResourceType type, int amount, float respawnTime = 0f)
        {
            resourceType      = type;
            this.amount       = amount;
            this.respawnTime  = respawnTime;
            _collected        = false;
        }

        // -------------------------------------------------------

        public bool IsAvailable() => !_collected;

        public ResourceDrop Collect()
        {
            if (!IsAvailable())
            {
                Debug.LogWarning($"[ResourceNode] Already collected: {resourceType}");
                return null;
            }

            _collected = true;
            return new ResourceDrop(resourceType, amount);
        }

        // Called by ResourceNodeMB after respawnTime seconds elapse.
        public void Respawn()
        {
            _collected = false;
        }
    }
}
