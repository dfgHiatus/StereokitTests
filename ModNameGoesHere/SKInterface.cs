using System.Runtime.InteropServices;

namespace Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeData
    {
        public Eye leftEye;                 //!< Left eye data
        public Eye rightEye;                //!< Right eye data
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Eye
    {
        public Pose pose;                        //!< Pose of the eye
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
        protected EyeData gazeData;

        public EyeData GetGazeData()
        {
            return gazeData;
        }

        public abstract bool Initialize();
        public abstract void Update();
        public abstract void Teardown();
    }

}
