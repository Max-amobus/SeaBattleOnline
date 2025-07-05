namespace Shared
{
    public enum MessageType
    {
        Connect,
        Ready,
        Start,
        Turn,
        Shot,
        Result,
        Chat,
        Restart,         
        GameOver,
        Disconnected,
        Error,
        Waiting,
        System,          
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
