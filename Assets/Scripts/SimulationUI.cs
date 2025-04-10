using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Anaglyph.DisplayCapture.Barcodes;

public class SimulationUI : MonoBehaviour
{
    public static SimulationUI Instance { get; private set; }

    [Header("Fade In and Out effect for Logo")]
    // public FadeScript _fadeScript;

    // Instruction label text
    public TextMeshProUGUI instructionTxt;

    public GameObject restartButton;
    public GameObject tryAgainButton;

    // Ok Button
    public GameObject okButton;

    public GameObject viewFinderUIObj;

    public GameObject logoUiObj;
    public GameObject panelViewObj;

    // Panel For Correct Answer
    public GameObject correctAnswerObj;

    // Panel for Wrong Answer
    public GameObject wrongAnswerObj;

    private bool isRestarting = false;



    void Start()
    {
        Instance = this;

        RestartSimulation(); // Start the simulation in a reset state
    }

    public void TurnOnViewFinder()
    {
        viewFinderUIObj.SetActive(true);
    }

    public void TurnOffViewFinder()
    {
        viewFinderUIObj.SetActive(false);
    }

    public void TurnOnLogo()
    {
        logoUiObj.SetActive(true);
    }

    public void TurnOffLogo()
    {
        logoUiObj.SetActive(false);
    }

    public void TurnOnPanelView()
    {
        panelViewObj.SetActive(true);
    }

    public void TurnOffPanelView()
    {
        panelViewObj.SetActive(false);
    }

    public void CorrectAnswerPanelOn()
    {
        correctAnswerObj.SetActive(true);
    }

    public void CorrectAnswerPanelOff()
    {
        correctAnswerObj.SetActive(false);
    }

    public void WrongAnswerPanelOn()
    {
        wrongAnswerObj.SetActive(true);
    }

    public void WrongAnswerPanelOff()
    {
        wrongAnswerObj.SetActive(false);
    }

    public void SimulationStartText()
    {
        // instructionTxt.text = "Scan QRCode to begin training";
        TurnOffPanelView();
        restartButton.SetActive(false);
        tryAgainButton.SetActive(false);
        okButton.SetActive(false);

        SimulationManager.Instance.EnableTracking();
    }

    // public void TriggerToolSelectUI()
    // {
    //     instructionTxt.text = "There are three tools in front of you: a standard wrench, a torque wrench, and a brake-fan gauge. Each tool has a specific function, but only one is correct for removing the caliper bolt from the rear brake mock-up. Pick up the correct tool to solve the problem.";
    //     FindObjectOfType<FixedPositionHandler>()?.UpdatePosition(); // Call after Mocap
    //     SimulationManager.Instance.EnableTracking();
    // }

    public void TriggerWrongAnswerUI()
    {
        instructionTxt.text = "You selected the brake-fan gauge. That tool is used for measuring pad thickness, not for removing the caliper bolt. Would you like to try again?";
        AudioManager.Instance.WrongAnswerSound();
        SimulationManager.Instance.TriggerWrongAnswerOne();
        WrongAnswerPanelOn();
        // TurnOnPanelView();
        tryAgainButton.SetActive(true);
        // StartCoroutine(RestartScan());
    }

    public void TriggerTWrenchAnswer()
    {
        instructionTxt.text = "You selected the torque wrench. That tool is correct for assembly, but not for removing the caliper bolt. Let’s try again.";
        AudioManager.Instance.WrongAnswerSound();
        SimulationManager.Instance.TriggerWrongAnswerTwo();
        WrongAnswerPanelOn();

        tryAgainButton.SetActive(true);
        // StartCoroutine(RestartScan());
    }

    public void TriggeringRestartScan()
    {
        // StartCoroutine(RestartScan());

        TurnOffPanelView();
        CorrectAnswerPanelOff();
        WrongAnswerPanelOff();
        TurnOnViewFinder();
        SimulationManager.Instance.EnableTracking();
    }

    // IEnumerator RestartScan()
    // {
    //     yield return new WaitForSeconds(10);
    //     TurnOffPanelView();
    //     CorrectAnswerPanelOff();
    //     WrongAnswerPanelOff();
    //     TurnOnViewFinder();
    //     SimulationManager.Instance.EnableTracking();
    //     // StopCoroutine(RestartScan());
    // }

