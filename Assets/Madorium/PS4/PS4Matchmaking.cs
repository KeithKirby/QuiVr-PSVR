using Sony.NP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PS4;
//using UnityEngine.PS4;

[RequireComponent(typeof(PS4Common))]
public class PS4Matchmaking : MonoBehaviour
{
#if UNITY_PS4
    // Callbacks for Create room
    RequestCallbacks<Sony.NP.Matching.RoomsResponse> _searchRoomRequests = new RequestCallbacks<Sony.NP.Matching.RoomsResponse>();
    RequestCallbacks<Sony.NP.Matching.RoomResponse> _createRoomReq = new RequestCallbacks<Sony.NP.Matching.RoomResponse>();
    RequestCallbacks<Sony.NP.Matching.RoomResponse> _joinRoomReq = new RequestCallbacks<Sony.NP.Matching.RoomResponse>();

    Sony.NP.Matching.SessionImage sessionImage = new Sony.NP.Matching.SessionImage();
    Sony.NP.Matching.Room currentRoom = null;
    Sony.NP.Matching.AttributeMetadata[] attributesMetadata;
    int nextMaxBoost = 10;

    static PS4Matchmaking _inst;
    bool matchingInitialized = false;
    PS4Common _ps4Common;

    public static PS4Matchmaking Inst { get { return _inst; } }

    // Use this for initialization
    IEnumerator Start()
    {
        if (_inst == null)
        {
            _inst = this;
            if (!PS4NpInit.Initialised)
                yield return new WaitForEndOfFrame();            

            CreateAttributeMetaData();
            sessionImage.SessionImgPath = Application.streamingAssetsPath + "/PS4SessionImage.jpg";
            PS4Matchmaking.Inst.SetInitConfiguration(true);
        }
    }

    public void CreateAttributeMetaData()
    {
        attributesMetadata = new Sony.NP.Matching.AttributeMetadata[1];
        attributesMetadata[0] = Sony.NP.Matching.AttributeMetadata.CreateBinaryAttribute("ROOM_ID", Sony.NP.Matching.AttributeScope.Room, Sony.NP.Matching.RoomAttributeVisibility.Search, 64);
    }

    public Sony.NP.Matching.AttributeMetadata FindAttributeMetaData(string name)
    {
        for (int i = 0; i < attributesMetadata.Length; i++)
        {
            if (attributesMetadata[i].Name == name)
            {
                return attributesMetadata[i];
            }
        }

        OnScreenLog.AddError("FindAttributeMetaData : Can't find attribute metadata with name : " + name);
        return new Sony.NP.Matching.AttributeMetadata();
    }

    public Sony.NP.Matching.Attribute[] CreateDefaultAttributeValues()
    {
        string testBinary = "";
        byte[] data = System.Text.Encoding.ASCII.GetBytes(testBinary);

        //Sony.NP.Matching.Attribute[] attributes = new Sony.NP.Matching.Attribute[1];
        //attributes[0] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("NPC_NAME"), data);     

        Sony.NP.Matching.Attribute[] attributes = new Sony.NP.Matching.Attribute[1];

        // Internal Room attributes
        //attributes[0] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("HOST_NAME"), 3);
        attributes[0] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("ROOM_ID"), data);

        // Member attributes
        //attributes[0] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("CURRENT_STATE"), data);
        //attributes[1] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("MAX_BOOSTS"), nextMaxBoost);
        //attributes[2] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("MAX_TEAMS"), 3);

        // Room search attributes
        //attributes[3] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("NPC_NAME"), data);
        //attributes[4] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("TRACK_ID"), 1);
        //attributes[5] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("COUNTRY_ID"), 2);
        //attributes[6] = Sony.NP.Matching.Attribute.CreateIntegerAttribute(FindAttributeMetaData("SEASON_ID"), 3);



        // External Room attributes
        //attributes[8] = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("SPECIAL_MESSAGE"), data);

        nextMaxBoost++;

