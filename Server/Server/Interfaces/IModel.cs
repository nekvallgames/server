using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IModel<T>
    {
        void Add(T item);

        List<T> Items { get; }
    }
}
