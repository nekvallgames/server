using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Templates
{
    public abstract class BaseModel<T> : IModel<T>
    {
        public List<T> Items { get; } = new List<T>();

        /// <summary>
        /// Добавити елемент до списку
        /// </summary>
        public void Add(T item)
        {
            if (!CanAddHook(item))
                return;

            Items.Add(item);
            AfterAddHook(item);
        }

        /// <summary>
        /// Видалити елемент із списку
        /// </summary>
        public void Remove(T item)
        {
            Items.Remove(item);
            AfterRemoveHook(item);
        }

        /// <summary>
        /// Заоверайдити метот, що би реалізувати перевірку, 
        /// чи можно/не можно добавити поточний єлемент до списку
        /// </summary>
        protected virtual bool CanAddHook(T item) { return true; }
        /// <summary>
        /// Виконається після добавлення поточного єлементу до списку
        /// </summary>
        protected virtual void AfterAddHook(T item) { }
        /// <summary>
        /// Виконається після видалення поточного єлементу із списка
        /// </summary>
        protected virtual void AfterRemoveHook(T item) { }

    }
}
