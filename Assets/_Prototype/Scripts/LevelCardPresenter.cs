// ============================================================
//  LevelCardPresenter.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — plain C# class (not a MonoBehaviour)
//
//  Owns one cloned LevelCard.uxml TemplateContainer.
//  Queries its named elements, binds LocationData to them,
//  and handles click → LevelController notification.
//
//  Highlighting: USS class "card--selected" toggled on card root.
//  Availability rows: built in code as Labels (dynamic count).
//  Wildcard badge: shown/hidden via display style in code.
// ============================================================

using UnityEngine.UIElements;

namespace ContractorCo
{
    public class LevelCardPresenter
    {
        // ── USS class names ───────────────────────────────────
        private const string CLASS_SELECTED       = "card--selected";
        private const string CLASS_AVAILABILITY_MET  = "availability--present";
        private const string CLASS_AVAILABILITY_NONE = "availability--absent";

        // Abundance index → readable label
        private static readonly string[] AbundanceLabels =
            { "none", "scarce", "moderate", "abundant" };

        // ── Queried elements ──────────────────────────────────
        private readonly VisualElement _cardRoot;
        private readonly Label         _locationName;
        private readonly Label         _locationType;
        private readonly Label         _wildcardBadge;
        private readonly VisualElement _availabilityList;

        // ── Bound data ────────────────────────────────────────
        private readonly LocationData   _location;
        private readonly LevelController _levelCtrl;
        private readonly BoardPresenter  _board;

        // -------------------------------------------------------
        //  Constructor
        // -------------------------------------------------------

        public LevelCardPresenter(TemplateContainer clone,
                                  LocationData location,
                                  LevelController levelCtrl,
                                  BoardPresenter board)
        {
            _location  = location;
            _levelCtrl = levelCtrl;
            _board     = board;

            _cardRoot         = clone.Q<VisualElement>("level-card");
            _locationName     = clone.Q<Label>("location-name");
            _locationType     = clone.Q<Label>("location-type");
            _wildcardBadge    = clone.Q<Label>("wildcard-badge");
            _availabilityList = clone.Q<VisualElement>("availability-list");

            Render();

            _cardRoot.RegisterCallback<ClickEvent>(_ => OnClicked());
        }

        // -------------------------------------------------------
        //  Render
        // -------------------------------------------------------

        private void Render()
        {
            _locationName.text  = _location.locationName;
            _locationType.text  = _location.locationType.ToString();

            // Show wildcard badge only when relevant
            _wildcardBadge.style.display = _location.isWildcard
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            // Build one availability row per ResourceType in code
            _availabilityList.Clear();
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                int abundance = _location.GetAvailability(type);

                var row = new VisualElement();
                row.AddToClassList("availability-row");

                var typeLabel = new Label(type.ToString());
                typeLabel.AddToClassList("availability-type");

                var valueLabel = new Label(AbundanceLabels[abundance]);
                valueLabel.AddToClassList("availability-value");
                valueLabel.AddToClassList(abundance > 0
                    ? CLASS_AVAILABILITY_MET
                    : CLASS_AVAILABILITY_NONE);

                row.Add(typeLabel);
                row.Add(valueLabel);
                _availabilityList.Add(row);
            }

            RefreshHighlight();
        }

        // -------------------------------------------------------
        //  Highlight
        // -------------------------------------------------------

        public void RefreshHighlight()
        {
            if (_location.IsSelected())
                _cardRoot.AddToClassList(CLASS_SELECTED);
            else
                _cardRoot.RemoveFromClassList(CLASS_SELECTED);
        }

        // -------------------------------------------------------
        //  Click handler
        // -------------------------------------------------------

        private void OnClicked()
        {
            _levelCtrl.OnLocationSelected(_location);
            _board.RefreshAllLevelHighlights();
            _board.OnSelectionChanged();
        }
    }
}
