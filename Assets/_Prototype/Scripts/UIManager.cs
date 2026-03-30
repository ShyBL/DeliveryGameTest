// ============================================================
//  UIManager.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  Single entry point for all UI Toolkit UI.
//  Owns the UIDocument and bootstraps every presenter.
//  GameController talks only to UIManager — never to individual
//  presenters directly.
//
//  Scene setup:
//    1. Create a GameObject "UIManager"
//    2. Attach this script + a UIDocument component
//    3. Assign BoardScreen.uxml to the UIDocument's Source Asset
//    4. Assign HUD.uxml to the HudDocument's Source Asset
//    5. Wire Inspector references below
// ============================================================

using UnityEngine;
using UnityEngine.UIElements;

namespace ContractorCo
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Documents")]
        [Tooltip("UIDocument whose Source Asset is BoardScreen.uxml")]
        [SerializeField] private UIDocument _boardDocument;

        [Tooltip("UIDocument whose Source Asset is HUD.uxml")]
        [SerializeField] private UIDocument _hudDocument;

        [Header("UXML Templates — assign asset references")]
        [SerializeField] private VisualTreeAsset _contractCardTemplate;
        [SerializeField] private VisualTreeAsset _levelCardTemplate;
        [SerializeField] private VisualTreeAsset _checklistRowTemplate;

        [Header("Controllers — for presenter wiring")]
        [SerializeField] private MissionController _missionCtrl;
        [SerializeField] private LevelController   _levelCtrl;

        // Presenters — created in code, not MonoBehaviours
        private BoardPresenter _boardPresenter;
        private HUDPresenter   _hudPresenter;

        // -------------------------------------------------------
        //  Public accessors (GameController uses these)
        // -------------------------------------------------------

        public BoardPresenter Board => _boardPresenter;
        public HUDPresenter   HUD   => _hudPresenter;

        // -------------------------------------------------------
        //  Unity lifecycle
        // -------------------------------------------------------

        private void Awake()
        {
            VisualElement boardRoot = _boardDocument.rootVisualElement;
            VisualElement hudRoot   = _hudDocument.rootVisualElement;

            _boardPresenter = new BoardPresenter(
                boardRoot,
                _contractCardTemplate,
                _levelCardTemplate,
                _missionCtrl,
                _levelCtrl
            );

            _hudPresenter = new HUDPresenter(hudRoot, _checklistRowTemplate);

            // HUD starts hidden — shown when Scavenging begins
            _hudPresenter.Hide();
        }
    }
}
