using System.Collections.Generic;

public class EnemyStateMachine
{
    private Enemy _enemy;

    private IEnemyState _currentState;
    public IEnemyState CurrentState => _currentState;

    public EnemyStateMachine(Enemy enemy, IEnemyState state)
    {
        _enemy = enemy;
        _currentState = state;
    }

    public void ChangeState(IEnemyState newState)
    {
        if (_currentState != null && _currentState != newState)
        {
            _currentState.Exit(_enemy);
        }
        _currentState = newState;
        _currentState.Enter(_enemy);
    }

    public void Update()
    {
        _currentState?.Execute(_enemy);
    }
}