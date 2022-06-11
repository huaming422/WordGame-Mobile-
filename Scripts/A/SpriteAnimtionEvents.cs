using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteAnimtionEvents : MonoBehaviour
{
	[Serializable]
	public class FrameEvent
	{
		public int frame;

		public UnityEvent frameEvent;
	}

	public FrameEvent[] frameEvents;

	private SpriteAnimaiton spriteAnimation;

	private void Start()
	{
		spriteAnimation = GetComponent<SpriteAnimaiton>();
		if (spriteAnimation == null || frameEvents.IsEmpty())
		{
			return;
		}
		Dictionary<int, UnityEvent> dictionary = new Dictionary<int, UnityEvent>();
		for (int i = 0; i < frameEvents.Length; i++)
		{
			FrameEvent frameEvent = frameEvents[i];
			if (!dictionary.ContainsKey(frameEvent.frame))
			{
				dictionary.Add(frameEvent.frame, frameEvent.frameEvent);
			}
		}
		spriteAnimation.frameEvents = dictionary;
	}
}
