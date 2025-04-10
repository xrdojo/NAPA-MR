using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Anaglyph.DisplayCapture.Barcodes
{
    public class AnimationTrigger : MonoBehaviour
    {
        [SerializeField] private Animator animator; // Reference to the Animator component

        private float currentAnimationDuration;
        private float timer;
        private bool isPlaying;
        private int lastStateID;


        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogError("Animator component not found!", this);
                    enabled = false;
                    return;
                }
            }
        }

        void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Detect new animation
            if (stateInfo.fullPathHash != lastStateID)
            {
                lastStateID = stateInfo.fullPathHash;
                isPlaying = true;
                timer = 0f;

                currentAnimationDuration = GetCurrentAnimationDuration(stateInfo);
            }

            if (isPlaying)
            {
                timer += Time.deltaTime;

                if (timer >= currentAnimationDuration)
                {
                    StartCoroutine(QueueReassembly());
                    isPlaying = false;
                    timer = 0f;
                }
            }
        }

        IEnumerator QueueReassembly()
        {
            yield return new WaitForSeconds(currentAnimationDuration);

            SimulationManager.Instance.TriggerReAssembly();
            Indicator.Instance.eventOne.SetActive(false);
        }

        private float GetCurrentAnimationDuration(AnimatorStateInfo stateInfo)
        {
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;

            foreach (AnimationClip clip in controller.animationClips)
            {
                return clip.length / stateInfo.speed;
            }

            Debug.LogWarning("Could not determine animation duration. Using default of 1 second.");
            return 1f;
        }

        public void ResetTimer()
        {
            isPlaying = false;
            timer = 0f;
            lastStateID = 0;
        }
    }
}