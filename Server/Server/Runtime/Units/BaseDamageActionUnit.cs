using Plugin.Interfaces.Actions;
using Plugin.Tools;
using System;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Базовий клас для юнітів, котрі мають основний єкшен, це нанесення урона
    /// </summary>
    public abstract class BaseDamageActionUnit : BaseUnit, IDamageAction
    {
        public int Power { get; set; }
        public abstract int OriginalPower { get; }   


        public int Capacity { get; set; }
        public abstract int OriginalCapacity { get; set; }

        public abstract Int2[] DamageActionArea { get; }


        public BaseDamageActionUnit(string gameId, int ownerActorId, int unitId, int instanceUnitId) : base(gameId, ownerActorId, unitId, instanceUnitId)
        {
            
        }

        public void Initialize()
        {
            
        }

        public void ReviveAction()
        {
            Reload();
        }

        public void Reload()
        {
            Capacity = OriginalCapacity;
        }

        public bool CanExecuteAction()
        {
            return Capacity > 0;
        }

        public void SpendAction()
        {
            // Capacity--;      // TODO розкомітити, коли прикручу збільшення патронів в режимі перестрілки із віпом
            if (Capacity < 0){
                Capacity = 0;
                throw new Exception($"BaseDamageActionUnit :: Used() capacity decrease below zero. ActorId = {OwnerActorNr}, unitId = {UnitId}, instanceId = {InstanceId}");
            }
        }
    }
}
