using WebSocketSharp;
using System.Threading;
using System;
using Newtonsoft.Json;

namespace SKEyeTracking
{
    public class SKWebsocketEyeDevice : SKInterface
    {
        public EyeDataLite lastData;
        private Thread _worker = null;
        private CancellationTokenSource _worker_ct = new CancellationTokenSource();
        private string URIString = "ws://127.0.0.1:8080/";
        private WebSocket cws = null;
        private bool didLoad = false;

        public override void OnInit()
        {
            bool isConnected = false;
            VerifyClosedSocket();
            cws = new WebSocket(URIString);
            cws.Connect();
            isConnected = cws.ReadyState == WebSocketState.Open;
            didLoad = isConnected;
            VerifyClosedSocket();
        }

        public override void Teardown()
        {
            _worker_ct.Cancel();
        }

        private void VerifyDeadThread()
        {
            if (_worker != null)
            {
                if (_worker.IsAlive)
                    _worker.Abort();
            }
            _worker_ct = new CancellationTokenSource();
            _worker = null;
        }

        private void VerifyClosedSocket()
        {
            if (cws != null)
            {
                if (cws.ReadyState == WebSocketState.Open)
                    cws.Close();
            }
            cws = null;
        }

        public void StartThread()
        {
            VerifyDeadThread();
            _worker = new Thread(() =>
            {
                // Start the Socket
                VerifyClosedSocket();
                cws = new WebSocket(URIString);
                cws.OnMessage += (sender, args) => lastData = JsonConvert.DeserializeObject<EyeDataLite>(args.Data.ToString()); // Assumes EyeData
                cws.Connect();
                Thread.Sleep(2500);
                if (cws.ReadyState == WebSocketState.Open)
                {
                    // Start the loop
                    bool isLoading = false;
                    while (!_worker_ct.IsCancellationRequested)
                    {
                        if (cws.ReadyState == WebSocketState.Open)
                        {
                            isLoading = false;
                        }
                        else
                        {
                            if (didLoad && !isLoading)
                            {
                                // Socket will randomly force close because it thinks its being DOSsed
                                // We'll just re-open it if it thinks this
                                isLoading = true;
                                cws = new WebSocket(URIString);
                                cws.OnMessage += (sender, args) => lastData = JsonConvert.DeserializeObject<EyeDataLite>(args.Data.ToString()); // Assumes EyeData
                                cws.Connect();
                            }
                        }
                        // Please don't change this to anything lower than 50
                        Thread.Sleep(100);
                    }
                }
                // Close the Socket
                VerifyClosedSocket();
                // The thread will abort on its own
            });
            _worker.Start();
        }
    }
}