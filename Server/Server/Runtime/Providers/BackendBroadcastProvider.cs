using System;
using Google.Cloud.Firestore;

namespace Plugin.Runtime.Providers
{
    public class BackendBroadcastProvider
    {
        // cloud.google.com/firestore/docs/create-database-server-client-library#c_1

        private FirestoreDb db;

        /*IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "e8QzZOnzGAlaQ8HGs0ihX3BegbiaFWMpRevKq2Mh",
            BasePath = "https://shootermobile2d-151b3-default-rtdb.firebaseio.com"
        };

        IFirebaseClient client;*/

        public BackendBroadcastProvider()
        {
            
        }

        public void Connect()
        {
            int cc = 2;

            // string path = AppDomain.CurrentDomain.BaseDirectory + @"shootermobile2d.json";
            // Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            // 
            
            // 

            try {
                //db = FirestoreDb.Create("shootermobile2d-151b3");

                //client = new FireSharp.FirebaseClient(config);
                // 

                // db = new FirestoreDbBuilder
                // {
                //     ProjectId = "shootermobile2d-151b3",
                //     EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                // }.Build();
            }
            catch (Exception e)
            {
                int a = 2;
            }

            int b = 2;

        }
    }
}