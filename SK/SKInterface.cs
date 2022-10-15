using System.Runtime.InteropServices;
using System.Text;

namespace SKEyeTracking.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeData
    {
        public Eye leftEye;                 //!< Left eye data
        public Eye rightEye;                //!< Right eye data
        public Eye combinedEye;             //!< Combined eye data
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Eye
    {
        public Pose pose;                        //!< Pose of the eye
        public bool eyeTrackingPresent;          //<! If the hardware is capable of providing eye tracking.
        public bool eyesTracked;                 //<! If eye hardware is available and app has permission, then this is the tracking state of the eyes. Eyes may move out of bounds, hardware may fail to detect eyes, or who knows what else!
        public long eyesSampleTime;              //<! This is the OpenXR time of the eye tracker sample associated with the current value of .
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Pose
    {
        public Vec3 forward;                      //!< Calculates the forward direction from this pose. This is done by multiplying the orientation with Vec3.Forward. Remember that Forward points down the -Z axis!
        public Quat orientation;                  //!< Orientation of the pose, stored as a rotation from Vec3.Forward.
        public Vec3 position;                     //!< Location of the pose.
        public Ray ray;                           //!< This creates a ray starting at the Pose’s position, and pointing in the ‘Forward’ direction. The Ray direction is a unit vector/normalized.
        public Vec3 right;                        //!< Calculates the right (+X) direction from this pose. This is done by multiplying the orientation with Vec3.Right.
        public Vec3 up;                           //!< Calculates the up (+Y) direction from this pose. This is done by multiplying the orientation with Vec3.Up.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Ray
    {
        public Vec3 direction;              //!< The direction the ray is facing, typically does not require being a unit vector, or normalized direction.
        public Vec3 position;               //!< The position or origin point of the Ray.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quat
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }


    public abstract class SKInterface
    {
        protected EyeData eyeData;
        protected const string memoryMappedFileName = "StereokitEyeTracking";
        private readonly StringBuilder sb = new StringBuilder();

        public string GetDebugData()
        {
            sb.Clear();

            sb.AppendLine($"Left eye data:");
            sb.AppendLine($"Pose:");
            sb.AppendLine($"Forward: {PrintStruct(eyeData.leftEye.pose.forward)}");
            sb.AppendLine($"Orientation: {PrintStruct(eyeData.leftEye.pose.orientation)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.leftEye.pose.position)}");
            sb.AppendLine($"Ray:");
            sb.AppendLine($"Direction: {PrintStruct(eyeData.leftEye.pose.ray.direction)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.leftEye.pose.ray.position)}");
            sb.AppendLine($"Right: {PrintStruct(eyeData.leftEye.pose.right)}");
            sb.AppendLine($"Up: {PrintStruct(eyeData.leftEye.pose.up)}");
            sb.AppendLine($"Eye tracking present: {eyeData.leftEye.eyeTrackingPresent}");
            sb.AppendLine($"Eyes tracked: {eyeData.leftEye.eyesTracked}");
            sb.AppendLine($"Eyes sample time: {eyeData.leftEye.eyesSampleTime}");
            sb.AppendLine($"");
            sb.AppendLine($"Right eye data:");
            sb.AppendLine($"Pose:");
            sb.AppendLine($"Forward: {PrintStruct(eyeData.rightEye.pose.forward)}");
            sb.AppendLine($"Orientation: {PrintStruct(eyeData.rightEye.pose.orientation)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.rightEye.pose.position)}");
            sb.AppendLine($"Ray:");
            sb.AppendLine($"Direction: {PrintStruct(eyeData.rightEye.pose.ray.direction)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.rightEye.pose.ray.position)}");
            sb.AppendLine($"Right: {PrintStruct(eyeData.rightEye.pose.right)}");
            sb.AppendLine($"Up: {PrintStruct(eyeData.rightEye.pose.up)}");
            sb.AppendLine($"Eye tracking present: {eyeData.rightEye.eyeTrackingPresent}");
            sb.AppendLine($"Eyes tracked: {eyeData.rightEye.eyesTracked}");
            sb.AppendLine($"Eyes sample time: {eyeData.rightEye.eyesSampleTime}");
            sb.AppendLine($"");
            sb.AppendLine($"Combined eye data:");
            sb.AppendLine($"Pose:");
            sb.AppendLine($"Forward: {PrintStruct(eyeData.combinedEye.pose.forward)}");
            sb.AppendLine($"Orientation: {PrintStruct(eyeData.combinedEye.pose.orientation)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.combinedEye.pose.position)}");
            sb.AppendLine($"Ray:");
            sb.AppendLine($"Direction: {PrintStruct(eyeData.combinedEye.pose.ray.direction)}");
            sb.AppendLine($"Position: {PrintStruct(eyeData.combinedEye.pose.ray.position)}");
            sb.AppendLine($"Right: {PrintStruct(eyeData.combinedEye.pose.right)}");
            sb.AppendLine($"Up: {PrintStruct(eyeData.combinedEye.pose.up)}");
            sb.AppendLine($"Eye tracking present: {eyeData.combinedEye.eyeTrackingPresent}");
            sb.AppendLine($"Eyes tracked: {eyeData.combinedEye.eyesTracked}");
            sb.AppendLine($"Eyes sample time: {eyeData.combinedEye.eyesSampleTime}");
            sb.AppendLine($"");

            return sb.ToString();
        }

        public abstract bool Initialize();
        public abstract bool Update();
        public abstract void Teardown();

        private string PrintStruct(Vec3 vec3)
        {
            return $"X: {vec3.x}, Y: {vec3.y}, Z: {vec3.z}";
        }

        private string PrintStruct(Quat quat)
        {
            return $"X: {quat.x}, Y: {quat.y}, Z: {quat.z}, W: {quat.w}";
        }
    }
}
