using Plugin.Interfaces;
using Plugin.Templates;

namespace Plugin.Models.Private
{
    public class ActorsPrivateModel : BaseModel<IActorScheme>, IPrivateModel
    {
        public bool Any { get; internal set; }
    }
}
