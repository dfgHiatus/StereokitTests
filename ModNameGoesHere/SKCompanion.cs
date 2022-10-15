using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class SKCompanion : SKInterface
    {
        private MemoryMappedFile MemMapFile;
        private MemoryMappedViewAccessor ViewAccessor;
        private Process CompanionProcess;
        
        public override bool Initialize()
        {
            if (!VarjoAvailable())
            {
                Logger.Error("Varjo headset isn't detected");
                return false;
            }
            string modDir = GetModuleDir();
            string exePath = Path.Combine(modDir, "VarjoCompanion.exe");
            if (!File.Exists(exePath))
            {
                Logger.Error("SterokitCompanion executable wasn't found!");
                return false;
            }
            CompanionProcess = new Process();
            CompanionProcess.StartInfo.WorkingDirectory = modDir;
            CompanionProcess.StartInfo.FileName = exePath;
            CompanionProcess.Start();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    MemMapFile = MemoryMappedFile.OpenExisting("StereokitEyeTracking");
                    ViewAccessor = MemMapFile.CreateViewAccessor();
                    return true;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("StereokitEyeTracking mapped file doesn't exist; the companion app probably isn't running");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not open the mapped file: " + ex);
                    return false;
                }
                Thread.Sleep(500);
            }

            return false;
        }

        public override void Teardown()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
