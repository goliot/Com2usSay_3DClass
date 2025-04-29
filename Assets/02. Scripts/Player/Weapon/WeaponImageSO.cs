using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WeaponImageSO", menuName = "Scriptable Objects/WeaponImageSO")]
public class WeaponImageSO : ScriptableObject
{
    public List<WeaponImageData> Datas;

    private Dictionary<EWeaponType, WeaponImageData> _weaponDict;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _weaponDict = new Dictionary<EWeaponType, WeaponImageData>();
        foreach (var data in Datas)
        {
            if (!_weaponDict.ContainsKey(data.Type))
                _weaponDict.Add(data.Type, data);
        }
    }

    public WeaponImageData GetWeaponImageData(EWeaponType type)
    {
        if(_weaponDict == null)
        {
            Init();
        }

        if (_weaponDict.TryGetValue(type, out var data))
            return data;

        Debug.LogWarning($"Weapon ID '{type}' not found!");
        return null;
    }
}
