using System.Collections;

public class DictionaryEntryComparer : IComparer
{
	private CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();

	public int Compare(object x, object y)
	{
		DictionaryEntry dictionaryEntry = (DictionaryEntry)x;
		DictionaryEntry dictionaryEntry2 = (DictionaryEntry)y;
		return comparer.Compare(dictionaryEntry.Key, dictionaryEntry2.Key);
	}
}
