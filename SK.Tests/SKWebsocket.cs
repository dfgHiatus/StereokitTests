using Newtonsoft.Json;
using System.Threading;

namespace Tests
{
    public class SKWebsocket : SKInterface
    {
        private NetworkClass networkClass;
        private Thread _worker;
        private CancellationTokenSource _worker_ct = new CancellationTokenSource();

        public override void OnInit()
        {
            networkClass = new NetworkClass();
            
            _worker = new Thread(() =>
            {
                while (!_worker_ct.IsCancellationRequested)
                {
                    networkClass.buffer = JsonConvert.SerializeObject(eyeDataLite);
                    networkClass.broadcastData();
                    Thread.Sleep(150);
                }
            });
            
            // This is called last as it starts the core loop
            base.OnInit(); 
            _worker.Start();
            base.OnRun(collectLite: true);
        }

        public override void Teardown()
        {
            _worker_ct.Cancel();
            _worker_ct.Dispose();
            _worker.Join();
            base.Teardown();
        }
    }
}
