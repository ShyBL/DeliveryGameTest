// ============================================================
//  ContractCardView.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  Renders one contract card on the Board.
//  On click, notifies MissionController via FindObjectOfType.
//  Updates its own highlight, then tells all other cards to
//  update theirs (so only one card is highlighted at a time).
//
//  Attach to: ContractCard UI prefab
//  Inspector : wire TMP labels, requirement line prefab,
//              highlight Image, and Button
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ContractorCo
{
    public class ContractCardView : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI _factionLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;

        [Header("Requirements")]
        [SerializeField] private Transform       _requirementsContainer;
        [SerializeField] private TextMeshProUGUI _requirementLinePrefab;

        [Header("Selection")]
        [SerializeField] private Image _highlight;
        [SerializeField] private Color _normalColor   = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private Color _selectedColor = new Color(1f, 0.85f, 0f, 0.25f);

        [Header("Interaction")]
        [SerializeField] private Button _button;

        private Contract          _bound;
        private MissionController _missionCtrl;

        // -------------------------------------------------------
        //  Unity lifecycle
        // -------------------------------------------------------

        private void Awake()
        {
            _missionCtrl = FindObjectOfType<MissionController>();
            _button.onClick.AddListener(OnClicked);
        }

        // -------------------------------------------------------
        //  Binding
        // -------------------------------------------------------

        public void Bind(Contract contract)
        {
            _bound = contract;
            Render();
        }

        private void Render()
        {
            _factionLabel.text = _bound.faction.ToString();
            _nameLabel.text    = _bound.displayName;

            // Clear previous requirement lines
            foreach (Transform child in _requirementsContainer)
                Destroy(child.gameObject);

            foreach (var req in _bound.GetRequirements())
            {
                var line = Instantiate(_requirementLinePrefab, _requirementsContainer);
                line.text = req.ToString();
            }

            RefreshHighlight();
        }

        public void RefreshHighlight() =>
            _highlight.color = _bound.IsSelected() ? _selectedColor : _normalColor;

        // -------------------------------------------------------
        //  Interaction
        // -------------------------------------------------------

        private void OnClicked()
        {
            _missionCtrl.OnContractSelected(_bound);

            // Refresh all contract cards so only this one is highlighted
            foreach (var card in FindObjectsOfType<ContractCardView>())
                card.RefreshHighlight();
        }
    }
}
