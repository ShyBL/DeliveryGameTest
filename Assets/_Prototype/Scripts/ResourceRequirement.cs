// ============================================================
//  ResourceRequirement.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies
//
//  Represents one line item in a Contract:
//  e.g. "3x Scraps" or "1x HighEndComponents"
// ============================================================

namespace ContractorCo
{
    [System.Serializable]
    public class ResourceRequirement
    {
        public ResourceType resourceType;
        public int          amount;

        public ResourceRequirement(ResourceType type, int amount)
        {
            resourceType = type;
            this.amount  = amount;
        }

        public override string ToString() => $"{amount}x {resourceType}";
    }
}
