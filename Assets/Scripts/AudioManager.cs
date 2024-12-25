using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private AudioSource audioSource;
    private int lastPlayedIndex = -1;

    // с такой реализацией audioSource нужно ко всем дочерним класса прикреплять... такое себе.
    // мб есть другой способ?
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    // убрал отдельные переменные для индексов, так как их передавать нет смысла по сути из-за того,
    // что они все = -1. тогда ведь можно одну внутри сделать, не?
    public int PlaySoundByRandom(AudioClip[] sounds) {
        int randomIndex;

        if (sounds.Length == 1) {
            randomIndex = 0;
        } else {
            randomIndex = Random.Range(0, sounds.Length - 1);

            // рандомный звук + не воспроизводим прошлый звук
            if (randomIndex >= lastPlayedIndex) {
                randomIndex++;
            }
        }

        lastPlayedIndex = randomIndex;
        audioSource.clip = sounds[randomIndex];
        audioSource.Play();

        return lastPlayedIndex;
    }

    // для музыки на фоне
    public void PlaySoundByTiming() {

    }
}
