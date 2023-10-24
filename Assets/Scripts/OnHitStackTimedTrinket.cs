using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OnHitStackTimedTrinket
{
    const int ACTIVE = 0;
    const int PASSIVE = 1;
    const int ABILITY = 2;

    float _duration;
    public OnHitStackTimedTrinketSO TrinketData;
    int _stacks;
    public bool IsFinished;
    public OnHitStackTimedTrinket(OnHitStackTimedTrinketSO trinketData)
    {
        TrinketData = trinketData;
    }
    public void Activate()
    {
        _duration = TrinketData.Duration;
        if (_stacks >= TrinketData.MaxStacks)
            return;
        Apply();
    }
    public void Apply()
    {
        _stacks++;
        var bonus = TrinketData.StackBonus;
        bonus.Source = this;
        Game.PSystems.PlayerWeapons[ACTIVE].ApplyModifiers(new List<StatModifier> { bonus });
    }
    public void End()
    {
        Game.PSystems.PlayerWeapons[ACTIVE].ClearSourcedModifiers(this);
    }
    public void Tick(float delta)
    {
        _duration -= delta;
        if (_duration <= 0)
        {
            End();
            IsFinished = true;
        }
    }
}

