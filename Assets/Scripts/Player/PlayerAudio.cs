using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! нужно попробовать сделать так, чтобы на разных поверхностях был разный звук шагов и приземления

public class PlayerAudio : AudioManager {
    [Header("Footsteps sound")]
    [SerializeField] private AudioClip[] footstepsSounds;

    [Header("Jump sound")]
    [SerializeField] private AudioClip[] jumpStartSounds;
    [SerializeField] private AudioClip[] jumpLandSounds;

    public void PlayFootstepSound() {
        PlaySoundByRandom(footstepsSounds);
    }

    public void PlayJumpStartSound() {
        PlaySoundByRandom(jumpStartSounds);
    }

    public void PlayJumpLandSound() {
        PlaySoundByRandom(jumpLandSounds);
    }
}
