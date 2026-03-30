// ============================================================
//  Enums.cs
//  Place in: Assets/Scripts/Enums/
//  No dependencies.
// ============================================================

namespace ContractorCo
{
    public enum ResourceType
    {
        Scraps,
        RawMaterials,
        HighEndComponents
    }

    public enum ClientFaction
    {
        Corporation,
        Settler,
        BlackMarket,
        Government
    }

    public enum LocationType
    {
        IndustrialRuin,     // Heavy Scraps              [fixed slot]
        MiningZone,         // Heavy RawMaterials        [fixed slot]
        FrontierColony,     // Mixed spread              [wildcard]
        CorporateOutpost,   // Heavy HighEndComponents   [wildcard]
        BlackSite,          // HighEnd + Scraps          [wildcard]
        MilitaryDepot,      // Raw + HighEnd             [wildcard]
        CrashSite           // Scraps + surprise HighEnd [wildcard]
    }

    public enum GameState
    {
        Board,
        Scavenging
        // Delivering — v2
        // Reward     — v2
    }
}
