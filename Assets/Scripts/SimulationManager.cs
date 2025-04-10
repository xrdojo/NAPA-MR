using System;
using System.Collections;
using System.Collections.Generic;
using Anaglyph.DisplayCapture;
using Anaglyph.DisplayCapture.Barcodes;
using UnityEngine;


public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;

    public BarcodeReader barcodeReader;


    private AudioSource audioSource;

    public enum SimulationEvent
    {
        Intro,
        BGMusic,
        ReAssembly,
        SelectBrakeFan,
        SelectTorqueWrench,
        SelectStandardWrench,
        Completion
    }

    [System.Serializable]
    public class AudioClipEntry
    {
        public SimulationEvent eventType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField] private List<AudioClipEntry> audioClips = new List<AudioClipEntry>();

    private Dictionary<SimulationEvent, List<AudioClipEntry>> eventToClips;

    private HashSet<SimulationEvent> playedEvents;

    void Start()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        eventToClips = new Dictionary<SimulationEvent, List<AudioClipEntry>>();
        playedEvents = new HashSet<SimulationEvent>();

        foreach(SimulationEvent evt in System.Enum.GetValues(typeof(SimulationEvent)))
        {
            eventToClips[evt] = new List<AudioClipEntry>();
        }

        foreach(var entry in audioClips)
        {
            if(entry.clip != null)
            {
                eventToClips[entry.eventType].Add(entry);
            }
        }

        // SimulationUI.Instance.SimulationStartText();
    }


    public void PlayAudioForEvent(SimulationEvent eventType, Action onAudioComplete = null)
{
    // Allow replays for WrongAnswerFx and SelectBrakeFan, skip check for those
    bool canReplay =  eventType == SimulationEvent.SelectBrakeFan || eventType == SimulationEvent.SelectTorqueWrench;

    if (!canReplay && playedEvents.Contains(eventType))
    {
        Debug.Log($"Audio for {eventType} has already been played; skipping.");
        return;
    }

    if (eventToClips.ContainsKey(eventType) && eventToClips[eventType].Count > 0)
    {
        var clipsForEvent = eventToClips[eventType];
        AudioClipEntry selectedEntry = clipsForEvent[UnityEngine.Random.Range(0, clipsForEvent.Count)];

        // Stop any current audio to avoid overlap
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Stopped currently playing audio to prevent overlap.");
        }

        // Assign the new clip and play it
        audioSource.clip = selectedEntry.clip;
        audioSource.volume = selectedEntry.volume;
        audioSource.Play();

        Debug.Log($"Played audio for {eventType}: {selectedEntry.clip.name}, volume: {selectedEntry.volume}");

        // Only add to playedEvents if not meant to be replayed
        if (!canReplay)
        {
            playedEvents.Add(eventType);
        }

        // Start Coroutine to wait until audio finishes
        StartCoroutine(WaitForAudioToEnd(selectedEntry.clip.length, onAudioComplete));
    }
    else
    {
        Debug.LogWarning($"No audio clips found for event: {eventType}");
    }
}

 


    private IEnumerator WaitForAudioToEnd(float duration, System.Action onAudioComplete)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Audio finishes playing.");

        onAudioComplete?.Invoke();
    }

    public void DisableTracking()
    {
        barcodeReader.enabled = false;
    }

    public void EnableTracking()
    {
        barcodeReader.enabled = true;
    }

    public void TriggerIntro()
    {
        PlayAudioForEvent(SimulationEvent.Intro, () => {
            SimulationUI.Instance.ResetSimulationState();
        });
    }

    public void TriggerReAssembly()
    {
        PlayAudioForEvent(SimulationEvent.ReAssembly, () => {
            SimulationUI.Instance.TriggeringRestartScan();
        });
    }


    public void TriggerWrongAnswerOne()
    {
        PlayAudioForEvent(SimulationEvent.SelectBrakeFan, () => {
            SimulationUI.Instance.TriggeringRestartScan();
        });
    }

    public void TriggerWrongAnswerTwo()
    {
        PlayAudioForEvent(SimulationEvent.SelectTorqueWrench, () => {
            SimulationUI.Instance.TriggeringRestartScan();
        });
    }

    public void TriggerCorrectAnswer()
    {
        PlayAudioForEvent(SimulationEvent.SelectStandardWrench, () => {
            SimulationUI.Instance.TriggerNextTrainingStep();
        });
    }


    public void TriggerCompletion()
    {
        // PlayAudioForEvent(SimulationEvent.Completion);
        PlayAudioForEvent(SimulationEvent.Completion, () => {
            // This will run after the audio clip finishes playing
            SimulationUI.Instance.TriggerTrainingComplete();
        });
    }

    public void PlayBGMusic()
    {
        PlayAudioForEvent(SimulationEvent.BGMusic);
    }

    public void ResetState()
    {
        // Reset any internal state (e.g., flags, timers)
        EnableTracking(); // Ensure tracking is enabled for a fresh start
        playedEvents.Clear();
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Stopped audio during reset.");
        }
        // Stop any sounds if playing
        // Reset any other state as needed
    }


    public void AddAudioClip(SimulationEvent eventType, AudioClip clip, float volume = 1)
    {
        AudioClipEntry newEntry = new AudioClipEntry
        {
            eventType = eventType,
            clip = clip,
            volume = volume
        };

        audioClips.Add(newEntry);
        eventToClips[eventType].Add(newEntry);
    }
}
