using Plugin.Schemes.Public;

namespace Plugin.Runtime.Services
{
    public class PlotPublicService
    {
        private const string jsonName = "plot_pvp";

        public PlotPublicScheme Data { get; }

        public PlotPublicService(JsonReaderService jsonReaderService)
        {
            Data = jsonReaderService.Read<PlotPublicScheme>(jsonName);
        }
    }
}
