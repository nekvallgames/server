namespace Plugin.Interfaces
{
    public interface IStatesModel<T> where T : IState
    {
        void Add(T state);
        T Get(string stateName);
        bool Has(string stateName);
    }
}
