// ============================================================
//  ResourceDrop.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies
//
//  Payload returned by ResourceNode.Collect().
//  Passed up to PlayerController to add to inventory.
// ============================================================

namespace ContractorCo
{
    [System.Serializable]
    public class ResourceDrop
    {
        public ResourceType resourceType;
        public int          amount;

        public ResourceDrop(ResourceType type, int amount)
        {
            resourceType = type;
            this.amount  = amount;
        }

        public override string ToString() => $"Drop({amount}x {resourceType})";
    }
}
