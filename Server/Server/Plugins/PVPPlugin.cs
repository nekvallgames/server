﻿using Google.Cloud.Firestore;
using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Runtime.Services.PlotStates;
using Plugin.Runtime.Services.PlotStates.States;
using Plugin.Schemes;
using System;
using System.Diagnostics;
using System.IO;

namespace Plugin.Plugins.PVP
{
    /// <summary>
    /// Плагин для режиму PVP
    /// </summary>
    public class PVPPlugin : PluginHook
    {
        public const string NAME = "PVPPlugin";

        /// <summary>
        /// Имя плагина. Что бы текущий плагин выполнял логику созданой комнаты, 
        /// нужно имя плагина передать в параметрах, при создании комнаты
        /// </summary>
        public override string Name => NAME; // anything other than "Default" or "ErrorPlugin"

        /// <summary>
        /// Реалізувати логіку плагіна
        /// </summary>
        protected override void PluginImplementation()
        {
            // Для поточної ігрової кімнати створити стейт машину із стейтами,
            // котрі потрібні для ігрового режиму 
            plotService = new PlotStatesService();
            plotService.Add(new IState[] { new AccumulateState(plotService, host, 2, SyncStartState.NAME),
                                           new SyncStartState(plotService, host, 2, WaitStepResult.NAME),
                                           new WaitStepResult(plotService, host, 2, SyncState.NAME),
                                           new SyncState(plotService, host, WaitStepResult.NAME)
                                          });

            // запустити ігровий сценарій
            plotService.ChangeState(AccumulateState.NAME);

            int a = 2;

            TestConnect();
        }

        private string fireconfig = @"
        {
          ""type"": ""service_account"",
          ""project_id"": ""shootermobile2d-151b3"",
          ""private_key_id"": ""760bf112047e23d222013cd22d5505874d96450f"",
          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDGTyluZPggvOst\nAQ4c5CrAHYqAxx0NBSsvCeNuf4IRbVuFCGTQPF1QUPd211KIv62QR/4HjkwcYJ48\nDWK61qz1ElOt4tQXeS+aMMlmuufRmA5A/jMq0OadoO4WZFWnhjgi+3Lp6C5YJOYt\nvS1ntwQbL7CXW/UhWU2NVfa2l0pU/Mt1O6n7Gk4nzy3SUmfy7TyvyQncLLtO3mHb\nxGOdl+YMIpJR3TbzqHPklBkt8T02w2rGaU3WiB1Q3mSOe7qXkxkbqz70XcEZ/ky8\n4owHyXAYle5U1YLwG/5yFYZwMTd9IpOiUJ/a4FZ0kB62ChVPSOeL0YHl8Wd62cRM\nZ1pVc5YPAgMBAAECggEAMl6NAXM36ujROx8XfJMfoKhgM5FfeZ0keNAkKj7MxKxo\nIIQfRxzt/qZyFIdgeUdmSE1g4n5sKpEH3+zudNJuH9lF09bXVBb91kYqAaPbmcrr\nY+qtCGUDDmp/zsQxVVmsDIadi3fjvZOxu1BfpgZATr9wNSGwQ4YxQoLhJUOMJ8WT\nIOUOO41KNkDKV17Nf/EftskZx2QW6iBujmsvXSNlUDFOodKCPjTwcH/D3H5prr4Z\nAimER3Lpeb2yMoycA1ueaRjXlghg/rueQmVa13JVmQFjQ45rFWMHJ94KIK7gyCLj\nA8hBHgwKVlp9NAB9WN+IduvX7Ial3cia5jQekwUAoQKBgQDv4ByQGMlE+2RCy5NK\n4G3IULJ2gy1Cw23d7VHhTAApx09I40qrTKTY4fOP9U4GkUNIxkEeCqQ3DeqB/HVV\nVrS/cn8dcbhstyloyUj1p8gECme5RdhpAZz2/s6YHFTzD6utKjJrPEj07WWIn3Jy\njPTtzQfhMNz0CHIelNBv7cpR9wKBgQDTo8JwtHaoG5u423ulx954xXBRs8RpGcxZ\nQb6L6Qi74BU4OINJ2YgSJSpTsrZfgZvxJxa1lFp9FuOVjFpjKY8+bco3nKrxTAfH\np2MgX91LxP3h0AA6kaqa5Bw9HmsGKFNDv5SUbS0LTo8Et70rtEINwwcJ7N3Jp8NT\nrDleQNnWqQKBgQCdPmta5nSh3K3Hv42Jiv9MTeH/IFR+LIAL9/Mx3lFJEddeb2jW\nTALQDaZNG0APvA/AV/39xHoLgPrWRPlhfNo1mcfMNGzyD0t83l4OcvMB+xQtdqS7\niQwtObGSMxhY0U/3zu6IL1ef/zMM0YEDqHUyobHlo9NjwVmVxHWyuc1j1wKBgGFw\n/m8hx5fyzim4dB+J/HYigWo8mGvDgwB9cpu8OVc2/s4ZxujTjQC9t9U9bRZf+Ep5\nssz7dwYs6a/LJTqOwfL+XaJpMZNTbCmSeeNH//2Iv1OejtxogOI7sHy7ao81NdD1\nfhUwhxKzosmLRz++CMOJTc5WDm9wHFCD9Q3eUe2xAoGAPJ/HAzNb5qlYmtJaW/wA\ncYMNeS7w1Un/y1jSmZCP5IWRQIKdIxb5sl55hiq3HVTUALzO5rFiMQ3ibNy8QnU6\nU22vckxrMAH2UJO/EXE8ZetBq6R3vQAWweFPifH2Lmv0hTiEx9I3Jpezb7ROShlR\nsPUHgY+eO3h32iGaN7/UWXA=\n-----END PRIVATE KEY-----\n"",
          ""client_email"": ""firebase-adminsdk-y5vfv@shootermobile2d-151b3.iam.gserviceaccount.com"",
          ""client_id"": ""115486415534508354715"",
          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
          ""token_uri"": ""https://oauth2.googleapis.com/token"",
          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-y5vfv%40shootermobile2d-151b3.iam.gserviceaccount.com"",
          ""universe_domain"": ""googleapis.com""
        }";

