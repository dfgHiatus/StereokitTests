using BaseX;
using FrooxEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

// StereokitEye

namespace SKEyeTracking
{
    public class SKCompanionDevice : SKInterface
    {
        private MemoryMappedFile MemMapFile;
        private MemoryMappedViewAccessor ViewAccessor;
        private Process CompanionProcess;

        public override void OnInit()
        {
            var modDir = Path.Combine(Engine.Current.AppPath, "nml_mods");

            CompanionProcess = new Process();
            CompanionProcess.StartInfo.WorkingDirectory = modDir;
            CompanionProcess.StartInfo.FileName = Path.Combine(modDir, "SK.Tests.exe");
            CompanionProcess.Start();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    MemMapFile = MemoryMappedFile.OpenExisting("StereokitEyeTracking");
                    ViewAccessor = MemMapFile.CreateViewAccessor();
                    UniLog.Log("Connected to the Varjo Companion App!");
                    return;
                }
                catch (FileNotFoundException)
                {
                    UniLog.Log($"Trying to connect to the Varjo Companion App. Attempt {i}/5");
                }
                catch (Exception ex)
                {
                    UniLog.Log("Could not open the mapped file: " + ex);
                    return;
                }
                Thread.Sleep(500);
            }
        }

        public override void Update()
        {
            if (MemMapFile == null) return;
            ViewAccessor.Read(0, out eyeData);
        }

        public override void UpdateLite()
        {
            if (MemMapFile == null) return;
            ViewAccessor.Read(0, out eyeDataLite);
        }

        public override void Teardown()
        {
            if (MemMapFile == null) return;
            ViewAccessor.Write(0, ref eyeDataLite);
            MemMapFile.Dispose();
            CompanionProcess.Close();
        }
    }
}
