using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public AnimationCurve HpCurve;
    public AnimationCurve HpDevCurve;
    private float _baseHp = 6f;
    private float _curveResolution = 120f;
    private float _curveMultiplier = 1;
    private float _time;
    public GameObject _enemyPrefab;
    public Transform _player;
    [SerializeField] private float _secondsToSpawn = 0.5f;
    [SerializeField] private int _spawnLimit = 200;
    List<GameObject> _spawnedEnemies = new List<GameObject>();

    public TextMeshProUGUI debug;

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > _curveResolution)
        {
            _curveMultiplier++;
            _time = 0;
        }
    }
    private void Awake()
    {
        Game.Instance.OnLevelReady += OnLevelReady;
        Game.Spawner = this;
    }

    private void OnLevelReady(Game obj)
    {
        _player = Game.PSystems.PlayerObject.transform;
        InvokeRepeating("Spawn", 1, _secondsToSpawn);
        Game.Instance.OnLevelReady -= OnLevelReady;
    }
    void Spawn()
    {
        _spawnedEnemies.RemoveAll(s => s == null);
        int attempts = 5;
        bool pointFound = false;
        Vector3 spawnPoint = Vector3.zero;
        while (attempts > 0 && pointFound == false && _spawnedEnemies.Count < _spawnLimit)
        {
            var x = Random.Range(-45, 45);
            var y = 0f; //Arbitrary;
            var z = Random.Range(-45, 45);
            spawnPoint = new Vector3(x, y, z);

            if (Vector3.Distance(_player.position, spawnPoint) > 10f)
            {
                pointFound = true;
                foreach (var obj in _spawnedEnemies)
                {
                    if (Vector3.Distance(obj.transform.position, spawnPoint) < 5f)
                    {
                        pointFound = false;
                        break;
                    }
                }
                if (pointFound)
                    break;
            }
            attempts--;
        }
        if (pointFound)
        {
            var go = Instantiate(_enemyPrefab, spawnPoint, Quaternion.identity, gameObject.transform);
            var enemy = go.GetComponent<NewEnemyBase>();
            enemy.MainTarget = Game.PSystems.PlayerObject.transform;
            var hpVal = _curveMultiplier + (HpCurve.Evaluate(_time) * _curveMultiplier);
            var hpDev = HpDevCurve.Evaluate(_time);

            if (_curveMultiplier % 3 == 0)
                hpDev *= _curveMultiplier / 3;

            var min = _baseHp + (hpVal - hpDev);
            var max = _baseHp + (hpVal + hpDev);
            enemy.SetHp(min, max);
            _spawnedEnemies.Add(go);

            if (debug != null)
            {
                debug.text = $"HpVal: {hpVal}\n";
                debug.text += $"HpDev: {hpDev}\n";
                debug.text += $"Mult: {_curveMultiplier}\n";
            }
        }
    }

    public void StopSpawn()
    {
        CancelInvoke();
    }

}
