using System;

public class SampleCodeTools
{
	public static string[] baseComnonTypes = new string[8] { "float", "double", "char", "byte", "short", "int", "long", "string" };

	public static bool IsBaseType(string type)
	{
		if (Array.IndexOf(baseComnonTypes, type) == -1)
		{
			return false;
		}
		return true;
	}
}
