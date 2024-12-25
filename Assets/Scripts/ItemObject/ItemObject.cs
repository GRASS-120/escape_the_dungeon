using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! причина проблемы - у объектов нет коллайдера просто (есть только у пака с данжем)

public class ItemObject : InteractiveObject {
    [SerializeField] private InteractiveObjectSO objectSO;

    private void Update() {
        
    }

    public override InteractiveObjectSO GetObjectSO() {
        Debug.Log(objectSO);
        return objectSO;
    }

    public override void Interact(Player player) {
        Debug.Log("huy");
    }
}
