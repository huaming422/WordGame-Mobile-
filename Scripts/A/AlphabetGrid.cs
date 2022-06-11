using System.Collections.Generic;
using UnityEngine;

public class AlphabetGrid : SuperGird<AlphabetTableItem>
{
	private List<AlphabetTableItem> _allnodes;

	public List<AlphabetTableItem> nodes
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

	public List<List<AlphabetTableItem>> gridNodes
	{
		get
		{
			return _gridNodes;
		}
	}

	public AlphabetGrid(Vector3 startPos, int horiCount, int vertCount, float nodeWith, float nodeHeight)
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
			_allnodes = new List<AlphabetTableItem>();
			for (int i = 0; i < _gridNodes.Count; i++)
			{
				_allnodes.AddRange(_gridNodes[i]);
			}
		}
	}
}
