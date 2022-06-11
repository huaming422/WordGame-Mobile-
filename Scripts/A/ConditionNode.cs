using System.Collections.Generic;

public class ConditionNode
{
	private bool _result = true;

	private List<Condition> _mustCondition = new List<Condition>();

	private List<Condition> _orCondition = new List<Condition>();

	private List<ConditionNode> _mustChildNodes = new List<ConditionNode>();

	private List<ConditionNode> _orChildNodes = new List<ConditionNode>();

	public void AddChilNode(ConditionNode node, bool isMust)
	{
		if (isMust)
		{
			_mustChildNodes.Add(node);
		}
		else
		{
			_orChildNodes.Add(node);
		}
	}

	public void AddChilNodes(List<ConditionNode> nodes, bool isMust)
	{
		if (!nodes.IsEmpty())
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				AddChilNode(nodes[i], isMust);
			}
		}
	}

	public void AddCondition(Condition condition, bool isMust)
	{
		if (isMust)
		{
			_mustCondition.Add(condition);
		}
		else
		{
			_orCondition.Add(condition);
		}
	}

	public void AddConditions(List<Condition> conditions, bool isMust)
	{
		if (!conditions.IsEmpty())
		{
			for (int i = 0; i < conditions.Count; i++)
			{
				AddCondition(conditions[i], isMust);
			}
		}
	}

	public bool Check(string src)
	{
		_result = _orCondition.Count > 0 && CheckOrConditons(src);
		if (_result)
		{
			return _result;
		}
		_result = _orChildNodes.Count > 0 && CheckOrChildNodes(src);
		if (_result)
		{
			return _result;
		}
		_result = _mustChildNodes.Count == 0 && CheckMustCondtions(src);
		if (_result)
		{
			return _result;
		}
		_result = _mustCondition.Count == 0 && CheckMustChildNodes(src);
		if (_result)
		{
			return _result;
		}
		_result = CheckMustCondtions(src) && CheckMustChildNodes(src);
		return _result;
	}

	private bool CheckOrConditons(string src)
	{
		for (int i = 0; i < _orCondition.Count; i++)
		{
			if (_orCondition[i].Check(src))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMustCondtions(string src)
	{
		for (int i = 0; i < _mustCondition.Count; i++)
		{
			if (!_mustCondition[i].Check(src))
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckOrChildNodes(string src)
	{
		for (int i = 0; i < _orChildNodes.Count; i++)
		{
			if (_orChildNodes[i].Check(src))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckMustChildNodes(string src)
	{
		for (int i = 0; i < _mustChildNodes.Count; i++)
		{
			if (!_mustChildNodes[i].Check(src))
			{
				return false;
			}
		}
		return true;
	}
}
