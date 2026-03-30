// ============================================================
//  HUDView.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  In-level heads-up display. Shows:
//    • Current inventory counts per ResourceType
//    • Contract checklist (✓ when requirement met)
//
//  Updated by PlayerController via:
//    UpdateInventory()  — called from ResourceInventory.OnChanged event
//    UpdateChecklist()  — called after each collection
//
//  Attach to: HUD Canvas (always-on-screen overlay)
//  Inspector : assign TMP labels and checklist container
// ============================================================

using UnityEngine;
using TMPro;

namespace ContractorCo
{
    public class HUDView : MonoBehaviour
    {
        [Header("Inventory Counts")]
        [SerializeField] private TextMeshProUGUI _scrapsCount;
        [SerializeField] private TextMeshProUGUI _rawCount;
        [SerializeField] private TextMeshProUGUI _hiEndCount;
        [SerializeField] private TextMeshProUGUI _totalCount;

        [Header("Contract Checklist")]
        [SerializeField] private Transform       _checklistContainer;
        [SerializeField] private TextMeshProUGUI _checklistLinePrefab;

        // -------------------------------------------------------
        //  Inventory  (driven by ResourceInventory.OnChanged)
        // -------------------------------------------------------

        public void UpdateInventory(ResourceInventory inventory)
        {
            _scrapsCount.text = inventory.GetCount(ResourceType.Scraps).ToString();
            _rawCount.text    = inventory.GetCount(ResourceType.RawMaterials).ToString();
            _hiEndCount.text  = inventory.GetCount(ResourceType.HighEndComponents).ToString();
            _totalCount.text  = $"{inventory.GetTotal()} / {inventory.maxCapacity}";
        }

        // -------------------------------------------------------
        //  Contract Checklist  (driven by PlayerController)
        // -------------------------------------------------------

        public void UpdateChecklist(Contract contract, ResourceInventory inventory)
        {
            // Clear old lines
            foreach (Transform child in _checklistContainer)
                Destroy(child.gameObject);

            if (contract == null) return;

            foreach (var req in contract.GetRequirements())
            {
                int  have = inventory.GetCount(req.resourceType);
                bool met  = have >= req.amount;

                var line   = Instantiate(_checklistLinePrefab, _checklistContainer);
                line.text  = $"{(met ? "✓" : "○")}  {req.resourceType}  {have} / {req.amount}";
                line.color = met ? Color.green : Color.white;
            }
        }
    }
}
