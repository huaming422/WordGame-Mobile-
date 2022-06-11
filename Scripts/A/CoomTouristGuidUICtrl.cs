using UnityEngine;

public class CoomTouristGuidUICtrl : TouristGuideUICtrl
{
	public bool isAutoDestroy;

	public float dealyTime = 3f;

	private void Start()
	{
		if (touristGuideUI != null)
		{
			touristGuideUI.StartGuide(nowTouristGuideIndex, null);
		}
		Animation temp = GetComponent<Animation>();
		if (temp != null)
		{
			temp.ReserverPlay(string.Empty);
		}
		if (!isAutoDestroy)
		{
			return;
		}
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			float time = 0f;
			if (temp != null)
			{
				temp.OrderPlay(string.Empty);
				AnimationState animationState = temp[temp.clip.name];
				time = Mathf.Abs(animationState.length / animationState.speed);
			}
			SingleObject<SchudleManger>.instance.Schudle(DestroySelf, time);
		}, dealyTime);
	}

	public void OnClickButton()
	{
		TouristGuideManager.SetTouristGuideState(nowTouristGuideIndex, 1);
		Animation component = GetComponent<Animation>();
		if (component == null)
		{
			if (touristGuideUI != null)
			{
				touristGuideUI.EndGuide(nowTouristGuideIndex, null);
			}
			SingleObject<UIManager>.instance.Close(myToutistUIAsset);
			return;
		}
		component.OrderPlay(string.Empty);
		SingleObject<SchudleManger>.instance.Schudle(delegate
		{
			if (touristGuideUI != null)
			{
				touristGuideUI.EndGuide(nowTouristGuideIndex, null);
			}
			SingleObject<UIManager>.instance.Close(myToutistUIAsset);
		}, 0.5f);
	}

	private void DestroySelf()
	{
		TouristGuideManager.SetTouristGuideState(nowTouristGuideIndex, 1);
		if (touristGuideUI != null)
		{
			touristGuideUI.EndGuide(nowTouristGuideIndex, null);
		}
		SingleObject<UIManager>.instance.Close(myToutistUIAsset);
	}
}
