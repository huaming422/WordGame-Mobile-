using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SerchFilter : SingleSysObj<SerchFilter>
{
	private ConditionNode _rootNode;

	public void BuildFilter(string filter)
	{
		_rootNode = new ConditionNode();
		BuildConditionNode(_rootNode, filter);
	}

	public bool CheckSrc(string src)
	{
		if (_rootNode == null)
		{
			return true;
		}
		return _rootNode.Check(src);
	}

	private void BuildConditionNode(ConditionNode parentNode, string src)
	{
		for (int i = 0; i < src.Length; i++)
		{
			if (src[i] != '(')
			{
				continue;
			}
			int num = ((i <= 0) ? (-1) : GetSymbolIndex(src[i - 1]));
			ConditionNode conditionNode = new ConditionNode();
			string backBrackContent = GetBackBrackContent(i, src);
			if (string.IsNullOrEmpty(backBrackContent))
			{
				return;
			}
			if (num == -1)
			{
				num = ((backBrackContent.Length + i >= src.Length - 2) ? 1 : GetSymbolIndex(src[backBrackContent.Length + i + 2]));
				if (num == -1)
				{
					num = 1;
				}
			}
			parentNode.AddChilNode(conditionNode, num == 1);
			BuildConditionNode(conditionNode, backBrackContent);
			src.Remove(i, backBrackContent.Length + 1);
		}
		List<string> mustExpre = GetMustExpre(src);
		AddConditons(parentNode, mustExpre, true);
		List<string> orExpre = GetOrExpre(src);
		AddConditons(parentNode, orExpre, false);
	}

	private int GetSymbolIndex(char sysmbol)
	{
		switch (sysmbol)
		{
		case '|':
			return 0;
		case '&':
			return 1;
		default:
			return -1;
		}
	}

	private string GetBackBrackContent(int startIndex, string src)
	{
		int num = 0;
		for (int i = startIndex; i < src.Length; i++)
		{
			if (src[i] == '(')
			{
				num++;
			}
			if (src[i] == ')')
			{
				num--;
				if (num == 0)
				{
					return src.Substring(startIndex + 1, i - startIndex);
				}
			}
		}
		return string.Empty;
	}

	private void AddConditons(ConditionNode node, List<string> conditons, bool isMust)
	{
		if (!conditons.IsEmpty())
		{
			for (int i = 0; i < conditons.Count; i++)
			{
				node.AddCondition(GetCondition(conditons[i]), isMust);
			}
		}
	}

	private List<string> GetMustExpre(string searchContent)
	{
		int num = searchContent.IndexOf("&");
		if (num == -1)
		{
			return null;
		}
		string[] array = searchContent.Split('&');
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split('|');
				list.Add(array2[0]);
			}
		}
		return list;
	}

	private List<string> GetOrExpre(string searchContent)
	{
		int num = searchContent.IndexOf("|");
		if (num == -1)
		{
			return null;
		}
		string[] array = searchContent.Split('|');
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split('&');
				list.Add(array2[0]);
			}
		}
		return list;
	}

	private Condition GetCondition(string conditon)
	{
		string[] array = conditon.Split(':');
		Func<string, string, bool> checker = null;
		switch (array[0])
		{
		case "d":
			checker = CheckIsDisContanis;
			break;
		case "dr":
			checker = CheckIsDisContanisNoRepeat;
			break;
		}
		if (Regex.IsMatch(array[0], "[0-9]"))
		{
			checker = CheckPosAlphabet;
		}
		return new Condition(checker, conditon);
	}

	private bool CheckLength(string conditon, string src)
	{
		string[] array = conditon.Split(':');
		int result = 0;
		int result2 = 0;
		if (array[1].IndexOf("-") != -1)
		{
			string[] array2 = array[1].Split('-');
			if (!int.TryParse(array2[0], out result))
			{
				return true;
			}
			if (!int.TryParse(array2[1], out result2))
			{
				return true;
			}
		}
		else
		{
			if (!int.TryParse(array[1], out result))
			{
				return true;
			}
			result2 = result;
		}
		return src.Length <= result2 && src.Length >= result;
	}

	private bool CheckPosAlphabet(string conditon, string src)
	{
		string[] array = conditon.Split(':');
		int num = int.Parse(array[0]);
		char c = array[1][0];
		if (num > src.Length)
		{
			return false;
		}
		return src[num - 1] == c;
	}

	private bool CheckIsDisContanis(string conditon, string src)
	{
		string[] array = conditon.Split(':');
		string text = array[1];
		for (int i = 0; i < src.Length; i++)
		{
			if (text.IndexOf(src[i]) == -1)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckIsDisContanisNoRepeat(string conditon, string src)
	{
		string[] array = conditon.Split(':');
		string text = array[1];
		for (int i = 0; i < src.Length; i++)
		{
			int num = text.IndexOf(src[i]);
			if (num == -1)
			{
				return false;
			}
			text = text.Remove(num, 1);
		}
		return true;
	}
}
