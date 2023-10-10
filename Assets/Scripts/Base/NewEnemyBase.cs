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
    [SerializeField] private GameObject _damageText;

    private readonly Dictionary<EffectSO, TimedEffect> _effects = new Dictionary<EffectSO, TimedEffect>();

    public float BaseSpeed;

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
        _RB.velocity = direction * BaseSpeed;
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
            DrawDefaultInspector();
        }
    }
}
