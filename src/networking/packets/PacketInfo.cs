namespace StepmaniaServer {
    // commands for Client -> Server
    public enum SMClientCommand {
        Ping = 0,
        PingR = 1,
        Hello = 2,
        GameStartRequest = 3,
        GameOverNotice = 4,
        GameStatusUpdate = 5,
        StyleUpdate = 6,
        ChatMessage = 7,
        RequestStartGame = 8,
        // Reserved = 9,
        ScreenChanged = 10,
        PlayerOptions = 11,
        SMOnlinePacket = 12,
        // Reserved = 13,
        // Reserved = 14,
        XMLPacket = 15
    }
    
    // Commands for Server -> Client
    public enum SMServerCommand {
        Ping = 128,
        PingR = 129,
        Hello = 130,
        AllowGameStart = 131,
        GameOverStatus = 132,
        ScoreboardUpdate = 133,
        SystemMessage = 134,
        ChatMessage = 135,
        RequestStartGame = 136,
        UpdateUserList = 137,
        SelectMusicScreen = 138,
        // Reserved = 139,
        SMOnlinePacket = 140,
        ServerInformation = 141,
        AttackClient = 142,
        XMLPacket = 143
    }

    // SMO Commands for Client -> Server
    public enum SMOClientCommand {
        Login = 0,
        EnterRoom = 1,
        CreateRoom = 2,
        RoomInfo = 3
    }

    // SMO Commands for Server -> Client
    public enum SMOServerCommand {
        Login = 0,
        RoomUpdate = 1,
        GeneralInfo = 2,
        RoomInfo = 3
    }

    // Net Screens used in ScreenChanged Packet
    public enum SMScreen {
        ExitedScreenNetSelectMusic = 0,
        EnteredScreenNetSelectMusic = 1,
        NotSent = 2,
        EnteredOptionsScreen = 3,
        ExitedEvaluationScreen = 4,
        EnteredEvaluationScreen = 5,
        ExitedScreenNetRoom = 6,
        EnteredScreenNetRoom = 7
    }
}
