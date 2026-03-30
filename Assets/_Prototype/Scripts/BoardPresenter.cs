// ============================================================
//  BoardPresenter.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — plain C# class (not a MonoBehaviour)
//
//  Manages the BoardScreen.uxml tree at runtime.
//  Clones ContractCard.uxml and LevelCard.uxml into their
//  respective list containers. Wires the confirm button.
//  Fires OnConfirmClicked up to GameController.
//
//  Does NOT know about game logic — only queries, populates,
//  shows, hides, and fires events.
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContractorCo
{
    public class BoardPresenter
    {
        // GameController subscribes to this
        public event System.Action OnConfirmClicked;

        // ── Queried elements ───────────────────────────────────
        private readonly VisualElement _root;
        private readonly VisualElement _contractsList;
        private readonly VisualElement _locationsList;
        private readonly Button        _confirmButton;
        private readonly Label         _confirmSummary;

        // ── Templates ─────────────────────────────────────────
        private readonly VisualTreeAsset _contractCardTemplate;
        private readonly VisualTreeAsset _levelCardTemplate;

        // ── Controllers ───────────────────────────────────────
        private readonly MissionController _missionCtrl;
        private readonly LevelController   _levelCtrl;

        // ── Child presenters ──────────────────────────────────
        private readonly List<ContractCardPresenter> _contractCards = new List<ContractCardPresenter>();
        private readonly List<LevelCardPresenter>    _levelCards    = new List<LevelCardPresenter>();

        // ── USS class names ───────────────────────────────────
        private const string CLASS_BTN_DISABLED = "confirm-button--disabled";
        private const string CLASS_BTN_ENABLED  = "confirm-button--enabled";

        // -------------------------------------------------------
        //  Constructor
        // -------------------------------------------------------

        public BoardPresenter(VisualElement root,
                              VisualTreeAsset contractCardTemplate,
                              VisualTreeAsset levelCardTemplate,
                              MissionController missionCtrl,
                              LevelController levelCtrl)
        {
            _root                 = root;
            _contractCardTemplate = contractCardTemplate;
            _levelCardTemplate    = levelCardTemplate;
            _missionCtrl          = missionCtrl;
            _levelCtrl            = levelCtrl;

            // Query structural elements once at construction
            _contractsList = root.Q<VisualElement>("contracts-list");
            _locationsList = root.Q<VisualElement>("locations-list");
            _confirmButton = root.Q<Button>("confirm-button");
            _confirmSummary = root.Q<Label>("confirm-summary");

            _confirmButton.clicked += () => OnConfirmClicked?.Invoke();

            SetConfirmEnabled(false);
        }

        // -------------------------------------------------------
        //  Population  (called by GameController each new run)
        // -------------------------------------------------------

        public void Populate(List<Contract> contracts, List<LocationData> locations)
        {
            ClearCards();

            foreach (var contract in contracts)
            {
                // Clone the UXML template — gets a fresh TemplateContainer
                TemplateContainer clone = _contractCardTemplate.Instantiate();
                _contractsList.Add(clone);

                var presenter = new ContractCardPresenter(clone, contract, _missionCtrl, this);
                _contractCards.Add(presenter);
            }

            foreach (var location in locations)
            {
                TemplateContainer clone = _levelCardTemplate.Instantiate();
                _locationsList.Add(clone);

                var presenter = new LevelCardPresenter(clone, location, _levelCtrl, this);
                _levelCards.Add(presenter);
            }
        }

        // -------------------------------------------------------
        //  Called by card presenters after any selection change
        // -------------------------------------------------------

        public void OnSelectionChanged()
        {
            bool ready = _missionCtrl.GetSelected() != null
                      && _levelCtrl.GetSelected()   != null;

            SetConfirmEnabled(ready);

            // Update summary label
            if (ready)
            {
                string contract = _missionCtrl.GetSelected().displayName;
                string location = _levelCtrl.GetSelected().locationName;
                _confirmSummary.text = $"{contract}  ×  {location}";
            }
            else
            {
                _confirmSummary.text = "Select a contract and a location to begin.";
            }
        }

        // -------------------------------------------------------
        //  Visibility
        // -------------------------------------------------------

        public void Show() => _root.style.display = DisplayStyle.Flex;
        public void Hide() => _root.style.display = DisplayStyle.None;

        // -------------------------------------------------------
        //  Confirm button state
        // -------------------------------------------------------

        public void SetConfirmEnabled(bool enabled)
        {
            _confirmButton.SetEnabled(enabled);

            if (enabled)
            {
                _confirmButton.RemoveFromClassList(CLASS_BTN_DISABLED);
                _confirmButton.AddToClassList(CLASS_BTN_ENABLED);
            }
            else
            {
                _confirmButton.RemoveFromClassList(CLASS_BTN_ENABLED);
                _confirmButton.AddToClassList(CLASS_BTN_DISABLED);
            }
        }

        // -------------------------------------------------------
        //  Refresh all card highlights (called after selection)
        // -------------------------------------------------------

        public void RefreshAllContractHighlights()
        {
            foreach (var card in _contractCards) card.RefreshHighlight();
        }

        public void RefreshAllLevelHighlights()
        {
            foreach (var card in _levelCards) card.RefreshHighlight();
        }

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private void ClearCards()
        {
            _contractsList.Clear();
            _locationsList.Clear();
            _contractCards.Clear();
            _levelCards.Clear();
        }
    }
}
