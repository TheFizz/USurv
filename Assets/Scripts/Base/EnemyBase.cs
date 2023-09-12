using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable, IEnemyMoveable
{
    public abstract float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public Rigidbody _RB { get; set; }
    public abstract float MoveSpeed { get; set; }
    public abstract Vector2 HealthRange { get; set; }
    public bool damageable = true;

    private Renderer _renderer;
    private Color _baseColor;
    private Transform _playerTransform;
    private Vector3 _moveTarget;
    private GameObject _damageText;
    private Transform _damageTextAnchor;
    bool feared = false;
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        MaxHealth = Random.Range(HealthRange.x, HealthRange.y);
        CurrentHealth = MaxHealth;

        _RB = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _damageText = (GameObject)Resources.Load("Prefabs/Service/DamageText");
        _damageTextAnchor = transform.Find("DamageTextAnchor");
    }
    void Update()
    {
        if (!feared)
            _moveTarget = _playerTransform.position;
        if (Vector3.Distance(transform.position, _moveTarget) > 1.5)
            MoveEnemy(_moveTarget);
    }
    public void Damage(float damageAmount)
    {
        Vector3 cameraAngle = Camera.main.transform.eulerAngles;
        var damageText = Instantiate(_damageText, _damageTextAnchor.position, Quaternion.identity);
        damageText.transform.rotation = Quaternion.Euler(cameraAngle.x, cameraAngle.y, cameraAngle.z);
        damageText.GetComponent<DamageText>().Setup(Mathf.RoundToInt(damageAmount));

        if (damageable)
            CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0f)
        {
            Die();
        }

        StartCoroutine(ShowDamage());
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void MoveEnemy(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        _RB.velocity = direction * MoveSpeed;

        var targetLook = target;
        targetLook.y = transform.position.y;
        transform.LookAt(targetLook);
    }

    public void ReceiveTempFearEffect(float time)
    {
        StartCoroutine(Fear(time));
    }

    IEnumerator ShowDamage()
    {
        var tmpColor = _renderer.material.color;
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = tmpColor;
    }

    IEnumerator Fear(float time)
    {
        feared = true;

        _moveTarget = (transform.forward * -1) * 100;

        _renderer.material.color = Color.blue;
        yield return new WaitForSeconds(time);
        _renderer.material.color = _baseColor;
        feared = false;
    }
}
