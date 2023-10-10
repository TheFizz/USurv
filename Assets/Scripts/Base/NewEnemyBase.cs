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

    [HideInInspector] public string ID;
    private Rigidbody _RB;
    private Vector3 _target;
    public Transform MainTarget;
    private Color _baseColor;
    private Renderer _renderer;
    [SerializeField] private GameObject _damageText;
    private Dictionary<AilmentType, Tuple<float, float>> _ailments = new Dictionary<AilmentType, Tuple<float, float>>();

    private float _baseSpeed;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); //Bad
        ID = Game.GenerateId();
        gameObject.name = $"Enemy<{ID}>";
        _RB = GetComponent<Rigidbody>();
        _baseSpeed = EnemyData.MoveSpeed;
        _renderer = GetComponentInChildren<Renderer>();
        _baseColor = _renderer.material.GetColor("_Overlay");
        Game.EnemyPool.Add(ID, this);
    }
    void Update()
    {
        HandleAilments();

        var distanceToPlayer = Vector3.Distance(transform.position, _target);
        if (!_ailments.ContainsKey(AilmentType.Fear) && !_ailments.ContainsKey(AilmentType.Knockback))
        {
            _target = MainTarget.position;
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
                        _renderer.material.SetColor("_Overlay",_baseColor);
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
        Vector3 cameraAngle = Game.MainCamera.transform.eulerAngles;
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
        Instantiate(DropOnDeath, pos, Quaternion.identity);
        Game.EnemyPool.Remove(ID);
        Destroy(gameObject);
        Game.Room.KillIncrease(1);
    }
    public void Kill()
    {
        if (gameObject == null)
            return;
        var pos = gameObject.transform.position;
        pos.y = 1f;
        Instantiate(DropOnDeath, pos, Quaternion.identity);
        Destroy(gameObject);
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
            _renderer.material.SetColor("_Overlay", Color.blue);
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
        var tmpColor = _renderer.material.GetColor("_Overlay");
        _renderer.material.SetColor("_Overlay", Color.red);
        yield return new WaitForSeconds(0.1f);
        _renderer.material.SetColor("_Overlay", tmpColor);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!Game.IsInLayerMask(collision.gameObject.layer, EnemyData.TargetLayer))
            return;
        Game.PSystems.DamageManager.Damage(EnemyData.AttackDamage, ID);
    }

}
