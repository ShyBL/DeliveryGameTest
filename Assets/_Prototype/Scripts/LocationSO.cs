// ============================================================
//  LocationSO.cs
//  Place in: Assets/Scripts/ScriptableObjects/
//  Layer   : ScriptableObject (data authoring)
//
//  How to create:
//    Right-click in Project → Create > ContractorCo > LocationSO
//    Set locationType, isWildcard, resource abundances
//    Fixed slots (isWildcard = false) → assign to LevelController.fixedLocations
//    Wildcard slots (isWildcard = true) → assign to LevelController.wildcardPool
//
//  Suggested setup (v1):
//    2 fixed SOs : IndustrialRuin, MiningZone
//    4+ wildcard SOs : FrontierColony, CorporateOutpost, BlackSite, CrashSite
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    [CreateAssetMenu(menuName = "ContractorCo/LocationSO", fileName = "New Location")]
    public class LocationSO : ScriptableObject
    {
        [Header("Identity")]
        public string       locationName;
        public LocationType locationType;

        [Header("Slot Type")]
        [Tooltip("False = always dealt as a fixed slot. True = enters the wildcard pool.")]
        public bool isWildcard;

        [Header("Resource Availability")]
        [Tooltip("Add one entry per ResourceType you want available here. " +
                 "Omit a type entirely to treat it as 0 (none). " +
                 "0=none  1=scarce  2=moderate  3=abundant")]
        public List<ResourceAvailabilityEntry> resourceAvailability;

        // -------------------------------------------------------

        /// Creates a fresh runtime LocationData every time it is called.
        /// LevelController calls this once per deal per SO.
        public LocationData ToLocationData()
        {
            string id  = $"{locationType}_{System.Guid.NewGuid().ToString().Substring(0, 6)}";

            var dict = new Dictionary<ResourceType, int>();
            if (resourceAvailability != null)
                foreach (var entry in resourceAvailability)
                    dict[entry.resourceType] = entry.abundance;

            return new LocationData(id, locationName, locationType, isWildcard, dict);
        }
    }
}
