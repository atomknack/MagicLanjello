using UnityEngine;
using UKnack.Values;
using UKnack.Preconcrete.Values;
using UKnack.Attributes;
using System;
using UKnack.Events;

namespace MagicLanjello.CellPlaceholder.SOValues
{

    [CreateAssetMenu(fileName = "SOValueCellPlaceholder_CellPlaceholderStruct", menuName = "MagicLanjello/CellPlaceholder/SOValue<CellPlaceholderStruct>", order = 110)]
    public sealed class SOValueCellPlaceholder_CellPlaceholderStruct : SOValueImmutableWithSubscribingAction<CellPlaceholderStruct>
    {
        [SerializeField]
        [ValidReference]
        private SOValue<int> _orientation;

        [SerializeField]
        [ValidReference]
        private SOValue<short> _cellMesh;

        [SerializeField]
        [ValidReference]
        private SOValue<byte> _material;

        [NonSerialized]
        private bool _subscribedToDependency = false;

        public override CellPlaceholderStruct GetValue() =>
            new CellPlaceholderStruct(_orientation.GetValue(), _cellMesh.GetValue(), _material.GetValue());


        protected override void AfterUnsubscribing()
        {
            if (_subscribedToDependency == false)
                return;
            if (SubscribersCount() > 0)
                return;

            _orientation.UnsubscribeNullSafe(OrinetationChanged);
            _cellMesh.UnsubscribeNullSafe(CellMeshChanged);
            _material.UnsubscribeNullSafe(MaterialChanged);

            _subscribedToDependency = false;
            }

        protected override void BeforeSubscribing()
        {
            if (_subscribedToDependency)
                return;

            if (_orientation == null)
                throw new System.ArgumentNullException(nameof(_orientation));
            if (_cellMesh == null)
                throw new System.ArgumentNullException(nameof(_cellMesh));
            if (_material == null)
                throw new System.ArgumentNullException(nameof(_material));

            _orientation.Subscribe(OrinetationChanged);
            _cellMesh.Subscribe(CellMeshChanged);
            _material.Subscribe(MaterialChanged);

            _subscribedToDependency = true;
        }
        private void OrinetationChanged(int _) => InvokeSubscribers(this, GetValue());
        private void CellMeshChanged(short _) => InvokeSubscribers(this, GetValue());
        private void MaterialChanged(byte _) => InvokeSubscribers(this, GetValue());


    }

}