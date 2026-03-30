// ============================================================
//  BoardView.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  The central selection screen shown at the start of each run.
//  Instantiates ContractCardView and LevelCardView prefabs,
//  and fires OnConfirmClicked up to GameController.
//
//  Attach to: Board UI Panel (Canvas child)
//  Inspector : assign card prefabs, containers, confirm button
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    public class BoardView : MonoBehaviour
    {
        [Header("Card Containers")]
        [Tooltip("Parent transform for contract cards (use HorizontalLayoutGroup)")]
        [SerializeField] private Transform _contractContainer;
        [Tooltip("Parent transform for level cards (use HorizontalLayoutGroup)")]
        [SerializeField] private Transform _locationContainer;

        [Header("Card Prefabs")]
        [SerializeField] private ContractCardView _contractCardPrefab;
        [SerializeField] private LevelCardView    _levelCardPrefab;

        [Header("Confirm Button")]
        [SerializeField] private ConfirmButtonView _confirmButton;

        // GameController subscribes to this
        public event System.Action OnConfirmClicked;

        private readonly List<ContractCardView> _contractCards = new List<ContractCardView>();
        private readonly List<LevelCardView>    _levelCards    = new List<LevelCardView>();

        // -------------------------------------------------------
        //  Unity lifecycle
        // -------------------------------------------------------

        private void Start()
        {
            _confirmButton.OnClicked += () => OnConfirmClicked?.Invoke();
        }

        // -------------------------------------------------------
        //  Population
        // -------------------------------------------------------

        /// Called by GameController at the start of each run.
        /// Clears old cards and instantiates fresh ones.
        public void Populate(List<Contract> contracts, List<LocationData> locations)
        {
            ClearCards();

            foreach (var contract in contracts)
            {
                var card = Instantiate(_contractCardPrefab, _contractContainer);
                card.Bind(contract);
                _contractCards.Add(card);
            }

            foreach (var location in locations)
            {
                var card = Instantiate(_levelCardPrefab, _locationContainer);
                card.Bind(location);
                _levelCards.Add(card);
            }
        }

        // -------------------------------------------------------
        //  Visibility / state
        // -------------------------------------------------------

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void SetConfirmEnabled(bool enabled) =>
            _confirmButton.SetEnabled(enabled);

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private void ClearCards()
        {
            foreach (var c in _contractCards) if (c != null) Destroy(c.gameObject);
            foreach (var l in _levelCards)    if (l != null) Destroy(l.gameObject);
            _contractCards.Clear();
            _levelCards.Clear();
        }
    }
}
