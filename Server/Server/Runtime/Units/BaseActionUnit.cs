using Plugin.Interfaces.Actions;
using Plugin.Parameters;
using Plugin.Schemes.Public;
using Plugin.Tools;
using System;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Базовий клас для юнітів, котрі мають основний єкшен, це нанесення урона
    /// </summary>
    public abstract class BaseActionUnit : BaseUnit, IDamageAction
    {
        public int Damage { get; set; }
        public abstract int OriginalDamage { get; }   


        public int ActionCapacity { get; set; }
        public abstract int OriginalActionCapacity { get; set; }

        public abstract Int2[] ActionArea { get; }

        public BaseActionUnit(UnitFactoryParameters parameters) : base(parameters)
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
            ActionCapacity = OriginalActionCapacity;
        }

        public bool CanExecuteAction()
        {
            return ActionCapacity > 0;
        }

        public void SpendAction()
        {
            // Capacity--;      // TODO розкомітити, коли прикручу збільшення патронів в режимі перестрілки із віпом
            if (ActionCapacity < 0){
                ActionCapacity = 0;
                throw new Exception($"BaseDamageActionUnit :: Used() capacity decrease below zero. ActorId = {OwnerActorNr}, unitId = {UnitId}, instanceId = {InstanceId}");
            }
        }
    }
}
