using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviourTESTING : MonoBehaviour
{
    public float _projectileSpeed = 5;
    private float _maxDistance;
    private Vector3 _sourcePoint;
    private int _pierceCount = 0;
    public GameObject ArrowModel;
    public ParticleSystem Particles;
    private bool _canMove = true;
    private WeaponBaseRanged _weapon;

    public EffectSO effect;

    // Update is called once per frame
    void Update()
    {
        if (Particles == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_canMove)
        {
            transform.position += transform.forward * Time.deltaTime * _projectileSpeed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag != "Enemy")
            return;

        var ee = other.GetComponent<IEffectable>();
        ee.AddEffect(effect.InitializeEffect(ee));

        if (_pierceCount <= 0)
        {
            Destroy(ArrowModel);
            Particles?.Stop();
            _canMove = false;
        }
        _pierceCount--;
    }
}
