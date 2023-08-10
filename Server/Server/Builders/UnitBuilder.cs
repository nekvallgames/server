using Plugin.Interfaces;
using Plugin.Parameters;
using Plugin.Runtime.Services;
using Plugin.Runtime.Units;
using System;
using System.Diagnostics;

namespace Plugin.Builders
{
    public class UnitBuilder
    {
        private UnitInstanceService _unitInstanceService;
        private UnitsPublicModelService _unitsPublicModelService;
        private IncreaseUnitDamageService _increaseUnitDamageService;
        private IncreaseUnitHealthService _increaseUnitHealthService;

        public UnitBuilder(UnitInstanceService unitInstanceService, 
                           UnitsPublicModelService unitsPublicModelService,
                           IncreaseUnitDamageService increaseUnitDamageService,
                           IncreaseUnitHealthService increaseUnitHealthService)
        {
            _unitInstanceService = unitInstanceService;
            _unitsPublicModelService = unitsPublicModelService;
            _increaseUnitDamageService = increaseUnitDamageService;
            _increaseUnitHealthService = increaseUnitHealthService;
        }

        /// <summary>
        /// Создать юнит, указав его тип
        /// </summary>
        public IUnit CreateUnit(string gameId, int ownerActorNr, int unitId, int level)
        {
            var parameters = new UnitFactoryParameters(gameId,
                                                       ownerActorNr,
                                                       unitId,
                                                       _unitInstanceService.GetInstance(gameId, ownerActorNr, unitId),
                                                       level,
                                                       _unitsPublicModelService,
                                                       _increaseUnitHealthService,
                                                       _increaseUnitDamageService);

            switch (unitId)
            {
                case UnitPistol.UnitId: return Create<UnitPistol>(parameters);
                case UnitShotGun.UnitId: return Create<UnitShotGun>(parameters);
                case UnitUmp45.UnitId: return Create<UnitUmp45>(parameters);
                case UnitSniper.UnitId: return Create<UnitSniper>(parameters);
                case UnitPoison.UnitId: return Create<UnitPoison>(parameters);
                case UnitTrash.UnitId: return Create<UnitTrash>(parameters);
                case UnitRoadBlock.UnitId: return Create<UnitRoadBlock>(parameters);
                case UnitBarrel.UnitId: return Create<UnitBarrel>(parameters);
                case UnitLuke.UnitId: return Create<UnitLuke>(parameters);
                case UnitBagBarrier.UnitId: return Create<UnitBagBarrier>(parameters);
                case UnitIronFenceBarrier.UnitId: return Create<UnitIronFenceBarrier>(parameters);

                default:{
                        Debug.Fail($"UnitBuilder :: CreateUnit() I can't create unitId = {unitId}, for actorNr = {ownerActorNr}.");
                        return null;
                    }
                    break;
            }
        }

        private IUnit Create<T>(UnitFactoryParameters parameters) where T : IUnit
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }
    }
}
