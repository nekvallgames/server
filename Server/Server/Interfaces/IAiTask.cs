namespace Plugin.Interfaces
{
    public interface IAiTask : IName
    {
        void ExecuteTask(string gameId);
    }
}
