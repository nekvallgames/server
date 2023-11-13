﻿using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Runtime.Services.AI.Outputs;
using Plugin.Schemes;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії, як саме і кому потрібно примінити хілку
    /// </summary>
    public class AIHealingTask : IAiTask
    {
        public const string TASK_NAME = "AIHealingTask";
        public string Name => TASK_NAME;

        /// <summary>
        /// Вказати в відсотках, від 0-100, скільки в юніта повинно бути життів,
        /// що б його вже можно було лікувати
        /// Наприклад, вказати 70 відсотків, то коли в юніта буде здоров'я менше чим 70 відсотків,
        /// то поточного юніта будуть лікувати 
        /// </summary>
        private const int HEALING_LINE = 70;

        private UnitsService _unitsService;
        private HealingDecisionService _healingDecisionService;
        private ActorService _actorService;
        private SyncRoomService _syncRoomService;

        public AIHealingTask(UnitsService unitsService,
                             HealingDecisionService healingDecisionService,
                             ActorService actorService,
                             SyncRoomService syncRoomService)
        {
            _unitsService = unitsService;
            _healingDecisionService = healingDecisionService;
            _actorService = actorService;
            _syncRoomService = syncRoomService;
        }

        public void ExecuteTask(string gameId)
        {
            int aiActorNr = _actorService.GetAiActor(gameId).ActorNr;

            var patients = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, aiActorNr, ref patients);

            if (patients.Count <= 0){
                return;    // в гравця не має взагалі юнітів, котрих можно вилікувати хілками
            }

            var decisions = new List<IUnitDecision>();
            foreach (IUnit patient in patients)
            {
                // Перебираємо кожного кандидата, і для кожного знаходимо рішення,
                // чи потрібно вказаного кандидата лікувати
                decisions.Add(_healingDecisionService.Decision<UnitDecisionScheme>(patient));
            }

            // allowOutputs - це список юнітів, котрих потрібно вилікувати
            var allowDecisions = decisions.FindAll(x => x.Output.OutputId == AllowOutput.OUTPUT_ID);
            if (!allowDecisions.Any()){
                return;    // не має рішень, які дозволяють прийняти рішення
            }

            // сортуємо юнітів по пріорітету лікування. 
            allowDecisions.Sort((x, y) => x.Output.Sum.CompareTo(y.Output.Sum));
            allowDecisions.Reverse();

            // підготовити дані із юнітами, котрих будемо лікувати
            var patientUnits = new List<UnitPatientData>();
            PreparePatientData(ref allowDecisions, ref patientUnits);

            // Список із юнітів, котрі мають хілки
            var medpacUnits = new List<UnitAdditionalData>();
            GetMedpacUnits(gameId, aiActorNr, ref medpacUnits);

            if (medpacUnits.Count <= 0){
                return;    // гравець не має юнітів із хілками
            }

            for (int i = 0; i < patientUnits.Count; i++)
            {
                if (!HasUnitsWhoCanHealing(medpacUnits))
                    continue;    // гравець не має юнітів, котрі мають хілку

                UnitPatientData pacient = patientUnits[i];

                // Перевіряємо, чи потрібно тратити хілку на поточного юніта
                if (!NeedToHealing(pacient))
                    continue;

                IUnit unitMedPack = GetUnitWithMedPack(medpacUnits);
                // var healingTypeComponent = _entityManager.GetComponentData<HealingTypeComponent>(unitMedPack.UnitEntity);

                // імітуємо лікуваннія поточного юніта
                // pacient.CurrHealth += (unitMedPack as IHealingAdditionalComponent).GetHealthPower();
                IHealthComponent healthComponent = (pacient.Unit as IHealthComponent);
                healthComponent.HealthCapacity += (unitMedPack as IHealingAdditionalComponent).GetHealthPower();
                if (healthComponent.HealthCapacity > healthComponent.HealthCapacityMax){
                    healthComponent.HealthCapacity = healthComponent.HealthCapacityMax;
                }

                // потратить хілку у медика
                ReduceAdditionalCapacity(unitMedPack, ref medpacUnits);

                // створити синхронізацію лікування юніта
                _syncRoomService.SyncAdditionalByUnit(gameId,
                                                      aiActorNr,
                                                      unitMedPack.UnitId,
                                                      unitMedPack.InstanceId,
                                                      pacient.Unit.OwnerActorNr,
                                                      pacient.Unit.UnitId,
                                                      pacient.Unit.InstanceId);

                // Перевіряємо, чи потрібно тратити хілку на поточного юніта
                if (NeedToHealing(pacient))
                {
                    i--;    // відкатуємо ітератор назад, що би знову спробувати вилікувати поточного юніта
                }
            }
        }

        /// <summary>
        /// Перевіряємо, чи потрібно тратити хілку на поточного юніта?
        /// </summary>
        private bool NeedToHealing(UnitPatientData pacient)
        {
            IHealthComponent healthComponent = pacient.Unit as IHealthComponent;

            int healthInPercent = (healthComponent.HealthCapacity / healthComponent.HealthCapacityMax) * 100;
            return healthInPercent < HEALING_LINE;
        }

        /// <summary>
        /// Підготовити дані із юнітами, котрих будемо лікувати
        /// </summary>
        private void PreparePatientData(ref List<IUnitDecision> allowDecisions, ref List<UnitPatientData> unitPatientData)
        {
            foreach (IUnitDecision decision in allowDecisions)
            {
                IUnit pacient = decision.Units[0];

                if (!(pacient is IHealthComponent))
                    continue;    // юніта не можна вилікувати, він не має життів

                int currHealth = _unitsService.GetHealthWithoutBuff(pacient);

                unitPatientData.Add(new UnitPatientData(pacient, currHealth, (pacient as IHealthComponent).HealthCapacityMax));
            }
        }

        /// <summary>
        /// Отримати список юнітів, котрі мають хілки
        /// </summary>
        private void GetMedpacUnits(string gameId, int actorNr, ref List<UnitAdditionalData> unitsData)
        {
            var unitsWithMedicPack = new List<IUnit>();
            _unitsService.GetUnitsWithMedicPack(gameId, actorNr, ref unitsWithMedicPack);

            if (unitsWithMedicPack.Count <= 0)
                return;

            foreach (IUnit unit in unitsWithMedicPack)
            {
                unitsData.Add(new UnitAdditionalData(unit,
                    (unit as IHealingAdditionalComponent).AdditionalCapacity));
            }
        }

        /// <summary>
        /// Зменшити капасіті додаткового єкшена вказаного юніта
        /// </summary>
        private void ReduceAdditionalCapacity(IUnit unit, ref List<UnitAdditionalData> medpacUnits)
        {
            UnitAdditionalData data =
                medpacUnits.Find(x => x.Unit.UnitId == unit.UnitId && x.Unit.InstanceId == unit.InstanceId);

            data.AdditionalCapacity--;
        }

        /// <summary>
        /// Чи є ще юніти, котрі мають хілки?
        /// </summary>
        private bool HasUnitsWhoCanHealing(List<UnitAdditionalData> unitsData)
        {
            return unitsData.Any(x => x.AdditionalCapacity > 0);
        }

        /// <summary>
        /// Отримати юніта із аптечкою
        /// </summary>
        private IUnit GetUnitWithMedPack(List<UnitAdditionalData> unitsData)
        {
            return unitsData.First(x => x.AdditionalCapacity > 0).Unit;
        }

        /// <summary>
        /// Додатковий клас, котрий зберігає в собі дані про юніта та капасіті додаткового єкшена
        /// </summary>
        internal class UnitAdditionalData
        {
            public IUnit Unit;
            public int AdditionalCapacity;

            public UnitAdditionalData(IUnit unit, int additionalCapacity)
            {
                Unit = unit;
                AdditionalCapacity = additionalCapacity;
            }
        }

        /// <summary>
        /// Додатковий клас, котрий зберігає в собі дані про юніта, котрого будемо лікувати
        /// </summary>
        internal class UnitPatientData
        {
            public IUnit Unit;
            public int CurrHealth;
            public int MaxHealth;

            public UnitPatientData(IUnit unit, int currHealth, int maxHealth)
            {
                Unit = unit;
                CurrHealth = currHealth;
                MaxHealth = maxHealth;
            }
        }
    }
}