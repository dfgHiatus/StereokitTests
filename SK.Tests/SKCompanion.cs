using StereoKit;
using System;

namespace Tests
{
    public class SKCompanion : SKInterface
    {
        public override bool Initialize()
        {
            SK.Initialize(new SKSettings { 
                appName = "StereokitEyeTracking",
                disableUnfocusedSleep = true,
                #if DEBUG
                displayPreference = DisplayMode.Flatscreen
                #else
                displayPreference = DisplayMode.MixedReality
                #endif
            });
            SK.Run(CollectEyeInformation);

            return true;
        }

        public EyeData GetEyeData()
        {
            return eyeData;
        }

        public override bool Update()
        {
            return SK.Step();
        }

        private void CollectEyeInformation()
        {
            #if DEBUG
            Log.Info(GetDebugData());
            #endif
            
            #region Left Eye
                        eyeData.leftEye.pose.forward.x = Input.Eyes.Forward.x;
                        eyeData.leftEye.pose.forward.y = Input.Eyes.Forward.y;
                        eyeData.leftEye.pose.forward.z = Input.Eyes.Forward.z;
            
                        eyeData.leftEye.pose.orientation.x = Input.Eyes.orientation.x;
                        eyeData.leftEye.pose.orientation.y = Input.Eyes.orientation.y;
                        eyeData.leftEye.pose.orientation.z = Input.Eyes.orientation.z;
                        eyeData.leftEye.pose.orientation.w = Input.Eyes.orientation.w;
            
                        eyeData.leftEye.pose.position.x = Input.Eyes.position.x;
                        eyeData.leftEye.pose.position.y = Input.Eyes.position.y;
                        eyeData.leftEye.pose.position.z = Input.Eyes.position.z;

                        eyeData.leftEye.pose.ray.direction.x = Input.Eyes.Ray.direction.x;
                        eyeData.leftEye.pose.ray.direction.y = Input.Eyes.Ray.direction.y;
                        eyeData.leftEye.pose.ray.direction.z = Input.Eyes.Ray.direction.z;

                        eyeData.leftEye.pose.right.x = Input.Eyes.Ray.position.x;
                        eyeData.leftEye.pose.right.y = Input.Eyes.Ray.position.y;
                        eyeData.leftEye.pose.right.z = Input.Eyes.Ray.position.z;

                        eyeData.leftEye.pose.up.x = Input.Eyes.Up.x;
                        eyeData.leftEye.pose.up.y = Input.Eyes.Up.y;
                        eyeData.leftEye.pose.up.z = Input.Eyes.Up.z;

                        eyeData.leftEye.eyesTracked = Input.EyesTracked.IsActive();
                        eyeData.leftEye.eyesSampleTime = Backend.XRType == BackendXRType.OpenXR ?
                            Backend.OpenXR.EyesSampleTime : 0;
            #endregion Left Eye

            #region Right Eye
                        eyeData.rightEye.pose.forward.x = Input.Eyes.Forward.x;
                        eyeData.rightEye.pose.forward.y = Input.Eyes.Forward.y;
                        eyeData.rightEye.pose.forward.z = Input.Eyes.Forward.z;

                        eyeData.rightEye.pose.orientation.x = Input.Eyes.orientation.x;
                        eyeData.rightEye.pose.orientation.y = Input.Eyes.orientation.y;
                        eyeData.rightEye.pose.orientation.z = Input.Eyes.orientation.z;
                        eyeData.rightEye.pose.orientation.w = Input.Eyes.orientation.w;

                        eyeData.rightEye.pose.position.x = Input.Eyes.position.x;
                        eyeData.rightEye.pose.position.y = Input.Eyes.position.y;
                        eyeData.rightEye.pose.position.z = Input.Eyes.position.z;

                        eyeData.rightEye.pose.ray.direction.x = Input.Eyes.Ray.direction.x;
                        eyeData.rightEye.pose.ray.direction.y = Input.Eyes.Ray.direction.y;
                        eyeData.rightEye.pose.ray.direction.z = Input.Eyes.Ray.direction.z;

                        eyeData.rightEye.pose.right.x = Input.Eyes.Ray.position.x;
                        eyeData.rightEye.pose.right.y = Input.Eyes.Ray.position.y;
                        eyeData.rightEye.pose.right.z = Input.Eyes.Ray.position.z;

                        eyeData.rightEye.pose.up.x = Input.Eyes.Up.x;
                        eyeData.rightEye.pose.up.y = Input.Eyes.Up.y;
                        eyeData.rightEye.pose.up.z = Input.Eyes.Up.z;

                        eyeData.rightEye.eyesTracked = Input.EyesTracked.IsActive();
                        eyeData.rightEye.eyesSampleTime = Backend.XRType == BackendXRType.OpenXR ?
                            Backend.OpenXR.EyesSampleTime : 0;
            #endregion Right Eye

            #region Combined Eye
                        eyeData.combinedEye.pose.forward.x = Input.Eyes.Forward.x;
                        eyeData.combinedEye.pose.forward.y = Input.Eyes.Forward.y;
                        eyeData.combinedEye.pose.forward.z = Input.Eyes.Forward.z;

                        eyeData.combinedEye.pose.orientation.x = Input.Eyes.orientation.x;
                        eyeData.combinedEye.pose.orientation.y = Input.Eyes.orientation.y;
                        eyeData.combinedEye.pose.orientation.z = Input.Eyes.orientation.z;
                        eyeData.combinedEye.pose.orientation.w = Input.Eyes.orientation.w;

                        eyeData.combinedEye.pose.position.x = Input.Eyes.position.x;
                        eyeData.combinedEye.pose.position.y = Input.Eyes.position.y;
                        eyeData.combinedEye.pose.position.z = Input.Eyes.position.z;

                        eyeData.combinedEye.pose.ray.direction.x = Input.Eyes.Ray.direction.x;
                        eyeData.combinedEye.pose.ray.direction.y = Input.Eyes.Ray.direction.y;
                        eyeData.combinedEye.pose.ray.direction.z = Input.Eyes.Ray.direction.z;

                        eyeData.combinedEye.pose.right.x = Input.Eyes.Ray.position.x;
                        eyeData.combinedEye.pose.right.y = Input.Eyes.Ray.position.y;
                        eyeData.combinedEye.pose.right.z = Input.Eyes.Ray.position.z;

                        eyeData.combinedEye.pose.up.x = Input.Eyes.Up.x;
                        eyeData.combinedEye.pose.up.y = Input.Eyes.Up.y;
                        eyeData.combinedEye.pose.up.z = Input.Eyes.Up.z;

                        eyeData.combinedEye.eyesTracked = Input.EyesTracked.IsActive();
                        eyeData.combinedEye.eyesSampleTime = Backend.XRType == BackendXRType.OpenXR ? 
                            Backend.OpenXR.EyesSampleTime : 0;
            #endregion Combined Eye
        }

        public override void Teardown()
        {
            SK.Shutdown();
        }
    }
}
