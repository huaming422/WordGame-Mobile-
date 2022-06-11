using System.Collections;
using System.Collections.Generic;

public class KeyValuePairComparer : IComparer<KeyValuePair<string, string>>
{
	private CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();

	public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
	{
		return comparer.Compare(x.Key, y.Key);
	}
}
