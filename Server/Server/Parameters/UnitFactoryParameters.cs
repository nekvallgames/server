using Plugin.Runtime.Services;

namespace Plugin.Parameters
{
    public struct UnitFactoryParameters
    {
        public string GameId { get; }
        public int OwnerActorNr { get; }
        public int UnitId { get; }
        public int InstanceId { get; }
        public int Level { get; }
        public UnitsPublicModelService UnitsPublicModelService { get; }
        public IncreaseUnitDamageService IncreaseUnitDamageService { get; }
        public IncreaseUnitHealthService IncreaseUnitHealthService { get; }

        public UnitFactoryParameters(string gameId,
                                     int ownerActorNr,
                                     int unitId,
                                     int instanceId,
                                     int level,
                                     UnitsPublicModelService unitsPublicModelService,
                                     IncreaseUnitHealthService increaseUnitHealthService,
                                     IncreaseUnitDamageService increaseUnitDamageService)
        {
            GameId = gameId;
            OwnerActorNr = ownerActorNr;
            UnitId = unitId;
            InstanceId = instanceId;
            Level = level;
            UnitsPublicModelService = unitsPublicModelService;
            IncreaseUnitHealthService = increaseUnitHealthService;
            IncreaseUnitDamageService = increaseUnitDamageService;
        }
    }
}