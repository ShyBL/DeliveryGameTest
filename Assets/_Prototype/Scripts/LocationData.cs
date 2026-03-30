// ============================================================
//  LocationData.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies
//
//  Runtime data for one level/location.
//  Built by LocationSO.ToLocationData() — never new'd directly.
//  Implements ISelectable so BoardView can highlight it.
// ============================================================

using System.Collections.Generic;

namespace ContractorCo
{
    public class LocationData : ISelectable
    {
        public string       locationId   { get; private set; }
        public string       locationName { get; private set; }
        public LocationType locationType { get; private set; }
        public bool         isWildcard   { get; private set; }

        // Key: ResourceType, Value: abundance 0–3
        // 0 = none  1 = scarce  2 = moderate  3 = abundant
        private readonly Dictionary<ResourceType, int> _availability;

        private bool _isSelected;

        public LocationData(string id, string name, LocationType type,
                            bool isWildcard,
                            Dictionary<ResourceType, int> availability)
        {
            locationId       = id;
            locationName     = name;
            locationType     = type;
            this.isWildcard  = isWildcard;
            _availability    = availability ?? new Dictionary<ResourceType, int>();
        }

        // -------------------------------------------------------
        //  ISelectable
        // -------------------------------------------------------

        public void Select()     => _isSelected = true;
        public void Deselect()   => _isSelected = false;
        public bool IsSelected() => _isSelected;

        // -------------------------------------------------------
        //  Query
        // -------------------------------------------------------

        /// Returns 0–3 abundance for a given resource type.
        public int GetAvailability(ResourceType type) =>
            _availability.TryGetValue(type, out int v) ? v : 0;

        public bool IsResourceAvailable(ResourceType type) =>
            GetAvailability(type) > 0;

        public override string ToString() =>
            $"[Location:{locationId}] {locationName} ({locationType})" +
            (isWildcard ? " [wildcard]" : "");
    }
}
