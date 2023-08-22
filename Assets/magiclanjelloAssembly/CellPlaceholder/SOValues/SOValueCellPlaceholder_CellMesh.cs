using DoubleEngine.Atom;
using System;
using System.Collections;
using System.Collections.Generic;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.Preconcrete.Values;
using UnityEngine;


namespace MagicLanjello.CellPlaceholder.SOValues
{
    [CreateAssetMenu(fileName = "SOValueCellPlaceholder_CellMesh", menuName = "MagicLanjello/CellPlaceholder/SOValue<short> - CellMesh", order = 110)]
    public sealed class SOValueCellPlaceholder_CellMesh : SOValueImmutableWithSubscribingAction<short>
    {
        [SerializeField][ValidReference] private SOEvent<short> _cellMeshChanged;
        [SerializeField][ValidReference] private SOEvent _nextCellMesh;


        [NonSerialized]
        private bool _subscribedToDependency = false;

        [NonSerialized]
        private short _cellMesh = DEMaterials.DefaultMaterial.id;

        public override byte GetValue() => _cellMesh;

        private void SetMaterialWithInvokeSubscribers(byte value)
        {
            throw new System.NotImplementedException(); 
            // _material = DEMaterials.ValidateMaterial(value);
            InvokeSubscribers(this, _cellMesh);
        }
        private void NextMaterial() => 
            SetMaterialWithInvokeSubscribers(DEMaterials.NextMaterialId(_cellMesh));
        
        protected override void AfterUnsubscribing()
        {
            if (_subscribedToDependency == false)
                return;
            if (SubscribersCount() > 0)
                return;

            _materialChanged.UnsubscribeNullSafe(SetMaterialWithInvokeSubscribers);
            _nextCellMesh.UnsubscribeNullSafe(NextMaterial);

            _subscribedToDependency = false;
        }

        protected override void BeforeSubscribing()
        {
            if (_subscribedToDependency)
                return;

            NullChecks();
            _materialChanged.Subscribe(SetMaterialWithInvokeSubscribers);
            _nextCellMesh.Subscribe(NextMaterial);

            _subscribedToDependency = true;
        }

        private void NullChecks()
        {
            if (_materialChanged == null) throw new ArgumentNullException(nameof(_materialChanged));
            if (_nextCellMesh == null) throw new ArgumentNullException(nameof(_nextCellMesh));
        }
    }
}