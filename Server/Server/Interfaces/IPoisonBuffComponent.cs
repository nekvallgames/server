namespace Plugin.Interfaces
{
    /// <summary>
    /// Повісити поточний компонент на юніта, якщо на нього може впливати яд
    /// </summary>
    public interface IPoisonBuffComponent
    {
        int PoisonBuff { get; set; }
    }
}
