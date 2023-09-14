namespace Kryz.CharacterStats
{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300,
    }

    public enum StatModParam
    {
        AttackSpeed,
        AttackDamage,
        AttackArc,
        AttackRange
    }
    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType Type;
        public readonly StatModParam Param;
        public readonly int Order;
        public readonly object Source;

        public StatModifier(float value, StatModType type, StatModParam param, int order, object source)
        {
            Value = value;
            Type = type;
            Param = param;
            Order = order;
            Source = source;
        }

        public StatModifier(float value, StatModType type, StatModParam param) : this(value, type, param, (int)type, null) { }

        public StatModifier(float value, StatModType type, StatModParam param, int order) : this(value, type, param, order, null) { }

        public StatModifier(float value, StatModType type, StatModParam param, object source) : this(value, type, param, (int)type, source) { }
    }
}
