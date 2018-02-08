using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

public class Safepoint : Checkpoint 
{
    public override void PushCheckpointManager()
    {
        CheckpointManager.instance.Safepoint(transform, child.transform);
    }
}
