// ============================================================
//  LevelController.cs
//  Place in: Assets/Scripts/Controllers/
//  Layer   : Controller — MonoBehaviour
//
//  Manages location pools and selection.
//  Always deals exactly 3 locations per run:
//    slot 0 → first fixed SO  (e.g. IndustrialRuin)
//    slot 1 → second fixed SO (e.g. MiningZone)
//    slot 2 → random wildcard drawn from wildcardPool
//
//  Attach to: GameController GameObject (or child)
//  Inspector : assign LocationSO assets to both lists
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace ContractorCo
{
    public class LevelController : MonoBehaviour
    {
        [Header("Fixed Slots (always dealt — assign 2 SOs)")]
        [Tooltip("Recommend: slot 0 = IndustrialRuin, slot 1 = MiningZone")]
        [SerializeField] private List<LocationSO> _fixedLocations;

        [Header("Wildcard Pool (1 drawn randomly per run)")]
        [SerializeField] private List<LocationSO> _wildcardPool;

        [Header("Spawner")]
        [SerializeField] private LocationResourceSpawner _spawner;

        // GameController subscribes to this
        public event System.Action OnLocationSelectionChanged;

        private List<LocationData> _dealtLocations = new List<LocationData>();
        private LocationData       _selected;

        // -------------------------------------------------------
        //  Deal
        // -------------------------------------------------------

        /// Builds the 3-location board offer for this run.
        /// Fixed slots first, then one wildcard.
        public List<LocationData> DealLocations()
        {
            _dealtLocations.Clear();
            _selected = null;

            if (_fixedLocations == null || _fixedLocations.Count == 0)
            {
                Debug.LogError("[LevelController] fixedLocations is empty — assign LocationSOs in Inspector.");
                return _dealtLocations;
            }

            foreach (var so in _fixedLocations)
                _dealtLocations.Add(so.ToLocationData());

            if (_wildcardPool != null && _wildcardPool.Count > 0)
                _dealtLocations.Add(DrawWildcard().ToLocationData());
            else
                Debug.LogWarning("[LevelController] wildcardPool is empty — only fixed slots dealt.");

            Debug.Log($"[LevelController] Dealt {_dealtLocations.Count} locations.");
            return new List<LocationData>(_dealtLocations);
        }

        // -------------------------------------------------------
        //  Selection  (called by LevelCardView)
        // -------------------------------------------------------

        public void OnLocationSelected(LocationData location)
        {
            if (_selected != null) _selected.Deselect();
            _selected = location;
            _selected.Select();
            Debug.Log($"[LevelController] Selected → {location}");
            OnLocationSelectionChanged?.Invoke();
        }

        public LocationData GetSelected() => _selected;

        public void ClearSelection()
        {
            if (_selected != null) _selected.Deselect();
            _selected = null;
            OnLocationSelectionChanged?.Invoke();
        }

        // -------------------------------------------------------
        //  Load  (called by GameController after board confirm)
        // -------------------------------------------------------

        /// Spawns resource nodes for the chosen location.
        /// Extend here in v2 to trigger scene transitions.
        public void LoadLevel(LocationData location)
        {
            Debug.Log($"[LevelController] Loading {location}");
            _spawner.SpawnNodes(location);
            // TODO v2: AsyncOperation scene load here
        }

        // -------------------------------------------------------
        //  Helpers
        // -------------------------------------------------------

        private LocationSO DrawWildcard()
        {
            int idx = Random.Range(0, _wildcardPool.Count);
            return _wildcardPool[idx];
        }
    }
}
