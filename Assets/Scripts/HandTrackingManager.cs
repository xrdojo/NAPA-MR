using UnityEngine;
using UnityEngine.XR.Management;

// public class HandTrackingManager : MonoBehaviour
// {
//     public enum HandTrackingMode
//     {
//         ControllersOnly,
//         HandsOnly,
//         ControllersAndHands
//     }

//     public void SetHandTrackingMode(HandTrackingMode mode)
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         var ovrMode = mode switch
//         {
//             HandTrackingMode.ControllersOnly => Meta.XR.Feature.HandTrackingMode.ControllersOnly,
//             HandTrackingMode.HandsOnly => Meta.XR.Feature.HandTrackingMode.HandsOnly,
//             HandTrackingMode.ControllersAndHands => Meta.XR.Feature.HandTrackingMode.ControllersAndHands,
//             _ => Meta.XR.Feature.HandTrackingMode.ControllersAndHands
//         };

//         Meta.XR.Feature.MetaXRHandTracking.SetHandTrackingMode(ovrMode);
//         Debug.Log($"Hand tracking mode set to: {ovrMode}");
// #endif
//     }
// }
