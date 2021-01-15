public static class PhysicsConstant
{
    public readonly static float GRAVITY = -(9.81f * 2f);
}

public enum ECharacterRace
{
    Footman = 1,
    OrcWarrior = 2
}

public enum ECHaracterAction
{
    Idle = 1,
    Move,
    Attack,
    Jump
}

public static class SocketEventConstant
{
    public readonly static string CONNECT = "connection";
    public readonly static string DISCONNECT = "disconnect";
    
    public readonly static string LOGIN = "login";
    public readonly static string LOGIN_OTHER = "login_other";
    public readonly static string LOGOUT = "logout";

    public readonly static string PLAYER_INPUT_ACTION = "player_input_action";

    public readonly static string REQUEST_AD_DATA = "request_ad_data";
}