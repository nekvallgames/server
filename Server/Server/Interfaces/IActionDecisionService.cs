namespace Plugin.Interfaces
{
    public interface IActionDecisionService
    {
        /// <summary>
        /// Чи може поточний клас прийняти рішення кого атакувати для вказаного юніта?
        /// </summary>
        bool CanDecision(IUnit unit);

        /// <summary>
        /// Прийняти рішення, якого юніта потрібно атакувати
        /// </summary>
        /// <param name="unitHunter">юніт, котрий буде атакувати</param>
        /// <param name="targetActorId">id актора, юнітів котрого будемо атакувати</param>
        T Decision<T>(IUnit unitHunter, string gameId, int targetActorId) where T : IUnitDecision;
    }
}
