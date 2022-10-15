using BaseX;
using SKEyeTracking.Data;

namespace SKEyeTracking
{
    public static class SKExtensions
    {
        public static float3 Vec3ToFloat3(this Vec3 vec3)
        {
            return new float3(vec3.x, vec3.y, vec3.z);
        }
    }
}
