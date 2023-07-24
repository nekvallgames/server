using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    /// <summary>
	/// Сервіс, котрий буде реалізовувати систему сигналів
	/// </summary>
    public class SignalBus
    {
		public delegate void EventDelegate<T>(T e) where T : ISignal;
		private delegate void EventDelegate(ISignal e);

		private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
		private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();

		/// <summary>
		/// Подписаться на слушание сигнала
		/// </summary>
		public void Subscrible<T>(EventDelegate<T> del) where T : ISignal
		{
			// Early-out if we've already registered this delegate
			if (delegateLookup.ContainsKey(del))
				return;

			// Create a new non-generic delegate which calls our generic one.
			// This is the delegate we actually invoke.
			EventDelegate internalDelegate = (e) => del((T)e);
			delegateLookup[del] = internalDelegate;

			EventDelegate tempDel;
			if (delegates.TryGetValue(typeof(T), out tempDel))
			{
				delegates[typeof(T)] = tempDel += internalDelegate;
			}
			else
			{
				delegates[typeof(T)] = internalDelegate;
			}
		}

		/// <summary>
		/// Отписатся от слушания сигнала
		/// </summary>
		public void Unsubscrible<T>(EventDelegate<T> del) where T : ISignal
		{
			EventDelegate internalDelegate;
			if (delegateLookup.TryGetValue(del, out internalDelegate))
			{
				EventDelegate tempDel;
				if (delegates.TryGetValue(typeof(T), out tempDel))
				{
					tempDel -= internalDelegate;
					if (tempDel == null)
					{
						delegates.Remove(typeof(T));
					}
					else
					{
						delegates[typeof(T)] = tempDel;
					}
				}

				delegateLookup.Remove(del);
			}
		}

		/// <summary>
		/// Создать сигнал
		/// </summary>
		public void Fire(ISignal newEvent)
		{
			EventDelegate del;
			if (delegates.TryGetValue(newEvent.GetType(), out del))
			{
				del.Invoke(newEvent);
			}
		}
	}
}
