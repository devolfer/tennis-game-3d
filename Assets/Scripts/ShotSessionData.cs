using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotSession", menuName = "Scriptable Object/Training/Session/Shot")]
public class ShotSessionData : TrainingSessionData {
    public float shootInterval;
    public List<ShotTask> shotTaskList;

    public override float GetProgress() {
        int numTasks = shotTaskList.Count;
        if (numTasks == 0) return 1f;
        float sum = 0f;
        
        foreach (ShotTask shotTask in shotTaskList) {
            if (!shotTask.Played) break;
            sum += 1f;
        }

        return sum / numTasks;
    }

    public override void ResetProgress() {
        foreach (ShotTask shotTask in shotTaskList) {
            shotTask.Reset();
        }
    }
}