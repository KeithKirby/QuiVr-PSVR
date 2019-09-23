using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent (typeof(EMOpenCloseMotion))]
public class Notification : MonoBehaviour {

    class PendingNotification
    {
        public Note Note;
        public float TriggerTime = -1; // Starts as -1, calculated on first update from TimeDelay
        public float TimeDelay;
    }

    public static Notification instance;
    public static float NotificationTime = 4f;
    public Text TitleText;
    public Text BodyText;
    public Text DetailText;
    public Image IconImage;

    EMOpenCloseMotion motion;
    List<Note> notes;
    static List<PendingNotification> _pendingNotifications = new List<PendingNotification>();
    static bool _pendingNotificationsDirty = false;

    void Awake () {
        instance = this;
        notes = new List<Note>();
        motion = GetComponent<EMOpenCloseMotion>();
        //TestNotes();
    }

    [AdvancedInspector.Inspect]
    public void TestNotification()
    {
        Note testNote = new Note();
        testNote.title = "Test Notification";
        testNote.body = "The quick brown fox jumps over the lazy dog";
        testNote.icon = ItemDatabase.v.ItemIcons[0];
        Notify(testNote);
    }

    void UpdateDisplay(Note note)
    {
        TitleText.text = note.title;
        TitleText.color = note.titleColor;
        BodyText.text = note.body;
        BodyText.color = note.bodyColor;
        DetailText.text = note.detail;
        DetailText.color = note.detailColor;
        IconImage.sprite = note.icon;
    }

    public static void Notify(Note note)
    {
        if (instance != null)
        {
            Debug.LogFormat("Notify {0} {1}", note.title, note.detail);
            instance.AddNote(note);
        }
        else
        {
            Debug.LogFormat("Notify FAILED, no instance {0} {1}", note.title, note.detail);
        }
        
    }

    public static void Notify(string title, string message)
    {
        Note n = new Note(title, message);
        Notify(n);
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
    }

    bool notifying;
    void Update()
    {
        if(_pendingNotificationsDirty)
        {
            foreach(var n in _pendingNotifications)
            {
                if(n.TriggerTime == -1)
                    n.TriggerTime = Time.time + n.TimeDelay;
            }
            _pendingNotifications.Sort((a, b) => { return a.TriggerTime.CompareTo(b.TriggerTime); });
            _pendingNotificationsDirty = false;
        }

        if (_pendingNotifications.Count > 0)
        {
            if(_pendingNotifications[0].TriggerTime <= Time.time )
            {
                Debug.LogFormat("ShowPendingNotification {0}", _pendingNotifications[0].Note.title);
                AddNote(_pendingNotifications[0].Note);
                _pendingNotifications.RemoveAt(0);
            }
        }

        if (notes.Count > 0 && !notifying)
        {
            notifying = true;
            StopCoroutine("ShowNextNote");
            StartCoroutine("ShowNextNote");
        }
    }
    
    public static void NotifyDelayed(Note n, float t)
    {
        Debug.LogFormat("NotifyDelayed {0} {1}", n.title, t );
        _pendingNotifications.Add(new PendingNotification { Note = n, TimeDelay = t });
        _pendingNotificationsDirty = true;
    }

    IEnumerator NoteDelay(Note n, float t)
    {
        yield return new WaitForSeconds(t);
        Notify(n);
    }

    IEnumerator ShowNextNote()
    {
        motion.SetStateToClose();
        Note n = notes[0];
        UpdateDisplay(n);
        notes.RemoveAt(0);
        motion.Open();
        if(n.ShowTime > 0)
            yield return new WaitForSeconds(n.ShowTime);
        else
            yield return new WaitForSeconds(NotificationTime/(notes.Count+1f));
        motion.Close();
        while(motion.motionState != EMBaseMotion.MotionState.Closed)
        {
            yield return true;
        }
        notifying = false;
        StopCoroutine("ShowNextNote");
    }

    [AdvancedInspector.Inspect]
    public void TestNotes()
    {
        Note a = new Note() { title = "Title", body = "Text A", detail = "Detail A" };
        Note b = new Note() { title = "Title", body = "Text B", detail = "Detail B" };
        Note c = new Note() { title = "Title", body = "Text C", detail = "Detail C" };

        Notification.NotifyDelayed(c, 12.0f);
        Notification.NotifyDelayed(a, 4.0f);
        Notification.NotifyDelayed(b, 8.0f);
    }
}

public class Note
{
    public Sprite icon;
    public string title = "";
    public string body = "";
    public string detail = "";
    public Color titleColor = Color.white;
    public Color bodyColor = Color.white;
    public Color detailColor = Color.white;
    public float ShowTime = -1;

    public Note()
    {
        icon = null;
    }

    public Note(string Title="", string Body="", string Detail="", Sprite icn = null, float dur=-1)
    {
        title = Title;
        body = Body;
        detail = Detail;
        icon = icn;
        ShowTime = dur;
    }

    public Note(ArmorOption o, bool temporary=false, string Reason="")
    {
        icon = ItemDatabase.v.ItemIcons[(int)o.Type];
        title = "New Item";
        if (temporary)
            title = "Temporary Item";
        body = o.ItemName;
        detail = Reason;
        titleColor = Color.white;
        bodyColor = ItemDatabase.v.RarityColors[0];
    }

    public static Note GainResource(int value)
    {
        Note n  = new Note();
        n.icon = ItemDatabase.v.ResourceIcon;
        n.title = "Gained Resource";
        if (value < 0)
            n.title = "Spent Resource";
        n.body = value + " " + I2.Loc.ScriptLocalization.Get("Resource");
        return n;
    }

    public static Note DuplicateResource(int value, ArmorOption item)
    {
        Note n = new Note();
        n.icon = ItemDatabase.v.ResourceIcon;
        Color rcol = ItemDatabase.v.RarityColors[item.rarity];
        string rhex = ColorUtil.ColorToHex(rcol);
        n.title = "Duplicate [<color=#"+rhex+">" + item.ItemName + "</color>] Disenchanted";
        n.body = value + " " +I2.Loc.ScriptLocalization.Get("Resource");
        return n;
    }
}
