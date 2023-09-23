using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private Transform _myTransform;
    private PlayerSystems _pSystems;
    public float InteractionRange;
    [SerializeField] private LayerMask _targetLayer;
    WeaponInteraction _wi = null;
    bool _interactionShown;
    private List<Tuple<KeyCode, InteractionType>> _options;

    // Start is called before the first frame update
    void Awake()
    {
        _myTransform = GetComponent<Transform>();
        _pSystems = Globals.PlayerSystems;
    }

    // Update is called once per frame
    void Update()
    {
        if (_wi != null)
        {
            foreach (var option in _options)
            {
                if (Input.GetKeyDown(option.Item1) && Game.PlayerInMenu == false)
                {
                    Globals.UIManager.WeaponInteraction(option.Item2, _wi.RewardName);
                    Destroy(_wi.transform.gameObject);
                    return;
                }
            }
        }
        Collider[] hitInteractions = Physics.OverlapSphere(_myTransform.position, InteractionRange, _targetLayer);
        if (hitInteractions.Length > 0)
        {
            var interaction = hitInteractions[0].GetComponent<WeaponInteraction>();
            _options = new List<Tuple<KeyCode, InteractionType>>(interaction.Options);
            var weapons = _pSystems.GetWeaponNames();
            if (weapons.Contains(interaction.RewardName))
                _options.RemoveAt(0);
            _wi = interaction;
            ShowInteraction(true);
        }
        else
        {
            _wi = null;
            ShowInteraction(false);
        }
    }

    void ShowInteraction(bool show)
    {
        if (_wi == null)
            return;
        if (show != _interactionShown)
            _interactionShown = show;
        else
            return;
        Globals.UIManager.ShowInteraction(_options, _wi.RewardName);
    }
}
