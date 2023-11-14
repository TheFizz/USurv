using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInteractionManager : MonoBehaviour
{
    public event InteractionHandler OnInteraction;
    public event InteractedHandler OnInteracted;

    private Transform _myTransform;
    public float InteractionRange;
    [SerializeField] private LayerMask _targetLayer;
    Interaction _wi = null;
    bool _interactionShown;
    private List<Tuple<KeyCode, InteractionType>> _options;

    private void Start()
    {
        _myTransform = GetComponent<Transform>();
        Game.PSystems.SubscribeInteracted();
    }
    // Update is called once per frame
    void Update()
    {
        if (_wi != null)
        {
            foreach (var option in _options)
            {
                if (Input.GetKeyDown(option.Item1) && Game.Room.PlayerInMenu == false)
                {
                    OnInteracted?.Invoke(option.Item2, _wi.InteractionTitle);
                    Destroy(_wi.transform.gameObject);
                    return;
                }
            }
        }
        Collider[] hitInteractions = Physics.OverlapSphere(_myTransform.position, InteractionRange, _targetLayer);
        if (hitInteractions.Length > 0)
        {
            var interactions = hitInteractions.Where(x => x.GetComponent<Interaction>() != null).ToList();
            interactions.OrderBy(x => Vector3.Distance(x.transform.position, Game.PSystems.PlayerObject.transform.position));
            if (interactions.Count < 1)
                return;

            var interaction = interactions[0].GetComponent<Interaction>();

            _wi = interaction;
            _options = new List<Tuple<KeyCode, InteractionType>>(interaction.Options);
            var weapons = Game.PSystems.GetWeaponNames();
            if (weapons.Contains(interaction.InteractionTitle))
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

        OnInteraction?.Invoke(_options, _wi?.InteractionTitle);
    }
    private void OnDestroy()
    {
        Game.PSystems.UnSubscribeInteracted();
    }
}
