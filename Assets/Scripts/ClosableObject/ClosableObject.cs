using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosableObject : InteractiveObject {
    public event EventHandler OnClose;
    public event EventHandler OnOpen;
    public event EventHandler OnLocked;

    [SerializeField] private InteractiveObjectSO objectSO;

    [Header("Object states")]
    [SerializeField] private bool isOpened = false;
    [SerializeField] private bool isLocked = false; 

    private ClosableObjectAudio audioPlayer;

    private void Awake() {
        audioPlayer = GetComponent<ClosableObjectAudio>();
    }

    public override void Interact(Player player) {
        if (isOpened) {
            Close();
        } else {
            Open();
        }
    }

    public override InteractiveObjectSO GetObjectSO() {
        return objectSO;
    }

    public void Close() {
        isOpened = false;
        OnClose?.Invoke(this, EventArgs.Empty);
        audioPlayer.PlayCloseSound(); 
    }

    public void Open() {
        if (isLocked) {
            OnLocked?.Invoke(this, EventArgs.Empty);
            audioPlayer.PlayIsLockedSound();
        } else {
            isOpened = true;
            OnOpen?.Invoke(this, EventArgs.Empty);
            audioPlayer.PlayOpenSound(); 
        }
    }

    public void Unlock() {
        // if (isLocked && key != null) {
        //      isLocked = false;
        //      audioPlayer.PlayUnlockSound();
        // }
    }

    public bool IsOpened() {
        return isOpened;
    }

    public bool IsLocked() {
        return isLocked;
    }
}
