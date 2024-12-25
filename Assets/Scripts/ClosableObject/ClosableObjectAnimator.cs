using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosableObjectAnimator : MonoBehaviour {
    [SerializeField] private ClosableObject closableObject;
    
    private Animator animator;
    private const string IS_OPENED = "isOpened";
    private const string LOCKED = "locked";

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        closableObject.OnOpen += ClosableObject_OnOpen;
        closableObject.OnClose += ClosableObject_OnClose;
        closableObject.OnLocked += ClosableObject_OnLocked;
    }

    private void ClosableObject_OnLocked(object sender, EventArgs e) {
        animator.SetTrigger(LOCKED);
    }

    private void ClosableObject_OnClose(object sender, EventArgs e) {
        animator.SetBool(IS_OPENED, false);
    }

    private void ClosableObject_OnOpen(object sender, EventArgs e) {
        animator.SetBool(IS_OPENED, true);
    }
}
