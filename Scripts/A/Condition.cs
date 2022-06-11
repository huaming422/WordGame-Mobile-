using System;

public struct Condition
{
	public bool result;

	public Func<string, string, bool> checker;

	public string condition;

	public Condition(Func<string, string, bool> checker, string condition)
	{
		result = true;
		this.checker = checker;
		this.condition = condition;
	}

	public bool Check(string src)
	{
		if (checker == null)
		{
			return result;
		}
		result = checker(condition, src);
		return result;
	}
}
