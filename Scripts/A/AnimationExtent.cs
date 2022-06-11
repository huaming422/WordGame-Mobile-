using UnityEngine;

public static class AnimationExtent
{
	public static void OrderPlay(this Animation animation, string clipName = "", float speed = 1f)
	{
		if (animation == null)
		{
			return;
		}
		AnimationState animationState = null;
		if (!string.IsNullOrEmpty(clipName))
		{
			animationState = animation[clipName];
		}
		else
		{
			AnimationClip clip = animation.clip;
			if (clip == null)
			{
				return;
			}
			animationState = animation[clip.name];
		}
		if (!(animationState == null))
		{
			animationState.speed = Mathf.Abs(speed);
			animationState.time = 0f;
			if (!string.IsNullOrEmpty(clipName))
			{
				animation.Play(clipName);
			}
			else
			{
				animation.Play();
			}
		}
	}

	public static void ReserverPlay(this Animation animation, string clipName = "", float speed = 1f)
	{
		if (animation == null)
		{
			return;
		}
		AnimationState animationState = null;
		if (!string.IsNullOrEmpty(clipName))
		{
			animationState = animation[clipName];
		}
		else
		{
			AnimationClip clip = animation.clip;
			if (clip == null)
			{
				return;
			}
			animationState = animation[clip.name];
		}
		if (!(animationState == null))
		{
			animationState.speed = 0f - Mathf.Abs(speed);
			animationState.time = animationState.length;
			if (!string.IsNullOrEmpty(clipName))
			{
				animation.Play(clipName);
			}
			else
			{
				animation.Play();
			}
		}
	}

	public static void MyStop(this Animation animation, string clipName = "", bool isRest = false)
	{
		if (animation == null)
		{
			return;
		}
		AnimationState animationState = null;
		if (!string.IsNullOrEmpty(clipName))
		{
			animationState = animation[clipName];
			if (!(animationState == null))
			{
				if (isRest)
				{
					animation.Play(clipName);
					animationState.time = 0f;
					animationState.enabled = true;
					animation.Sample();
					animationState.enabled = false;
				}
				animation.Stop(clipName);
			}
			return;
		}
		AnimationClip clip = animation.clip;
		if (clip == null)
		{
			return;
		}
		animationState = animation[clip.name];
		if (!(animationState == null))
		{
			if (isRest)
			{
				animation.Play(clip.name);
				animationState.time = 0f;
				animationState.enabled = true;
				animation.Sample();
				animationState.enabled = false;
			}
			animation.Stop();
		}
	}

	public static void MyStop(this Animation animation, bool isRest)
	{
		animation.MyStop(string.Empty, isRest);
	}
}
