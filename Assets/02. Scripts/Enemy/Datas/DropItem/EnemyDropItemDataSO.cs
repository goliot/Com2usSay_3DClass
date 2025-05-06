using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDropItemDataSO", menuName = "Scriptable Objects/EnemyDropItemDataSO")]
public class EnemyDropItemDataSO : ScriptableObject
{
    public List<EnemyDropItemEntry> ItemList;

    private Dictionary<EEnemyType, EnemyDropItemEntry> _itemDict;

    public EnemyDropItemEntry this[int index] => ItemList[index];

    public int Count => ItemList.Count;

    // Dictionary 초기화
    public void Initialize()
    {
        if (_itemDict != null) return;

        _itemDict = new Dictionary<EEnemyType, EnemyDropItemEntry>();
        foreach (var item in ItemList)
        {
            if (!_itemDict.ContainsKey(item.EnemyType))
                _itemDict.Add(item.EnemyType, item);
            else
                Debug.LogWarning($"중복된 EnemyType이 있습니다: {item.EnemyType}");
        }
    }

    // Dictionary 접근용 메서드
    public EnemyDropItemEntry GetEntry(EEnemyType type)
    {
        if (_itemDict == null) Initialize();

        if (_itemDict.TryGetValue(type, out var entry))
            return entry;

        Debug.LogWarning($"EnemyType {type}에 해당하는 아이템 정보가 없습니다.");
        return null;
    }
}
