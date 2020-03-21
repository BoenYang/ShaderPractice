using System.Collections.Generic;
using NetworkProtocal;

public class MobaData
{
    public static List<PlayerInfo> RoomPlayerInfos;

    public static uint MyId;

    public static List<PlayerGameInfo> PlayerGameInfos;

    public static PlayerInfo GetPlayerInfoById(uint id)
    {
        for (int i = 0; i < RoomPlayerInfos.Count; i++)
        {
            if (RoomPlayerInfos[i].id == id)
            {
                return RoomPlayerInfos[i];
            }
        }

        return null;
    }

    public static PlayerInfo GetMyPlayerInfo()
    {
        return MobaData.GetPlayerInfoById(MobaData.MyId);
    }
}
