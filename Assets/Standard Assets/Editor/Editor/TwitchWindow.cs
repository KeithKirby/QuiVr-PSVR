using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TwitchWindow : EditorWindow
{
	string oauth = ""; //Keep this secret - Get it here: http://www.twitchapps.com/tmi/
    string nickName = "user123";
    private string server = "irc.twitch.tv";
    private int port = 6667;

	public class MsgEvent : UnityEngine.Events.UnityEvent<string> { }
    public MsgEvent messageRecievedEvent = new MsgEvent();

    private string buffer = string.Empty;
    private bool stopThreads = true;
    private Queue<string> commandQueue = new Queue<string>();
    private List<string> recievedMsgs = new List<string>();
    private System.Threading.Thread inProc, outProc;
	private System.Net.Sockets.TcpClient sock;

	string msgs = "";
    string chatMsgToSend = "";

	Vector2 ScrollPos;

	[MenuItem ("Window/Twitch Chat")]
    public static void  ShowWindow () 
    {
		EditorWindow.GetWindow(typeof(TwitchWindow));
    }

    void OnGUI () 
    {
        EditorGUIUtility.labelWidth = 80;

        if (stopThreads)
    	{
            oauth = EditorGUILayout.PasswordField("OAuth", oauth);
            nickName = EditorGUILayout.TextField("Username",nickName);
            if (GUILayout.Button ("Connect"))
			{
				stopThreads = false;
       	 		StartIRC();
			}
    	}	
		ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.ExpandHeight(true));
			GUILayout.TextArea(msgs, GUILayout.ExpandHeight(true));
 		EditorGUILayout.EndScrollView();
		//GUILayout.TextArea(msgs, GUILayout.Height(position.height-60));
		if (Event.current.keyCode == KeyCode.Return) {
			if(TrySendMessage(chatMsgToSend))
			{
				chatMsgToSend = "";
			}
 		}
		chatMsgToSend = EditorGUILayout.TextField(chatMsgToSend);
		if(needUpdate)
    	{
			needUpdate = false;
			ScrollPos = new Vector2(0,10000000);
			OnGUI();
    	}
    }

    bool TrySendMessage(string val)
    {
    	if(val.Length > 0)
    	{
            if(val == "/clear")
            {
                msgs = "";
                return true;
            }
    		HandleMessage(nickName + ":" + val);
    		SendMsg(val);
    		return true;
    	}
    	return false;
    }

    void Update()
    {
        if(needUpdate)
        {
        	Repaint();
        }
    }

    void OnDestroy()
    {
		stopThreads = true;
    }

    void HandleMessage(string msg)
    {
		ScrollPos = new Vector2(0,10000000);
    	msgs += msg + "\n";
    }


    bool needUpdate;
    //IRC Connections
	private void StartIRC()
    {
		HandleMessage("Twitch: Connecting");
    	//Connect to Twitch Server
        sock = new System.Net.Sockets.TcpClient();
        sock.Connect(server, port);
        if (!sock.Connected)
        {
            Debug.LogWarning("Failed to connect!");
            return;
        }
        stopThreads = false;
        var networkStream = sock.GetStream();
        var input = new System.IO.StreamReader(networkStream);
       	var output = new System.IO.StreamWriter(networkStream);

        //Send username and password
       	output.WriteLine("PASS " + oauth);
        output.WriteLine("NICK " + nickName.ToLower());
        output.Flush();

        //output proc
       	outProc = new System.Threading.Thread(() => IRCOutputProcedure(output));
        outProc.Start();
        //input proc
        inProc = new System.Threading.Thread(() => IRCInputProcedure(input, networkStream));
        inProc.Start();
    }
	private void IRCInputProcedure(System.IO.TextReader input, System.Net.Sockets.NetworkStream networkStream)
    {
        while (!stopThreads)
        {
            if (!networkStream.DataAvailable)
                continue;

            buffer = input.ReadLine();

            //was message?
            if (buffer.Contains("PRIVMSG #"))
            {
				string delim = "PRIVMSG #"+nickName + " :";
				string message = buffer.Split('!')[0].Substring(1) + ": " + buffer.Split(new string[] {delim},System.StringSplitOptions.None)[1];
				HandleMessage(message);
				needUpdate = true;
            }

            //Send pong reply to any ping messages
            if (buffer.StartsWith("PING "))
            {
                SendCommand(buffer.Replace("PING", "PONG"));
            }

            //After server sends 001 command, we can join a channel
            if (buffer.Split(' ')[1] == "001")
            {
            	HandleMessage("Twitch: Joining Chat");
                SendCommand("JOIN #" + nickName.ToLower());
            }
        }
    }
    private void IRCOutputProcedure(System.IO.TextWriter output)
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        while (!stopThreads)
        {
            lock (commandQueue)
            {
                if (commandQueue.Count > 0) //do we have any commands to send?
                {
                    // https://github.com/justintv/Twitch-API/blob/master/IRC.md#command--message-limit 
                    //have enough time passed since we last sent a message/command?
                    if (stopWatch.ElapsedMilliseconds > 1750)
                    {
                        //send msg.
                        output.WriteLine(commandQueue.Peek());
                        output.Flush();
                        //remove msg from queue.
                        commandQueue.Dequeue();
                        //restart stopwatch.
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                }
            }
        }
    }
    public void SendCommand(string cmd)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue(cmd);
        }
    }
    public void SendMsg(string msg)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue("PRIVMSG #" + nickName.ToLower() + " :" + msg);
        }
    }

}
