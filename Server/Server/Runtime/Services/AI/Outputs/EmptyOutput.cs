namespace Plugin.Runtime.Services.AI.Outputs
{
    /// <summary>
    /// Output пустишка. Використати для ситуацій, коли ніодне рішення не було прийнято
    /// </summary>
    public class EmptyOutput : BaseOutput
    {
        public const int OUTPUT_ID = 1;
        public override int OutputId => OUTPUT_ID;
    }
}
