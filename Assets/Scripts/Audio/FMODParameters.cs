public static class FMODParameters 
{
    /// <summary>
    /// Global parameter labeled "GameStatus"
    /// </summary>
    public enum GameStatus : int
    {
        LIVE,
        PAUSED
    }

    /// <summary>
    /// Local parameter labeled "MovementStatus"
    /// </summary>
    public enum MovementStatus : int
    {
        IDLE,
        WALKING,
        CROUCHING
    }
    /// <summary>
    /// Local parameter labeled "MentalityStatus"
    /// </summary>
    public enum MentalityStatus : int
    {
        PASSIVE,
        AGGRO
    }
}