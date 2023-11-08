namespace Plugin.Schemes
{
    public class CellTemperatureScheme
    {
        public int PositionW { get; }
        public int PositionH { get; }
        public float Temperature { get; set; }

        public CellTemperatureScheme(int positionW, int positionH, float temperature)
        {
            PositionW = positionW;
            PositionH = positionH;
            Temperature = temperature;
        }
    }
}
