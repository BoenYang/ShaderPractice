namespace NetworkProtocal
{
    public enum GameCmd
    {
        ErrorResponse = 0,
        EnterRoomRequest = 1,
        EnterRoomResponse = 2,
        EnterRoomBroadcast = 3,

        ReadyRequest = 4,
        ReadyBroadcast = 5,

        EnterSceneBroadcast = 6,

        PlayerInited = 7,
    }
}