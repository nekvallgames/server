namespace Plugin.Runtime.Services.AI.Outputs
{
    /// <summary>
    /// Output рішення дозволити вказани рішення!
    /// </summary>
    public class AllowOutput : BaseOutput
    {
        public const int OUTPUT_ID = 2;
        public override int OutputId => OUTPUT_ID;

        public AllowOutput(float[] inputs,
                           float[] weight,
                           float activation,
                           bool isLog) : base(inputs, weight, activation, isLog)
        {

        }
    }
}