        string filepath = "";
        FirestoreDb database;

        private void TestConnect()
        {
            filepath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filepath, fireconfig);
            File.SetAttributes(filepath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            database = FirestoreDb.Create("shootermobile2d-151b3");

            int a = 2;

            File.Delete(filepath);

            /*string pathToJson = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\Plugin\\bin";


            // string path = AppDomain.CurrentDomain.BaseDirectory + @"shootermobile2d.json";
            string path = pathToJson + @"shootermobile2d.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            int aa = 2;

            try
            {
                FirestoreDb db = FirestoreDb.Create("shootermobile2d-151b3");
                int bb = 2;
            }
            catch (Exception e)
            {
                int a = 2;
            }*/
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Создать комнату" и при успешном создании комнаты на 
        /// стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            CreatePlotModelForActor(host.GameId, !info.IsJoin ? 1 : info.Request.ActorNr);
      
            base.OnCreateGame(info);
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Присоединится к текущей комнате" и при успешном 
        /// присоеденении к комнате на стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnJoin(IJoinGameCallInfo info)
        {
            CreatePlotModelForActor(host.GameId, info.ActorNr);

            base.OnJoin(info);
        }

        /// <summary>
        /// Створити модель даних для зберігання даних ігрового режиму,
        /// для нового гравця поточної ігрової кімнати
        /// </summary>
        private void CreatePlotModelForActor(string gameId, int actorId)
        {
            GameInstaller.GetInstance().plotsModelService.Add( new PVPPlotModelScheme(gameId, actorId) );
        }

        /// <summary>
        /// Виконається при закритті кімнати
        /// </summary>
        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            base.BeforeCloseGame(info);
        }
    }
}
