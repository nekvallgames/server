﻿using Google.Cloud.Firestore;
using Plugin.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Plugin.Runtime.Providers
{
    /// <summary>
    /// Провайдер, за допомогою котрого сервер підключиться до DB
    /// </summary>
    public class BackendBroadcastProvider : IBackendBroadcastProvider
    {
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
        public FirestoreDb Database { get; private set; }
        
        public void Connect()
        {
            filepath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filepath, fireconfig);
            File.SetAttributes(filepath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            Database = FirestoreDb.Create("shootermobile2d-151b3");

            File.Delete(filepath);
        }

        /// <summary>
        /// Отримати дані вказаного актора із database
        /// item1 - рейтинг гравця
        /// item2 - id юнітів, котрі в гравця знаходяться в колоді
        /// </summary>
        public async Task<(int, List<int>)> GetActorData(string profileId)
        {
            int rating = 0;
            var deck = new List<int>();

            DocumentReference docref = Database.Collection("users").Document(profileId);
            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            if (snap.Exists)
            {
                Dictionary<string, object> user = snap.ToDictionary();
                if (user.ContainsKey("decks"))
                {
                    IEnumerable enumerable = user["decks"] as IEnumerable;
                    var enumerator = enumerable.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var item = enumerator.Current;
                        int unitId = int.Parse(item.ToString());
                        deck.Add(unitId);
                    }
                }

                if (user.ContainsKey("rating"))
                {
                    rating = int.Parse(user["rating"].ToString());
                }
            }

            return (rating, deck);
        }

        /// <summary>
        /// Отримати ownCapacity юнітів вказаного гравця
        /// </summary>
        public async Task<List<(int, int)>> GetOwnCapacityUnits(string profileId)
        {
            var capacityUnits = new List<(int, int)>();

            Query qRef = Database.Collection("users").Document(profileId).Collection("units");
            QuerySnapshot snap = await qRef.GetSnapshotAsync();

            foreach (DocumentSnapshot docsnap in snap){
                capacityUnits.Add((int.Parse(docsnap.Id), docsnap.GetValue<int>("ownCapacity")));
            }

            return capacityUnits;
        }

        /// <summary>
        /// Перезаписати рейтинг вказаного гравця
        /// </summary>
        public async Task SetRating(string profileId, int capacity)
        {
            DocumentReference docref = Database.Collection("users").Document(profileId);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"rating",capacity}
            };

            DocumentSnapshot snap = await docref.GetSnapshotAsync();
            if (snap.Exists)
            {
                await docref.UpdateAsync(data);
            }
        }
    }
}