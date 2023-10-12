using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    private bool _isStunned = false;
    [SerializeField] private GameObject _damageText;

    private readonly Dictionary<EffectSO, TimedEffect> _effects = new Dictionary<EffectSO, TimedEffect>();

    public float BaseSpeed;
    public float RecvDamageAmp = 1f;
    public float DamageMult = 1f;
    public Vector3 ForceVector = Vector3.zero;

    public EffectSO tmpforce;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); //Bad
        ID = Game.GenerateId();
        gameObject.name = $"Enemy<{ID}>";
        _RB = GetComponent<Rigidbody>();
        BaseSpeed = EnemyData.MoveSpeed;
        _renderer = GetComponentInChildren<Renderer>();
        _baseColor = _renderer.material.GetColor("_Overlay");
        Game.EnemyPool.Add(ID, this);
    }
    void Update()
    {
        HandleEffects();

        var distanceToPlayer = Vector3.Distance(transform.position, _target);
        _target = MainTarget.position;
        if (distanceToPlayer > 1.5)
            if (_canMove)
                MoveTo(_target);
    }
    public float GetMass()
    {
        return _RB.mass;
    }
    public void SetMass(float mass)
    {
        _RB.mass = mass;
    }
    public void SetHp(float min, float max)
    {
        MaxHealth = Random.Range(min, max);
        CurrentHealth = MaxHealth;
    }
    private void HandleEffects()
    {
        foreach (var effect in _effects.Values.ToList())
        {
            effect.Tick(Time.deltaTime);
            if (effect.IsFinished)
            {
                _effects.Remove(effect.EffectData);
            }
        }
    }
    public void MoveTo(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        _RB.velocity = (direction * BaseSpeed) + ForceVector;
        var targetLook = target;
        targetLook.y = transform.position.y;
        transform.LookAt(targetLook);
    }
    public void Damage(float damageAmount, bool isCrit)
    {
        var damage = damageAmount * RecvDamageAmp;

        Vector3 cameraAngle = Game.MainCamera.transform.eulerAngles;
        var damageText = Instantiate(_damageText, _damageTextAnchor.position, Quaternion.identity);
        damageText.transform.rotation = Quaternion.Euler(cameraAngle.x, cameraAngle.y, cameraAngle.z);


        damageText.GetComponent<DamageText>().Setup(Mathf.RoundToInt(damage), isCrit);

        if (!_invulnerable)
            CurrentHealth -= damage;
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
    public void AddEffect(TimedEffect effect)
    {
        if (_effects.ContainsKey(effect.EffectData))
        {
            _effects[effect.EffectData].Activate();
        }
        else
        {
            _effects.Add(effect.EffectData, effect);
            effect.Activate();
        }
    }
    public void Stun(float duration)
    {
        StartCoroutine(StunCR(duration));
    }
    IEnumerator StunCR(float duration)
    {
        _isStunned = true;
        _canMove = false;
        _RB.velocity = Vector3.zero;
        yield return new WaitForSeconds(duration);
        _isStunned = false;
        _canMove = true;
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
        if (_isStunned)
            return;
        var damage = EnemyData.AttackDamage * DamageMult;
        Game.PSystems.DamageManager.Damage(damage, ID);
    }
    [CustomEditor(typeof(NewEnemyBase))]
    public class NewEnemyBaseEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            NewEnemyBase neb = (NewEnemyBase)target;
            if (GUILayout.Button("Apply Slow"))
            {
                neb.AddEffect(Game.Instance.GameEffects[0].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Burn"))
            {
                neb.AddEffect(Game.Instance.GameEffects[1].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Shatter"))
            {
                neb.AddEffect(Game.Instance.GameEffects[2].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Cripple"))
            {
                neb.AddEffect(Game.Instance.GameEffects[3].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Prayer"))
            {
                neb.AddEffect(Game.Instance.GameEffects[4].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Bleed"))
            {
                neb.AddEffect(Game.Instance.GameEffects[5].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Stun"))
            {
                neb.AddEffect(Game.Instance.GameEffects[6].InitializeEffect(neb));
            }
            if (GUILayout.Button("Apply Knockback"))
            {
                neb.AddEffect(neb.tmpforce.InitializeEffect(neb, new ForceData(Vector3.zero, 15, 0)));
            }
            DrawDefaultInspector();
        }
    }
}
