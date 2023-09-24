using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    // Start is called before the first frame update

    private float _speed = 4f;
    private float _ttl = .8f;
    private float _fadeOffset = .2f;
    private float _ttlOriginal;
    private int _damage;
    private bool _isCrit;
    private TextMeshPro _textMesh;
    public void Setup(int damage, bool isCrit)
    {
        _damage = damage;
        _isCrit = isCrit;
    }
    private void Start()
    {
        _textMesh = GetComponentInChildren<TextMeshPro>();
        _textMesh.fontSize = 9f;
        _ttlOriginal = _ttl;
        _textMesh.text = _damage.ToString();
        if (_isCrit)
            _textMesh.text += "!";
    }
    // Update is called once per frame
    void Update()
    {
        if (_ttl > 0)
        {
            transform.position += Vector3.up * _speed * Time.deltaTime;
            _ttl -= Time.deltaTime;
            _speed -= Mathf.Clamp(Time.deltaTime * 15, 0, _speed);

            if (_ttl <= (_ttlOriginal - _fadeOffset))
            {
                var percent = _ttl / (_ttlOriginal - _fadeOffset);
                _textMesh.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, (1 - percent) * -1);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
