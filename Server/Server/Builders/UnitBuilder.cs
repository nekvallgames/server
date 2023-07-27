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
        public IUnit CreateUnit(string gameId, int ownerActorNr, int unitId)
        {
            switch (unitId)
            {
                case UnitPistol.UnitId: return Create<UnitPistol>(gameId, ownerActorNr, unitId);
                case UnitShotGun.UnitId: return Create<UnitShotGun>(gameId, ownerActorNr, unitId);
                case UnitUmp45.UnitId: return Create<UnitUmp45>(gameId, ownerActorNr, unitId);
                case UnitSniper.UnitId: return Create<UnitSniper>(gameId, ownerActorNr, unitId);
                case UnitPoison.UnitId: return Create<UnitPoison>(gameId, ownerActorNr, unitId);
                case UnitTrash.UnitId: return Create<UnitTrash>(gameId, ownerActorNr, unitId);
                case UnitRoadBlock.UnitId: return Create<UnitRoadBlock>(gameId, ownerActorNr, unitId);
                case UnitBarrel.UnitId: return Create<UnitBarrel>(gameId, ownerActorNr, unitId);
                case UnitLuke.UnitId: return Create<UnitLuke>(gameId, ownerActorNr, unitId);
                case UnitBagBarrier.UnitId: return Create<UnitBagBarrier>(gameId, ownerActorNr, unitId);
                case UnitIronFenceBarrier.UnitId: return Create<UnitIronFenceBarrier>(gameId, ownerActorNr, unitId);

                default:{
                        Debug.Fail($"UnitBuilder :: CreateUnit() I can't create unitId = {unitId}, for actorNr = {ownerActorNr}.");
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
