using System;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий імітує факт, що гравець противник змінив свого VIP
    /// Це потрібно для AI та режиму PVE. 
    /// </summary>
    public class SimulateNotificationChangeVipService
    {
        private SignalBus _signalBus;

        private const int MIN_WAIT_BEFORE_SHOW = 5000;    // 5 sec
        private const int MAX_WAIT_BEFORE_SHOW = 10000;   // 10 sec

        public SimulateNotificationChangeVipService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public async void Execute()
        {
            Random random = new Random();

            await Task.Delay(random.Next(MIN_WAIT_BEFORE_SHOW, MAX_WAIT_BEFORE_SHOW));

            // ToastSignal toastSignal = new ToastSignal();
            // toastSignal.DescriptionTxt = "Leader is changed";
            // toastSignal.ImagePath = "UI/vip_icon";
            // _signalBus.Fire(toastSignal);
        }
    }
}
