using System.Collections.Generic;
using System;

[Serializable]
public class EnemyStatePerTypeEntry
{
    public EEnemyType EnemyType;
    public List<EnemyStateEntry> StateEntries = new List<EnemyStateEntry>();
}