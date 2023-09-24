namespace Plugin.Schemes
{
    public class CellTemperatureScheme
    {
        public int ActorId { get; }
        public int PositionW { get; }
        public int PositionH { get; }
        public float Temperature { get; set; }

        public CellTemperatureScheme(int actorId, int positionW, int positionH, float temperature)
        {
            ActorId = actorId;
            PositionW = positionW;
            PositionH = positionH;
            Temperature = temperature;
        }
    }
}
