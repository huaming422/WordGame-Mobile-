public class GameAsset
{
	public string abName;

	public string assetName;

	public GameAsset(string abName, string assetName)
	{
		this.abName = abName;
		this.assetName = assetName;
	}

	public override bool Equals(object obj)
	{
		GameAsset gameAsset = obj as GameAsset;
		if (gameAsset == null)
		{
			return false;
		}
		return gameAsset.abName == abName && gameAsset.assetName == assetName;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public bool EqualsKey(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		key = key.ToLower();
		return (abName + assetName).ToLower() == key;
	}
}
