using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoadTrigger : BaseButton {

    public string sceneName;

    protected override void DoStuff()
    {
        LevelManager.instance.ChangeLevel(sceneName);
    }
}
