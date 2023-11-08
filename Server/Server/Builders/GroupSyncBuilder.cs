using Plugin.Interfaces;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Builders
{
    public class GroupSyncBuilder : IGroupSyncComponents
    {
        /// <summary>
        /// Тип групи. Що саме синхронізуємо?
        /// </summary>
        public Enums.SyncGroup SyncGroup { get; private set; }
        /// <summary>
        /// Компоненти, з котрих формується синхронізація поточної дії юніта
        /// </summary>
        public List<ISyncComponent> SyncComponents { get; } = new List<ISyncComponent>();

        /// <summary>
        /// Створити группу із компонентів для синхронізації Vip
        /// </summary>
        public IGroupSyncComponents CreateVip(IUnit unit)
        {
            SyncGroup = Enums.SyncGroup.vip;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unit.UnitId, unit.InstanceId)
                .SyncVip(unit);                     // сохранить, что юнит стал/перестал быть VIP 

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для симуляції синхронізації Vip
        /// </summary>
        public IGroupSyncComponents CreateSimulateVip(IUnit unit, bool isVip)
        {
            SyncGroup = Enums.SyncGroup.vip;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unit.UnitId, unit.InstanceId)
                .SimulateSyncVip(unit, isVip);                     // сохранить, что юнит стал/перестал быть VIP 

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для синхронізації дії юніта
        /// </summary>
        /// <param name="unitID"> Указать ID юнита, который совершил атаку </param>
        /// <param name="instanceID"> Указать инстанс юнита, который совершил атаку </param>
        /// <param name="targetActorID"> Указать игрока, к которого атаковали. Это будет наш рпротивник </param>
        /// <param name="cellW"> позиция тача на игровой сетке </param>
        /// <param name="cellH"> позиция тача на игровой сетке </param>
        public IGroupSyncComponents CreateAction(int unitId,
                                                 int instanceId,
                                                 int targetActorId,
                                                 int cellW,
                                                 int cellH)
        {
            SyncGroup = Enums.SyncGroup.action;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unitId, instanceId)     // юнит, который атаковал
                .SyncAction(targetActorId, cellW, cellH);          // точка выстрела

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для синхронізації додаткової дії по вказаних координатах
        /// </summary>
        /// <param name="unitID"> Указать ID юнита, который совершил дополнительное (пассивное) действие </param>
        /// <param name="instanceID"> Указать инстанс юнита, который совершил дополнительное (пассивное) действие </param>
        /// <param name="targetActorID"> Указать игрока, на которого было применено действие </param>
        /// <param name="cellW"> позиция тача на игровой сетке </param>
        /// <param name="cellH"> позиция тача на игровой сетке </param>
        public IGroupSyncComponents CreateAdditionalByPos(int unitId,
                                                          int instanceId,
                                                          int targetActorId,
                                                          int cellW,
                                                          int cellH)
        {
            SyncGroup = Enums.SyncGroup.additionalByPos;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unitId, instanceId)     // юнит, который атаковал
                .SyncAdditionalByPos(targetActorId, cellW, cellH);          // точка, в которой было применено лечение

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для синхронізації додаткової дії для вказаного юніта
        /// </summary>
        /// <param name="unitID"> Указать ID юнита, который совершил дополнительное (пассивное) действие </param>
        /// <param name="instanceID"> Указать инстанс юнита, который совершил дополнительное (пассивное) действие </param>
        /// <param name="targetActorID"> Указать игрока, на которого было применено действие </param>
        /// <param name="targetUnitId"> Вказати юніт Id, до котрого буде примінена додаткова дія </param>
        /// <param name="targetUnitInstanceId"> Вказати юніт інстанс Id, до котрого буде примінена додаткова дія </param>
        public IGroupSyncComponents CreateAdditionalByUnit(int unitId,
                                                           int instanceId,
                                                           int targetActorId,
                                                           int targetUnitId,
                                                           int targetUnitInstanceId)
        {
            SyncGroup = Enums.SyncGroup.additionalByUnit;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unitId, instanceId)     // юнит, который атаковал
                .SyncAdditionalByUnit(targetActorId, targetUnitId, targetUnitInstanceId);          // вказати юніта, до котрого було примінено додаткова дія

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для синхронізації позиції юніта на ігровій сітці
        /// </summary>
        public IGroupSyncComponents CreatePositionOnGrid(IUnit unit)
        {
            SyncGroup = Enums.SyncGroup.positionOnGrid;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unit.UnitId, unit.InstanceId)
                .SyncPositionOnGrid(unit);

            return this;
        }

        public IGroupSyncComponents CreatePositionOnGrid(IUnit unit,
                                                         int positionInGridW,
                                                         int positionInGridH)
        {
            SyncGroup = Enums.SyncGroup.positionOnGrid;

            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unit.UnitId, unit.InstanceId)
                .SyncPositionOnGrid(positionInGridW, positionInGridH);

            return this;
        }

        /// <summary>
        /// Створити группу із компонентів для синхронізації лікування юніта
        /// </summary>
        public IGroupSyncComponents CreateSimulateHealing(IUnit unitMedic, IUnit unitPatient)
        {
            SyncComponentsBuilder
                .Build(this)
                .SyncUnitId(unitMedic.UnitId, unitMedic.InstanceId)     // юніт, котрий буде лікувати
                .SyncAdditionalByUnit(unitMedic.OwnerActorNr, unitPatient.UnitId, unitPatient.InstanceId);    // юніт, котрого потрібно лікувати

            return this;
        }
    }
}
