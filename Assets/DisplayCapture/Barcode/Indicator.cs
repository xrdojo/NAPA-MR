using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

namespace Anaglyph.DisplayCapture.Barcodes
{
	public class Indicator : MonoBehaviour
{
    public static Indicator Instance;

    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer => lineRenderer;

    [SerializeField] private TMP_Text textMesh;
    public TMP_Text TextMesh => textMesh;

    [SerializeField] private Transform reassemblyPositionMarker;

    private Vector3[] offsetPositions = new Vector3[4];

    public GameObject eventOne;
    public GameObject reassemblyOne;
    public GameObject wrenchObjectToSpawn;
    private GameObject spawnedWrenchObject;

    private HashSet<string> singleDetectionBarcodes = new HashSet<string>();
    private bool hasScannedMocap = false;
    private bool hasScannedWrench = false;

    public Vector3 EventOnePosition => reassemblyPositionMarker != null ? reassemblyPositionMarker.position : Vector3.zero;
    public Quaternion EventOneRotation => reassemblyPositionMarker != null ? reassemblyPositionMarker.rotation : Quaternion.identity;

    private void Start()
    {
        Instance = this;

        eventOne.SetActive(false);
        reassemblyOne.SetActive(false);

        singleDetectionBarcodes.Clear();
        hasScannedMocap = false;
        hasScannedWrench = false;
        spawnedWrenchObject = null;

        if (eventOne == null) Debug.LogWarning("eventOne is not assigned in the Inspector!");
        if (reassemblyOne == null) Debug.LogWarning("reassemblyOne is not assigned in the Inspector!");
        if (reassemblyPositionMarker == null) Debug.LogWarning("reassemblyPositionMarker is not assigned in the Inspector!");
        if (FixedPositionHandler.Instance == null) Debug.LogWarning("FixedPositionHandler.Instance is null!");
    }

    public void Set(BarcodeTracker.Result result) => Set(result.text, result.corners);

    public void Set(string text, Vector3[] corners)
{
    // Force "Mocap" to be scanned before anything else
    if (!hasScannedMocap)
    {
        if (text != "Mocap")
        {
            textMesh.text = "Please scan Mocap first!";
            Debug.Log($"Blocked scan of '{text}' — Mocap must be scanned first.");
            return;
        }
    }

    bool isSingleDetection = text == "Mocap" || text == "Standard Wrench";

    if (text == "Mocap" && hasScannedWrench && singleDetectionBarcodes.Contains("Mocap"))
    {
        singleDetectionBarcodes.Remove("Mocap");
    }

    if (isSingleDetection && singleDetectionBarcodes.Contains(text))
    {
        return;
    }

    if (isSingleDetection)
    {
        singleDetectionBarcodes.Add(text);
    }

    // Vector3 topCenter = (corners[0] + corners[1]) / 2f;
    // transform.position = topCenter;

    // Vector3 up = (corners[0] - corners[3]).normalized;
    // Vector3 right = (corners[2] - corners[3]).normalized;
    // Vector3 normal = -Vector3.Cross(up, right).normalized;

    // Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;

    // for (int i = 0; i < 4; i++)
    // {
    //     Vector3 dir = (corners[i] - center).normalized;
    //     // offsetPositions[i] = corners[i] + (dir * lineRenderer.startWidth);
    // }

    // transform.rotation = Quaternion.LookRotation(normal, up);

    // Drew's changes
    Vector3[] sortedCorners = SortCorners(corners);
    Vector3 topLeft = sortedCorners[0];
    Vector3 topRight = sortedCorners[1];
    Vector3 bottomRight = sortedCorners[2];
    Vector3 bottomLeft = sortedCorners[3];

    Vector3 topCenter = (topLeft + topRight) / 2f;
    transform.position = topCenter;

    Vector3 up = (topLeft - bottomLeft).normalized;
    Vector3 right = (bottomRight - bottomLeft).normalized;
    Vector3 normal = -Vector3.Cross(up, right).normalized;

    Vector3 center = (topLeft + topRight + bottomRight + bottomLeft) / 4f;

    for (int i = 0; i < 4; i++)
    {
    Vector3 dir = (corners[i] - center).normalized;
    // offsetPositions[i] = corners[i] + (dir * lineRenderer.startWidth);
    }

    // transform.rotation = Quaternion.LookRotation(normal, up);

    Quaternion baseRotation = Quaternion.LookRotation(normal, up);

    Quaternion flipYZ = Quaternion.Euler(0f, 180f, 180f);
    transform.rotation = baseRotation * flipYZ;





    // lineRenderer.SetPositions(offsetPositions);
    textMesh.text = text;

    if (text == "Mocap")
    {
        Debug.Log(text);
        if (!hasScannedMocap)
        {
            SimulationManager.Instance.DisableTracking();
            textMesh.text = "Object Detected!";
            // SimulationUI.Instance.TriggerToolSelectUI();
            SimulationUI.Instance.TurnOffViewFinder();
            SimulationUI.Instance.TurnOffPanelView();

            AnimFxManager.Instance.AnimOneSound();

            eventOne.SetActive(true);
            if (eventOne != null && reassemblyPositionMarker != null)
            {
                reassemblyPositionMarker.position = eventOne.transform.position;
                reassemblyPositionMarker.rotation = eventOne.transform.rotation;
                Debug.Log($"Set reassemblyPositionMarker to position: {reassemblyPositionMarker.position}, rotation: {reassemblyPositionMarker.rotation.eulerAngles}");

                if (FixedPositionHandler.Instance != null)
                {
                    FixedPositionHandler.Instance.UpdatePosition();
                }
            }
            else
            {
                Debug.LogWarning("eventOne or reassemblyPositionMarker is null; cannot set marker position!");
            }
            reassemblyOne.SetActive(false);

            // SimulationManager.Instance.DisableTracking();
            hasScannedMocap = true;
        }
        else if (hasScannedWrench)
        {
            SimulationManager.Instance.DisableTracking();
            textMesh.text = "Second Mocap Detected!";
            Debug.Log("Second Mocap scan detected after Standard Wrench");
            SimulationUI.Instance.TurnOffViewFinder();

            reassemblyOne.SetActive(true);

            AnimFxManager.Instance.AnimTwoSound();


            if (reassemblyPositionMarker != null)
            {
                reassemblyOne.transform.position = reassemblyPositionMarker.position;
                reassemblyOne.transform.rotation = reassemblyPositionMarker.rotation;
            }
        }
    }
    else if (text == "Standard Wrench")
    {
        Debug.Log("Detected Standard Wrench barcode");
        SimulationManager.Instance.DisableTracking();
        textMesh.text = "Correct Answer";
        AudioManager.Instance.CorrectAnswerSound();
        SimulationUI.Instance.TriggerCorrectAnswerUI();
        SimulationUI.Instance.TurnOffViewFinder();

        if (hasScannedMocap && eventOne != null)
        {
            eventOne.SetActive(false);
        }
        else
        {
            Debug.LogWarning("eventOne is null or Mocap hasn't been scanned yet!");
        }

        hasScannedWrench = true;
    }
    else if (text == "Brake-fan gauge")
    {
        Debug.Log(text);
        textMesh.text = "Wrong Answer";
        SimulationUI.Instance.TriggerWrongAnswerUI();
        SimulationUI.Instance.TurnOffViewFinder();

        if (hasScannedMocap)
        {
            eventOne.SetActive(false);
        }
        reassemblyOne.SetActive(false);
        SimulationManager.Instance.DisableTracking();
    }
    else if (text == "Torque Wrench")
    {
        Debug.Log(text);
        textMesh.text = "Wrong Answer";
        SimulationUI.Instance.TriggerTWrenchAnswer();
        SimulationUI.Instance.TurnOffViewFinder();

        if (hasScannedMocap)
        {
            eventOne.SetActive(false);
        }
        reassemblyOne.SetActive(false);
        SimulationManager.Instance.DisableTracking();
    }
}


