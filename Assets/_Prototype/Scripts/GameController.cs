// ============================================================
//  GameController.cs
//  Place in: Assets/_Prototype/Scripts/
//  Layer   : Controller - MonoBehaviour
//
//  Top-level orchestrator and state machine.
//  Owns sub-controllers and UIManager.
//  Drives all state transitions.
//
//  Attach to: a persistent GameController GameObject
// ============================================================

using UnityEngine;

namespace ContractorCo
{
    public class GameController : MonoBehaviour
    {
        [Header("Sub-Controllers")]
        [SerializeField] private MissionController _missionCtrl;
        [SerializeField] private LevelController   _levelCtrl;
        [SerializeField] private PlayerController  _playerCtrl;

        [Header("UI")]
        [SerializeField] private UIManager _uiManager;

        private GameState _state;

        private void Start()
        {
            _missionCtrl.OnContractSelectionChanged += HandleSelectionChanged;
            _levelCtrl.OnLocationSelectionChanged   += HandleSelectionChanged;
            _uiManager.Board.OnConfirmClicked       += HandleBoardConfirmed;

            OpenBoard();
        }

        private void OnDestroy()
        {
            _missionCtrl.OnContractSelectionChanged -= HandleSelectionChanged;
            _levelCtrl.OnLocationSelectionChanged   -= HandleSelectionChanged;
            _uiManager.Board.OnConfirmClicked       -= HandleBoardConfirmed;
        }

        private void ChangeState(GameState next)
        {
            _state = next;
            Debug.Log($"[GameController] State -> {_state}");
        }

        private void OpenBoard()
        {
            ChangeState(GameState.Board);
            var contracts = _missionCtrl.DealContracts(3);
            var locations = _levelCtrl.DealLocations();
            _uiManager.Board.Populate(contracts, locations);
            _uiManager.Board.SetConfirmEnabled(false);
            _uiManager.Board.Show();
            _uiManager.HUD.Hide();
        }

        private void StartScavenging(Contract contract, LocationData location)
        {
            ChangeState(GameState.Scavenging);
            _uiManager.Board.Hide();
            _playerCtrl.SetActiveContract(contract);
            _levelCtrl.LoadLevel(location);
            _uiManager.HUD.Show();
        }

        private void HandleSelectionChanged()
        {
            bool ready = _missionCtrl.GetSelected() != null
                      && _levelCtrl.GetSelected()   != null;
            _uiManager.Board.SetConfirmEnabled(ready);
        }

        private void HandleBoardConfirmed()
        {
            Contract     contract = _missionCtrl.GetSelected();
            LocationData location = _levelCtrl.GetSelected();

            if (contract == null || location == null)
            {
                Debug.LogWarning("[GameController] Confirm fired but selection is incomplete.");
                return;
            }
            StartScavenging(contract, location);
        }

        public void OnRunComplete()
        {
            _playerCtrl.GetInventory().Clear();
            _missionCtrl.ClearSelection();
            _levelCtrl.ClearSelection();
            OpenBoard();
        }
    }
}
