public abstract class TrainingTask {
    public bool Played;

    public virtual void Reset() {
        Played = false;
    }
}