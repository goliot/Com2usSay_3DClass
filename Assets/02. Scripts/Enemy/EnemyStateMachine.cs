using System.Collections.Generic;

public class EnemyStateMachine
{
    private Enemy _enemy;
    public Dictionary<EEnemyState, IEnemyState> StateDictionary { get; private set; }

    private IEnemyState _currentState;
    public IEnemyState CurrentState => _currentState;

    public EnemyStateMachine(Enemy enemy, Dictionary<EEnemyState, IEnemyState> stateDictionary)
    {
        _enemy = enemy;
        StateDictionary = stateDictionary;
        _currentState = StateDictionary[EEnemyState.Idle];
    }

    public void ChangeState(EEnemyState newState)
    {
        if (StateDictionary.TryGetValue(newState, out IEnemyState state))
        {
            if (_currentState != null && _currentState != state)
            {
                _currentState.Exit(_enemy);
            }
            _currentState = state;
            _currentState.Enter(_enemy);
        }
    }

    public void Update()
    {
        _currentState?.Execute(_enemy);
    }

    public void ModifyState(EEnemyState which, IEnemyState to)
    {
        StateDictionary[which] = to;
    }
}