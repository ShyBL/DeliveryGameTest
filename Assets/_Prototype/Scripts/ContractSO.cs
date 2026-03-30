// ============================================================
//  ContractSO.cs
//  Place in: Assets/_Prototype/Scripts/
//  Layer   : ScriptableObject (data authoring)
//
//  How to create:
//    Right-click in Project > Create > ContractorCo > ContractSO
//    Fill Inspector fields
//    Assign to MissionController.contractPool list
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    [CreateAssetMenu(menuName = "ContractorCo/ContractSO", fileName = "New Contract")]
    public class ContractSO : ScriptableObject
    {
        [Header("Identity")]
        public string        displayName;
        public string        contractorName;
        public ClientFaction faction;
        public int           payout;

        [Header("Requirements")]
        public List<ResourceRequirement> requirements;

        /// Creates a fresh runtime Contract every time it is called.
        /// MissionController calls this once per deal per SO.
        public Contract ToContract()
        {
            string id = $"{faction}_{System.Guid.NewGuid().ToString().Substring(0, 6)}";
            return new Contract(
                id,
                faction,
                displayName,
                contractorName,
                payout,
                new List<ResourceRequirement>(requirements)
            );
        }
    }
}
