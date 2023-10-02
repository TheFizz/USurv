using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    public event InteractionHandler OnInteraction;
    public event InteractedHandler OnInteracted;

    private Transform _myTransform;
    public float InteractionRange;
    [SerializeField] private LayerMask _targetLayer;
    WeaponInteraction _wi = null;
    bool _interactionShown;
    private List<Tuple<KeyCode, InteractionType>> _options;

    private void Awake()
    {
        Globals.PInteractionManager = this;
        _myTransform = GetComponent<Transform>();
    }
    private void Start()
    {
        Globals.PSystems.SubscribeInteracted();
    }
    // Update is called once per frame
    void Update()
    {
        if (_wi != null)
        {
            foreach (var option in _options)
            {
                if (Input.GetKeyDown(option.Item1) && Globals.Room.PlayerInMenu == false)
                {
                    OnInteracted?.Invoke(option.Item2, _wi.RewardName);
                    Destroy(_wi.transform.gameObject);
                    return;
                }
            }
        }
        Collider[] hitInteractions = Physics.OverlapSphere(_myTransform.position, InteractionRange, _targetLayer);
        if (hitInteractions.Length > 0)
        {
            var interaction = hitInteractions[0].GetComponent<WeaponInteraction>();
            _wi = interaction;
            _options = new List<Tuple<KeyCode, InteractionType>>(interaction.Options);
            var weapons = Globals.PSystems.GetWeaponNames();
            if (weapons.Contains(interaction.RewardName))
                _options.RemoveAt(0);
            ShowInteraction(true);
        }
        else
        {
            _wi = null;
            _options = null;
            ShowInteraction(false);
        }
    }

    void ShowInteraction(bool show)
    {
        if (show != _interactionShown)
            _interactionShown = show;
        else
            return;

        OnInteraction?.Invoke(_options, _wi?.RewardName);
    }
    private void OnDestroy()
    {
        Globals.PSystems.UnSubscribeInteracted();
    }
}
