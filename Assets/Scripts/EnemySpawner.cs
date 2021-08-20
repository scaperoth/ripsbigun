using System;
using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject _player;
        [SerializeField]
        int _maxEnemies = 3;
        [SerializeField]
        float _spawnDelaySeconds = 3f;

        [Serializable]
        private class SpawnableEnemy
        {
            [SerializeField]
            PooledObject _enemy;
            [SerializeField]
            float _spawnRate = .5f;

            public PooledObject Enemy { get { return _enemy; } }
            public float SpawnRate { get { return _spawnRate; } }
        }

        [SerializeField]
        SpawnableEnemy[] _spawnableEnemies;

        List<PooledObject> _spawnedEnemies = new List<PooledObject>();

        Transform _cameraTransform;
        float _cameraBounds = 3f;
        float _lastSpawnTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            _cameraTransform = Camera.main.transform;
            _lastSpawnTime = -_spawnDelaySeconds;
        }

        // Update is called once per frame
        void Update()
        {
            if (_spawnedEnemies.Count >= _maxEnemies)
            {
                _lastSpawnTime = Time.time;
                return;
            }

            if (_lastSpawnTime + _spawnDelaySeconds < Time.time)
            {
                PooledObject poolableEnemy = GetEnemyToSpawn();
                Vector3 spawnPosition = GetSpawnPosition();
                SpawnEnemy(poolableEnemy, spawnPosition);

                _lastSpawnTime = Time.time;
            }
        }

        Vector3 GetSpawnPosition()
        {
            float randomChance = UnityEngine.Random.Range(0f, 1f);
            int boundsMultiplier = 1;
            if (randomChance > .5f)
            {
                boundsMultiplier = -1;
            }

            return new Vector3(
                _cameraTransform.position.x + (boundsMultiplier * _cameraBounds),
                _player.transform.position.y,
                UnityEngine.Random.Range(-1.5f, 0f)
            );
        }

        PooledObject GetEnemyToSpawn()
        {
            PooledObject enemyToSpawn = null;
            int maxIter = 100;
            int currentIter = 0;
            while (enemyToSpawn == null)
            {
                if (currentIter > maxIter)
                {
                    enemyToSpawn = _spawnableEnemies[0].Enemy;
                    break;
                }

                int spawnIndex = UnityEngine.Random.Range(0, _spawnableEnemies.Length);
                SpawnableEnemy spawnableEnemy = _spawnableEnemies[spawnIndex];

                float spawnChance = UnityEngine.Random.Range(0f, 1f);
                if (spawnableEnemy.SpawnRate > spawnChance)
                {
                    enemyToSpawn = spawnableEnemy.Enemy;
                    break;
                }

                currentIter++;
            }

            return enemyToSpawn;
        }

        void SpawnEnemy(PooledObject enemy, Vector3 position)
        {
            var instance = Pool.Instance.Spawn(enemy, position, Quaternion.Euler(0f, 0f, 0f));
            instance.As<EnemyController>().SetPlayerTransform(_player.transform);
            _spawnedEnemies.Add(instance);
            instance.OnDespawn.AddListener(RemoveFromSpawnList);
        }

        void RemoveFromSpawnList(PooledObject obj)
        {
            _spawnedEnemies.Remove(obj);
        }
    }
}