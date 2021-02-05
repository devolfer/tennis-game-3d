using System;
using UnityEngine;

[Serializable]
public class ShotTask : TrainingTask {
    [SerializeField] private FixedShot ballMachineShot;
    [SerializeField] private ShotType taskShotType;
    [SerializeField] private CourtZone taskCourtZone;

    public FixedShot BallMachineShot => ballMachineShot;
    public ShotType TaskShotType => taskShotType;
    public CourtZone TaskCourtZone => taskCourtZone;

    public bool ShotZoneTaskAccomplished { get; set; }
    public bool ShotTypeTaskAccomplished { get; set; }

    public override void Reset() {
        base.Reset();

        ShotZoneTaskAccomplished = false;
        ShotTypeTaskAccomplished = false;
    }
}