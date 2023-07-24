using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Models.Private;
using Plugin.Runtime.Services.ExecuteAction;
using Plugin.Schemes;
using Plugin.Signals;
using Plugin.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Контроллер, который выполняет всякую логику, котора нужна для юнитов
    /// </summary>
    public class UnitsService
    {
        private UnitsPrivateModel _model;
        private OpStockService _opStockService;
        private ConvertService _convertService;
        private UnitBuilder _unitBuilder;
        private MoveService _moveService;

        public UnitsService(UnitsPrivateModel model, 
                            OpStockService opStockService, 
                            ConvertService convertService, 
                            UnitBuilder unitBuilder, 
                            SignalBus signalBus, 
                            MoveService moveService)
        {
            _model = model;

            _opStockService = opStockService;
            _convertService = convertService;
            _unitBuilder = unitBuilder;
            _moveService = moveService;

            signalBus.Subscrible<OpStockPrivateModelSignal>( OpStockModelChange );
        }

        /// <summary>
        /// Модель із операціями акторів була оновлена
        /// </summary>
        private void OpStockModelChange(OpStockPrivateModelSignal signalData)
        {
            // Якщо це операція choosedUnitsForGame, це означає, що потрібно для гравця створити юнітів
            if (signalData.OpCode == OperationCode.choosedUnitsForGame 
                && signalData.Status == OpStockPrivateModelSignal.StatusType.add)
            {
                // Отримати зі складу операцію, в якій знаходяться дані із юнітами, котрих обрав актор
                var opChoosedUnits = _opStockService.GetOp(signalData.GameId, signalData.ActorId, signalData.OpCode);

                var choosedUnitsScheme = _convertService.DeserializeObject<ChoosedUnitsScheme>(opChoosedUnits.Data.ToString());

                foreach( int unitId in choosedUnitsScheme.unitsId ){
                    CreateUnit(signalData.GameId, signalData.ActorId, unitId);
                }

                // Видалити оброблену операцію зі складу
                _opStockService.TakeOp(signalData.GameId, signalData.ActorId, signalData.OpCode);
            }
        }

        /// <summary>
        /// Створити юніта для актора
        /// </summary>
        public IUnit CreateUnit(string gameId, int actorId, int unitId)
        {
            IUnit unit = _unitBuilder.CreateUnit(gameId, actorId, unitId);

            _model.Add(unit);

            return unit;
        }

        /// <summary>
        /// Створити юніта для актора та позиціюнювати юніта на ігровій сітці
        /// </summary>
        public IUnit CreateUnit(string gameId, int actorId, int unitId, int posW, int posH)
        {
            IUnit unit = CreateUnit(gameId, actorId, unitId);

            _moveService.PositionOnGrid(unit, posW, posH);

            return unit;
        }

        public bool IsDead(IUnit unit)
        {
            return unit.IsDead;
        }

        /// <summary>
        /// Получить юнитов, которые находятся в позиции posW, posH на игровой сетке
        /// unitOwnerID - указать владельца юнита, выстрел по которому будем проверять, попали мы или нет
        /// posW - позиция на игровой сетке по ширине
        /// posH - позиция на игровой сетке по ширине
        /// </summary>
        public List<IUnit> GetUnitsUnderThisPosition(string gameId, int unitOwnerId, int posW, int posH )
        {
            var unitsUnderPos = new List<IUnit>();

            foreach (var unit in _model.Items)
            {
                // 1. Проверяем, юнит принадлежит игроку unitOwnerID
                if (unit.GameId == gameId && unit.OwnerActorId == unitOwnerId){
                    // 2. Проверяем, попали мы по этому юниту
                    if (IsPositionUnderUnitArea(unit, posW, posH)){
                        unitsUnderPos.Add(unit);
                    }
                }
            }

            return unitsUnderPos;
        }


        /// <summary>
        /// Восстановить все действия юнитов (перезарядить оружия, восстановить магию и т.д.)
        /// </summary>
        public void ReviveAction(string gameId, int actorId)
        {
            List<IUnit> units = _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorId == actorId && x is IActionComponent);

            foreach (IUnit unit in units){
                ((IActionComponent)unit).ReviveAction();
            }
        }

        public IUnit GetUnit(string gameId, int actorId, int unitId, int instanceId){
            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorId == actorId && x.UnitId == unitId && x.InstanceId == instanceId);
        }

        public List<IUnit> GetUnits(string gameId, int actorId)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorId == actorId);
        }

        public List<IUnit> GetUnits(string gameId, int actorId, int unitId)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorId == actorId && x.UnitId == unitId);
        }

        /// <summary>
        /// Нанести урон юниту
        /// </summary>
        public void SetDamage( IUnit unit, int damage )
        {
            bool hasHealth = HasComponent<IHealthComponent>(unit);
            bool hasArmor = HasComponent<IArmorComponent>(unit);

            if ((hasArmor && hasHealth) || hasArmor){
                SetDamageByArmor(unit, damage);
            }
            else
                if (hasHealth){
                    SetDamageByHealth(unit, damage);
                }
        }

        /// <summary>
        /// Вылечить текущего юнита
        /// </summary>
        public void Healing(IUnit unit, int healthPower)
        {
            if (!HasComponent<IHealthComponent>(unit)){
                Debug.Fail($"UnitService :: Healing() I can't healing unit, because this unit don't have IHealthComponent. ActorId = {unit.OwnerActorId}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}");
                return;
            }

            var healthComponent = (IHealthComponent)unit;

            int health = healthComponent.Capacity + healthPower;

            if (health > healthComponent.CapacityMax){
                health = healthComponent.CapacityMax;
            }

            ((IHealthComponent)unit).Capacity = health;
        }

        /// <summary>
        /// Ячейка posW, posH находится под Area юнита
        /// </summary>
        private bool IsPositionUnderUnitArea(IUnit unit, int posW, int posH)
        {
            // Позиция юнита на игровой сетке.  
            Int2 unitPos = unit.Position;

            // Ширина и высота юнита, которую он занимает на игровой сетке
            var bodySize = new Int2(unit.BodySize.x - 1 < 0 ? 0 : unit.BodySize.x - 1,
                                    unit.BodySize.y - 1 < 0 ? 0 : unit.BodySize.y - 1);

            if ((posW >= unitPos.x) && (posW <= (unitPos.x + bodySize.x))       // проверяем по ширине
                && (posH >= unitPos.y) && (posH <= (unitPos.y + bodySize.y)))   // проверяем по высоте
            {
                return true;
            }

            return false;


        }

        /// <summary>
        /// Игрок имеет живых юнитов?
        /// </summary>
        public bool HasAliveUnit(string gameId, int actorId)
        {
            return _model.Items.Any(x => x.GameId == gameId && x.OwnerActorId == actorId && !x.IsDead);
        }

        /// <summary>
        /// Перебрать всех юнитов, и вернуть истину, если есть мертвые юниты
        /// </summary>
        public bool HasAnyDeadUnit(string gameId, int actorId)
        {
            return _model.Items.Any(x => x.GameId == gameId && x.OwnerActorId == actorId && x.IsDead);
        }

        /// <summary>
        /// В поточного юніта є вказаний компонент?
        /// </summary>
        public bool HasComponent<T>(IUnit unit)
        {
            return typeof(T).IsAssignableFrom(unit.GetType());
        }

        /// <summary>
        /// Нанести урон по жизням
        /// </summary>
        private void SetDamageByHealth(IUnit unit, int damage)
        {
            int curr = ((IHealthComponent)unit).Capacity - damage;
            if (curr < 0) curr = 0;
            
            ((IHealthComponent)unit).Capacity = curr;
        }

        /// <summary>
        /// Нанести урон по броні
        /// </summary>
        private void SetDamageByArmor(IUnit unit, int damage)
        {
            int curr = ((IArmorComponent)unit).Capacity - damage;
            if (curr < 0) curr = 0;

            ((IArmorComponent)unit).Capacity = curr;
        }
    }

}
