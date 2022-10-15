using BaseX;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace SKEyeTracking.Data
{
    public class SKListener : SKInterface
    {
        private const string executableName = "SK.Tests.exe";

        private MemoryMappedFile MemMapFile;
        private MemoryMappedViewAccessor ViewAccessor;
        private Process CompanionProcess;
        
        public override bool Initialize()
        {
            if (!File.Exists(executableName))
            {
                UniLog.Error("SterokitCompanion executable wasn't found!");
                return false;
            }
            
            CompanionProcess = new Process();
            CompanionProcess.StartInfo.FileName = executableName;
            CompanionProcess.Start();

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    MemMapFile = MemoryMappedFile.OpenExisting(memoryMappedFileName);
                    ViewAccessor = MemMapFile.CreateViewAccessor();
                    return true;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"{memoryMappedFileName} mapped file doesn't exist; the companion app probably isn't running");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not open the mapped file {memoryMappedFileName}: " + ex);
                    return false;
                }
                Thread.Sleep(500);
            }

            return false;
        }

        public EyeData GetEyeData()
        {
            return eyeData;
        }

        public override void Teardown()
        {
            if (MemMapFile == null) return;
            ViewAccessor.Write(0, ref eyeData);
            MemMapFile.Dispose();
            CompanionProcess.Kill();
        }
        
        public override bool Update()
        {
            if (MemMapFile == null) return false;
            ViewAccessor.Read(0, out eyeData);
            return true;
        }
    }
}
