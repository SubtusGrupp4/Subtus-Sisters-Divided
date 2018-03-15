using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoadTrigger : BaseButton {

    public int levelIndex;

    protected override void DoStuff()
    {
        GameManager.instance.ChangeLevel(levelIndex);
    }
}
