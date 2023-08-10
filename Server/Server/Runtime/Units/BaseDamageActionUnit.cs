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
    public abstract class BaseDamageActionUnit : BaseUnit, IDamageAction
    {
        public int Damage { get; set; }
        public abstract int OriginalDamage { get; }   


        public int DamageCapacity { get; set; }
        public abstract int OriginalDamageCapacity { get; set; }

        public abstract Int2[] DamageActionArea { get; }

        public BaseDamageActionUnit(UnitFactoryParameters parameters) : base(parameters)
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
            DamageCapacity = OriginalDamageCapacity;
        }

        public bool CanExecuteAction()
        {
            return DamageCapacity > 0;
        }

        public void SpendAction()
        {
            // Capacity--;      // TODO розкомітити, коли прикручу збільшення патронів в режимі перестрілки із віпом
            if (DamageCapacity < 0){
                DamageCapacity = 0;
                throw new Exception($"BaseDamageActionUnit :: Used() capacity decrease below zero. ActorId = {OwnerActorNr}, unitId = {UnitId}, instanceId = {InstanceId}");
            }
        }
    }
}
