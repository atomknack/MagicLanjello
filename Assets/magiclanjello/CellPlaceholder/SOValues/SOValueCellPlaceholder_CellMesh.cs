using UnityEngine;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Preconcrete.Values;
using DoubleEngine;
using DoubleEngine.Atom;


namespace MagicLanjello.CellPlaceholder.SOValues
{
    [CreateAssetMenu(fileName = "SOValueCellPlaceholder_CellMesh", menuName = "MagicLanjello/CellPlaceholder/SOValue<short> - CellMesh", order = 110)]
    public sealed class SOValueCellPlaceholder_CellMesh : SOValueImmutableWithSubscribingAction<short>
    {
        [SerializeField][ValidReference] private SOEvent<short> _cellMeshChanged;
        [SerializeField][ValidReference] private SOEvent _nextCellMesh;

        [System.NonSerialized]
        private bool _subscribedToDependency = false;

        [System.NonSerialized]
        private short _cellMesh = CellPlaceholderStruct.DefaultPlaceholder.cellMesh;

        public override short GetValue() => _cellMesh;

        private void SetCellMeshWithInvokeSubscribers(short value)
        {
            _cellMesh = ThreeDimensionalCellMeshes.ValidateMeshId(value);
            InvokeSubscribers(this, _cellMesh);
        }
        private void NextCellMeshId() => 
            SetCellMeshWithInvokeSubscribers((short)((int)_cellMesh).NextIntCyclic(ThreeDimensionalCellMeshes.GetCount(), 0));
        
        protected override void AfterUnsubscribing()
        {
            if (_subscribedToDependency == false)
                return;
            if (SubscribersCount() > 0)
                return;

            _cellMeshChanged.UnsubscribeNullSafe(SetCellMeshWithInvokeSubscribers);
            _nextCellMesh.UnsubscribeNullSafe(NextCellMeshId);

            _subscribedToDependency = false;
        }

        protected override void BeforeSubscribing()
        {
            if (_subscribedToDependency)
                return;

            NullChecks();
            _cellMeshChanged.Subscribe(SetCellMeshWithInvokeSubscribers);
            _nextCellMesh.Subscribe(NextCellMeshId);

            _subscribedToDependency = true;
        }

        private void NullChecks()
        {
            if (_cellMeshChanged == null) throw new System.ArgumentNullException(nameof(_cellMeshChanged));
            if (_nextCellMesh == null) throw new System.ArgumentNullException(nameof(_nextCellMesh));
        }
    }
}