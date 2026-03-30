// ============================================================
//  HUDPresenter.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — plain C# class (not a MonoBehaviour)
//
//  Manages the HUD.uxml tree during the Scavenging state.
//  Subscribes to ResourceInventory.OnChanged so the inventory
//  panel refreshes automatically on every collection.
//  The checklist is rebuilt from scratch each time since
//  requirement count is small and fixed per contract.
//
//  Checklist rows use ChecklistRow.uxml clones.
//  "row--met" USS class toggled in code for check-mark/color.
// ============================================================

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ContractorCo
{
    public class HUDPresenter
    {
        // ── USS class names ───────────────────────────────────
        private const string CLASS_ROW_MET  = "row--met";
        private const string ICON_MET       = "✓";
        private const string ICON_UNMET     = "○";

        // ── Queried elements ──────────────────────────────────
        private readonly VisualElement _root;
        private readonly Label         _contractNameLabel;
        private readonly VisualElement _checklistRows;
        private readonly Label         _scrapsCount;
        private readonly Label         _rawCount;
        private readonly Label         _hiEndCount;
        private readonly Label         _totalCount;

        // ── Template ──────────────────────────────────────────
        private readonly VisualTreeAsset _checklistRowTemplate;

        // ── State ─────────────────────────────────────────────
        private Contract          _activeContract;
        private ResourceInventory _inventory;

        // -------------------------------------------------------
        //  Constructor
        // -------------------------------------------------------

        public HUDPresenter(VisualElement root, VisualTreeAsset checklistRowTemplate)
        {
            _root                 = root;
            _checklistRowTemplate = checklistRowTemplate;

            _contractNameLabel = root.Q<Label>("contract-name-label");
            _checklistRows     = root.Q<VisualElement>("checklist-rows");
            _scrapsCount       = root.Q<Label>("scraps-count");
            _rawCount          = root.Q<Label>("raw-count");
            _hiEndCount        = root.Q<Label>("hiend-count");
            _totalCount        = root.Q<Label>("total-count");
        }

        // -------------------------------------------------------
        //  Initialise for a run  (called by GameController / PlayerController)
        // -------------------------------------------------------

        public void SetContract(Contract contract, ResourceInventory inventory)
        {
            // Unsubscribe from previous inventory if any
            if (_inventory != null)
                _inventory.OnChanged -= OnInventoryChanged;

            _activeContract = contract;
            _inventory      = inventory;

            // Subscribe — HUD now auto-refreshes on every Add/Remove/Clear
            _inventory.OnChanged += OnInventoryChanged;

            _contractNameLabel.text = contract.displayName;

            RebuildChecklist();
            RefreshInventoryCounts();
        }

        // -------------------------------------------------------
        //  Auto-refresh from inventory event
        // -------------------------------------------------------

        private void OnInventoryChanged()
        {
            RefreshInventoryCounts();
            RefreshChecklistMet();   // re-evaluate ✓ states without full rebuild
        }

        // -------------------------------------------------------
        //  Inventory panel
        // -------------------------------------------------------

        private void RefreshInventoryCounts()
        {
            if (_inventory == null) return;

            _scrapsCount.text = _inventory.GetCount(ResourceType.Scraps).ToString();
            _rawCount.text    = _inventory.GetCount(ResourceType.RawMaterials).ToString();
            _hiEndCount.text  = _inventory.GetCount(ResourceType.HighEndComponents).ToString();
            _totalCount.text  = $"{_inventory.GetTotal()} / {_inventory.maxCapacity}";
        }

        // -------------------------------------------------------
        //  Checklist — full rebuild (called when contract changes)
        // -------------------------------------------------------

        private void RebuildChecklist()
        {
            _checklistRows.Clear();

            if (_activeContract == null) return;

            foreach (var req in _activeContract.GetRequirements())
            {
                TemplateContainer clone = _checklistRowTemplate.Instantiate();

                var rowRoot  = clone.Q<VisualElement>("checklist-row");
                var icon     = clone.Q<Label>("check-icon");
                var text     = clone.Q<Label>("requirement-text");

                text.text = req.ToString();

                // Tag with resource type so RefreshChecklistMet can find rows
                rowRoot.userData = req.resourceType;

                _checklistRows.Add(clone);
            }

            RefreshChecklistMet();
        }

        // -------------------------------------------------------
        //  Checklist — lightweight refresh (called on inventory change)
        // -------------------------------------------------------

        private void RefreshChecklistMet()
        {
            if (_activeContract == null || _inventory == null) return;

            var requirements = _activeContract.GetRequirements();

            // Walk cloned rows in order — same order as requirements list
            int idx = 0;
            foreach (VisualElement child in _checklistRows.Children())
            {
                if (idx >= requirements.Count) break;

                var req  = requirements[idx];
                bool met = _inventory.Has(req.resourceType, req.amount);

                var rowRoot = child.Q<VisualElement>("checklist-row");
                var icon    = child.Q<Label>("check-icon");

                icon.text = met ? ICON_MET : ICON_UNMET;

                if (met)
                    rowRoot.AddToClassList(CLASS_ROW_MET);
                else
                    rowRoot.RemoveFromClassList(CLASS_ROW_MET);

                idx++;
            }
        }

        // -------------------------------------------------------
        //  Visibility
        // -------------------------------------------------------

        public void Show() => _root.style.display = DisplayStyle.Flex;

        public void Hide()
        {
            // Unsubscribe from inventory events when HUD is hidden
            if (_inventory != null)
                _inventory.OnChanged -= OnInventoryChanged;

            _root.style.display = DisplayStyle.None;
        }
    }
}
