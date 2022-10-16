using Newtonsoft.Json;
using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace Tests
{
    public class SKCompanion : SKInterface
    {
        public override void OnInit()
        {
            base.OnInit();

            using (var memMapFile = MemoryMappedFile.CreateNew(name, Marshal.SizeOf(eyeDataLite)))
            {
                using (var accessor = memMapFile.CreateViewAccessor())
                {
                    Console.WriteLine("Eye tracking session has started!");
                    while (base.OnStep(collectLite: true))
                    {
                        accessor.Write(0, ref eyeDataLite);
                        Thread.Sleep(200);
                    }
                }
            }

            base.Teardown();
        }
    }
}
