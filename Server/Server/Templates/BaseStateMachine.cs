using Plugin.Interfaces;
using System;
using System.Diagnostics;

namespace Plugin.Templates
{
    /// <summary>
    /// Базовий клас стейт машини
    /// TState - вказати тип класу стейта, із котрим стейт машина буде працювати
    /// TModel - вказати клас
    /// </summary>
    public abstract class BaseStateMachine<TState, TModel>
        where TState : IState
        where TModel : IStatesModel<TState>
    {
        private TModel _model;

        /// <summary>
        /// Текущее игровое состояние
        /// </summary>
        public TState CurrState { get; private set; }

        public BaseStateMachine(TModel model)
        {
            _model = model;
        }

        /// <summary>
        /// Изменить состояние сценария
        /// </summary>
        protected void ChangeState(string nextStateName)
        {
            if (!_model.Has(nextStateName))
            {
                Debug.Fail($"{GetType()} :: ChangeState() I don't know this state {nextStateName}");
                return;
            }

            if (CurrState != null)
            {
                CurrState.ExitState();
                AfterExitStateHook(CurrState);
            }

            CurrState = _model.Get(nextStateName);

            BeforeEnterStateHook(CurrState);

            CurrState.EnterState();

            AfterEnterStateHook(CurrState);
        }

        /// <summary>
        /// Виконається до входу до вказаного стейта
        /// </summary>
        protected virtual void BeforeEnterStateHook(TState state)
        {

        }

        /// <summary>
        /// Виконається після входу до вказаного стейта
        /// </summary>
        protected virtual void AfterEnterStateHook(TState state)
        {

        }

        /// <summary>
        /// Виконається після виходу із вказаного стейта
        /// </summary>
        protected virtual void AfterExitStateHook(TState state)
        {

        }
    }
}
