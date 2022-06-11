internal interface ITweenValue
{
	bool ignoreTimeScale { get; }

	float duration { get; }

	void TweenValue(float percentage);

	bool ValidTarget();

	void Finish();
}
