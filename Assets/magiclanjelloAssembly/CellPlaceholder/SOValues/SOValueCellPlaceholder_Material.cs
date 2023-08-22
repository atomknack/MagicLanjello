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
    [CreateAssetMenu(fileName = "SOValueCellPlaceholder_Material", menuName = "MagicLanjello/CellPlaceholder/SOValue<byte> - Material", order = 110)]
    public sealed class SOValueCellPlaceholder_Material : SOValueImmutableWithSubscribingAction<byte>
    {
        [SerializeField][ValidReference] private SOEvent<byte> _materialChanged;
        [SerializeField][ValidReference] private SOEvent _nextMaterial;


        [NonSerialized]
        private bool _subscribedToDependency = false;

        [NonSerialized]
        private byte _material = DEMaterials.DefaultMaterial.id;

        public override byte GetValue() => _material;

        private void SetMaterialWithInvokeSubscribers(byte value)
        {
            throw new System.NotImplementedException(); 
            // _material = DEMaterials.ValidateMaterial(value);
            InvokeSubscribers(this, _material);
        }
        private void NextMaterial() => 
            SetMaterialWithInvokeSubscribers(DEMaterials.NextMaterialId(_material));
        
        protected override void AfterUnsubscribing()
        {
            if (_subscribedToDependency == false)
                return;
            if (SubscribersCount() > 0)
                return;

            _materialChanged.UnsubscribeNullSafe(SetMaterialWithInvokeSubscribers);
            _nextMaterial.UnsubscribeNullSafe(NextMaterial);

            _subscribedToDependency = false;
        }

        protected override void BeforeSubscribing()
        {
            if (_subscribedToDependency)
                return;

            NullChecks();
            _materialChanged.Subscribe(SetMaterialWithInvokeSubscribers);
            _nextMaterial.Subscribe(NextMaterial);

            _subscribedToDependency = true;
        }

        private void NullChecks()
        {
            if (_materialChanged == null) throw new ArgumentNullException(nameof(_materialChanged));
            if (_nextMaterial == null) throw new ArgumentNullException(nameof(_nextMaterial));
        }
    }
}