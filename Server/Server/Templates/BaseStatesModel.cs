using Plugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Templates
{
    /// <summary>
    /// Базовай клас для всіх моделей, котрі будуть зберігати в собі стейти для роботи стейт машини
    /// </summary>
    /// <typeparam name="T">вказати тип стейта, з котрим модель буде працювати</typeparam>
    public abstract class BaseStatesModel<T> : IStatesModel<T> where T : class, IState
    {
        /// <summary>
        /// Храним все состояния
        /// </summary>
        private Dictionary<string, T> _states = new Dictionary<string, T>();

        public void Add(T state)
        {
            if (_states.ContainsKey(state.Name))
            {
                Debug.Fail($"{this} :: Add() I already has current state {state.Name}");
                return;
            }

            _states.Add(state.Name, state);
        }

        public T Get(string stateName)
        {
            return _states[stateName];
        }

        public bool Has(string stateName)
        {
            return _states.ContainsKey(stateName);
        }
    }
}
