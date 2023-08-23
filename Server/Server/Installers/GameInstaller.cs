using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Models.Public;
using Plugin.Runtime.Providers;
using Plugin.Runtime.Services;
using Plugin.Runtime.Services.ExecuteAction;
using Plugin.Runtime.Services.ExecuteAction.Action;
using Plugin.Runtime.Services.ExecuteAction.Additional;
using Plugin.Runtime.Services.ExecuteOp;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Spawners;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Installers
{
    public class GameInstaller
    {
        private static GameInstaller _instance;
        public static GameInstaller GetInstance(){
            if (_instance == null){
                _instance = new GameInstaller();
            }
            return _instance;
        }

        public HostsService hostsService;
        public SignalBus signalBus;
        public UnitsService unitsService;
        public SyncService syncService;
        public MoveService moveService;
        public VipService vipService;
        public ActionService actionService;
        public SortHitOnGridService sortTargetOnGridService;
        public AdditionalService additionalService;
        public OpStockService opStockService;
        public GridService gridService;
        public PublicModelProvider publicModelProvider;
        public PrivateModelProvider privateModelProvider;
        public GridBuilder gridBuilder;
        public ConvertService convertService;
        public UnitInstanceService unitInstanceService;
        public UnitBuilder unitBuilder;
        public NotificationChangeVipService notificationChangeVipService;
        public StepSchemeBuilder stepSchemeBuilder;
        public LocationUnitsSpawner locationUnitsSpawner;
        public SyncStepService syncStepService;
        public PlotsModelService plotsModelService;
        public ActorService actorService;
        public BackendBroadcastService backendBroadcastService;
        public ExecuteOpStepSchemeService executeOpStepService;
        public ExecuteOpGroupService executeOpGroupService;
        public UnitLevelService unitLevelService;
        public JsonReaderService jsonReaderService;
        public ResultService resultService;
        public IncreaseUnitDamageService increaseUnitDamageService;
        public IncreaseUnitHealthService increaseUnitHealthService;
        public PlotPublicService plotPublicService;
        public UnitDamageMultiplicationService unitDamageMultiplicationService;
        public UnitsPublicModelService unitsPublicModelService;
        public BodyDamageConverterService bodyDamageConverterService;
        public SyncProgressService syncProgressService;
        public GameService gameService;


        public GameInstaller()
        {
            _instance = this;

            signalBus = new SignalBus();
            convertService = new ConvertService();

            jsonReaderService = new JsonReaderService();
            unitLevelService = new UnitLevelService(jsonReaderService);
            increaseUnitDamageService = new IncreaseUnitDamageService(jsonReaderService);
            increaseUnitHealthService = new IncreaseUnitHealthService(jsonReaderService);
            plotPublicService = new PlotPublicService(jsonReaderService);
            unitDamageMultiplicationService = new UnitDamageMultiplicationService(jsonReaderService);
            unitsPublicModelService = new UnitsPublicModelService(jsonReaderService);

            bodyDamageConverterService = new BodyDamageConverterService(unitDamageMultiplicationService);
            backendBroadcastService = new BackendBroadcastService(unitLevelService);

            publicModelProvider = new PublicModelProvider(new List<IPublicModel>
            {
                new LocationsPublicModel<LocationScheme>(convertService)
            });

            privateModelProvider = new PrivateModelProvider(new List<IPrivateModel>
            {
                new UnitsPrivateModel(),
                new OpStockPrivateModel(signalBus),
                new SyncPrivateModel(),
                new GridsPrivateModel(signalBus),
                new HostsPrivateModel(signalBus),
                new PlotsPrivateModel(),
                new ActorsPrivateModel()
            });

            actorService = new ActorService(privateModelProvider.Get<ActorsPrivateModel>(), signalBus);
            hostsService = new HostsService(privateModelProvider.Get<HostsPrivateModel>());
            plotsModelService = new PlotsModelService(privateModelProvider.Get<PlotsPrivateModel>());
            gridBuilder = new GridBuilder();
            unitInstanceService = new UnitInstanceService(privateModelProvider.Get<UnitsPrivateModel>());
            unitBuilder = new UnitBuilder(unitInstanceService, unitsPublicModelService, increaseUnitDamageService, increaseUnitHealthService);
            opStockService = new OpStockService(privateModelProvider.Get<OpStockPrivateModel>());
            syncService = new SyncService(privateModelProvider.Get<SyncPrivateModel>(), plotsModelService);
            stepSchemeBuilder = new StepSchemeBuilder(syncService);
            syncStepService = new SyncStepService(stepSchemeBuilder, convertService, hostsService, plotsModelService, actorService);
            moveService = new MoveService(syncService);
            unitsService = new UnitsService(privateModelProvider.Get<UnitsPrivateModel>(), unitBuilder, signalBus, moveService, plotPublicService);
            locationUnitsSpawner = new LocationUnitsSpawner(publicModelProvider, unitsService, signalBus);
            vipService = new VipService(syncService, unitsService);
            sortTargetOnGridService = new SortHitOnGridService();
            actionService = new ActionService(syncService, unitsService, sortTargetOnGridService, bodyDamageConverterService);
            additionalService = new AdditionalService(syncService, unitsService);
            gridService = new GridService(publicModelProvider, privateModelProvider, gridBuilder);
            notificationChangeVipService = new NotificationChangeVipService(hostsService, opStockService, signalBus);
            executeOpGroupService = new ExecuteOpGroupService(unitsService, moveService, vipService, actionService, additionalService);
            executeOpStepService = new ExecuteOpStepSchemeService(executeOpGroupService);
            resultService = new ResultService(backendBroadcastService);
            syncProgressService = new SyncProgressService(backendBroadcastService, plotsModelService, plotPublicService);
            gameService = new GameService(signalBus, plotsModelService, syncProgressService, actorService, hostsService, convertService);
        }
    }
}
