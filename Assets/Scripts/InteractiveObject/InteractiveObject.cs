using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour {
    // вообще по идее лучше будет сделать массив и чтобы скрипт был приереплен к главному префабу.
    // тогда не нужно было бы в каждый вариант префаба передавать SO?
    // не уверен что такой подход верный... с другой стороны, почему нет?

    public abstract void Interact(Player player);

    public abstract InteractiveObjectSO GetObjectSO();
}
