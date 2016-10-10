﻿using UnityEngine;

public class Toolbox : Singleton<Toolbox> {
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    void Awake() {
        // Your initialization code here
    }

    // (optional) allow runtime registration of global objects
    public T RegisterComponent<T>() where T : Component {
        return Instance.GetOrAddComponent<T>();
    }
}
