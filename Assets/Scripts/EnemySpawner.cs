using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject _player;
        [SerializeField]
        PooledObject[] _enemiesToSpawn;
        [SerializeField]
        int _maxEnemies = 3;

        List<PooledObject> _spawnedEnemies = new List<PooledObject>();

        Transform _cameraTransform;
        float _cameraBounds = 3f;
        float _lastSpawnTime = 0f;
        float _spawnRate = 4f;

        // Start is called before the first frame update
        void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if(_spawnedEnemies.Count >= _maxEnemies)
            {
                _lastSpawnTime = Time.time;
                return;
            }

            if (_lastSpawnTime + _spawnRate < Time.time)
            {
                PooledObject poolableEnemy = _enemiesToSpawn[Random.Range(0, _enemiesToSpawn.Length - 1)];

                float randomChance = Random.Range(0, 1);
                int boundsMultiplier = 1;
                if(randomChance > .5f)
                {
                    boundsMultiplier = -1;
                }

                Vector3 spawnPosition = new Vector3(
                    _cameraTransform.position.x + (boundsMultiplier * _cameraBounds),
                    _player.transform.position.y,
                    0
                );

                var instance = Pool.Instance.Spawn(poolableEnemy, spawnPosition, Quaternion.Euler(0f, 0f, 0f));
                instance.As<EnemyController>().SetPlayerTransform(_player.transform);
                _spawnedEnemies.Add(instance);
                instance.OnDespawn.AddListener(RemoveFromSpawnList);

                _lastSpawnTime = Time.time;
            }
        }

        void RemoveFromSpawnList(PooledObject obj)
        {
            _spawnedEnemies.Remove(obj);
        }
    }

}