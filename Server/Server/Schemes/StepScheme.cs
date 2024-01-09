using Plugin.Interfaces;
using Plugin.OpComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема для синхронізації дій ігрового кроку
    /// </summary>
    [Serializable]
    public class StepScheme : IOpScheme
    {
        public List<ActionOpComponent> syncActions = new List<ActionOpComponent>();
        public List<AdditionalByPosOpComponent> syncAdditionalByPos = new List<AdditionalByPosOpComponent>();
        public List<AdditionalByUnitOpComponent> syncAdditionalByUnit = new List<AdditionalByUnitOpComponent>();
        public List<PositionOnGridOpComponent> syncPositionOnGrid = new List<PositionOnGridOpComponent>();
        public List<UnitIdOpComponent> syncUnitId = new List<UnitIdOpComponent>();
        public List<VipOpComponent> syncVip = new List<VipOpComponent>();
                        
        public void Add(ISyncComponent component)
        {
            // TODO поки що не можу реалізувати по інакшому, не вистачає досвіду
            if (component.GetType() == typeof(ActionOpComponent))
                syncActions.Add((ActionOpComponent)component);
            else
            if (component.GetType() == typeof(AdditionalByPosOpComponent))
                syncAdditionalByPos.Add((AdditionalByPosOpComponent)component);
            else
            if (component.GetType() == typeof(PositionOnGridOpComponent))
                syncPositionOnGrid.Add((PositionOnGridOpComponent)component);
            else
            if (component.GetType() == typeof(UnitIdOpComponent))
                syncUnitId.Add((UnitIdOpComponent)component);
            else
            if (component.GetType() == typeof(AdditionalByUnitOpComponent))
                syncAdditionalByUnit.Add((AdditionalByUnitOpComponent)component);
            else
            if (component.GetType() == typeof(VipOpComponent))
                syncVip.Add((VipOpComponent)component);
        }

        /// <summary>
        /// Отримати компоненти синхронізації, вказав syncStep та groupIndex
        /// </summary>
        public List<ISyncComponent> Get(int syncStep, int groupIndex)
        {
            var syncComponents = new List<ISyncComponent>();

            DragAndDrop(ref syncActions, ref syncComponents);
            DragAndDrop(ref syncAdditionalByPos, ref syncComponents);
            DragAndDrop(ref syncPositionOnGrid, ref syncComponents);
            DragAndDrop(ref syncUnitId, ref syncComponents);
            DragAndDrop(ref syncAdditionalByUnit, ref syncComponents);
            DragAndDrop(ref syncVip, ref syncComponents);

            void DragAndDrop<T>(ref List<T> from, ref List<ISyncComponent> to) where T : ISyncComponent
            {
                if (from.Any(x => x.SyncStep == syncStep && x.GroupIndex == groupIndex)){
                    to.Add(from.First(x => x.SyncStep == syncStep && x.GroupIndex == groupIndex));
                }
            }

            return syncComponents;
        }

        /// <summary>
        /// Розділити StepScheme котрий зберігає в собі декілька кроків на StepSchemes 
        /// котрі зберігають в собі тільки окремі кроки. Наприклад, на вході StepScheme
        /// котрий зберігає в собі 2 кроки, переміщення та атаку, на виході ми отримаємо 
        /// 2 окремих StepScheme, переміщення і окремо атаку
        /// </summary>
        public static Dictionary<int, StepScheme> SeparateBySteps(StepScheme stepScheme)
        {
            Dictionary<int, StepScheme> steps = new Dictionary<int, StepScheme>();

            foreach (ActionOpComponent component in stepScheme.syncActions)
            {
                if (!steps.ContainsKey(component.SyncStep)){
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncActions.Add(component);
            }

            foreach (AdditionalByPosOpComponent component in stepScheme.syncAdditionalByPos)
            {
                if (!steps.ContainsKey(component.SyncStep)){
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncAdditionalByPos.Add(component);
            }

            foreach (AdditionalByUnitOpComponent component in stepScheme.syncAdditionalByUnit)
            {
                if (!steps.ContainsKey(component.SyncStep))
                {
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncAdditionalByUnit.Add(component);
            }

            foreach (PositionOnGridOpComponent component in stepScheme.syncPositionOnGrid)
            {
                if (!steps.ContainsKey(component.SyncStep))
                {
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncPositionOnGrid.Add(component);
            }

            foreach (UnitIdOpComponent component in stepScheme.syncUnitId)
            {
                if (!steps.ContainsKey(component.SyncStep))
                {
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncUnitId.Add(component);
            }

            foreach (VipOpComponent component in stepScheme.syncVip)
            {
                if (!steps.ContainsKey(component.SyncStep))
                {
                    steps.Add(component.SyncStep, new StepScheme());
                }
                steps[component.SyncStep].syncVip.Add(component);
            }

            return steps;
        }
    }
}
