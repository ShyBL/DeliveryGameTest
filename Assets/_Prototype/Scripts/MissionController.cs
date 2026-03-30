// ============================================================
//  MissionController.cs
//  Place in: Assets/Scripts/Controllers/
//  Layer   : Controller — MonoBehaviour
//
//  Manages the contract pool. Deals N random contracts per run.
//  Enforces single selection — deselects previous on new pick.
//  Fires OnContractSelectionChanged so GameController can
//  enable/disable the confirm button.
//
//  Attach to: GameController GameObject (or child)
//  Inspector : assign ContractSO assets to contractPool list
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    public class MissionController : MonoBehaviour
    {
        [Header("Contract Pool")]
        [Tooltip("Assign all ContractSO assets here. DealContracts() picks randomly.")]
        [SerializeField] private List<ContractSO> _contractPool;

        // GameController subscribes to this
        public event System.Action OnContractSelectionChanged;

        private List<Contract> _dealtContracts = new List<Contract>();
        private Contract       _selected;

        // -------------------------------------------------------
        //  Deal
        // -------------------------------------------------------

        /// Shuffle-picks 'count' contracts from the pool and converts
        /// them to runtime Contract objects. Clears previous selection.
        public List<Contract> DealContracts(int count)
        {
            _dealtContracts.Clear();
            _selected = null;

            if (_contractPool == null || _contractPool.Count == 0)
            {
                Debug.LogError("[MissionController] contractPool is empty — assign ContractSOs in Inspector.");
                return _dealtContracts;
            }

            var shuffled = new List<ContractSO>(_contractPool);
            Shuffle(shuffled);

            int take = Mathf.Min(count, shuffled.Count);
            for (int i = 0; i < take; i++)
                _dealtContracts.Add(shuffled[i].ToContract());

            Debug.Log($"[MissionController] Dealt {_dealtContracts.Count} contracts.");
            return new List<Contract>(_dealtContracts);
        }

        // -------------------------------------------------------
        //  Selection  (called by ContractCardView)
        // -------------------------------------------------------

        public void OnContractSelected(Contract contract)
        {
            if (_selected != null) _selected.Deselect();
            _selected = contract;
            _selected.Select();
            Debug.Log($"[MissionController] Selected → {contract}");
            OnContractSelectionChanged?.Invoke();
        }

        public Contract GetSelected() => _selected;

        public void ClearSelection()
        {
            if (_selected != null) _selected.Deselect();
            _selected = null;
            OnContractSelectionChanged?.Invoke();
        }

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