        return attributes;
    }
    
    private void OutputCreateRoom(Sony.NP.Matching.RoomResponse response)
    {
        if (response == null) return;

        OnScreenLog.Add("CreateRoom Response");

        if (response.Locked == false)
        {
            if (response.ReturnCode == Sony.NP.Core.ReturnCodes.ERROR_MATCHING_USER_IS_ALREADY_IN_A_ROOM)
            {
                OnScreenLog.Add("CreateRoom Response : User already in a room.");
            }
            else if (response.ReturnCode == Sony.NP.Core.ReturnCodes.NP_MATCHING2_ERROR_CONTEXT_NOT_STARTED)
            {
                OnScreenLog.Add("CreateRoom Response : Content not started. Probably another user has already use matching methods.");
            }
            else
            {
                OutputRoom(response.Room);
            }
        }
    }

    private void OutputRoom(Sony.NP.Matching.Room room)
    {
        OnScreenLog.Add("MatchingContext : " + room.MatchingContext);
        OnScreenLog.Add("ServerId : " + room.ServerId);
        OnScreenLog.Add("WorldId : " + room.WorldId);
        OnScreenLog.Add("RoomId : " + room.RoomId);

        Sony.NP.Matching.Attribute[] attributes = room.Attributes;
        OnScreenLog.Add("Num Attributes : " + attributes.Length);

        for (int i = 0; i < attributes.Length; i++)
        {
            //string output = "     " + BuildAttributeString(attributes[i]);
            //OnScreenLog.Add(output);
        }

        OnScreenLog.Add("Name : " + room.Name);

        Sony.NP.Matching.Member[] members = room.CurrentMembers;

        OnScreenLog.Add("Num Memebers : " + members.Length);

        for (int i = 0; i < members.Length; i++)
        {
            string output = "     Member : " + members[i].OnlineUser;

            output += "\n          Attributes = : ";

            for (int a = 0; a < members[i].MemberAttributes.Length; a++)
            {
                //output += "\n               " + BuildAttributeString(members[i].MemberAttributes[a]);
                output += "\n";
            }

            output += "\n          JoinedDate : " + members[i].JoinedDate;

            //output += "\n          SignalingInformation : " + BuildSignalingInformationString(members[i].SignalingInformation);

            output += "\n          Platform : " + members[i].Platform;
            output += "\n          RoomMemberId : " + members[i].RoomMemberId;
            output += "\n          IsOwner : " + members[i].IsOwner;
            output += "\n          IsMe : " + members[i].IsMe;

            OnScreenLog.Add(output, true);
        }

        OnScreenLog.Add("NumMaxMembers : " + room.NumMaxMembers);
        OnScreenLog.Add("Topology : " + room.Topology);
        OnScreenLog.Add("NumReservedSlots : " + room.NumReservedSlots);

        OnScreenLog.Add("IsNatRestricted : " + room.IsNatRestricted);
        OnScreenLog.Add("AllowBlockedUsersOfOwner : " + room.AllowBlockedUsersOfOwner);
        OnScreenLog.Add("AllowBlockedUsersOfMembers : " + room.AllowBlockedUsersOfMembers);
        OnScreenLog.Add("JoinAllLocalUsers : " + room.JoinAllLocalUsers);

        OnScreenLog.Add("OwnershipMigration : " + room.OwnershipMigration);
        OnScreenLog.Add("Visibility : " + room.Visibility);

        OnScreenLog.Add("Password : " + room.Password);
        OnScreenLog.Add("BoundSessionId : " + room.BoundSessionId);

        OnScreenLog.Add("IsSystemJoinable : " + room.IsSystemJoinable);
        OnScreenLog.Add("DisplayOnSystem : " + room.DisplayOnSystem);
        OnScreenLog.Add("HasChangeableData : " + room.HasChangeableData);
        OnScreenLog.Add("HasFixedData : " + room.HasFixedData);
        OnScreenLog.Add("IsCrossplatform : " + room.IsCrossplatform);
        OnScreenLog.Add("IsClosed : " + room.IsClosed);
    }

    public Sony.NP.Matching.CreateRoomRequest CreateRoomRequest(Sony.NP.Matching.Attribute[] attributes, bool isPrivate)
    {
        Sony.NP.Matching.CreateRoomRequest request = new Sony.NP.Matching.CreateRoomRequest();
        request.UserId = PS4Common.InitialUserId;

        //Sony.NP.Matching.Attribute[] attributes = CreateDefaultAttributeValues();

        request.Attributes = attributes;

        request.Name = "Room : Created Frame " + OnScreenLog.FrameCount;
        request.Status = "Room was created on frame " + OnScreenLog.FrameCount;

        Sony.NP.Matching.LocalizedSessionInfo[] localisedInfo = new Sony.NP.Matching.LocalizedSessionInfo[2];
        localisedInfo[0] = new Sony.NP.Matching.LocalizedSessionInfo("German session text", "German text session created on frame " + OnScreenLog.FrameCount, "de");
        localisedInfo[1] = new Sony.NP.Matching.LocalizedSessionInfo("French session text", "French text session created on frame " + OnScreenLog.FrameCount, "fr");

        request.MaxNumMembers = 4;
        request.OwnershipMigration = Sony.NP.Matching.RoomMigrationType.OwnerBind; // OwnerMigration means session will be deleted when joined users reaches 0
        request.Topology = Sony.NP.Matching.TopologyType.Mesh;
        request.Visibility = isPrivate ? Sony.NP.Matching.RoomVisibility.PrivateRoom : Sony.NP.Matching.RoomVisibility.PublicRoom;
        request.WorldNumber = 1;
        request.JoinAllLocalUsers = true;

        request.Image = sessionImage;

        request.FixedData = System.Text.Encoding.ASCII.GetBytes("Fixed room data test");
        request.ChangeableData = System.Text.Encoding.ASCII.GetBytes("Changeable room data test setup on frame " + OnScreenLog.FrameCount);

        return request;
    }

    public void CreateRoom(Sony.NP.Matching.CreateRoomRequest request, Action<Sony.NP.Matching.RoomResponse> callback)
    {
        if (!matchingInitialized)
        {
            Debug.Log("Cannot create room, not initialised yet.");
            return;
        }
        try
        {
            request.JoinAllLocalUsers = true;

            Sony.NP.Matching.RoomResponse response = new Sony.NP.Matching.RoomResponse();

            _createRoomReq.Add(callback, response);

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.CreateRoom(request, response);
            OnScreenLog.Add("CreateRoom Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void SearchRooms(Action<Sony.NP.Matching.RoomsResponse> callback)
    {
        try
        {
            Sony.NP.Matching.SearchRoomsRequest request = new Sony.NP.Matching.SearchRoomsRequest();
            request.UserId = PS4Common.InitialUserId;
            Sony.NP.Matching.RoomsResponse response = new Sony.NP.Matching.RoomsResponse();
            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.SearchRooms(request, response);
            OnScreenLog.Add("SearchRooms Async : Request Id = " + requestId);
            _searchRoomRequests.Add(callback, response);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void SearchRooms(Action<Sony.NP.Matching.RoomsResponse> callback, string roomName)
    {
        try
        {
            byte[] roomIdBin = System.Text.Encoding.ASCII.GetBytes(roomName);

            Sony.NP.Matching.SearchClause[] clauses = new Sony.NP.Matching.SearchClause[1];
            clauses[0].AttributeToCompare = Sony.NP.Matching.Attribute.CreateBinaryAttribute(FindAttributeMetaData("ROOM_ID"), roomIdBin);
            clauses[0].OperatorType = Matching.SearchOperatorTypes.Equals;

            Sony.NP.Matching.SearchRoomsRequest request = new Sony.NP.Matching.SearchRoomsRequest();
            request.UserId = PS4Common.InitialUserId;

            Sony.NP.Matching.RoomsResponse response = new Sony.NP.Matching.RoomsResponse();
            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.SearchRooms(request, response);
            OnScreenLog.Add("SearchRooms Async : Request Id = " + requestId);
            _searchRoomRequests.Add(callback, response);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void LeaveRoom()
    {
        if (null == currentRoom)
        {
            Debug.Log("LeaveRoom failed, not in room");
        }
        else
        {
            try
            {
                Sony.NP.Matching.LeaveRoomRequest request = new Sony.NP.Matching.LeaveRoomRequest();
                request.UserId = PS4Common.InitialUserId;
                request.RoomId = currentRoom.RoomId;

                Sony.NP.Matching.PresenceOptionData optiondata = new Sony.NP.Matching.PresenceOptionData();
                optiondata.DataAsString = "I'm out of here.";
                request.NotificationDataToMembers = optiondata;

                Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

                // Make the async call which will return the Request Id 
                int requestId = Sony.NP.Matching.LeaveRoom(request, response);
                OnScreenLog.Add("LeaveRoom Async : Request Id = " + requestId);
            }
            catch (Sony.NP.NpToolkitException e)
            {
                OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
            }
        }
    }

    public bool SendInvite()
    {
        if(!matchingInitialized)
        {
            Debug.Log("Cannot send invite, not initialised yet.");
            return false;
        }

        if(null == currentRoom)
        {
            Debug.Log("Could not SendInvite not in a room.");
            return false;
        }

        try
        {
            Sony.NP.Matching.SendInvitationRequest request = new Sony.NP.Matching.SendInvitationRequest();
            request.UserId = PS4Common.InitialUserId;

            request.RoomId = currentRoom.RoomId;
            //request.UserMessage = "Do you want to join a room in the NpToolkit2 sample?";
            request.UserMessage = I2.Loc.ScriptLocalization.Get("WorldUI/PS4InviteUserMessage");
            request.MaxNumberRecipientsToAdd = 1;
            request.RecipientsEditableByUser = true;
            request.EnableDialog = true;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.SendInvitation(request, response);
            OnScreenLog.Add("SendInvitation Async : Request Id = " + requestId);
            return true;
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
        return false;
    }

    public bool SendInvites(Sony.NP.Core.NpAccountId[] recipients)
    {
        if (!matchingInitialized)
        {
            Debug.Log("Cannot send invite, not initialised yet.");
            return false;
        }

        if (null == currentRoom)
        {
            Debug.Log("Could not SendInvite not in a room.");
            return false;
        }

        try
        {
            Sony.NP.Matching.SendInvitationRequest request = new Sony.NP.Matching.SendInvitationRequest();
            request.UserId = PS4Common.InitialUserId;

            request.RoomId = currentRoom.RoomId;
            //request.UserMessage = "Do you want to join a room in the NpToolkit2 sample?";
            request.UserMessage = I2.Loc.ScriptLocalization.Get("WorldUI/PS4InviteUserMessage");            
            request.RecipientsEditableByUser = false;
            request.Recipients = recipients;
            request.EnableDialog = false;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.SendInvitation(request, response);
            OnScreenLog.Add("SendInvitation Async : Request Id = " + requestId);
            return true;
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
        return false;
    }

    public void JoinRoom(Sony.NP.Matching.Room room, Action<Sony.NP.Matching.RoomResponse> complete)
    {
        try
        {
            Sony.NP.Matching.JoinRoomRequest request = new Sony.NP.Matching.JoinRoomRequest();
            request.UserId = PS4Common.InitialUserId;

            request.IdentifyRoomBy = Sony.NP.Matching.RoomJoiningType.Room;
            request.RoomId = room.RoomId;
            request.JoinAllLocalUsers = true;

            Sony.NP.Matching.RoomResponse response = new Sony.NP.Matching.RoomResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.JoinRoom(request, response);
            _joinRoomReq.Add(complete, response);
            OnScreenLog.Add("JoinRoom Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void JoinRoom(Sony.NP.Matching.NpSessionId sessionId, Action<Sony.NP.Matching.RoomResponse> complete)
    {
        try
        {
            Sony.NP.Matching.JoinRoomRequest request = new Sony.NP.Matching.JoinRoomRequest();
            request.UserId = PS4Common.InitialUserId;

            request.IdentifyRoomBy = Sony.NP.Matching.RoomJoiningType.BoundSessionId;
            request.BoundSessionId = sessionId;
            request.JoinAllLocalUsers = true;

            Sony.NP.Matching.RoomResponse response = new Sony.NP.Matching.RoomResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.JoinRoom(request, response);
            _joinRoomReq.Add(complete, response);
            OnScreenLog.Add("JoinRoom Async : Request Id = " + requestId);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void SetInitConfiguration(bool async)
    {
        try
        {
            Debug.Log("SetInitConfig");
            Sony.NP.Matching.SetInitConfigurationRequest request = new Sony.NP.Matching.SetInitConfigurationRequest();

            request.UserId = PS4Common.InitialUserId;
            request.Attributes = attributesMetadata;

            request.Async = async;

            Sony.NP.Core.EmptyResponse response = new Sony.NP.Core.EmptyResponse();

            // Make the async call which will return the Request Id 
            int requestId = Sony.NP.Matching.SetInitConfiguration(request, response);
            if (async == true)
            {
                OnScreenLog.Add("SetInitConfiguration Async : Request Id = " + requestId);
            }
            else
            {
                if (response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS) // Success
                {
                    matchingInitialized = true;
                }
            }
        }
        catch (Sony.NP.NpToolkitException e)
        {
            OnScreenLog.AddError("Exception : " + e.ExtendedMessage);
        }
    }

    public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Matching)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.MatchingSetInitConfiguration:

                    if (callbackEvent.Response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS) // Success
                    {
                        matchingInitialized = true;
                    }

                    //OutputSetInitConfiguration(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingGetWorlds:
                    //OutputGetWorlds(callbackEvent.Response as Sony.NP.Matching.WorldsResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingCreateRoom:
                    {
                        Sony.NP.Matching.RoomResponse cr = callbackEvent.Response as Sony.NP.Matching.RoomResponse;
                        if (cr != null && cr.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
                            currentRoom = cr.Room;
                        OutputCreateRoom(callbackEvent.Response as Sony.NP.Matching.RoomResponse);
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _createRoomReq.Complete(cr));                        
                    }
                    break;
                case Sony.NP.FunctionTypes.MatchingLeaveRoom:
                    if (callbackEvent.Response.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
                    {
                        currentRoom = null;
                        //foundRoom = null; // Just in-case the found room is the same as the one that was left. 
                    }
                    //OutputLeaveRoom(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingSearchRooms:
                    {
                        Sony.NP.Matching.RoomsResponse cr = callbackEvent.Response as Sony.NP.Matching.RoomsResponse;
                        if (cr != null && cr.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
                        {
                            if (cr.Rooms != null && cr.Rooms.Length > 0)
                            {
                                //foundRoom = cr.Rooms[0];
                            }
                        }

                        _searchRoomRequests.Complete(cr);

                        OutputSearchRooms(callbackEvent.Response as Sony.NP.Matching.RoomsResponse);
                    }
                    break;
                case Sony.NP.FunctionTypes.MatchingJoinRoom:
                    {
                        Sony.NP.Matching.RoomResponse cr = callbackEvent.Response as Sony.NP.Matching.RoomResponse;
                        if (cr != null && cr.ReturnCode == Sony.NP.Core.ReturnCodes.SUCCESS)
                        {
                            currentRoom = cr.Room;
                        }

                        UnityMainThreadDispatcher.Instance().Enqueue( ()=> { _joinRoomReq.Complete(cr); } );
                        //OutputJoinRoom(callbackEvent.Response as Sony.NP.Matching.RoomResponse);
                    }
                    break;
                case Sony.NP.FunctionTypes.MatchingGetRoomPingTime:
                    //OutputGetRoomPingTime(callbackEvent.Response as Sony.NP.Matching.GetRoomPingTimeResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingSendRoomMessage:
                    //OutputSendRoomMessage(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingGetAttributes:
                    //OutputGetAttributes(callbackEvent.Response as Sony.NP.Matching.RefreshRoomResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingGetData:
                    //OutputGetData(callbackEvent.Response as Sony.NP.Matching.GetDataResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingSendInvitation:
                    //OutputSendInvitation(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingSetRoomInfo:
                    //OutputSetRoomInfo(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                case Sony.NP.FunctionTypes.MatchingSetMembersAsRecentlyMet:
                    //OutputSetMembersAsRecentlyMet(callbackEvent.Response as Sony.NP.Core.EmptyResponse);
                    break;
                default:
                    break;
            }
        }

        // Notifications
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Notification)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.NotificationSessionInvitationEvent:
                    //HandleSessionInvitationEvent(callbackEvent.Response as Sony.NP.Matching.SessionInvitationEventResponse);
                    break;
                case Sony.NP.FunctionTypes.NotificationPlayTogetherHostEvent:
                    //HandlePlayTogetherHostEvent(callbackEvent.Response as Sony.NP.Matching.PlayTogetherHostEventResponse);
                    break;
                case Sony.NP.FunctionTypes.NotificationRefreshRoom:
                    //HandleRefreshRoom(callbackEvent.Response as Sony.NP.Matching.RefreshRoomResponse);
                    break;
                default:
                    break;
            }
        }
    }

    private void OutputSearchRooms(Sony.NP.Matching.RoomsResponse response)
    {
        if (response == null)
            return;
        OnScreenLog.Add("OutputSearchRooms Response");
        if (response.Locked == false)
        {
            Sony.NP.Matching.Room[] rooms = response.Rooms;
            for (int i = 0; i < rooms.Length; i++)
            {
                OutputRoomLite(rooms[i]);
            }
        }
    }

    // single line output for a room
    private void OutputRoomLite(Sony.NP.Matching.Room room)
    {
        string output = "RoomId = " + room.RoomId;
        output += " : Name = " + room.Name;
        Sony.NP.Matching.Member[] members = room.CurrentMembers;
        if (members != null)
        {
            output += " : # Members = " + room.CurrentMembers.Length;

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].IsOwner == true)
                {
                    output += " : Owner = " + members[i].OnlineUser.OnlineID;
                }
            }
        }
        else
        {
            output += " : # Members = 0";
        }
        OnScreenLog.Add(output);
    }
#endif
}