    private IEnumerator EnsurePositionAfterActivation(GameObject target, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForEndOfFrame();
        if (target != null)
        {
            target.transform.position = position;
            target.transform.rotation = rotation;
            Debug.Log($"Reapplied to {target.name}: position {target.transform.position}, rotation: {target.transform.rotation.eulerAngles}");
        }
    }

    public void ResetDetections()
    {
        singleDetectionBarcodes.Clear();
        hasScannedMocap = false;
        hasScannedWrench = false;
    }

    public void ResetSimulation()
    {
        ResetDetections();
        WrenchBrakeAnimTrigger.Instance.EnableWrenchObjs();

        if (eventOne != null) eventOne.SetActive(false);
        if (reassemblyOne != null) reassemblyOne.SetActive(false);

        if (spawnedWrenchObject != null)
        {
            Destroy(spawnedWrenchObject);
            spawnedWrenchObject = null;
            Debug.Log("Spawned wrench object destroyed and cleared.");
        }

        if (textMesh != null) textMesh.text = "";
        if (lineRenderer != null) lineRenderer.SetPositions(new Vector3[4]);
    }

    private Vector3[] SortCorners(Vector3[] corners)
    {
        if (corners == null || corners.Length != 4)
        {
            Debug.LogWarning("Expected exactly 4 corners from ZXing.");
            return corners;
        }

        // Step 1: Calculate the centroid (center of the 4 points)
        Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;

        // Step 2: Classify each point based on position relative to center
        Vector3 topLeft = Vector3.zero;
        Vector3 topRight = Vector3.zero;
        Vector3 bottomRight = Vector3.zero;
        Vector3 bottomLeft = Vector3.zero;

        foreach (var pt in corners)
        {
            if (pt.x < center.x && pt.y < center.y)
            topLeft = pt;
            else if (pt.x > center.x && pt.y < center.y)
            topRight = pt;
            else if (pt.x > center.x && pt.y > center.y)
            bottomRight = pt;
            else if (pt.x < center.x && pt.y > center.y)
            bottomLeft = pt;
        }

        return new Vector3[] { topLeft, topRight, bottomRight, bottomLeft };
        }

    }
}