using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Models.Private;
using Plugin.Runtime.Services.ExecuteAction;
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
        private UnitBuilder _unitBuilder;
        private MoveService _moveService;

        public UnitsService(UnitsPrivateModel model,  
                            UnitBuilder unitBuilder, 
                            SignalBus signalBus, 
                            MoveService moveService)
        {
            _model = model;

            _unitBuilder = unitBuilder;
            _moveService = moveService;
        }

        /// <summary>
        /// Створити юніта для актора
        /// </summary>
        public IUnit CreateUnit(string gameId, int actorNr, int unitId, int level = 0)
        {
            if (unitId == -1)
                return null;

            IUnit unit = _unitBuilder.CreateUnit(gameId, actorNr, unitId, level);

            _model.Add(unit);

            return unit;
        }

        /// <summary>
        /// Створити юніта для актора та позиціюнювати юніта на ігровій сітці
        /// </summary>
        public IUnit CreateUnit(string gameId, int actorNr, int unitId, int posW, int posH)
        {
            if (unitId == -1)
                return null;

            IUnit unit = CreateUnit(gameId, actorNr, unitId);

            if (unit == null)
                return null;

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
                if (unit.GameId == gameId && unit.OwnerActorNr == unitOwnerId){
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
        public void ReviveAction(string gameId, int actorNr)
        {
            List<IUnit> units = _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr && HasComponent<IActionComponent>(x));

            foreach (IUnit unit in units){
                ((IActionComponent)unit).ReviveAction();
            }
        }

        public IUnit GetUnit(string gameId, int actorNr, int unitId, int instanceId){
            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorNr == actorNr && x.UnitId == unitId && x.InstanceId == instanceId);
        }

        public List<IUnit> GetUnits(string gameId, int actorNr)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr);
        }

        public List<IUnit> GetUnits(string gameId, int actorNr, int unitId)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr && x.UnitId == unitId);
        }

        /// <summary>
        /// Нанести урон юниту
        /// </summary>
        public void SetDamage(IUnit unit, int damage)
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
                Debug.Fail($"UnitService :: Healing() I can't healing unit, because this unit don't have IHealthComponent. ActorId = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}");
                return;
            }

            var healthComponent = (IHealthComponent)unit;

            int health = healthComponent.HealthCapacity + healthPower;

            if (health > healthComponent.HealthCapacityMax){
                health = healthComponent.HealthCapacityMax;
            }

            ((IHealthComponent)unit).HealthCapacity = health;
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
        public bool HasAliveUnit(string gameId, int actorNr)
        {
            return _model.Items.Any(x => x.GameId == gameId && x.OwnerActorNr == actorNr && !x.IsDead);
        }

        /// <summary>
        /// Отримати кількість живих юнітів
        /// </summary>
        public int GetAliveUnitsCount(string gameId, int actorNr)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr && !x.IsDead && !HasComponent<IBarrier>(x)).Count;
        }

        /// <summary>
        /// Отримати живого юніта, котрий може стати випом
        /// </summary>
        public IUnit GetAnyAliveUnitWhoWillBeAbleToVip(string gameId, int actorNr)
        {
            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorNr == actorNr && !x.IsDead && HasComponent<IVipComponent>(x));
        }

        /// <summary>
        /// Отримати список юнітів, котрі можуть стати VIP-ами
        /// </summary>
        public void GetAliveUnitsWhoWillBeAbleToVip(string gameId, int actorNr, ref List<IUnit> vipUnits)
        {
            List<IUnit> units = GetUnits(gameId, actorNr);

            foreach (IUnit unit in units)
            {
                if (CanBecomeVip(unit))
                    vipUnits.Add(unit);
            }
        }

        /// <summary>
        /// Чи може вказаний юніт стати чи бути VIP-ом?
        /// </summary>
        public bool CanBecomeVip(IUnit unit)
        {
            return !unit.IsDead && HasComponent<IVipComponent>(unit);
        }

        /// <summary>
        /// Отримати кількість живих юнітів, котрі можуть стати віпами
        /// </summary>
        public int GetAliveUnitsCountWhoWillBeAbleToVip(string gameId, int actorNr)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr && !x.IsDead && HasComponent<IVipComponent>(x)).Count;
        }
        
        /// <summary>
        /// Отримати любого юніта, котрий може бути віпом
        /// </summary>
        public IUnit GetAnyUnitWhoWillBeAbleToVip(string gameId, int actorNr)
        {
            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorNr == actorNr && HasComponent<IVipComponent>(x));
        }
        
        /// <summary>
        /// Перебрать всех юнитов, и вернуть истину, если есть мертвые юниты
        /// </summary>
        public bool HasAnyDeadUnit(string gameId, int actorNr)
        {
            return _model.Items.Any(x => x.GameId == gameId && x.OwnerActorNr == actorNr && x.IsDead && !HasComponent<IBarrier>(x));
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
            int curr = ((IHealthComponent)unit).HealthCapacity - damage;
            if (curr < 0) curr = 0;
            
            ((IHealthComponent)unit).HealthCapacity = curr;
        }

        /// <summary>
        /// Нанести урон по броні
        /// </summary>
        private void SetDamageByArmor(IUnit unit, int damage)
        {
            int curr = ((IArmorComponent)unit).ArmorCapacity - damage;
            if (curr < 0) curr = 0;

            ((IArmorComponent)unit).ArmorCapacity = curr;
        }

        public void RemoveAllIfExist(string gameId)
        {
            List<IUnit> units = _model.Items.FindAll(x => x.GameId == gameId);
            foreach (IUnit unit in units)
            {
                _model.Items.Remove(unit);
            }
        }

        public void RemoveAllIfExist(string gameId, int actorNr)
        {
            List<IUnit> units = _model.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorNr);
            foreach (IUnit unit in units)
            {
                _model.Items.Remove(unit);
            }
        }

        /// <summary>
        /// Зробити юніта VIP-ом
        /// </summary>
        public void MakeVip(IUnit unit)
        {
            (unit as IVipComponent).Enable = true;
        }

        /// <summary>
        /// Юнит со статусом Vip живой?
        /// </summary>
        public bool VipIsAlive(string gameId, int actorNr)
        {
            List<IUnit> units = GetUnits(gameId, actorNr);

            foreach (IUnit unit in units)
            {
                if (unit is IVipComponent && !unit.IsDead && (unit as IVipComponent).Enable)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Получить количество живых юнитов
        /// </summary>
        public void GetAliveUnits(string gameId, int actorNr, ref List<IUnit> units)
        {
            foreach (IUnit unit in GetUnits(gameId, actorNr))
            {
                if (!unit.IsDead && !HasComponent<IBarrier>(unit))
                {
                    units.Add(unit);
                }                
            }
        }

        /// <summary>
        /// Со всех юнитов удалить хилки,
        /// что бы юниты не могли применить хилку
        /// </summary>
        public void RemoveAllMedicHealing(string gameId, int actorNr)
        {
            // перебираем всех юнитов игрока
            foreach (IUnit unit in GetUnits(gameId, actorNr))
            {
                if (unit is IHealthComponent)
                {
                    ((IHealthComponent)unit).HealthCapacity = 0;
                }
            }
        }

        /// <summary>
        /// Получить юнита, который VIP
        /// </summary>
        public IUnit GetVipUnit(string gameId, int actorNr)
        {
            List<IUnit> units = GetUnits(gameId, actorNr);

            foreach (IUnit unit in units)
            {
                if (unit is IVipComponent && ((IVipComponent)unit).Enable)
                {
                    return unit;
                }
            }

            return null;
        }
    }

}
