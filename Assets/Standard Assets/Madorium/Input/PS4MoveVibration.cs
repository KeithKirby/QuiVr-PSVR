using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;
#endif

public class PS4MoveVibration
{
    int _slot = 0;
    int _index = 0;
    float _time;

    public PS4MoveVibration(int slot, int index)
    {
        _slot = slot;
        _index = index;
    }

	public void InputUpdate()
    {
        if(_time > 0)
        {
            _time -= Time.unscaledDeltaTime;
            if(_time<0)
            {
                _time = 0;
#if !UNITY_EDITOR
                PS4Input.MoveSetVibration(_slot, _index, 0);
#endif
            }
        }
	}

    public void HapticPulse(int duration, int intensity)
    {
        if (duration > 0)
        {
            _time = (float)duration / 1000000.0f;
#if !UNITY_EDITOR
            PS4Input.MoveSetVibration(_slot, _index, intensity);
#endif
        }
        else
        {
            _time = 0;
#if !UNITY_EDITOR
            PS4Input.MoveSetVibration(_slot, _index, 0);
#endif
        }
    }
}