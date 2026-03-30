// ============================================================
//  ResourceInventory.cs
//  Place in: Assets/Scripts/Model/
//  Layer   : Model — no Unity dependencies
//
//  The player's current haul. Single source of truth for
//  what is being carried. Fires OnChanged so HUDView can
//  subscribe and refresh without polling.
// ============================================================

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ContractorCo
{
    public class ResourceInventory
    {
        // Views subscribe here — fired on any Add, Remove, or Clear
        public event Action OnChanged;

        private readonly Dictionary<ResourceType, int> _stock =
            new Dictionary<ResourceType, int>();

        public int maxCapacity { get; private set; }

        public ResourceInventory(int maxCapacity = 20)
        {
            this.maxCapacity = maxCapacity;

            // Seed every type at 0 so GetCount never KeyNotFound-throws
            foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
                _stock[t] = 0;
        }

        // -------------------------------------------------------
        //  Mutation
        // -------------------------------------------------------

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0) return;
            _stock[type] += amount;
            Debug.Log($"[Inventory] +{amount} {type}  ({GetTotal()}/{maxCapacity})");
            OnChanged?.Invoke();
        }

        public bool Remove(ResourceType type, int amount)
        {
            if (!Has(type, amount))
            {
                Debug.LogWarning($"[Inventory] Remove failed — need {amount} {type}, have {_stock[type]}");
                return false;
            }
            _stock[type] -= amount;
            OnChanged?.Invoke();
            return true;
        }

        public void Clear()
        {
            foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
                _stock[t] = 0;
            OnChanged?.Invoke();
        }

        // -------------------------------------------------------
        //  Query
        // -------------------------------------------------------

        public int  GetCount(ResourceType type)        => _stock[type];
        public bool Has(ResourceType type, int amount) => _stock[type] >= amount;
        public bool IsAtCapacity()                     => GetTotal() >= maxCapacity;

        public int GetTotal()
        {
            int n = 0;
            foreach (int v in _stock.Values) n += v;
            return n;
        }

        public bool CanFulfill(List<ResourceRequirement> requirements)
        {
            foreach (var req in requirements)
                if (!Has(req.resourceType, req.amount)) return false;
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("[Inventory] ");
            foreach (var kv in _stock)
                sb.Append($"{kv.Key}:{kv.Value}  ");
            return sb.ToString();
        }
    }
}
