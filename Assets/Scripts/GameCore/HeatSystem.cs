using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeatStatus
{
    Default,
    Cooling,
    Overheated
}

public class HeatSystem : MonoBehaviour
{
    [SerializeField] private float _cooldownMultiplier = 1;
    [SerializeField] private float _cdMultiplierNormal = 1;
    [SerializeField] private float _cdMultiplierCooling = 4;
    [SerializeField] private float _heatMultiplier = 3;

    private float _maxHeat = 100f;
    private float _currentHeat;
    private bool _isCooling = false;
    private bool _isOverheated = false;
    // Update is called once per frame
    void Update()
    {
        if (_isOverheated)
            return;
        _currentHeat -= (Time.deltaTime * _cooldownMultiplier);
        if (_currentHeat <= 0)
        {
            _currentHeat = 0;
            _isCooling = false;
            _cooldownMultiplier = _cdMultiplierNormal;
        }

    }
    public void AddHeat(float value)
    {
        value *= _heatMultiplier;

        if (_isCooling)
            return;
        _currentHeat += value;
        if (_currentHeat >= _maxHeat)
        {
            _currentHeat = _maxHeat;
            _isOverheated = true;
        }
    }
    public bool CanSwap()
    {
        return !_isCooling;
    }
    public void StartCooldown()
    {
        if (_isOverheated)
        {
            _isOverheated = false;
            _isCooling = true;
            _cooldownMultiplier = _cdMultiplierCooling;
        }
    }

    public HeatStatus GetHeatStatus()
    {
        if (_isOverheated)
            return HeatStatus.Overheated;
        else if (_isCooling)
            return HeatStatus.Cooling;
        else
            return HeatStatus.Default;
    }
    public float GetHeat()
    {
        return _currentHeat;
    }
}
