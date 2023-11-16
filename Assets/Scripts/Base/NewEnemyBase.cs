using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewEnemyBase : MonoBehaviour, IDamageable, IForceable, IStunnable, IDamaging, IMovingAI, IEffectable
{
    //IDamageable
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }
    public bool Damageable { get; set; } = true;
    public float InDmgFactor { get; set; } = 1f;

    //IForceable
    public Vector3 ForceVector { get; set; } = Vector3.zero;

    //IDamaging
    public float DamageAmount { get; set; }
    public float OutDmgFactor { get; set; } = 1;

    //IMovingAI
    public Transform MainTarget { get; set; }
    public float MoveSpeed { get; set; }

    //Self
    [field: SerializeField] public EnemyBaseSO EnemyData { get; set; }
    [field: SerializeField] public GameObject DropOnDeath { get; set; }

    [SerializeField] private string _platingMatMatch;
    [SerializeField] private string _baseMatMatch;
    [SerializeField] protected bool _canMove = true;
    [SerializeField] private bool _invulnerable = false;
    [SerializeField] private Transform _damageTextAnchor;

    [HideInInspector] public string ID;
    protected Rigidbody _RB;
    private Color _baseColor;
    private Renderer _renderer;
    private bool _canAttack = true;
    private Material _platingMat;
    private Material _baseMat;
    [SerializeField] private GameObject _damageText;

    private readonly Dictionary<EffectSO, TimedEffect> _effects = new Dictionary<EffectSO, TimedEffect>();

    bool isDying = false;
    public Plating Plating;
    private Transform _myTransform;

    //Unity
    void Start()
    {
        ID = Game.GenerateId();
        gameObject.name = $"Enemy<{ID}>";

        _myTransform = transform;
        _RB = GetComponent<Rigidbody>();
        _renderer = GetComponentInChildren<Renderer>();

        if (String.IsNullOrEmpty(_platingMatMatch))
            _platingMat = _renderer.material;
        else
           _platingMat = _renderer.materials.FirstOrDefault(x => x.name.StartsWith(_platingMatMatch));
        if (_platingMat == null)
            _platingMat = _renderer.material;


        if (String.IsNullOrEmpty(_baseMatMatch))
            _baseMat = _renderer.material;
        else
    _baseMat = _renderer.materials.FirstOrDefault(x => x.name.StartsWith(_baseMatMatch));
        if (_baseMat == null)
            _baseMat = _renderer.material;

        _platingMat.SetColor("_Overlay", Plating.ColorScheme.colorKeys[0].color);
        _baseColor = _baseMat.GetColor("_Overlay");

        MoveSpeed = EnemyData.MoveSpeed;
        DamageAmount = EnemyData.AttackDamage;
        Game.EnemyPool.Add(ID, this);
    }
    protected virtual void Update()
    {
        HandleEffects();
        if (_canMove)
            MoveTo(MainTarget.position);
        LookAt(MainTarget.position);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!Game.IsInLayerMask(collision.gameObject.layer, EnemyData.TargetLayer))
            return;
        if (!_canAttack)
            return;
        var damage = DamageAmount * OutDmgFactor;
        Game.PSystems.DamageManager.Damage(damage, false, ID);
    }


    //IDamageable
    public void Damage(float damageAmount, bool isCrit, string attackerID, bool overrideITime = false)
    {
        var damage = damageAmount * InDmgFactor;

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
        if (isDying)
            return;
        isDying = true;
        var pos = gameObject.transform.position;
        pos.y = 1f;
        Instantiate(DropOnDeath, pos, Quaternion.identity);
        Game.EnemyPool.Remove(ID);
        Destroy(gameObject);
        Game.Room.KillIncrease(1);
    }


    //IForceable
    public float GetMass()
    {
        return _RB.mass;
    }
    public void SetMass(float mass)
    {
        _RB.mass = mass;
    }
    public Transform GetTransform()
    {
        return _myTransform;
    }


    //IStunnable
    public void SetStunned(bool stunned)
    {
        _canMove = !stunned;
        _canAttack = !stunned;
        if (stunned)
            _RB.velocity = Vector3.zero;
    }


    //IMovingAI
    public void MoveTo(Vector3 target)
    {
        var direction = (target - _myTransform.position).normalized;
        _RB.velocity = (direction * MoveSpeed) + ForceVector;
        var targetLook = target;
    }
    public void LookAt(Vector3 target)
    {
        target.y = _myTransform.position.y;
        _myTransform.LookAt(target);
    }


    //IEffectable
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
    public void HandleEffects()
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


    //Self
    public void SetHp(float min, float max)
    {
        MaxHealth = Random.Range(min, max);
        CurrentHealth = MaxHealth;
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
    IEnumerator ShowDamage()
    {
        _baseMat.SetColor("_Overlay", Color.red);
        yield return new WaitForSeconds(0.1f);
        _baseMat.SetColor("_Overlay", _baseColor);
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
            DrawDefaultInspector();
        }
    }
}
