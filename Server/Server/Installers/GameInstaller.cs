﻿using Photon.Hive.Plugin;
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
using System;
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
        public SortTargetOnGridService sortTargetOnGridService;
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

        public ExecuteOpStepSchemeService executeOpStepService;
        public ExecuteOpGroupService executeOpGroupService;

        public GameInstaller()
        {
            _instance = this;

            signalBus = new SignalBus();
            convertService = new ConvertService();

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
                new PlotsPrivateModel()
            });

            hostsService = new HostsService(privateModelProvider.Get<HostsPrivateModel>());
            plotsModelService = new PlotsModelService(privateModelProvider.Get<PlotsPrivateModel>());
            gridBuilder = new GridBuilder();
            unitInstanceService = new UnitInstanceService(privateModelProvider.Get<UnitsPrivateModel>());
            unitBuilder = new UnitBuilder(unitInstanceService);
            opStockService = new OpStockService(privateModelProvider.Get<OpStockPrivateModel>());
            syncService = new SyncService(privateModelProvider.Get<SyncPrivateModel>(), plotsModelService);
            stepSchemeBuilder = new StepSchemeBuilder(syncService);
            syncStepService = new SyncStepService(stepSchemeBuilder, convertService, hostsService);
            moveService = new MoveService(syncService);
            unitsService = new UnitsService(privateModelProvider.Get<UnitsPrivateModel>(), opStockService, convertService, unitBuilder, signalBus, moveService);
            locationUnitsSpawner = new LocationUnitsSpawner(publicModelProvider, unitsService, signalBus);
            vipService = new VipService(syncService, unitsService);
            sortTargetOnGridService = new SortTargetOnGridService();
            actionService = new ActionService(syncService, unitsService, sortTargetOnGridService);
            additionalService = new AdditionalService(syncService, unitsService);
            gridService = new GridService(publicModelProvider, privateModelProvider, gridBuilder, signalBus, hostsService);
            notificationChangeVipService = new NotificationChangeVipService(hostsService, opStockService, signalBus);
            executeOpGroupService = new ExecuteOpGroupService(unitsService, moveService, vipService, actionService, additionalService);
            executeOpStepService = new ExecuteOpStepSchemeService(executeOpGroupService);
        }
    }
}
