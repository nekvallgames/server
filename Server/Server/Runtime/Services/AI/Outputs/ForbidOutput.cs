namespace Plugin.Runtime.Services.AI.Outputs
{
    /// <summary>
    /// Output рішення заборонити вказани рішення!
    /// </summary>
    public class ForbidOutput : BaseOutput
    {
        public const int OUTPUT_ID = 3;
        public override int OutputId => OUTPUT_ID;

        public ForbidOutput(float[] inputs,
                            float[] weight,
                            float activation,
                            bool isLog) : base(inputs, weight, activation, isLog)
        {

        }
    }
}
