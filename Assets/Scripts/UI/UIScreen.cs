namespace HoldMyBeer.UI {
    /// <summary>
    /// All screens that can exist inside a UIDocument.
    /// Scene decides which subset is actually wired.
    /// </summary>
    public enum UIScreen {
        MainMenu,
        Settings,
        About,
        HUD,
        Pause,
        Endgame
    }
}