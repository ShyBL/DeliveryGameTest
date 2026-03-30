// ============================================================
//  ISelectable.cs
//  Place in: Assets/Scripts/Interfaces/
//  Implemented by: Contract, LocationData
// ============================================================

namespace ContractorCo
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
        bool IsSelected();
    }
}
