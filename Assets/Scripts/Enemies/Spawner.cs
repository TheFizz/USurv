using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject _enemyPrefab;
    public GameObject _player;
    [SerializeField] private float _secondsToSpawn = 0.5f;
    [SerializeField] private int _spawnLimit = 200;
    List<GameObject> _spawnedEnemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 1, _secondsToSpawn);

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
            var y = 0.8f; //Arbitrary;
            var z = Random.Range(-45, 45);
            spawnPoint = new Vector3(x, y, z);

            if (Vector3.Distance(_player.transform.position, spawnPoint) > 10f)
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
            _spawnedEnemies.Add(GameObject.Instantiate(_enemyPrefab, spawnPoint, Quaternion.identity, gameObject.transform));
    }
}