    public void TryAgainUI()
    {
        TurnOffPanelView();
        TurnOnViewFinder();
        SimulationManager.Instance.EnableTracking();
    }

    public void TriggerCorrectAnswerUI()
    {
        instructionTxt.text = "Correct tool";
        restartButton.SetActive(false);
        tryAgainButton.SetActive(false);
        okButton.SetActive(true);
        CorrectAnswerPanelOn();
        AudioManager.Instance.CorrectAnswerSound();
        SimulationManager.Instance.TriggerCorrectAnswer();

        // StartCoroutine(TriggerAnimTwo());
    }

    public void TriggerMainAnimTwo()
    {
        // TriggerNextTrainingStep();
        StartCoroutine(TriggerAnimTwo());
    }

    IEnumerator TriggerAnimTwo()
    {
        yield return new WaitForSeconds(5);

        TriggerNextTrainingStep();

        StopCoroutine(TriggerAnimTwo());
    }


    public void TriggerNextTrainingStep()
    {
        restartButton.SetActive(false);
        tryAgainButton.SetActive(false);
        okButton.SetActive(false);
        CorrectAnswerPanelOff();
        TurnOffPanelView();
        TurnOnViewFinder();

        SimulationManager.Instance.EnableTracking();
    }

    public void TriggerTrainingComplete()
    {
        instructionTxt.text = "Congratulations! You have successfully completed this Mixed Reality demonstration";
        restartButton.SetActive(true);
        tryAgainButton.SetActive(false);
        okButton.SetActive(false);

        // Turning on hand tracking visuals
        HandTrackingToggle.Instance.ShowHands();

        TurnOnPanelView();
        TurnOffViewFinder();
    }

    public void EndSimulation()
    {
        StartCoroutine(TriggerEndSimUI());
    }

    IEnumerator TriggerEndSimUI()
    {
        yield return new WaitForSeconds(10);
        SimulationManager.Instance.TriggerCompletion();
        TriggerRestartUI();
    }

    public void TriggerRestartUI()
    {
        instructionTxt.text = "Congratulations! You have successfully completed this Mixed Reality demonstration";
    }

    // Enhanced method to restart the simulation
    public void RestartSimulation()
    {
        if (isRestarting) return;
        isRestarting = true;



        // StopAllCoroutines();
        StartCoroutine(HandleSimulationRestart());
    }

    private IEnumerator HandleSimulationRestart()
    {
        PrepareUIForRestart();

        yield return ShowLogoSequence();

        // SimulationManager.Instance.TriggerIntro();

        // ResetSimulationState();

        isRestarting = false;
    }

    public void TriggerStartSim()
    {
        SimulationManager.Instance.TriggerIntro();

        HandTrackingToggle.Instance.HideHands();

        logoUiObj.SetActive(false);

    }

    private void PrepareUIForRestart()
    {
        TurnOffViewFinder();
        TurnOffPanelView();
        CorrectAnswerPanelOff();
        WrongAnswerPanelOff();

        if (Indicator.Instance != null)
        {
            Indicator.Instance.reassemblyOne?.SetActive(false);
        }
    }




    private IEnumerator ShowLogoSequence()
    {
        if (logoUiObj != null)
        {
            
            logoUiObj.SetActive(true);
            Debug.Log("Displaying Logo");
            SimulationManager.Instance.DisableTracking();
            yield return new WaitForSeconds(5f);

            // logoUiObj.SetActive(false);
            // Debug.Log("Hiding Logo");
        }
        else
        {
            Debug.LogWarning("Logo is not assigned.");
        }
    }

    public void ResetSimulationState()
    {
        SimulationStartText();
        TurnOnViewFinder();

        SimulationManager.Instance?.PlayBGMusic();

        if (Indicator.Instance != null)
            Indicator.Instance.ResetSimulation();
        else
            Debug.LogWarning("Indicator.Instance is null.");

        if (SimulationManager.Instance != null)
            SimulationManager.Instance.ResetState();
        else
            Debug.LogWarning("SimulationManager.Instance is null.");
    }

    public void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}