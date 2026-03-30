// ============================================================
//  LevelCardView.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  Renders one location card on the Board.
//  On click, notifies LevelController via FindObjectOfType.
//  Updates its own highlight, then refreshes all other cards.
//
//  Attach to: LevelCard UI prefab
//  Inspector : wire TMP labels, availability line prefab,
//              highlight Image, and Button
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ContractorCo
{
    public class LevelCardView : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private TextMeshProUGUI _typeLabel;

        [Header("Availability Lines")]
        [SerializeField] private Transform       _availabilityContainer;
        [SerializeField] private TextMeshProUGUI _availabilityLinePrefab;

        [Header("Selection")]
        [SerializeField] private Image _highlight;
        [SerializeField] private Color _normalColor   = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private Color _selectedColor = new Color(0f, 0.8f, 1f, 0.25f);

        [Header("Interaction")]
        [SerializeField] private Button _button;

        private LocationData    _bound;
        private LevelController _levelCtrl;

        private static readonly string[] AbundanceLabels =
            { "none", "scarce", "moderate", "abundant" };

        // -------------------------------------------------------
        //  Unity lifecycle
        // -------------------------------------------------------

        private void Awake()
        {
            _levelCtrl = FindObjectOfType<LevelController>();
            _button.onClick.AddListener(OnClicked);
        }

        // -------------------------------------------------------
        //  Binding
        // -------------------------------------------------------

        public void Bind(LocationData location)
        {
            _bound = location;
            Render();
        }

        private void Render()
        {
            _nameLabel.text = _bound.locationName;
            _typeLabel.text = _bound.locationType.ToString() +
                              (_bound.isWildcard ? "  [wildcard]" : "");

            foreach (Transform child in _availabilityContainer)
                Destroy(child.gameObject);

            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                int abundance = _bound.GetAvailability(type);
                var line      = Instantiate(_availabilityLinePrefab, _availabilityContainer);
                line.text     = $"{type}: {AbundanceLabels[abundance]}";
                line.color    = abundance == 0 ? Color.gray : Color.white;
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
            _levelCtrl.OnLocationSelected(_bound);

            // Refresh all level cards so only this one is highlighted
            foreach (var card in FindObjectsOfType<LevelCardView>())
                card.RefreshHighlight();
        }
    }
}
