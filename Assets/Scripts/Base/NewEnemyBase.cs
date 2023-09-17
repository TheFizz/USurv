using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewEnemyBase : MonoBehaviour, IEnemyDamageable
{
    [field: SerializeField] public EnemyBaseSO EnemyData { get; set; }
    [field: SerializeField] public GameObject DropOnDeath { get; set; }
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    private Rigidbody _RB;
    //private Renderer _renderer;
    private Color _baseColor;
    private Transform _playerTransform;
    private Vector3 _target;
    private Dictionary<string, float> _ailments = new Dictionary<string, float>();
    private bool _isAttacking;
    private GameObject _damageText;
    [SerializeField]
    private Transform _damageTextAnchor;
    [SerializeField] private bool _invulnerable = false;
    [SerializeField] private bool _canMove = true;


    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        MaxHealth = Random.Range(EnemyData.HealthMin, EnemyData.HealthMax);
        CurrentHealth = MaxHealth;
        _RB = GetComponent<Rigidbody>();
        _damageText = (GameObject)Resources.Load("Prefabs/Service/DamageText");
        _playerTransform = Globals.PlayerTransform;
    }
    void Update()
    {
        HandleAilments();

        var distanceToPlayer = Vector3.Distance(transform.position, _target);
        if (!_ailments.ContainsKey("fear"))
        {
            _target = _playerTransform.position;
        }
        if (distanceToPlayer > 1.5)
            if (_canMove)
                MoveTo(_target);

        if (distanceToPlayer < 5)
        {
            if (!_isAttacking)
                StartAttack();
        }
        else if (_isAttacking)
            StopAttack();
    }

    private void HandleAilments()
    {
        List<string> ailmentNames = new List<string>(_ailments.Keys);
        foreach (var name in ailmentNames)
        {
            var t = _ailments[name];
            t -= Time.deltaTime;
            if (t <= 0)
            {
                _ailments.Remove(name);

                if (name == "fear")
                {
                    //_renderer.material.color = _baseColor;
                }
            }
            else
                _ailments[name] = t;
        }
    }

    public void MoveTo(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        _RB.velocity = direction * EnemyData.MoveSpeed;

        var targetLook = target;
        targetLook.y = transform.position.y;
        transform.LookAt(targetLook);
    }
    protected void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, EnemyData.AttackRange, LayerMask.GetMask("Player"));
        foreach (var hitEnemy in hitEnemies)
        {
            var enemy = hitEnemy.GetComponent<IDamageable>();
            enemy.Damage(EnemyData.AttackDamage);
        }
    }
    public virtual void StartAttack()
    {
        _isAttacking = true;
        InvokeRepeating("Attack", 1, EnemyData.AttackSpeed);
    }
    public virtual void StopAttack()
    {
        _isAttacking = false;
        CancelInvoke();
    }
    public void Damage(float damageAmount, bool overrideITime = false)
    {
        Vector3 cameraAngle = Globals.MainCamera.transform.eulerAngles;
        var damageText = Instantiate(_damageText, _damageTextAnchor.position, Quaternion.identity);
        damageText.transform.rotation = Quaternion.Euler(cameraAngle.x, cameraAngle.y, cameraAngle.z);
        damageText.GetComponent<DamageText>().Setup(Mathf.RoundToInt(damageAmount));

        if (!_invulnerable)
            CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
        //StartCoroutine(ShowDamage());
    }
    public void Die()
    {
        var pos = gameObject.transform.position;
        pos.y = 1f;
        Destroy(gameObject);
        Instantiate(DropOnDeath, pos, Quaternion.identity);
    }
    public void ReceiveAilment(string name, float time)
    {
        if (_ailments.ContainsKey(name))
            _ailments[name] += time;
        else
        {
            _ailments.Add(name, time);
        }

        if (name == "fear")
        {
            //_renderer.material.color = Color.blue;
            _target = (transform.forward * -1) * 100;
        }
    }
    IEnumerator ShowDamage()
    {
        // var tmpColor = _renderer.material.color;
        // _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        // _renderer.material.color = tmpColor;
    }

    private void OnDrawGizmos()
    {
        var capsule = GetComponent<CapsuleCollider>();
        var maxVectorValue = Globals.GetLargestValue(capsule.gameObject.transform.localScale, true);
        Gizmos.DrawWireSphere(transform.position, capsule.radius * maxVectorValue);
    }
}
