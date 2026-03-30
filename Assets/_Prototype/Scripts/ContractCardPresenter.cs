// ============================================================
//  ContractCardPresenter.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — plain C# class (not a MonoBehaviour)
//
//  Owns one cloned ContractCard.uxml TemplateContainer.
//  Queries its named elements, binds Contract data to them,
//  and handles click → MissionController notification.
//
//  Highlighting is done by toggling the USS class "card--selected"
//  on the root element of the clone — no inline styles needed.
// ============================================================

using UnityEngine.UIElements;

namespace ContractorCo
{
    public class ContractCardPresenter
    {
        // ── USS class names ───────────────────────────────────
        private const string CLASS_SELECTED   = "card--selected";
        private const string CLASS_FACTION_PREFIX = "faction--"; // e.g. faction--corporation

        // ── Queried elements ──────────────────────────────────
        private readonly VisualElement _cardRoot;
        private readonly Label         _factionLabel;
        private readonly Label         _contractName;
        private readonly Label         _contractorName;
        private readonly Label         _payoutLabel;
        private readonly VisualElement _requirementsList;

        // ── Bound data ────────────────────────────────────────
        private readonly Contract          _contract;
        private readonly MissionController _missionCtrl;
        private readonly BoardPresenter    _board;

        // -------------------------------------------------------
        //  Constructor
        // -------------------------------------------------------

        public ContractCardPresenter(TemplateContainer clone,
                                     Contract contract,
                                     MissionController missionCtrl,
                                     BoardPresenter board)
        {
            _contract    = contract;
            _missionCtrl = missionCtrl;
            _board       = board;

            // The clone's root is the TemplateContainer itself;
            // the actual card element is its first child.
            _cardRoot         = clone.Q<VisualElement>("contract-card");
            _factionLabel     = clone.Q<Label>("faction-label");
            _contractName     = clone.Q<Label>("contract-name");
            _contractorName   = clone.Q<Label>("contractor-name");
            _payoutLabel      = clone.Q<Label>("payout-label");
            _requirementsList = clone.Q<VisualElement>("requirements-list");

            Render();

            // Register click on the whole card surface
            _cardRoot.RegisterCallback<ClickEvent>(_ => OnClicked());
        }

        // -------------------------------------------------------
        //  Render  (called once on construction)
        // -------------------------------------------------------

        private void Render()
        {
            _factionLabel.text   = _contract.faction.ToString().ToUpper();
            _contractName.text   = _contract.displayName;
            _contractorName.text = _contract.contractorName;
            _payoutLabel.text    = $"c {_contract.payout}";

            // Faction-specific USS class for colour theming in USS
            _cardRoot.AddToClassList($"{CLASS_FACTION_PREFIX}{_contract.faction.ToString().ToLower()}");

            // Requirement rows built in code — one Label per requirement
            _requirementsList.Clear();
            foreach (var req in _contract.GetRequirements())
            {
                var row = new Label(req.ToString());
                row.AddToClassList("requirement-row");
                _requirementsList.Add(row);
            }

            RefreshHighlight();
        }

        // -------------------------------------------------------
        //  Highlight  (toggled by BoardPresenter after any click)
        // -------------------------------------------------------

        public void RefreshHighlight()
        {
            if (_contract.IsSelected())
                _cardRoot.AddToClassList(CLASS_SELECTED);
            else
                _cardRoot.RemoveFromClassList(CLASS_SELECTED);
        }

        // -------------------------------------------------------
        //  Click handler
        // -------------------------------------------------------

        private void OnClicked()
        {
            _missionCtrl.OnContractSelected(_contract);

            // Tell BoardPresenter to refresh all contract card highlights
            // and re-evaluate confirm button state
            _board.RefreshAllContractHighlights();
            _board.OnSelectionChanged();
        }
    }
}
