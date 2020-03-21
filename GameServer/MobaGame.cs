using System;
using System.Collections.Generic;
using NetworkProtocal;

namespace GameServer
{
    public class MobaGame
    {

        private Dictionary<uint,MobaPlayer> m_PlayerDict;

        private bool m_GameRunning = false;

        private uint m_currentPlayerId;

        private enum MobaGameState
        {
            Init,
            WaitingStart,
            WaitingPlayerInit,
            Playing,
        }

        private MobaGameState m_currentState;


        public uint NewPlayerId()
        {
            m_currentPlayerId++;
            return m_currentPlayerId;
        }

        public bool HasPlayerReady()
        {
            foreach (var p in m_PlayerDict.Values)
            {
                if (p.IsReady)
                {
                    return true;
                }
            }

            return false;
        }

        public MobaGame()
        {
            m_PlayerDict = new Dictionary<uint, MobaPlayer>();
            m_currentPlayerId = 0;
            m_currentState = MobaGameState.Init;
        }

        public MobaPlayer CreatePlayer(ClientSession session)
        {
            if (!m_PlayerDict.ContainsKey(session.Sid))
            {
                MobaPlayer player = new MobaPlayer(session,this.OnPlayerMessage);
                m_PlayerDict.Add(session.Sid, player);
                player.InitGameInfo((uint)this.m_PlayerDict.Count);
                return player;
            }
            return m_PlayerDict[session.Sid];
        }

        private void OnPlayerMessage(MobaPlayer player, NetMessage msg)
        {
            GameCmd cmd = (GameCmd)msg.Header.MessageId;
            switch (cmd)
            {
                case GameCmd.EnterRoomRequest:
                    EnterRoomRequest enterRoomRequest = PBUtils.PBDeserialize<EnterRoomRequest>(msg.Data);
                    player.PlayerInfo = enterRoomRequest.playerInfo;

                    EnterRoomResponse response = new EnterRoomResponse();
                    response.playerInfos = new List<PlayerInfo>(m_PlayerDict.Count);
                    foreach (var p in m_PlayerDict.Values) {
                        response.playerInfos.Add(p.PlayerInfo);
                    }

                    player.Send(GameCmd.EnterRoomResponse,response);

                    EnterRoomBroadcast broadcast = new EnterRoomBroadcast();
                    broadcast.playerInfo = player.PlayerInfo;
                    Broadcast(GameCmd.EnterRoomBroadcast, broadcast, player);

                    m_currentState = MobaGameState.WaitingStart;
                    break;
                case GameCmd.ReadyRequest:
                    ReadyRequest readyRequest = PBUtils.PBDeserialize<ReadyRequest>(msg.Data);
                    player.SetReady(readyRequest.ready);
                    
                    ReadyBroadcast readyBroadcast = new ReadyBroadcast();
                    readyBroadcast.playerId = player.Id;
                    readyBroadcast.ready = readyRequest.ready;
                    
                    Broadcast(GameCmd.ReadyBroadcast,readyBroadcast);
                    break;
            }
        }

        private void Broadcast<T>(GameCmd cmd,T t, MobaPlayer exclude = null)
        {
            foreach (var player in m_PlayerDict.Values) {
                if (exclude != null && player == exclude)
                {
                    continue;
                }
                player.Send(cmd, t);
            }
        }

        public void Update()
        {

            if (m_currentState == MobaGameState.Init)
            {
                return;
            }

            if (m_currentState == MobaGameState.WaitingStart)
            {
                if (this.HasPlayerReady())
                {
                    EnterSceneBroadcast enterSceneBroadcast = new EnterSceneBroadcast();
                    enterSceneBroadcast.playerGameInfos = new List<PlayerGameInfo>();
                    foreach (var p in m_PlayerDict.Values) {
                        enterSceneBroadcast.playerGameInfos.Add(p.GameInfo);
                    }
                    Broadcast(GameCmd.EnterSceneBroadcast,enterSceneBroadcast);
                    m_currentState = MobaGameState.WaitingPlayerInit;
                }
            }

            if (m_currentState == MobaGameState.WaitingPlayerInit)
            {
                return;
            }
        }

    }
}