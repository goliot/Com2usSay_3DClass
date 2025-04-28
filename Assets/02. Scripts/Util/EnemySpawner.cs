using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnCoolTime;

    private float _timer = 0;

    private void Awake()
    {
        _timer = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnCoolTime)
        {
            RandomSpawn();
            _timer = 0f;
        }
    }

    private void RandomSpawn()
    {
        Vector3 spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;

        GameObject enemy = EnemyPoolManager.Instance.GetObject((EEnemyType)Random.Range(0, (int)EEnemyType.Count), spawnPosition);
    }
}
