using UnityEngine;
using UKnack.Values;
using UKnack.Preconcrete.Values;
using UKnack.MagicLanjello;
using UKnack.Attributes;

namespace UKnack.Concrete.Values
{

    [System.Obsolete("WIP")]
    [CreateAssetMenu(fileName = "SOValueMutable_CellPlaceholderStruct", menuName = "UKnackDoubleEngine/SOValueMutable/CellPlaceholder", order = 110)]
    public sealed partial class SOValueMutable_CellPlaceholderStruct : SOValueMutable<CellPlaceholderStruct>
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

        [SerializeField]
        private USetOrDefault<CellPlaceholderStruct> _value = new (CellPlaceholderStruct.DefaultPlaceholder);

        public override CellPlaceholderStruct RawValue { 
            get => new CellPlaceholderStruct(_orientation.GetValue(), _cellMesh.GetValue(), _material.GetValue()); 
            protected set => throw new System.InvalidOperationException(); }


    }

}