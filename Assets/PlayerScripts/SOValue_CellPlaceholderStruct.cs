using UnityEngine;
using UKnack.Values;
using UKnack.Preconcrete.Values;
using UKnack.MagicLanjello;

namespace UKnack.Concrete.Values
{

    [CreateAssetMenu(fileName = "SOValue_CellPlaceholder", menuName = "UKnackDoubleEngine/SOValueMutable/CellPlaceholderStruct", order = 110)]
    public sealed class SOValueMutable_Concrete_CellPlaceholderStruct : SOValueMutable<CellPlaceholderStruct>
    {
        [SerializeField]
        private USetOrDefault<CellPlaceholderStruct> _value;

        public override CellPlaceholderStruct RawValue { get => _value.Value; protected set => _value.Value = value; }


    }

}