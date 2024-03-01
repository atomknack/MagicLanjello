using UnityEngine;
using System.ComponentModel;
using UKnack.Attributes;
using UKnack.Values;
using UKnack.Preconcrete.UI.SimpleToolkit;

namespace MagicLanJello.UI.SimpleToolkit
{

    [AddComponentMenu("UKnack/UI.SimpleToolkit/EffortlessUpdaterFromValueSO/Magic_Temp_long")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public sealed class EffortlessUpdaterFromValueSO_Temp_long : EffortlessUpdaterFromValueSO<long>
    {
        [SerializeField][ValidReference(nameof(ValidIValue))] private SOValue<long> _value;

        protected override IValue<long> GetValidValueProvider() 
            => ValidIValue(_value);

        protected override string RawValueToStringConversion(long value)
            => value.ToString();
    

        public static IValue<long> ValidIValue(UnityEngine.Object value)
        {
            if (value==null)
                throw new System.ArgumentNullException(nameof(value));
            var asIValue = value as IValue<long>;
            if (asIValue == null)
                throw new System.InvalidCastException($"{value.GetType()} is not compatible with {typeof(IValue<int>)}");
            return asIValue;
        }
    }
}

