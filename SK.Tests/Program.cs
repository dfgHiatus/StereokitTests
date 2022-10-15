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

            using (var memMapFile = MemoryMappedFile.CreateNew("StereokitEyeTracking", Marshal.SizeOf(companion.GetEyeData())))
            {
                using (var accessor = memMapFile.CreateViewAccessor())
                {
                    while (companion.Update())
                    {
                        eyeData = companion.GetEyeData();
                        accessor.Write(0, ref eyeData);
                    }
                }
            }

            companion.Teardown();
        }
    }
}
