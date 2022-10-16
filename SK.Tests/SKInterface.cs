using StereoKit;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeData
    {
        public Eye leftEye;                      //!< Left eye data
        public Eye rightEye;                     //!< Right eye data
        public Eye combinedEye;                  //!< Combined eye data
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeDataLite
    {
        public Eye combinedEye;                  //!< Combined eye data
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Eye
    {
        public Pose pose;                         //!< Pose of the eye
        public bool eyeTrackingPresent;           //<! If the hardware is capable of providing eye tracking.
        public bool eyesTracked;                  //<! If eye hardware is available and app has permission, then this is the tracking state of the eyes. Eyes may move out of bounds, hardware may fail to detect eyes, or who knows what else!
        public long eyesSampleTime;               //<! This is the OpenXR time of the eye tracker sample associated with the current value of .
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
        public Vec3 direction;                    //!< The direction the ray is facing, typically does not require being a unit vector, or normalized direction.
        public Vec3 position;                     //!< The position or origin point of the Ray.
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

    public abstract class SKInterface : IDisposable
    {
        protected EyeData eyeData;
        protected EyeDataLite eyeDataLite;
        protected const string name = "StereokitEyeTracking";
        private readonly StringBuilder sb = new StringBuilder(); // Intended use case is mutually exclusive access, so no need to lock

#if DEBUG
        Matrix descPose = Matrix.TR(-0.5f, 0, -0.5f, StereoKit.Quat.LookDir(1, 0, 1));
        string description = "If the hardware supports it, and permissions are granted, eye tracking is as simple as grabbing Input.Eyes!\n\nThis scene is raycasting your eye ray at the indicated plane, and the dot's red/green color indicates eye tracking availability! On flatscreen you can simulate eye tracking with Alt+Mouse.";
        Matrix titlePose = Matrix.TRS(V.XYZ(-0.5f, 0.05f, -0.5f), StereoKit.Quat.LookDir(1, 0, 1), 2);
        string title = "Eye Tracking";

        List<LinePoint> points = new List<LinePoint>();
        StereoKit.Vec3 previous;

        long lastEyesSampleTime;
        DateTime demoStartTime;
        int uniqueSamplesCount;
#endif

        public virtual void OnInit()
        {
            SK.Initialize(new SKSettings
            {
                appName = name,
                disableUnfocusedSleep = true,
#if DEBUG
                displayPreference = DisplayMode.Flatscreen
#else
                displayPreference = DisplayMode.MixedReality
#endif
            });

            AppDomain.CurrentDomain.ProcessExit += new EventHandler((object sender, EventArgs e) => Teardown());
        }

        public virtual void OnRun(bool collectLite)
        {
            if (collectLite)
                SK.Run(UpdateLite);
            else
                SK.Run(Update);
        }

        public virtual bool OnStep(bool collectLite)
        {
            if (collectLite)
                return SK.Step(UpdateLite);
            else
                return SK.Step(Update);
        }

        public virtual void Update()
        {
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

            RunSK();
        }

        public virtual void UpdateLite()
        {
            #region Combined Eye Lite
            eyeDataLite.combinedEye.pose.forward.x = Input.Eyes.Forward.x;
            eyeDataLite.combinedEye.pose.forward.y = Input.Eyes.Forward.y;
            eyeDataLite.combinedEye.pose.forward.z = Input.Eyes.Forward.z;

            eyeDataLite.combinedEye.pose.orientation.x = Input.Eyes.orientation.x;
            eyeDataLite.combinedEye.pose.orientation.y = Input.Eyes.orientation.y;
            eyeDataLite.combinedEye.pose.orientation.z = Input.Eyes.orientation.z;
            eyeDataLite.combinedEye.pose.orientation.w = Input.Eyes.orientation.w;

            eyeDataLite.combinedEye.pose.position.x = Input.Eyes.position.x;
            eyeDataLite.combinedEye.pose.position.y = Input.Eyes.position.y;
            eyeDataLite.combinedEye.pose.position.z = Input.Eyes.position.z;

            eyeDataLite.combinedEye.pose.ray.direction.x = Input.Eyes.Ray.direction.x;
            eyeDataLite.combinedEye.pose.ray.direction.y = Input.Eyes.Ray.direction.y;
            eyeDataLite.combinedEye.pose.ray.direction.z = Input.Eyes.Ray.direction.z;

            eyeDataLite.combinedEye.pose.right.x = Input.Eyes.Ray.position.x;
            eyeDataLite.combinedEye.pose.right.y = Input.Eyes.Ray.position.y;
            eyeDataLite.combinedEye.pose.right.z = Input.Eyes.Ray.position.z;

            eyeDataLite.combinedEye.pose.up.x = Input.Eyes.Up.x;
            eyeDataLite.combinedEye.pose.up.y = Input.Eyes.Up.y;
            eyeDataLite.combinedEye.pose.up.z = Input.Eyes.Up.z;

            eyeDataLite.combinedEye.eyesTracked = Input.EyesTracked.IsActive();
            eyeDataLite.combinedEye.eyesSampleTime = Backend.XRType == BackendXRType.OpenXR ?
                Backend.OpenXR.EyesSampleTime : 0;
            #endregion Combined Eye Lite    

            RunSK();
        }

        private void RunSK()
        {
#if DEBUG
            Plane plane = new Plane(new StereoKit.Vec3(0.5f, 0, -0.5f), V.XYZ(-0.5f, 0, 0.5f));
            Matrix quadPose = Matrix.TRS(new StereoKit.Vec3(0.54f, 0, -0.468f), StereoKit.Quat.LookDir(plane.normal), 0.5f);
            Mesh.Quad.Draw(Material.Default, quadPose);
            if (Input.Eyes.Ray.Intersect(plane, out StereoKit.Vec3 at))
            {
                Color stateColor = Input.EyesTracked.IsActive()
                    ? new Color(0, 1, 0)
                    : new Color(1, 0, 0);
                Default.MeshSphere.Draw(Default.Material, Matrix.TS(at, 3 * U.cm), stateColor);
                if (StereoKit.Vec3.DistanceSq(at, previous) > U.cm * U.cm)
                {
                    previous = at;
                    points.Add(new LinePoint { pt = at, color = Color.White });
                    if (points.Count > 20)
                        points.RemoveAt(0);
                }

                LinePoint pt = points[points.Count - 1];
                pt.pt = at;
                points[points.Count - 1] = pt;
            }

            for (int i = 0; i < points.Count; i++)
            {
                LinePoint pt = points[i];
                pt.thickness = (i / (float)points.Count) * 3 * U.cm;
                points[i] = pt;
            }

            Lines.Add(points.ToArray());

            Text.Add(title, titlePose);
            Text.Add(description, descPose, V.XY(0.4f, 0), TextFit.Wrap, TextAlign.TopCenter, TextAlign.TopLeft);

            if (Backend.XRType == BackendXRType.OpenXR)
            {
                if (Backend.OpenXR.EyesSampleTime != lastEyesSampleTime)
                {
                    lastEyesSampleTime = Backend.OpenXR.EyesSampleTime;
                    uniqueSamplesCount++;
                }

                double sampleFrequency = uniqueSamplesCount / (DateTime.UtcNow - demoStartTime).TotalSeconds;
                Text.Add($"Eye tracker sampling frequency: {sampleFrequency:0.#} Hz", Matrix.T(V.XYZ(0, -0.55f, -0.1f)) * quadPose);
            }
#endif
        }

        public virtual void Teardown()
        {
            SK.Shutdown();
        }

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

        public string GetDebugDataLite()
        {
            sb.Clear();
            
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

        private string PrintStruct(Vec3 vec3)
        {
            return $"X: {vec3.x}, Y: {vec3.y}, Z: {vec3.z}";
        }

        private string PrintStruct(Quat quat)
        {
            return $"X: {quat.x}, Y: {quat.y}, Z: {quat.z}, W: {quat.w}";
        }

        public void Dispose()
        {
            Teardown();
        }
    }
}
