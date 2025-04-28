using System.Collections.Generic;
using System;

[Serializable]
public class EnemyTypeStateEntry
{
    public EEnemyType EnemyType;
    public List<EnemyStateEntry> StateEntries = new List<EnemyStateEntry>();
}