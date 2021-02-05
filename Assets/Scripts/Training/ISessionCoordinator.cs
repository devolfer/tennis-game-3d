using System;

public interface ISessionCoordinator {
    void SetupSession(TrainingSessionData sessionData);

    void StartSession();

    void EndSession();

    void CleanupSession();

    event Action OnSessionComplete;
}