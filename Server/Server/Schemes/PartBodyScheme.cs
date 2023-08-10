using Plugin.Tools;

namespace Plugin.Schemes
{
    public struct PartBodyScheme
    {
        public int wIndex;
        public int hIndex;
        public Enums.PartBody partBody;

        public PartBodyScheme(int wIndex, int hIndex, Enums.PartBody partBody)
        {
            this.wIndex = wIndex;
            this.hIndex = hIndex;
            this.partBody = partBody;
        }
    }
}
