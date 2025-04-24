using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public List<EnemyStat> EnemyStats;

    private Dictionary<EEnemyType, EnemyStat> _enemyDict;

    public void Init()
    {
        _enemyDict = new Dictionary<EEnemyType, EnemyStat>();
        foreach (var data in EnemyStats)
        {
            if (!_enemyDict.ContainsKey(data.EnemyType))
                _enemyDict.Add(data.EnemyType, data);
        }
    }

    public EnemyStat GetData(EEnemyType type)
    {
        if (_enemyDict == null)
            Init();

        if (_enemyDict.TryGetValue(type, out var data))
            return data;

        Debug.LogWarning($"Weapon ID '{type}' not found!");
        return null;
    }
}
