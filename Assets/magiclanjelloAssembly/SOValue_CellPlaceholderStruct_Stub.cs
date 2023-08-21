using UnityEngine;
using UKnack.Values;
using UKnack.Preconcrete.Values;
using UKnack.MagicLanjello;

namespace UKnack.Concrete.Values
{

    [CreateAssetMenu(fileName = "SOValueMutable_CellPlaceholderStruct_Stub", menuName = "UKnackDoubleEngine/SOValueMutable/CellPlaceholderSTUB", order = 110)]
    public sealed class SOValueMutable_CellPlaceholderStruct_Stub : SOValueMutable<CellPlaceholderStruct>
    {
        [SerializeField]
        private USetOrDefault<CellPlaceholderStruct> _value = new (CellPlaceholderStruct.DefaultPlaceholder);

        public override CellPlaceholderStruct RawValue { get => _value.Value; protected set => _value.Value = value; }


    }

}