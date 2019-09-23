using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class infinite_fantasy_demo : MonoBehaviour {
	

	public AudioMixer fantasy_mixer;
	public Dropdown Music_Dropdown;
	public Dropdown Background_Dropdown;
	private float bpm;
	private int beatsPerMeasure = 4;
	private double singleMeasureTime;
	private double delayEvent;
	private bool running = false;
	private int i;
	private int y;
	private int j;
	private int k;
	private bool reset = false;
	private bool first_run = false;
	private double time;
	private float reset_time;
	public float reset_timer;
	public bool stop_music;
	public bool use_triggers;
	private bool in_time;
	private bool switch_audio_source = false;
	private bool main_theme_isPlaying;
	private bool adventure_isPlaying;
	private bool sad_isPlaying;
	private bool dark_isPlaying;
	private bool happy_isPlaying;
	private bool heroic_isPlaying;
	private bool mysterious_isPlaying;
	private bool battle_isPlaying;

	
	public Transform enemy_1;
	public Transform enemy_2;
	public Transform enemy_3;
	public Transform enemy_4;
	public Transform enemy_5;
	public Transform Player;
	private bool ambiant_play;
	public bool adventure;
	public bool dark;
	public bool heroic;
	public bool happy;
	public bool mysterious;
	public bool sad;
	public bool battle;
	public bool ambiant_light;
	public bool ambiant_dark;
	public bool play_transition;
	public bool background;

	private float enemy1;
	private float enemy2;
	private float enemy3;
	private float enemy4;
	private float enemy5;
	
	private float soft_mood;
	private float med_mood;
	private float forte_mood;
	private float soft_distance = 5000;
	private float med_distance = 5000;
	private float forte_distance = 5000;
	public float nearest_enemy;

	public int trigger_med;
	public int trigger_forte;
	
	private bool soft_trigger = false;
	private bool med_trigger = false;
	private bool forte_trigger = false;
	
	private int random_theme;
	private int random_ambiant;

	private float background_volume_set = -25.0f;
	public float fadeout_speed = 15.0f;
	public float fadein_speed = 25.0f;
	
	private AudioSource audio_soft1;
	private AudioSource audio_med1;
	private AudioSource audio_forte1;
	private AudioSource audio_soft2;
	private AudioSource audio_med2;
	private AudioSource audio_forte2;
	private AudioSource audio_end;
	private AudioSource audio_ambiant1;
	private AudioSource audio_ambiant2;
	private AudioSource audio_background;


	private bool forest_birds = false;
	private bool battle_amp = false;
	private bool castle = false;
	private bool cave = false;
	private bool docks = false;
	private bool forest_creek = false;
	private bool crickets = false;
	private bool tavern = false;
	private bool torches = false;
	private bool plains = false;
	private bool winter = false;

	private float audio_end_vol = 0.0f;
	private float audio_soft_vol = -80.0f;
	private float audio_med_vol = -80.0f;
	private float audio_forte_vol = -80.0f;
	private float audio_background_vol = -25.0f;
	private float audio_ambiant_vol = -10.0f;
	
	public bool soft;
	public bool med;
	public bool forte;
		
	public bool soft_isPlaying;
	public bool med_isPlaying;
	public bool forte_isPlaying;
	public bool ambiant_isPlaying;

	private int set_bpm;
	
	public bool start = false;
	
	private AudioSource audio_demo;
	
	private Object[] AudioArray_soft_adventure;
	private Object[] AudioArray_med_adventure;
	private Object[] AudioArray_forte_adventure;
	private Object[] AudioArray_end_soft_adventure;
	private Object[] AudioArray_end_med_adventure;
	private Object[] AudioArray_end_forte_adventure;

	private Object[] AudioArray_soft_dark;
	private Object[] AudioArray_med_dark;
	private Object[] AudioArray_forte_dark;
	private Object[] AudioArray_end_soft_dark;
	private Object[] AudioArray_end_med_dark;
	private Object[] AudioArray_end_forte_dark;

	private Object[] AudioArray_soft_mysterious;
	private Object[] AudioArray_med_mysterious;
	private Object[] AudioArray_forte_mysterious;
	private Object[] AudioArray_end_soft_mysterious;
	private Object[] AudioArray_end_med_mysterious;
	private Object[] AudioArray_end_forte_mysterious;

	private Object[] AudioArray_soft_happy;
	private Object[] AudioArray_med_happy;
	private Object[] AudioArray_forte_happy;
	private Object[] AudioArray_end_soft_happy;
	private Object[] AudioArray_end_med_happy;
	private Object[] AudioArray_end_forte_happy;

	private Object[] AudioArray_soft_heroic;
	private Object[] AudioArray_med_heroic;
	private Object[] AudioArray_forte_heroic;
	private Object[] AudioArray_end_soft_heroic;
	private Object[] AudioArray_end_med_heroic;
	private Object[] AudioArray_end_forte_heroic;

	private Object[] AudioArray_soft_sad;
	private Object[] AudioArray_med_sad;
	private Object[] AudioArray_forte_sad;
	private Object[] AudioArray_end_soft_sad;
	private Object[] AudioArray_end_med_sad;
	private Object[] AudioArray_end_forte_sad;

	private Object[] AudioArray_ambiant_light;
	private Object[] AudioArray_ambiant_dark;
	private Object[] AudioArray_background;

	private Object[] AudioArray_forte_battle;
	private Object[] AudioArray_end_forte_battle;
	
	private Object[] AudioArray_demo;
	
	// Use this for initialization
	void Start () {
		soft = true;
		main_theme_isPlaying = false;
		adventure_isPlaying = false;
		sad_isPlaying = false;
		dark_isPlaying = false;
		happy_isPlaying = false;
		heroic_isPlaying = false;
		mysterious_isPlaying = false;
		battle_isPlaying = false;
		ambiant_play = false;

		adventure = true;
		use_triggers = false;
		stop_music = false;
		reset_timer = 5.0f;
		first_run = false;
		beatsPerMeasure = 4;
		reset_time = -0.1f;
		i = 0;
		k = 0;
		bpm = set_bpm;
		audio_demo = (AudioSource)gameObject.AddComponent <AudioSource>();
		
		audio_background = (AudioSource)gameObject.AddComponent <AudioSource>();

		audio_soft1 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_med1 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_forte1 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_end = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_ambiant1 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_ambiant2 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_soft2 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_med2 = (AudioSource)gameObject.AddComponent <AudioSource>();
		audio_forte2 = (AudioSource)gameObject.AddComponent <AudioSource>();

				
		AudioArray_soft_adventure = Resources.LoadAll("adventure/soft",typeof(AudioClip));
		AudioArray_med_adventure = Resources.LoadAll("adventure/med",typeof(AudioClip));
		AudioArray_forte_adventure = Resources.LoadAll("adventure/forte",typeof(AudioClip));
		AudioArray_end_soft_adventure = Resources.LoadAll("adventure/soft_end",typeof(AudioClip));
		AudioArray_end_med_adventure = Resources.LoadAll("adventure/med_end",typeof(AudioClip));
		AudioArray_end_forte_adventure = Resources.LoadAll("adventure/forte_end",typeof(AudioClip));

		AudioArray_soft_dark = Resources.LoadAll("dark/soft",typeof(AudioClip));
		AudioArray_med_dark = Resources.LoadAll("dark/med",typeof(AudioClip));
		AudioArray_forte_dark = Resources.LoadAll("dark/forte",typeof(AudioClip));
		AudioArray_end_soft_dark = Resources.LoadAll("dark/soft_end",typeof(AudioClip));
		AudioArray_end_med_dark = Resources.LoadAll("dark/med_end",typeof(AudioClip));
		AudioArray_end_forte_dark = Resources.LoadAll("dark/forte_end",typeof(AudioClip));

		AudioArray_soft_mysterious = Resources.LoadAll("mysterious/soft",typeof(AudioClip));
		AudioArray_med_mysterious = Resources.LoadAll("mysterious/med",typeof(AudioClip));
		AudioArray_forte_mysterious = Resources.LoadAll("mysterious/forte",typeof(AudioClip));
		AudioArray_end_soft_mysterious = Resources.LoadAll("mysterious/soft_end",typeof(AudioClip));
		AudioArray_end_med_mysterious = Resources.LoadAll("mysterious/med_end",typeof(AudioClip));
		AudioArray_end_forte_mysterious = Resources.LoadAll("mysterious/forte_end",typeof(AudioClip));

		AudioArray_soft_happy = Resources.LoadAll("happy/soft",typeof(AudioClip));
		AudioArray_med_happy = Resources.LoadAll("happy/med",typeof(AudioClip));
		AudioArray_forte_happy = Resources.LoadAll("happy/forte",typeof(AudioClip));
		AudioArray_end_soft_happy = Resources.LoadAll("happy/soft_end",typeof(AudioClip));
		AudioArray_end_med_happy = Resources.LoadAll("happy/med_end",typeof(AudioClip));
		AudioArray_end_forte_happy = Resources.LoadAll("happy/forte_end",typeof(AudioClip));

		AudioArray_soft_heroic = Resources.LoadAll("heroic/soft",typeof(AudioClip));
		AudioArray_med_heroic = Resources.LoadAll("heroic/med",typeof(AudioClip));
		AudioArray_forte_heroic = Resources.LoadAll("heroic/forte",typeof(AudioClip));
		AudioArray_end_soft_heroic = Resources.LoadAll("heroic/soft_end",typeof(AudioClip));
		AudioArray_end_med_heroic = Resources.LoadAll("heroic/med_end",typeof(AudioClip));
		AudioArray_end_forte_heroic = Resources.LoadAll("heroic/forte_end",typeof(AudioClip));

		AudioArray_soft_sad = Resources.LoadAll("sad/soft",typeof(AudioClip));
		AudioArray_med_sad = Resources.LoadAll("sad/med",typeof(AudioClip));
		AudioArray_forte_sad = Resources.LoadAll("sad/forte",typeof(AudioClip));
		AudioArray_end_soft_sad = Resources.LoadAll("sad/soft_end",typeof(AudioClip));
		AudioArray_end_med_sad = Resources.LoadAll("sad/med_end",typeof(AudioClip));
		AudioArray_end_forte_sad = Resources.LoadAll("sad/forte_end",typeof(AudioClip));

		AudioArray_ambiant_light = Resources.LoadAll("ambiant/light",typeof(AudioClip));
		AudioArray_ambiant_dark = Resources.LoadAll("ambiant/dark",typeof(AudioClip));
		AudioArray_background = Resources.LoadAll("background",typeof(AudioClip));

		AudioArray_end_forte_battle = Resources.LoadAll("battle/forte_end",typeof(AudioClip));
		AudioArray_forte_battle = Resources.LoadAll("battle/forte",typeof(AudioClip));



		AudioArray_demo = Resources.LoadAll("demo",typeof(AudioClip));
		RandomStructure ();

		audio_ambiant1.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Ambiant")[0];
		audio_ambiant2.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Ambiant")[0];
		audio_background.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Background")[0];
		audio_end.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("End")[0];
		audio_soft1.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Soft")[0];
		audio_med1.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Med")[0];
		audio_forte1.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Forte")[0];
		audio_soft2.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Soft")[0];
		audio_med2.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Med")[0];
		audio_forte2.outputAudioMixerGroup = fantasy_mixer.FindMatchingGroups("Forte")[0];

		audio_demo.clip = AudioArray_demo [0] as AudioClip;

		SetTempo ();

		Music_Dropdown.onValueChanged.AddListener(delegate {
			MusicDropdownValueChangedHandler(Music_Dropdown);
		});

		Background_Dropdown.onValueChanged.AddListener(delegate {
			BackgroundDropdownValueChangedHandler(Background_Dropdown);
		});

	}
	
	// Update is called once per frame
	void Update () {

		SetVolumes ();

		if (!stop_music & use_triggers) {
			CheckDistanceToTrigger ();
			CheckMood ();
		}
		if (stop_music) {
			soft = false;
			med = false;
			forte = false;
			ambiant_play = false;
			StopAll ();
		}

		
		if (y == 1 | y == 5 | y == 9 | y == 13) {
			if (!soft & !med & !forte & !ambiant_play){
				if (main_theme_isPlaying){
					PlayEnd ();
				}

				StopAll();
			}
		}
		
		if (start) {
			if (!first_run) {
				RandomStructure();
				SetTempo ();
				singleMeasureTime = AudioSettings.dspTime + 2.0F;
				running = true;
				i = 0;
				y = 0;
			}
			first_run = true;
			Counter();
		}	
		
		
		
		
		
	}
	
	public void Counter (){
		//Debug.Log ("Counter was executed");

		if (!running)
			return;
		time = AudioSettings.dspTime;
		
		
		if (time + 1.0F > singleMeasureTime) {

		}
		if (in_time) {
			if (y == 1 | y == 5 | y == 9) {
				Fantasy_Play ();
			}

		}
		
		
		//THE most important part of this script: this is the metronome, keeping count of the measures and making sure the audio is in sync
		if (time + 1.0F > singleMeasureTime & start) {
			i += 1;
			y += 1; 
			j += 1;
			k += 1;
			if (y == 9){
				SetTempo ();
			}
		
			singleMeasureTime += 60.0F / bpm * beatsPerMeasure;
			in_time = true;	
			//Debug.Log ("The y int equals  " + y + " " + bpm);
			//PlayDemo();
		} else {
			in_time = false;
		}
		
	}
	
	public void Fantasy_Play(){

		if (y == 9){
			if (!adventure & !sad & !dark & !heroic & !mysterious & !happy & !battle) {
				if (main_theme_isPlaying){
					PlayEnd();
				}
			}
		}

		if (y == 9 & !ambiant_light &!ambiant_dark & play_transition) {
			PlayTransition();
			SetTempo ();
		}

		if (y == 9) {
				y = 1;
		}



		if (y == 5 & !ambiant_light & !ambiant_dark & !ambiant_isPlaying & play_transition) {
			play_transition = false;
			y = 1;
		}

		if (y == 1 & !play_transition){
				switch_audio_source = !switch_audio_source;
				RandomStructure();
				SetTempo ();
				SetMood ();

			if (!ambiant_light & !ambiant_dark){
				main_theme_isPlaying = true;
				isPlaying();
				if(switch_audio_source){
					audio_soft1.PlayOneShot (audio_soft1.clip, 1.0f);	
					audio_med1.PlayOneShot (audio_med1.clip, 1.0f);	
					audio_forte1.PlayOneShot (audio_forte1.clip, 1.0f);	
				}
				if(!switch_audio_source){
					audio_soft2.PlayOneShot (audio_soft2.clip, 1.0f);	
					audio_med2.PlayOneShot (audio_med2.clip, 1.0f);	
					audio_forte2.PlayOneShot (audio_forte2.clip, 1.0f);	
				}

			}

			if (ambiant_light | ambiant_dark){
				ambiant_isPlaying = true;
				if(switch_audio_source){
					audio_ambiant1.PlayOneShot (audio_ambiant1.clip, 1.0f);	
				}
				if(!switch_audio_source){
					audio_ambiant2.PlayOneShot (audio_ambiant2.clip, 1.0f);	
				}
			}


		}


	}
	
	
	
	
	
	
	public void Stop_onClick(){
		reset = true;
		stop_music = true;
		
	}

	public void Soft_onClick(){
		start = true;
		soft = true;
		med = false;
		forte = false;

	}
	
	public void Med_onClick(){
		start = true;
		med = true;
		soft = false;
		forte = false;
	}
	
	public void Forte_onClick(){
		start = true;
		forte = true;
		med = false;
		soft = false;

	}

	public void Adventure_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = true;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}

	public void Battle_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;
		
		adventure = true;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = true;
		forte = true;
		
	}

	public void Heroic_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = false;
		heroic = true;
		dark = false;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}

	public void Dark_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = false;
		heroic = false;
		dark = true;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}

	public void Mysterious_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = false;
		heroic = false;
		dark = false;
		mysterious = true;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}

	public void Happy_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = false;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = true;
		sad = false;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}
	public void Sad_onClick(){
		if (ambiant_isPlaying) {
			play_transition = true;
		}
		ambiant_play = false;

		adventure = false;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = false;
		sad = true;
		ambiant_light = false;
		ambiant_dark = false;
		battle = false;


	}
	public void Ambiant_Light_onClick(){
		start = true;
		ambiant_play = true;
		adventure = false;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = true;
		ambiant_dark = false;
		battle = false;

		play_transition = false;
	}
	public void Ambiant_Dark_onClick(){
		start = true;
		ambiant_play = true;
		adventure = false;
		heroic = false;
		dark = false;
		mysterious = false;
		happy = false;
		sad = false;
		ambiant_light = false;
		ambiant_dark = true;
		play_transition = false;
		battle = false;

	}

	public void Background(){
		if (forest_birds) {
			audio_background.clip = AudioArray_background [0] as AudioClip;
		}
		if (battle_amp) {
			audio_background.clip = AudioArray_background [1] as AudioClip;
		}
		if (castle) {
			audio_background.clip = AudioArray_background [2] as AudioClip;
		}
		if (cave) {
			audio_background.clip = AudioArray_background [3] as AudioClip;
		}
		if (docks) {
			audio_background.clip = AudioArray_background [4] as AudioClip;
		}
		if (forest_creek) {
			audio_background.clip = AudioArray_background [5] as AudioClip;
		}
		if (crickets) {
			audio_background.clip = AudioArray_background [6] as AudioClip;
		}
		if (tavern) {
			audio_background.clip = AudioArray_background [7] as AudioClip;
		}
		if (torches) {
			audio_background.clip = AudioArray_background [8] as AudioClip;
		}
		if (plains) {
			audio_background.clip = AudioArray_background [9] as AudioClip;
		}
		if (winter) {
			audio_background.clip = AudioArray_background [10] as AudioClip;
		}

		if (background) {
			audio_background.loop = true;
			audio_background.Play ();	

		}
	}
	
	public void RandomStructure(){

		random_theme = Random.Range (0, AudioArray_soft_adventure.Length);
		random_ambiant = Random.Range (0, AudioArray_ambiant_light.Length);


	}
	
	public void StopAll(){

			if (!soft & !med & !forte & !soft_isPlaying & !med_isPlaying & !forte_isPlaying) {
				i = 0;
				y = 0;
				k = 0;
				j = 0;
				singleMeasureTime = 0;
				time = 0;
				start = false;
				running = false;
				first_run = false;
			}
	}
		

	
		
	public void SetVolumes(){
		fantasy_mixer.SetFloat ("End", audio_end_vol);
		fantasy_mixer.SetFloat ("Soft", audio_soft_vol);
		fantasy_mixer.SetFloat ("Med", audio_med_vol);
		fantasy_mixer.SetFloat ("Forte", audio_forte_vol);
		fantasy_mixer.SetFloat ("Background", audio_background_vol);
		fantasy_mixer.SetFloat ("Ambiant", audio_ambiant_vol);



		
		if (audio_soft_vol > -1.0f | audio_med_vol > -1.0f | audio_forte_vol > -1.0f) {
			fadein_speed = 60.0f;
		}
		if (soft & audio_soft_vol > -10.0f) {
			fadein_speed = 8.0f;
		}
		if (med & audio_med_vol > -10.0f) {
			fadein_speed = 8.0f;
		}
		if (forte & audio_forte_vol > -10.0f) {
			fadein_speed = 8.0f;
		}

		if (ambiant_light | ambiant_dark) {
			if (audio_ambiant_vol < -10.0f) {
				audio_ambiant_vol += fadein_speed * Time.deltaTime;	
			}
		}

		if (!ambiant_light & !ambiant_dark) {
			if (audio_ambiant_vol > -80.0f) {
				audio_ambiant_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_ambiant_vol < -30.0f) {
				if (play_transition & ambiant_isPlaying) {
					y = 8;
					SetTempo();
					Counter ();
					ambiant_isPlaying = false;

				}

			}
		}

		if (background) {
			if (audio_background_vol < background_volume_set) {
				audio_background_vol += fadein_speed * Time.deltaTime;	
			}
			if (audio_background_vol > background_volume_set + 0.1f) {
				audio_background_vol -= fadeout_speed * Time.deltaTime;	
			}
		}
		if (!background) {
			if (audio_background_vol > -80.0f) {
				audio_background_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_background_vol < -70.0f){
				audio_background.Stop ();
			}
		}

		if (soft) {
			if (audio_soft_vol < 0.0f) {
				audio_soft_vol += fadein_speed * Time.deltaTime;	
			}
			if (audio_med_vol > -80.0f & audio_soft_vol > -5.0f) {
				audio_med_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_forte_vol > -80.0f & audio_soft_vol > -5.0f) {
				audio_forte_vol -= fadeout_speed * Time.deltaTime;	
			}
		}
		
		if (med) {
			if (audio_med_vol < 0.0f) {
				audio_med_vol += fadein_speed * Time.deltaTime;	
			}
			if (audio_soft_vol > -80.0f & audio_med_vol > -5.0f) {
				audio_soft_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_forte_vol > -80.0f & audio_med_vol > -5.0f) {
				audio_forte_vol -= fadeout_speed * Time.deltaTime;	
			}
		}
		
		if (forte) {
			if (audio_forte_vol < 0.0f) {
				audio_forte_vol += fadein_speed * Time.deltaTime;	
			}
			if (audio_med_vol > -80.0f & audio_forte_vol > -5.0f) {
				audio_med_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_soft_vol > -80.0f & audio_forte_vol > -5.0f) {
				audio_soft_vol -= fadeout_speed * Time.deltaTime;	
			}
		}
		
		if (!soft & !med & !forte) {
			if (audio_forte_vol > -80.0f) {
				audio_forte_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_forte_vol < -70.0f) {
				forte_isPlaying = false;
			}
			if (audio_med_vol > -80.0f) {
				audio_med_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_med_vol < -70.0f) {
				med_isPlaying = false;
			}
			if (audio_soft_vol > -80.0f) {
				audio_soft_vol -= fadeout_speed * Time.deltaTime;	
			}
			if (audio_soft_vol < -70.0f) {
				soft_isPlaying = false;
			}
			
		}
	}

		
		
		
		
		
	
	public void CheckDistanceToTrigger (){
		try{
			enemy1 = Vector3.Distance(Player.position, enemy_1.position);
		}catch{
			//do nothing
		}
		try{
			enemy2 = Vector3.Distance(Player.position, enemy_2.position);
		}catch{
			//do nothing
		}
		try{
			enemy3 = Vector3.Distance(Player.position, enemy_3.position);
		}catch{
			//do nothing
		}
		try{
			enemy4 = Vector3.Distance(Player.position, enemy_4.position);
		}catch{
			//do nothing
		}
		try{
			enemy5 = Vector3.Distance(Player.position, enemy_5.position);
		}catch{
			//do nothing
		}
		if (enemy1 == 0 | enemy_1 == null) {
			enemy1 = 5000;
		}
		if (enemy2 == 0 | enemy_2 == null) {
			enemy2 = 5000;
		}
		if (enemy3 == 0 | enemy_3 == null) {
			enemy3 = 5000;
		}
		if (enemy4 == 0 | enemy_4 == null) {
			enemy4 = 5000;
		}
		if (enemy5 == 0 | enemy_5 == null) {
			enemy5 = 5000;
		}
		
		float[] distance_to_enemy = {enemy1 ,
			enemy2,
			enemy3,
			enemy4 ,
			enemy5};
		System.Array.Sort(distance_to_enemy);
		nearest_enemy = distance_to_enemy[0];
		
	}
	
	public void CheckMood (){
		
		
		if (nearest_enemy > trigger_med) {
			if (!start){
				start = true;
			}
			if (reset_time >= 0){
				reset_time -= Time.deltaTime;
			}
			if (reset_time < 0) {
				soft = true;
				med = false;
				forte = false;
			}
			
		}
		if (nearest_enemy < trigger_med  & nearest_enemy > trigger_forte) {
			if (!start){
				start = true;
			}
			if (reset_time >= 0){
				reset_time -= Time.deltaTime;
			}
			if (reset_time < 0) {
				soft = false;
				med = true;
				forte = false;
			}
		}
		if (nearest_enemy < trigger_forte) {
			if (!start){
				start = true;
			}
			reset_time = reset_timer;
			soft = false;
			med = false;
			forte = true;
		}
	}
	
	public void SetTempo(){
		if (adventure){
			bpm = 100;
		}
		if (heroic){
			bpm = 120;
		}
		if (dark){
			bpm = 80;
		}
		if (mysterious){
			bpm = 100;
		}
		if (happy){
			bpm = 120;
		}
		if (sad){
			bpm = 80;
		}
		if (ambiant_light | ambiant_dark) {
			bpm = 120;
		}
		if (battle) {
			bpm = 140;
		}
	}

	public void PlayTransition(){
		if (adventure & soft){
			audio_end.clip = AudioArray_end_soft_adventure [0] as AudioClip;
		}
		if (heroic & soft){
			audio_end.clip = AudioArray_end_soft_heroic [0] as AudioClip;

		}
		if (dark & soft){
			audio_end.clip = AudioArray_end_soft_dark [0] as AudioClip;

		}
		if (mysterious & soft){
			audio_end.clip = AudioArray_end_soft_mysterious [0] as AudioClip;

		}
		if (happy & soft){
			audio_end.clip = AudioArray_end_soft_happy [0] as AudioClip;

		}
		if (sad & soft){
			audio_end.clip = AudioArray_end_soft_sad [0] as AudioClip;

		}

		if (adventure & med){
			audio_end.clip = AudioArray_end_med_adventure [0] as AudioClip;
		}
		if (heroic & med){
			audio_end.clip = AudioArray_end_med_heroic [0] as AudioClip;
			
		}
		if (dark & med){
			audio_end.clip = AudioArray_end_med_dark [0] as AudioClip;
			
		}
		if (mysterious & med){
			audio_end.clip = AudioArray_end_med_mysterious [0] as AudioClip;
			
		}
		if (happy & med){
			audio_end.clip = AudioArray_end_med_happy [0] as AudioClip;
			
		}
		if (sad & med){
			audio_end.clip = AudioArray_end_med_sad [0] as AudioClip;
			
		}

		if (adventure & forte){
			audio_end.clip = AudioArray_end_forte_adventure [0] as AudioClip;
		}
		if (heroic & forte){
			audio_end.clip = AudioArray_end_forte_heroic [0] as AudioClip;
			
		}
		if (dark & forte){
			audio_end.clip = AudioArray_end_forte_dark [0] as AudioClip;
			
		}
		if (mysterious & forte){
			audio_end.clip = AudioArray_end_forte_mysterious [0] as AudioClip;
			
		}
		if (happy & forte){
			audio_end.clip = AudioArray_end_forte_happy [0] as AudioClip;
			
		}
		if (sad & forte){
			audio_end.clip = AudioArray_end_forte_sad [0] as AudioClip;
			
		}
		if (battle & forte){
			audio_end.clip = AudioArray_end_forte_battle [0] as AudioClip;
			
		}

		audio_end.PlayOneShot (audio_end.clip, 1.0f);

	}

	public void SetMood(){
		
		if(switch_audio_source){

			if (adventure){
				audio_soft1.clip = AudioArray_soft_adventure [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_adventure [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_adventure [random_theme] as AudioClip;
			}
			if (heroic){
				audio_soft1.clip = AudioArray_soft_heroic [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_heroic [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_heroic [random_theme] as AudioClip;
				
				
			}
			if (battle){
				audio_forte1.clip = AudioArray_forte_battle [random_theme] as AudioClip;
				
				
			}
			if (dark){
				audio_soft1.clip = AudioArray_soft_dark [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_dark [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_dark [random_theme] as AudioClip;
				
			}
			if (mysterious){
				audio_soft1.clip = AudioArray_soft_mysterious [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_mysterious [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_mysterious [random_theme] as AudioClip;
				
			}
			if (happy){
				audio_soft1.clip = AudioArray_soft_happy [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_happy [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_happy [random_theme] as AudioClip;
				
			}
			if (sad){
				audio_soft1.clip = AudioArray_soft_sad [random_theme] as AudioClip;
				audio_med1.clip = AudioArray_med_sad [random_theme] as AudioClip;
				audio_forte1.clip = AudioArray_forte_sad [random_theme] as AudioClip;	
			}
			if (ambiant_light){
				audio_ambiant1.clip = AudioArray_ambiant_light [random_ambiant] as AudioClip;

			}
			if (ambiant_dark){
				audio_ambiant1.clip = AudioArray_ambiant_dark [random_ambiant] as AudioClip;
				
			}
		}
		if(!switch_audio_source){
			if (adventure){
				audio_soft2.clip = AudioArray_soft_adventure [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_adventure [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_adventure [random_theme] as AudioClip;
			}
			if (heroic){
				audio_soft2.clip = AudioArray_soft_heroic [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_heroic [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_heroic [random_theme] as AudioClip;
				
				
			}
			if (battle){
				audio_forte2.clip = AudioArray_forte_battle [random_theme] as AudioClip;
				
				
			}
			if (dark){
				audio_soft2.clip = AudioArray_soft_dark [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_dark [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_dark [random_theme] as AudioClip;
				
			}
			if (mysterious){
				audio_soft2.clip = AudioArray_soft_mysterious [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_mysterious [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_mysterious [random_theme] as AudioClip;
				
			}
			if (happy){
				audio_soft2.clip = AudioArray_soft_happy [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_happy [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_happy [random_theme] as AudioClip;
				
			}
			if (sad){
				audio_soft2.clip = AudioArray_soft_sad [random_theme] as AudioClip;
				audio_med2.clip = AudioArray_med_sad [random_theme] as AudioClip;
				audio_forte2.clip = AudioArray_forte_sad [random_theme] as AudioClip;
				
			}
			if (ambiant_light){
				audio_ambiant2.clip = AudioArray_ambiant_light [random_ambiant] as AudioClip;
				
			}
			if (ambiant_dark){
				audio_ambiant2.clip = AudioArray_ambiant_dark [random_ambiant] as AudioClip;
				
			}
		}



	}

	private void MusicDropdownValueChangedHandler(Dropdown target) {
		if (target.value == 0) {
			adventure = false;
			heroic = false;
			dark = false;
			mysterious = false;
			happy = false;
			sad = false;
			battle = false;
			ambiant_light = false;
			ambiant_dark = false;
			soft = false;
			med = false;
			forte = false;
		}
		if (target.value == 1) {
			Ambiant_Light_onClick ();
		}
		if (target.value == 2) {
			Ambiant_Dark_onClick ();
		}
		if (target.value == 3) {
			Adventure_onClick ();
		}
		if (target.value == 4) {
			Heroic_onClick ();
		}
		if (target.value == 5) {
			Mysterious_onClick();
		}
		if (target.value == 6) {
			Happy_onClick();
		}
		if (target.value == 7) {
			Sad_onClick ();
		}
		if (target.value == 8) {
			Dark_onClick ();
		}
		if (target.value == 9) {
			Battle_onClick ();

		}
	}

	private void BackgroundDropdownValueChangedHandler(Dropdown target) {
		if (target.value == 0) {
			Background_Stop();

		}
		if (target.value == 1) {
			Background_Forest_Birds();

		}
		if (target.value == 2) {
			Background_Battle();

		}
		if (target.value == 3) {
			Background_Castle();
		}
		if (target.value == 4) {
			Background_Cave();
		}
		if (target.value == 5) {
			Background_Docks();
		}
		if (target.value == 6) {
			Background_Forest_Creek();
		}
		if (target.value == 7) {
			Background_Crickets();
		}
		if (target.value == 8) {
			Background_Tavern();

		}
		if (target.value == 9) {
			Background_Torches();

		}
		if (target.value == 10) {
			Background_Plains ();

		}
		if (target.value == 11) {
			Background_Winter ();
		}
	}

	public void PlayEnd(){

		if (adventure_isPlaying) {
			if (random_theme == 1|random_theme == 3 | random_theme == 5| random_theme == 7){
				if (soft){
					audio_end.clip = AudioArray_end_soft_adventure [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_adventure [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_adventure [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}

		if (dark_isPlaying) {
			if (random_theme == 1|random_theme == 4 | random_theme == 5| random_theme == 7){
				if (soft){
					audio_end.clip = AudioArray_end_soft_dark [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_dark [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_dark [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}

		if (heroic_isPlaying) {
			if (random_theme == 1|random_theme == 2 | random_theme == 5){
				if (soft){
					audio_end.clip = AudioArray_end_soft_heroic [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_heroic [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_heroic [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}

		if (battle_isPlaying) {
			if (random_theme == 0|random_theme == 1 | random_theme == 2|random_theme == 4|random_theme == 5 | random_theme == 6){
				
				if (forte){
					audio_end.clip = AudioArray_end_forte_battle [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}
		
		
		if (mysterious_isPlaying) {
			if (random_theme == 1|random_theme == 2 | random_theme == 5){
				if (soft){
					audio_end.clip = AudioArray_end_soft_mysterious [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_mysterious [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_mysterious [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}

		if (happy_isPlaying) {
			if (random_theme == 1|random_theme == 3 | random_theme == 5){
				if (soft){
					audio_end.clip = AudioArray_end_soft_happy [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_happy [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_happy [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}

		if (sad_isPlaying) {
			if (random_theme == 1 | random_theme == 5){
				if (soft){
					audio_end.clip = AudioArray_end_soft_sad [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (med){
					audio_end.clip = AudioArray_end_med_sad [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
				if (forte){
					audio_end.clip = AudioArray_end_forte_sad [1] as AudioClip;
					audio_end.PlayOneShot (audio_end.clip, 1.0f);	
					y = -1;
				}
			}
		}
		main_theme_isPlaying = false;
		adventure_isPlaying = false;
		sad_isPlaying = false;
		dark_isPlaying = false;
		happy_isPlaying = false;
		heroic_isPlaying = false;
		mysterious_isPlaying = false;
		battle_isPlaying = false;

	}

	public void isPlaying(){
		if (adventure) {
			adventure_isPlaying = true;
		}
		if (dark) {
			dark_isPlaying = true;
		}
		if (heroic) {
			heroic_isPlaying = true;
		}
		if (sad) {
			sad_isPlaying = true;
		}
		if (happy) {
			happy_isPlaying = true;
		}
		if (mysterious) {
			mysterious_isPlaying = true;
		}
		if (battle) {
			battle_isPlaying = true;
		}

	}

	public void Background_Forest_Birds(){
		background = true;
		background_volume_set = -22.0f;
		forest_birds = true;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;
		Background();
	}

	public void Background_Battle(){
		background = true;
		background_volume_set = -15.0f;
		
		forest_birds = false;
		battle_amp = true;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;
		Background();
	}

	public void Background_Castle(){
		background = true;
		background_volume_set = -20.0f;
		forest_birds = false;
		battle_amp = false;
		castle = true;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;		
		Background();
	}

	public void Background_Cave(){
		background_volume_set = -15.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = true;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;		
		Background();
	}

	public void Background_Docks(){
		background_volume_set = -20.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = true;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;		
		Background();
	}

	public void Background_Forest_Creek(){
		background_volume_set = -20.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = true;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;		
		Background();
	}

	public void Background_Crickets(){
		background_volume_set = -22.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = true;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;		
		Background();
	}
	public void Background_Tavern(){
		background_volume_set = -22.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = true;
		torches = false;
		plains = false;
		winter = false;	
		Background();
	}

	public void Background_Torches(){
		background_volume_set = -17.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = true;
		plains = false;
		winter = false;	
		Background();
	}

	public void Background_Plains(){
		background_volume_set = -15.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = true;
		winter = false;	
		Background();
	}
	public void Background_Winter(){
		background_volume_set = -15.0f;
		
		background = true;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = true;	
		Background();

	}

	public void Background_Stop(){
		background = false;
		forest_birds = false;
		battle_amp = false;
		castle = false;
		cave = false;
		docks = false;
		forest_creek = false;
		crickets = false;
		tavern = false;
		torches = false;
		plains = false;
		winter = false;
		Background();
	}




	public void PlayDemo(){

		if (in_time) {
			if (j == 8) {
				if (soft) {
					audio_demo.volume = 0.3f;
				}
				
				if (med) {
					audio_demo.volume = 0.5f;
				}
				if (forte) {
					audio_demo.volume = 0.9f;
				}
				//audio_demo.PlayScheduled(time);
				j = 0;
			}
			

		}
	}
	
}

