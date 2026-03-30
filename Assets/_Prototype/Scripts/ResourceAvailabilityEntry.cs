// ============================================================
//  ResourceAvailabilityEntry.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies (except Range attr)
//
//  Serialisable key-value pair used inside LocationSO so
//  designers can set resource abundance per type in Inspector.
// ============================================================

using UnityEngine;

namespace ContractorCo
{
    [System.Serializable]
    public class ResourceAvailabilityEntry
    {
        public ResourceType resourceType;

        [Range(0, 3)]
        [Tooltip("0 = none  1 = scarce  2 = moderate  3 = abundant")]
        public int abundance;
    }
}
