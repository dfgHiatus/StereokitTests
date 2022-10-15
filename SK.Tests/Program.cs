using StereoKit;
using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            SKCompanion companion = new SKCompanion();
            companion.Initialize();
            
            EyeData eyeData = new EyeData();

            using (var memMapFile = MemoryMappedFile.CreateNew("StereokitEyeTracking", Marshal.SizeOf(companion.GetGazeData())))
            {
                using (var accessor = memMapFile.CreateViewAccessor())
                {
                    while (companion.Update())
                    {
                        eyeData = companion.GetGazeData();
                        accessor.Write(0, ref eyeData);
                    }
                }
            }

            companion.Teardown();
        }
    }
}
