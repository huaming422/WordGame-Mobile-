using System.Collections.Generic;

public class GridState
{
	private struct NodeState
	{
		public int index;

		public string text;

		public NodeState(int index, string text)
		{
			this.index = index;
			this.text = text;
		}
	}

	private List<NodeState> states;

	public static void ShowState(List<DesignGridNode> nodes, GridState state)
	{
		if (nodes.IsEmpty())
		{
			return;
		}
		List<NodeState> list = state.states;
		if (!list.IsEmpty())
		{
			for (int i = 0; i < list.Count; i++)
			{
				nodes[list[i].index].SetText(list[i].text);
			}
		}
	}

	public static GridState GetState(List<DesignGridNode> nodes)
	{
		List<NodeState> list = new List<NodeState>();
		for (int i = 0; i < nodes.Count; i++)
		{
			NodeState item = new NodeState(i, nodes[i].data);
			list.Add(item);
		}
		GridState gridState = new GridState();
		gridState.states = list;
		return gridState;
	}
}
