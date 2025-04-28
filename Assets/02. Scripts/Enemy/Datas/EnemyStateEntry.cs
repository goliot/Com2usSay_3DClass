using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStateEntry
{
    public EEnemyState State;
    public ScriptableObject StateLogic; // 나중에 IEnemyState로 변환해서 쓸 것
}
