namespace TapjoyUnity.Internal.UnityCompat
{
	public class JsonUtilityCompat
	{
		internal static string ToJson(object obj)
		{
			if (UnityDependency.ToJson != null)
			{
				return UnityDependency.ToJson(obj);
			}
			return "";
		}
	}
}
