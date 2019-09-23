using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ColourEffect
{
    public Text Target;

    public void Flash(Color from, Color to, float duration)
    {
        _duration = duration;
        _start = from;
        _end = to;
        _time = 0;
    }
    public void Update()
    {
        if(_time < _duration)
        {
            _time += Time.unscaledDeltaTime;
            if(_time >= _duration)
                _time = _duration;
            float t = Mathf.Sin(Mathf.PI * _time / _duration);
            Target.color = Color.Lerp(_start, _end, t);
        }
    }
    Color _start;
    Color _end;
    float _time = 0;
    float _duration = 0;
}

public class ShardDisplay : MonoBehaviour {

    public Text Current;
    public Text Changed;
    public Color AddColor = new Color(0.3f, 1.0f, 0.3f);
    public Color RemoveColor = new Color(1.0f, 0.3f, 0.3f);

    ColourEffect _flashEffect = new ColourEffect();

    private int _currentVal;
    private int _changedVal;

    public int CurrentVal
    {
        set
        {
            _currentVal = value;
            Current.text = _currentVal.ToString();
        }
    }

    public int ChangedVal
    {
        set
        {
            _changedVal = value;
            if (_changedVal > 0)
            {
                Changed.enabled = true;
                Changed.text = String.Format("+{0}",_changedVal);
                _flashEffect.Flash(AddColor, Color.white, 0.5f);
            }
            else if (_changedVal < 0)
            {
                Changed.enabled = true;
                Changed.text = String.Format("{0}", _changedVal);
                _flashEffect.Flash(RemoveColor, Color.white, 0.5f);
            }
            else
            {
                Changed.enabled = false;
            }
        }
    }

    // Use this for initialization
    IEnumerator Start () {
        while (false == Armory.ValidFetch)
            yield return null;        
        CurrentVal = Armory.instance.GetResource();
        ChangedVal = 0;
        _flashEffect.Target = Changed;
    }

    public void OnEnable()
    {
        EnchantmentPublisher.ItemSelected += EnchantmentPublisher_ItemSelected;
        EnchantmentPublisher.ShardsChanged += EnchantmentPublisher_ShardsChanged;
    }

    public void OnDisable()
    {
        EnchantmentPublisher.ItemSelected -= EnchantmentPublisher_ItemSelected;
        EnchantmentPublisher.ShardsChanged -= EnchantmentPublisher_ShardsChanged;
    }

    private void EnchantmentPublisher_ShardsChanged()
    {
        CurrentVal = Armory.instance.GetResource();        
    }

    private void EnchantmentPublisher_ItemSelected(ItemChest chest, ArmorOption item, bool canUse)
    {
        if (null != item && canUse)
        {
            int cost = ItemDatabase.v.ResourceValues[item.rarity];
            ChangedVal = chest.ChestType == ItemChest.ChestTypeNames.Enchant ? -cost : cost;
        }
        else
        {
            ChangedVal = 0;
        }
    }

    // Update is called once per frame
    void Update () {
        _flashEffect.Update();
    }
}
