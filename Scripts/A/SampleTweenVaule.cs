using UnityEngine;
using UnityEngine.Events;

public class SampleTweenVaule
{
	public static void ColorTween(MonoBehaviour contaner, Color startColor, Color targetColor, float time, UnityAction<Color> tweenThing, UnityAction finishCallback = null, int colorModel = 0)
	{
		TweenRunner<ColorTween> tweenRunner = new TweenRunner<ColorTween>();
		ColorTween info = default(ColorTween);
		info.startColor = startColor;
		info.targetColor = targetColor;
		info.duration = time;
		info.tweenThing = tweenThing;
		info.finshCallback = finishCallback;
		info.SetColorTweenMode(colorModel);
		tweenRunner.StartTween(contaner, info);
	}

	public static void FloatTween(MonoBehaviour contaner, float start, float target, float time, UnityAction<float> tweenThing, UnityAction finishCallback = null)
	{
		TweenRunner<FloatTween> tweenRunner = new TweenRunner<FloatTween>();
		FloatTween info = default(FloatTween);
		info.startValue = start;
		info.targetValue = target;
		info.duration = time;
		info.tweenThing = tweenThing;
		info.finshCallback = finishCallback;
		tweenRunner.StartTween(contaner, info);
	}

	public static void Float0To1Tween(MonoBehaviour contaner, float time, UnityAction<float> tweenThing, UnityAction finishCallback = null)
	{
		FloatTween(contaner, 0f, 1f, time, tweenThing, finishCallback);
	}

	public static void IntTween(MonoBehaviour contaner, int start, int target, float time, UnityAction<int> tweenThing, UnityAction finishCallback = null)
	{
		TweenRunner<IntTween> tweenRunner = new TweenRunner<IntTween>();
		IntTween info = default(IntTween);
		info.startValue = start;
		info.targetValue = target;
		info.duration = time;
		info.tweenThing = tweenThing;
		info.finshCallback = finishCallback;
		tweenRunner.StartTween(contaner, info);
	}
}
