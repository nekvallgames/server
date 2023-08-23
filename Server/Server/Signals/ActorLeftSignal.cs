using Plugin.Interfaces;

namespace Plugin.Signals
{
    /// <summary>
    /// Подія, коли актор покинув гру
    /// </summary>
    public class ActorLeftSignal : ISignal
    {
        public IActorScheme Actor { get; }

        public ActorLeftSignal(IActorScheme actor)
        {
            Actor = actor;
        }
    }
}
