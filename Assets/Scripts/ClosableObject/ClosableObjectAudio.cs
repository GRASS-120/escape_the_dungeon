using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! сменить звуки дверей!!!

public class ClosableObjectAudio : AudioManager {
    [Header("ClosableObject sound")]
    [SerializeField] private AudioClip[] openSound;
    [SerializeField] private AudioClip[] closeSound;
    [SerializeField] private AudioClip[] isLockedSound;
    [SerializeField] private AudioClip[] unlockSound;

    // вообще функции тут повторяются, мб можно как-то это оптимизировать? укоротить?
    public void PlayOpenSound() {
        PlaySoundByRandom(openSound);
    }

    public void PlayCloseSound() {
        PlaySoundByRandom(closeSound);
    }

    public void PlayIsLockedSound() {
        PlaySoundByRandom(isLockedSound);
    }

    public void PlayUnlockSound() {
        PlaySoundByRandom(unlockSound);
    }
}
