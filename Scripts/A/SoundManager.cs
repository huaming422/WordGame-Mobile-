using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleObject<SoundManager>
{
	private Hashtable sounds = new Hashtable();

	private List<AudioSource> m_AudioCantaner = new List<AudioSource>();

	private Dictionary<string, AudioSource> m_WrokingAudioSource = new Dictionary<string, AudioSource>();

	private string nowPlayLoopPath = string.Empty;

	private void Awake()
	{
	}

	private void Add(string key, AudioClip value)
	{
		if (sounds[key] == null && !(value == null))
		{
			sounds.Add(key, value);
		}
	}

	private AudioClip Get(string key)
	{
		if (sounds[key] == null)
		{
			return null;
		}
		return sounds[key] as AudioClip;
	}

	private AudioClip LoadAudioClip(string path)
	{
		AudioClip audioClip = Get(path);
		if (audioClip == null)
		{
			int num = path.LastIndexOf("/");
			string abName = path.Substring(0, num + 1);
			string assetName = path.Substring(num + 1);
			audioClip = SingleObject<ResourceManager>.instance.LoadAsset<AudioClip>(abName, assetName, this);
			if (audioClip == null)
			{
				return null;
			}
			Add(path, audioClip);
		}
		return audioClip;
	}

	public bool CanPlayMusic()
	{
		int @int = PlayerPrefs.GetInt("datakey_101", ProjectConfig.canPlayMusic);
		return @int == 1;
	}

	public void SetCanPlayMusic(bool canPlay)
	{
		PlayerPrefs.SetInt("datakey_101", canPlay ? 1 : 0);
	}

	private AudioSource GetCanUseAudioSorce()
	{
		AudioSource audioSource = null;
		foreach (AudioSource item in m_AudioCantaner)
		{
			if (item != null && (item.clip == null || !item.isPlaying))
			{
				audioSource = item;
			}
		}
		if (audioSource != null)
		{
			return audioSource;
		}
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.loop = true;
		m_AudioCantaner.Add(audioSource);
		return audioSource;
	}

	private void PlayBacksound(string name, bool canPlay)
	{
		nowPlayLoopPath = name;
		AudioSource value = null;
		if (!canPlay)
		{
			if (m_WrokingAudioSource.TryGetValue(name, out value))
			{
				value.Stop();
				value.clip = null;
				m_WrokingAudioSource.Remove(name);
				Util.ClearMemory();
			}
		}
		else
		{
			if (m_WrokingAudioSource.ContainsKey(name))
			{
				return;
			}
			value = GetCanUseAudioSorce();
			if (!(value == null))
			{
				AudioClip audioClip = LoadAudioClip(name);
				if (!(audioClip == null))
				{
					value.clip = audioClip;
					value.Play();
					m_WrokingAudioSource.Add(name, value);
				}
			}
		}
	}

	public void PlayNowLoopSound(bool isPlay = true)
	{
		if (!string.IsNullOrEmpty(nowPlayLoopPath))
		{
			PlayBacksound(nowPlayLoopPath, isPlay);
		}
	}

	public bool CanPlaySound()
	{
		int @int = PlayerPrefs.GetInt("datakey_100", ProjectConfig.canPlaySound);
		return @int == 1;
	}

	public void SetCanPlaySound(bool canPlay)
	{
		PlayerPrefs.SetInt("datakey_100", canPlay ? 1 : 0);
	}

	private void Play(AudioClip clip, Vector3 position)
	{
		if (CanPlaySound())
		{
			AudioSource.PlayClipAtPoint(clip, position);
		}
	}

	private void PlayLoopEffectSound(string name, bool isStop = false)
	{
		AudioSource value = null;
		if (isStop)
		{
			if (m_WrokingAudioSource.TryGetValue(name, out value))
			{
				value.Stop();
				value.clip = null;
				m_WrokingAudioSource.Remove(name);
				Util.ClearMemory();
			}
		}
		else
		{
			if (!CanPlaySound() || m_WrokingAudioSource.ContainsKey(name))
			{
				return;
			}
			value = GetCanUseAudioSorce();
			if (!(value == null))
			{
				AudioClip audioClip = LoadAudioClip(name);
				if (!(audioClip == null))
				{
					value.clip = audioClip;
					value.Play();
					m_WrokingAudioSource.Add(name, value);
				}
			}
		}
	}

	public void StopAllBackGroundMusic()
	{
		if (m_WrokingAudioSource.Count < 1)
		{
			return;
		}
		foreach (string key in m_WrokingAudioSource.Keys)
		{
			AudioSource value = null;
			if (m_WrokingAudioSource.TryGetValue(key, out value))
			{
				value.Stop();
				value.clip = null;
			}
		}
		m_WrokingAudioSource.Clear();
		Util.ClearMemory();
	}

	private void PlaySound(string name, bool isLoop = false, bool isStop = false)
	{
		if (isLoop)
		{
			if (isStop)
			{
				PlayBacksound(name, false);
			}
			else
			{
				PlayBacksound(name, CanPlayMusic());
			}
			return;
		}
		AudioClip audioClip = LoadAudioClip(name);
		if (!(audioClip == null))
		{
			Play(audioClip, Camera.main.transform.position);
		}
	}

	public void PlayWordCuteSound(string name, bool isLoop = false, bool isStop = false)
	{
		string text = "Sound/" + name;
		PlaySound(text, isLoop, isStop);
	}
}
