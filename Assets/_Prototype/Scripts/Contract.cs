// ============================================================
//  Contract.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies (except Mathf)
//
//  One job card shown on the Board.
//  Built by ContractSO.ToContract() — never new'd directly.
//  Implements ISelectable so BoardView can highlight it.
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    public class Contract : ISelectable
    {
        public string        contractId      { get; private set; }
        public ClientFaction faction         { get; private set; }
        public string        displayName     { get; private set; }
        public string        contractorName  { get; private set; }
        public int           payout          { get; private set; }

        private readonly List<ResourceRequirement> _requirements;
        private bool _isSelected;

        public Contract(string id, ClientFaction faction, string displayName,
                        string contractorName, int payout,
                        List<ResourceRequirement> requirements)
        {
            contractId            = id;
            this.faction          = faction;
            this.displayName      = displayName;
            this.contractorName   = contractorName;
            this.payout           = payout;
            _requirements         = requirements ?? new List<ResourceRequirement>();
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

        /// Returns a defensive copy — callers cannot mutate internals.
        public List<ResourceRequirement> GetRequirements() =>
            new List<ResourceRequirement>(_requirements);

        public bool IsCompletable(ResourceInventory inventory) =>
            inventory.CanFulfill(_requirements);

        /// 0.0–1.0: how close the player is to filling this contract
        /// with what is currently in inventory.
        public float GetFulfillmentRatio(ResourceInventory inventory)
        {
            if (_requirements.Count == 0) return 1f;

            float sum = 0f;
            foreach (var req in _requirements)
            {
                int have = Mathf.Min(inventory.GetCount(req.resourceType), req.amount);
                sum += (float)have / req.amount;
            }
            return sum / _requirements.Count;
        }

        public override string ToString() =>
            $"[Contract:{contractId}] {faction} — {displayName}";
    }
}
