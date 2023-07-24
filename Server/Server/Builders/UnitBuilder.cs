using Plugin.Interfaces;
using Plugin.Runtime.Services;
using Plugin.Runtime.Units;
using System;
using System.Diagnostics;

namespace Plugin.Builders
{
    public class UnitBuilder
    {
        private UnitInstanceService _unitInstanceService;

        public UnitBuilder(UnitInstanceService unitInstanceService)
        {
            _unitInstanceService = unitInstanceService;
        }

        /// <summary>
        /// Создать юнит, указав его тип
        /// gameId - вказати ігрову кімнату, котрій належить поточний юніт
        /// ownerActorId - владелец юнита
        /// unitId       - уникальный ID юнита
        /// </summary>
        public IUnit CreateUnit(string gameId, int ownerActorId, int unitId)
        {
            switch (unitId)
            {
                case UnitPistol.UnitId: return Create<UnitPistol>(gameId, ownerActorId, unitId);
                case UnitShotGun.UnitId: return Create<UnitShotGun>(gameId, ownerActorId, unitId);
                case UnitUmp45.UnitId: return Create<UnitUmp45>(gameId, ownerActorId, unitId);
                case UnitSniper.UnitId: return Create<UnitSniper>(gameId, ownerActorId, unitId);
                case UnitPoison.UnitId: return Create<UnitPoison>(gameId, ownerActorId, unitId);
                case UnitTrash.UnitId: return Create<UnitTrash>(gameId, ownerActorId, unitId);
                case UnitRoadBlock.UnitId: return Create<UnitRoadBlock>(gameId, ownerActorId, unitId);
                case UnitBarrel.UnitId: return Create<UnitBarrel>(gameId, ownerActorId, unitId);
                case UnitLuke.UnitId: return Create<UnitLuke>(gameId, ownerActorId, unitId);
                case UnitBagBarrier.UnitId: return Create<UnitBagBarrier>(gameId, ownerActorId, unitId);
                case UnitIronFenceBarrier.UnitId: return Create<UnitIronFenceBarrier>(gameId, ownerActorId, unitId);

                default:{
                        Debug.Fail($"UnitBuilder :: CreateUnit() I can't create unitId = {unitId}, for actorId = {ownerActorId}.");
                        return null;
                    }
                    break;
            }
        }

        private IUnit Create<T>(string gameId, int actorId, int unitId) where T : IUnit
        {
            int instance = _unitInstanceService.GetInstance(gameId, actorId, unitId);

            return (T)Activator.CreateInstance(typeof(T), gameId, actorId, unitId, instance);
        }
    }
}
