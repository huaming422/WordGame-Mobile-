using System.Collections.Generic;
using UnityEngine;

public class DesignGrid : SuperGird<DesignGridNode>
{
	private List<DesignGridNode> _allnodes;

	public List<DesignGridNode> nodes
	{
		get
		{
			if (_allnodes != null)
			{
				return _allnodes;
			}
			CreteAllAlphabetNodes();
			return _allnodes;
		}
	}

	public List<List<DesignGridNode>> gridNodes
	{
		get
		{
			return _gridNodes;
		}
	}

	public DesignGrid(Vector3 startPos, int horiCount, int vertCount, float nodeWith, float nodeHeight)
	{
		_startPos = startPos;
		_horiNodeCount = horiCount;
		_vertNodeCount = vertCount;
		_gridNodeWidth = nodeWith;
		_gridNodeHeight = nodeHeight;
		CreateGridNodes();
	}

	private void CreteAllAlphabetNodes()
	{
		if (_gridNodes != null)
		{
			_allnodes = new List<DesignGridNode>();
			for (int i = 0; i < _gridNodes.Count; i++)
			{
				_allnodes.AddRange(_gridNodes[i]);
			}
		}
	}
}
