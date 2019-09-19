using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PS4;

public class PS4MatchmakingHelper : MonoBehaviour
{
#if UNITY_PS4
    string _roomId = "";
    Sony.NP.Matching.Room _currentRoom = null;
    PS4Photon _photonHelper;
    int _maxPlayers = 4;
    MatchState _state = MatchState.Ready;
    PS4Invitation _invite;
    PS4MatchmakingHelper _matchmaking;
    
    static PS4MatchmakingHelper _inst;

    static public PS4MatchmakingHelper Inst { get { return _inst; } }

    public bool InvitePending
    {
        get
        {
            if (null != _invite && (_invite.AcceptingInvite || _invite.PlayTogetherInvitation != null || _invite.CurrentInvitation != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // Flag for when you shoot the portal, used to block invites and multiple joins happening at the same time
    public bool JoiningRandomRoom
    {
        get;
        set;
    }

    public bool CanJoinGame
    {
        get
        {
            return _state == MatchState.Ready;
        }
    }

    enum MatchState
    {
        Ready,
        JoinOrCreateGame,
        JoinGame
    }

    PS4MatchmakingHelper()
    {
        JoiningRandomRoom = false;
    }

    // Use this for initialization
    void Start () {
        if (null != _inst)
            throw new Exception("Tried to create two instances of PS4MatchmakingHelper!");
        _inst = this;
        _photonHelper = GameObject.FindObjectOfType<PS4Photon>();
        _invite = GameObject.FindObjectOfType<PS4Invitation>();
        _matchmaking = GameObject.FindObjectOfType<PS4MatchmakingHelper>();
    }

    public void Update()
    {
		/*
        if (Input.GetButtonDown("Cross"))
        {
            PS4Matchmaking.Inst.SendInvite();
        }
		*/

        if (null != _invite && !_invite.AcceptingInvite && !JoiningRandomRoom)
        {
            if(null!=_invite.PlayTogetherInvitation) // Join from PlayTogether
            {
                var invitation = _invite.PlayTogetherInvitation;
                _invite.ClearInvitation();
                _invite.AcceptingInvite = true;
                if (!PS4Plus.Inst.IsChecking)
                {
                    PS4Plus.Inst.CheckAvailabilityAll((availOk) =>
                    {
                        if (availOk)
                        {
                            PS4Plus.Inst.CheckPlus((ok) =>
                            {
                                if (ok)
                                {
                                    GameState.LeaveMultiplayer( ()=>
                                    {
                                        PS4Photon.instance.DoConnect(
                                        (didConnect) =>
                                        {
                                            if (didConnect)
                                            {
                                                GameState.CreateRoomPS4(true,
                                                    (createdOk) =>
                                                    {
                                                        _invite.AcceptingInvite = false;
                                                        if (createdOk)
                                                        {
                                                            PS4Invitation.DumpPlayTogetherDetails(invitation);
                                                            Sony.NP.Core.NpAccountId[] recipients = new Sony.NP.Core.NpAccountId[invitation.Invitees.Length];
                                                            for (int i = 0; i < invitation.Invitees.Length; ++i)
                                                                recipients[i] = invitation.Invitees[i].AccountId;
                                                            PS4Matchmaking.Inst.SendInvites(recipients);
                                                        }
                                                    });
                                            }
                                            else
                                            {
                                                _invite.AcceptingInvite = false;
                                            }
                                        });
                                    });
                                }
                                else
                                {
                                    PS4Plus.Inst.DisplayJoinPlusDialog();
                                    _invite.AcceptingInvite = false;
                                }
                            }); // Will drop silently if in invalid state
                        }
                        else
                        {
                            _invite.AcceptingInvite = false;
                        }
                    });
                }
                else
                {
                    _invite.AcceptingInvite = false;
                }
            }
            else if (null != _invite.CurrentInvitation) // Join from invite
            {
                var invitation = _invite.CurrentInvitation;
                _invite.AcceptingInvite = true;
                _invite.ClearInvitation();                
                if (!PS4Plus.Inst.IsChecking)
                {
                    PS4Plus.Inst.CheckAvailabilityAll((availOk) =>
                    {
                        if (availOk)
                        {
                            PS4Plus.Inst.CheckPlus((ok) =>
                                {
                                    GameState.LeaveMultiplayer(() =>
                                    {
                                        if (ok)
                                        {
                                            PS4Photon.instance.DoConnect(
                                                (didConnect) =>
                                                {
                                                    if (didConnect)
                                                    {
                                                        GameState.AcceptInvite(invitation);
                                                    }
                                                    _invite.AcceptingInvite = false;
                                                });
                                        }
                                        else
                                        {
                                            PS4Plus.Inst.DisplayJoinPlusDialog();
                                            _invite.AcceptingInvite = false;
                                        }
                                    });
                                }); // Will drop silently if in invalid state
                        }
                        else
                        {
                            _invite.AcceptingInvite = false; // Not available, fail
                        }
                    });
                }
            }
        }
    }

    public void CreateRoom(Action<bool> onComplete, bool isPrivate)
    {
        Sony.NP.Matching.Attribute[] attributes = new Sony.NP.Matching.Attribute[1];
        _roomId = System.Guid.NewGuid().ToString();
        byte[] roomIdBin = System.Text.Encoding.ASCII.GetBytes(_roomId);
        attributes[0] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(PS4Matchmaking.Inst.FindAttributeMetaData("ROOM_ID"), roomIdBin);

        var req = PS4Matchmaking.Inst.CreateRoomRequest(attributes, isPrivate);
        PS4Matchmaking.Inst.CreateRoom(req, (response) => { OnCreateRoom(response, onComplete); });
    }

    Action<bool> _onPUNCreateRoomComplete;
    void OnCreateRoom(Sony.NP.Matching.RoomResponse response, Action<bool> onComplete)
    {
        OnScreenLog.Add("OnCreateRoom");
        if (response != null && response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
        {
            _currentRoom = response.Room;            
            bool ok = _photonHelper.CreatePrivate("", _maxPlayers, _roomId);
            OnScreenLog.Add(string.Format("Photon CreatePrivate {0} {1}", _roomId, ok ? "success" : "fail"));
            _onPUNCreateRoomComplete = onComplete;
        }
        else
        {
            _state = MatchState.Ready;
            onComplete(false);
            _onPUNCreateRoomComplete = null;
        }
    }

    // Photon create room
    public void OnCreatedRoom()
    {
        OnScreenLog.Add("OnCreatedRoom");
        _state = MatchState.Ready;
        if (null!=_onPUNCreateRoomComplete)
        {
            var cb = _onPUNCreateRoomComplete;
            _onPUNCreateRoomComplete = null;
            cb(true);
        }
    }

    // Photon create room
    public void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        OnScreenLog.Add("OnPhotonCreateRoomFailed");
        _state = MatchState.Ready;
        if (null != _onPUNCreateRoomComplete)
        {
            var cb = _onPUNCreateRoomComplete;
            _onPUNCreateRoomComplete = null;
            cb(false);
        }
    }

    public void JoinRandomOrCreate(Action<bool> onComplete)
    {
        if (_state == MatchState.Ready)
        {
            _state = MatchState.JoinOrCreateGame;
            PS4Matchmaking.Inst.SearchRooms((r) => { OnJoinRandomSearchComplete(r, onComplete); });
        }
    }

    void OnJoinRandomSearchComplete(Sony.NP.Matching.RoomsResponse response, Action<bool> onComplete)
    {
        Debug.Log("OnSearchRoomsComplete " + response.ReturnCode);
        if (response != null && response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
        {
            var room = PickBestRoom(response.Rooms);
            if (null != room)
            {
                Debug.Log("JoinRoom" + room);
                JoinPSNRoom(room, onComplete);
            }
            else
            {
                Debug.Log("CreateRoom");
                CreateRoom(onComplete, false);
            }
        }
    }

    void OnSearchRoomsComplete(Sony.NP.Matching.RoomsResponse response)
    {
        OnScreenLog.Add("OnSearchRoomsComplete " + response.ReturnCode);
        if (response != null && response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
        {
            var room = PickBestRoom(response.Rooms);
            if (null != room)
                JoinPSNRoom(room);
            else
                OnScreenLog.Add("No rooms found");
        }
    }

    Sony.NP.Matching.Room PickBestRoom(Sony.NP.Matching.Room[] r)
    {
        var validRooms = new List<Sony.NP.Matching.Room>();
        for(int i=0;i<r.Length;++i)
        {
            var room = r[i];            
            if (room.CurrentMembers.Length < (int)room.NumMaxMembers)
            {
                validRooms.Add(room);
            }
        }
        if (validRooms.Count > 0)
        {
            validRooms.Shuffle();
            return validRooms[0];
        }
        else
        {
            return null;
        }
    }

    // From shooting the join teleporter in the lobby
    void JoinPSNRoom(Sony.NP.Matching.Room room, Action<bool> onJoin = null)
    {
        if (_state == MatchState.JoinOrCreateGame)
        {
            _state = MatchState.JoinGame;
            PS4Matchmaking.Inst.JoinRoom(room, (response) => { OnJoinPSNRoom(response, onJoin); });
        }
        else
        {
            Debug.Log("JoinRoom failed, not in ready state " + _state ); // Note callback is not called as this has probably happened because there is already a request in flight
        }
    }

    // From invite
    public void JoinPSNRoom(Sony.NP.Matching.NpSessionId session, Action<bool> onJoin)
    {
        if (_state == MatchState.Ready)
        {
            _state = MatchState.JoinGame;
            PS4Matchmaking.Inst.JoinRoom(session, (response) => { OnJoinPSNRoom(response, onJoin); });
        }
        else
        {
            Debug.Log("JoinRoom failed, not in ready state " + _state); // Note callback is not called as this has probably happened because there is already a request in flight
        }
    }

    public static string UnsafeAsciiBytesToString(string buffer)
    {
        for (int i = 0; i < buffer.Length; ++i)
        {
            if(buffer[i]==0)
            {
                return buffer.Substring(0, i);
            }
        }
        return buffer;
    }

    Action<bool> _onPhotonJoined;

    public void OnJoinPSNRoom(Sony.NP.Matching.RoomResponse response, Action<bool> onJoin)
    {
        bool error = false;
        if (response != null && response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
        {
            var atts = response.Room.Attributes;
            string roomName = "";
            for (int i = 0;i<atts.Length;++i)
            {
                var attrib = atts[i];
                if(attrib.Metadata.Name == "ROOM_ID")
                {
                    roomName = System.Text.Encoding.ASCII.GetString(attrib.BinValue);
                    roomName = UnsafeAsciiBytesToString(roomName);
                }
            }
            Debug.LogFormat("JoinPhotonRoom({0})", roomName);
            if (PhotonNetwork.JoinRoom(roomName))
            {
                _onPhotonJoined = onJoin;
                _state = MatchState.Ready;                
            }
            else
            {
                Debug.LogFormat("PhotonNetwork.JoinRoom failed");
                error = true;
            }
        }
        else
        {
            Debug.LogFormat("OnJoinPSNRoom failed, invalid response");
            error = true;
        }

        if(error)
        {
            Debug.LogFormat("JoinPhotonRoom FAILED");            
            LeaveRoom();
            _onPhotonJoined = null;
            _state = MatchState.Ready;
            if (null != onJoin)   
                onJoin(false);
        }
    }

    // Photon joined room
    public void OnJoinedRoom()
    {
        Debug.LogFormat("Photon - OnJoinedRoom() {0} other players", PhotonNetwork.otherPlayers.Length);
        PS4Plus.Inst.IsMultiplePlayersActive = PhotonNetwork.otherPlayers.Length >= 1;
        _state = MatchState.Ready;
        if (null!=_onPhotonJoined)
        {
            var cb = _onPhotonJoined;
            _onPhotonJoined = null;
            cb(true);
        }
    }

    public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Photon - OnPhotonJoinRoomFailed()");
        _state = MatchState.Ready;
        if (null != _onPhotonJoined)
        {
            var cb = _onPhotonJoined;
            _onPhotonJoined = null;
            cb(false);
        }
    }

    public void Invite()
    {
        PS4Matchmaking.Inst.SendInvite();
    }

    public void ListPSNRooms()
    {
        PS4Matchmaking.Inst.SearchRooms(null);
    }

    public void ListPhotonRooms()
    {
        var rooms = PhotonNetwork.GetRoomList();
        OnScreenLog.Add("Dump Photon rooms");
        for (int i = 0; i < rooms.Length; ++i)
        {
            var roomInfo = rooms[i];
            OnScreenLog.Add(roomInfo.Name);
            //PhotonNetwork.JoinRoom(roomInfo.Name);
        }
    }

    public void LeaveRoom()
    {
        PS4Matchmaking.Inst.LeaveRoom();
        PhotonNetwork.LeaveRoom();
    }

    public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.LogFormat("OnPhotonPlayerConnected players {0}", PhotonNetwork.otherPlayers.Length);
        PS4Plus.Inst.IsMultiplePlayersActive = PhotonNetwork.otherPlayers.Length >= 1;
    }

    public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.LogFormat("OnPhotonPlayerDisconnected players {0}", PhotonNetwork.otherPlayers.Length);
        PS4Plus.Inst.IsMultiplePlayersActive = PhotonNetwork.otherPlayers.Length >= 1;
    }
#endif
}