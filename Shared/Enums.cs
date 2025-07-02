namespace Shared
{
    public enum MessageType
    {
        Connect,
        Start,
        Turn,
        Shot,
        Result,
        Chat,
        RestartRequest,
        RestartConfirm,
        GameOver,
        Disconnected,
        Error,
        Waiting,
        Unknown
    }

    public enum ShotResult
    {
        Miss,
        Hit,
        Kill
    }

    public enum TurnStatus
    {
        Your,
        Wait
    }

    public enum GameStatus
    {
        NotStarted,
        InProgress,
        Ended
    }
}
