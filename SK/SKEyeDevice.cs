using BaseX;
using FrooxEngine;
using SKEyeTracking.Data;

namespace SKEyeTracking
{
    public class SKEyeDevice : IInputDriver
    {
        private SKListener SKListener;
        private Eyes eyes;
        public int UpdateOrder => 100;

        public SKEyeDevice()
        {
            SKListener = new SKListener();
            SKListener.Initialize();
            
            Engine.Current.OnShutdown += Teardown;
        }

        private void Teardown()
        {
            SKListener.Teardown();
        }

        public void CollectDeviceInfos(DataTreeList list)
        {
            var eyeDataTreeDictionary = new DataTreeDictionary();
            eyeDataTreeDictionary.Add("Name", "Stereokit Eye Tracking");
            eyeDataTreeDictionary.Add("Type", "Eye Tracking");
            eyeDataTreeDictionary.Add("Model", "Stereokit Eye Model");
            list.Add(eyeDataTreeDictionary);
        }

        public void RegisterInputs(InputInterface inputInterface)
        {
            eyes = new Eyes(inputInterface, "Stereokit Eye Tracking");
        }

        public void UpdateInputs(float deltaTime)
        {
            SKListener.Update();
            var eyeData = SKListener.GetEyeData();

            // SK only tracks combined eye information
            eyes.IsEyeTrackingActive = eyeData.combinedEye.eyeTrackingPresent;

            UpdateEye(
                eyeData.leftEye.pose.forward.Vec3ToFloat3(),
                eyeData.leftEye.pose.position.Vec3ToFloat3(),
                eyeData.leftEye.eyesTracked,
                0.0035f,
                eyeData.leftEye.eyesTracked ? 1f : 0f,
                0f, 0f, 0f, deltaTime, eyes.LeftEye);
            UpdateEye(
                eyeData.rightEye.pose.forward.Vec3ToFloat3(),
                eyeData.rightEye.pose.position.Vec3ToFloat3(),
                eyeData.rightEye.eyesTracked,
                0.0035f,
                eyeData.rightEye.eyesTracked ? 1f : 0f,
                0f, 0f, 0f, deltaTime, eyes.RightEye);
            UpdateEye(
                eyeData.combinedEye.pose.forward.Vec3ToFloat3(), 
                eyeData.combinedEye.pose.position.Vec3ToFloat3(), 
                eyeData.combinedEye.eyesTracked, 
                0.0035f, 
                eyeData.combinedEye.eyesTracked ? 1f : 0f,
                0f, 0f, 0f, deltaTime, eyes.CombinedEye);

            eyes.ComputeCombinedEyeParameters();
            eyes.ConvergenceDistance = 0f;
            eyes.Timestamp = eyeData.combinedEye.eyesSampleTime;
            eyes.FinishUpdate();
        }

        private void UpdateEye(float3 gazeDirection, float3 gazeOrigin, bool status, float pupilSize, float openness,
            float widen, float squeeze, float frown, float deltaTime, FrooxEngine.Eye eye)
        {
            eye.IsDeviceActive = Engine.Current.InputInterface.VR_Active;
            eye.IsTracking = status;

            if (eye.IsTracking)
            {
                eye.UpdateWithDirection(gazeDirection);
                eye.RawPosition = gazeOrigin;
                eye.PupilDiameter = pupilSize;
            }

            eye.Openness = openness;
            eye.Widen = widen;
            eye.Squeeze = squeeze;
            eye.Frown = frown;
        }


    }
}