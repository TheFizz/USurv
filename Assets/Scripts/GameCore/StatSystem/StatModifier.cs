namespace Kryz.CharacterStats
{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300,
    }
    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType Type;
        public readonly StatParam Param;
        public readonly int Order;
        public readonly object Source;

        public StatModifier(float value, StatModType type, StatParam param, int order, object source)
        {
            Value = value;
            Type = type;
            Param = param;
            Order = order;
            Source = source;
        }

        public StatModifier(float value, StatModType type, StatParam param) : this(value, type, param, (int)type, null) { }

        public StatModifier(float value, StatModType type, StatParam param, int order) : this(value, type, param, order, null) { }

        public StatModifier(float value, StatModType type, StatParam param, object source) : this(value, type, param, (int)type, source) { }
    }
}
