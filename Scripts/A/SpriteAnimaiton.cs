using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimaiton : MonoBehaviour
{
	public enum PlayType
	{
		Onece,
		Loop,
		LoopPingPong
	}

	public Sprite[] sprites;

	public float speech = 0.1f;

	public PlayType playType;

	public bool isNeedSetNativeSize;

	public bool isAtuoPlay;

	public float autoPalyDealy = -1f;

	public bool noPlayIsHidden = true;

	public Dictionary<int, UnityEvent> frameEvents;

	private int nowIndex;

	private int step = 1;

	private Image image;

	private bool isPlayIng;

	private Sprite oldSprite;

	private float nowTimes;

	private bool canPlay = true;

	private UnityAction finishCallback;

	private bool isCheckDealy;

	private float nowDealyTime;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void Start()
	{
		if (isAtuoPlay)
		{
			Play();
		}
		else if (noPlayIsHidden)
		{
			image.enabled = false;
		}
	}

	private void Update()
	{
		if (canPlay)
		{
			CheckUpdateSprite();
			CheckDelay();
		}
	}

	private void CheckDelay()
	{
		if (isCheckDealy)
		{
			nowDealyTime -= Time.deltaTime;
			if (nowDealyTime <= 0f)
			{
				isCheckDealy = false;
				image.enabled = true;
				RealPlay();
			}
		}
	}

	private void CheckUpdateSprite()
	{
		if (isPlayIng)
		{
			nowTimes += Time.deltaTime;
			if (nowTimes >= speech)
			{
				nowTimes = 0f;
				ChangeSprite();
			}
		}
	}

	private void ChangeSprite()
	{
		if (sprites.IsEmpty())
		{
			isPlayIng = false;
			return;
		}
		nowIndex += step;
		if (nowIndex < sprites.Length && nowIndex >= 0)
		{
			image.sprite = sprites[nowIndex];
			if (isNeedSetNativeSize)
			{
				image.SetNativeSize();
			}
			ExcuteFrameEvents(nowIndex);
		}
		if (playType == PlayType.Onece)
		{
			DoOneceChange();
		}
		if (playType == PlayType.Loop)
		{
			DoLoopChange();
		}
		if (playType == PlayType.LoopPingPong)
		{
			DoLoopPingPongChange();
		}
		if (isPlayIng)
		{
		}
	}

	private void DoOneceChange()
	{
		if (nowIndex >= sprites.Length || nowIndex < 0)
		{
			isPlayIng = false;
			if (noPlayIsHidden)
			{
				image.enabled = false;
			}
			if (finishCallback != null)
			{
				finishCallback();
			}
		}
	}

	private void DoLoopChange()
	{
		bool flag = false;
		if (nowIndex < 0 && step < 0)
		{
			nowIndex = sprites.Length - 1;
			flag = true;
		}
		if (nowIndex >= sprites.Length && step > 0)
		{
			nowIndex = 0;
			flag = true;
		}
		if (flag)
		{
			if (autoPalyDealy > 0f && noPlayIsHidden)
			{
				image.enabled = false;
			}
			isPlayIng = false;
			Play();
		}
	}

	private void DoLoopPingPongChange()
	{
		bool flag = false;
		if (nowIndex >= sprites.Length && step > 0)
		{
			flag = true;
			nowIndex = sprites.Length - 1;
		}
		if (nowIndex < 0 && step < 0)
		{
			flag = true;
			nowIndex = 0;
		}
		if (flag)
		{
			step = -step;
			if (autoPalyDealy > 0f && noPlayIsHidden)
			{
				image.enabled = false;
			}
			isPlayIng = false;
			Play();
		}
	}

	private void ExcuteFrameEvents(int frame)
	{
		if (frameEvents == null)
		{
			return;
		}
		frame++;
		if (frameEvents.ContainsKey(frame))
		{
			UnityEvent unityEvent = frameEvents[frame];
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
	}

	private void RealPlay()
	{
		if (!sprites.IsEmpty())
		{
			nowTimes = 0f;
			image.sprite = sprites[nowIndex];
			if (isNeedSetNativeSize)
			{
				image.SetNativeSize();
			}
			isPlayIng = true;
			ExcuteFrameEvents(nowIndex);
		}
	}

	public void Play()
	{
		canPlay = true;
		if (autoPalyDealy > 0f)
		{
			if (noPlayIsHidden)
			{
				image.enabled = false;
			}
			isCheckDealy = true;
			nowDealyTime = autoPalyDealy;
		}
		else
		{
			if (noPlayIsHidden)
			{
				image.enabled = true;
			}
			RealPlay();
		}
	}

	public void Sotp(bool isRest = true, int resetIndex = 0)
	{
		canPlay = false;
		if (isRest && resetIndex < sprites.Length && resetIndex >= 0)
		{
			image.sprite = sprites[resetIndex];
		}
	}

	public void OlderPlay()
	{
		OlderPlay(null);
	}

	public void OlderPlay(UnityAction callback = null)
	{
		finishCallback = callback;
		playType = PlayType.Onece;
		step = 1;
		nowIndex = 0;
		Play();
	}

	public void ReversePlay()
	{
		ReversePlay();
	}

	public void ReversePlay(UnityAction callback = null)
	{
		finishCallback = callback;
		playType = PlayType.Onece;
		step = -1;
		nowIndex = sprites.Length - 1;
		Play();
	}
}
