using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewEnemyBase : MonoBehaviour, IEnemyDamageable
{
    [field: SerializeField] public EnemyBaseSO EnemyData { get; set; }
    [field: SerializeField] public GameObject DropOnDeath { get; set; }
    [HideInInspector] public float CurrentHealth { get; set; }
    [HideInInspector] public float MaxHealth { get; set; }

    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _invulnerable = false;
    [SerializeField] private Transform _damageTextAnchor;

    private string _id;
    private Rigidbody _RB;
    private Vector3 _target;
    private Color _baseColor;
    private Renderer _renderer;
    private GameObject _damageText;
    private Transform _playerTransform;
    private Dictionary<AilmentType, Tuple<float, float>> _ailments = new Dictionary<AilmentType, Tuple<float, float>>();

    private float _baseSpeed;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); //Bad
        _id = Globals.GenerateId();
        _RB = GetComponent<Rigidbody>();
        _baseSpeed = EnemyData.MoveSpeed;
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
        _playerTransform = Globals.PlayerTransform;
        _damageText = (GameObject)Resources.Load("Prefabs/Service/DamageText");
    }
    void Update()
    {
        HandleAilments();

        var distanceToPlayer = Vector3.Distance(transform.position, _target);
        if (!_ailments.ContainsKey(AilmentType.Fear) && !_ailments.ContainsKey(AilmentType.Knockback))
        {
            _target = _playerTransform.position;
        }
        if (distanceToPlayer > 1.5)
            if (_canMove)
                MoveTo(_target);
    }

    public void SetHp(float min, float max)
    {
        MaxHealth = Random.Range(min, max);
        CurrentHealth = MaxHealth;
    }
    private void HandleAilments()
    {
        foreach (AilmentType type in AilmentType.GetValues(typeof(AilmentType)))
        {
            if (_ailments.ContainsKey(type))
            {
                var ailment = _ailments[type];
                var tuple = new Tuple<float, float>(ailment.Item1 - Time.deltaTime, ailment.Item2);
                if (ailment.Item1 <= 0)
                {
                    _ailments.Remove(type);

                    if (type == AilmentType.Fear)
                    {
                        _renderer.material.color = _baseColor;
                    }
                }
                else
                    _ailments[type] = tuple;
            }
        }
    }
    public void MoveTo(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        _RB.velocity = direction * EnemyData.MoveSpeed;

        if (_ailments.ContainsKey(AilmentType.Knockback))
            return;
        var targetLook = target;
        targetLook.y = transform.position.y;
        transform.LookAt(targetLook);
    }
    public void Damage(float damageAmount, bool isCrit)
    {
        Vector3 cameraAngle = Globals.MainCamera.transform.eulerAngles;
        var damageText = Instantiate(_damageText, _damageTextAnchor.position, Quaternion.identity);
        damageText.transform.rotation = Quaternion.Euler(cameraAngle.x, cameraAngle.y, cameraAngle.z);


        damageText.GetComponent<DamageText>().Setup(Mathf.RoundToInt(damageAmount), isCrit);

        if (!_invulnerable)
            CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
        StartCoroutine(ShowDamage());
    }
    public void Die()
    {
        var pos = gameObject.transform.position;
        pos.y = 1f;
        Destroy(gameObject);
        Instantiate(DropOnDeath, pos, Quaternion.identity);
    }
    public void ReceiveAilment(AilmentType type, float time, float force = 0)
    {
        if (_ailments.ContainsKey(type))
        {
            var t = _ailments[type];
            _ailments[type] = new Tuple<float, float>(t.Item1 + time, t.Item2);
        }
        else
        {
            _ailments.Add(type, new Tuple<float, float>(time, force));
        }

        if (type == AilmentType.Fear)
        {
            _renderer.material.color = Color.blue;
            _target = (transform.forward * -1) * 100;
        }
        if (type == AilmentType.Knockback)
        {
            //_renderer.material.color = Color.blue;
            //_target = (_playerTransform.position - transform.position).normalized;
        }
    }
    IEnumerator ShowDamage()
    {
        var tmpColor = _renderer.material.color;
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = tmpColor;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!Globals.IsInLayerMask(collision.gameObject.layer, EnemyData.TargetLayer))
            return;
        Globals.DmgHandler.Damage(EnemyData.AttackDamage, _id);
    }
}
