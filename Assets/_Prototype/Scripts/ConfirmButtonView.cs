// ============================================================
//  ConfirmButtonView.cs
//  Place in: Assets/Scripts/Views/
//  Layer   : View — MonoBehaviour
//
//  The "Deploy" / confirm button on the Board.
//  Disabled until both a contract and a location are selected.
//  Fires OnClicked up to BoardView, which passes it to
//  GameController via its own OnConfirmClicked event.
//
//  Attach to: Confirm Button GameObject (Canvas child)
//  Inspector : assign Button component and TMP label
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ContractorCo
{
    public class ConfirmButtonView : MonoBehaviour
    {
        [SerializeField] private Button          _button;
        [SerializeField] private TextMeshProUGUI _label;

        [Header("Label Text")]
        [SerializeField] private string _enabledText  = "DEPLOY";
        [SerializeField] private string _disabledText = "Select a contract and location";

        [Header("Colors")]
        [SerializeField] private Color _enabledColor  = Color.white;
        [SerializeField] private Color _disabledColor = new Color(1f, 1f, 1f, 0.3f);

        // BoardView subscribes to this
        public event System.Action OnClicked;

        // -------------------------------------------------------
        //  Unity lifecycle
        // -------------------------------------------------------

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClicked?.Invoke());
            SetEnabled(false);  // disabled by default
        }

        // -------------------------------------------------------
        //  Public API  (called by BoardView)
        // -------------------------------------------------------

        public void SetEnabled(bool enabled)
        {
            _button.interactable = enabled;
            _label.text          = enabled ? _enabledText : _disabledText;
            _label.color         = enabled ? _enabledColor : _disabledColor;
        }

        public void SetLabel(string text) => _label.text = text;
    }
}
