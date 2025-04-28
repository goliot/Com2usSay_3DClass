using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyStateDataSO", menuName = "Enemy/EnemyStateDataSO")]
public class EnemyStateDataSO : ScriptableObject
{
    public List<EnemyStatePerTypeEntry> enemyTypeStates = new List<EnemyStatePerTypeEntry>();

    private Dictionary<EEnemyType, Dictionary<EEnemyState, IEnemyState>> _stateDictionary;

    private void Initialize()
    {
        if (_stateDictionary != null) return;

        _stateDictionary = new Dictionary<EEnemyType, Dictionary<EEnemyState, IEnemyState>>();

        foreach (var enemyTypeEntry in enemyTypeStates)
        {
            var stateDict = new Dictionary<EEnemyState, IEnemyState>();

            foreach (var stateEntry in enemyTypeEntry.StateEntries)
            {
                if (stateEntry.StateLogic is IEnemyState enemyState)
                {
                    stateDict[stateEntry.State] = enemyState;
                }
                else
                {
                    Debug.LogWarning($"StateLogic {stateEntry.StateLogic} does not implement IEnemyState");
                }
            }

            _stateDictionary[enemyTypeEntry.EnemyType] = stateDict;
        }
    }

    public Dictionary<EEnemyState, IEnemyState> GetStateDictionary(EEnemyType type)
    {
        Initialize();

        if (_stateDictionary.TryGetValue(type, out var stateDict))
        {
            return stateDict;
        }
        else
        {
            Debug.LogError($"No states found for enemy type: {type}");
            return null;
        }
    }
}
