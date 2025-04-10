using UnityEngine;

public class HandTrackingToggle : MonoBehaviour
{
    public static HandTrackingToggle Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public GameObject leftHand;
    public GameObject rightHand;

    public void HideHands()
    {
        if(leftHand) leftHand.SetActive(false);
        if(rightHand) rightHand.SetActive(false);
    }

    public void ShowHands()
    {
        if(leftHand) leftHand.SetActive(true);
        if(rightHand) rightHand.SetActive(true);
    }
}
