using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlphabetGirdCtrl : MonoBehaviour
{
	public Vector2 pading = new Vector2(10f, 0f);

	private AlphabetGrid _grid;

	private Vector2 _activeSize = Vector2.zero;

	private int _horiNodeCount;

	private int _vertNodeCount;

	private float _nodeWidth;

	public float nodeWidth
	{
		get
		{
			return _nodeWidth;
		}
	}

	public int hori
	{
		get
		{
			return _horiNodeCount;
		}
	}

	public int vert
	{
		get
		{
			return _vertNodeCount;
		}
	}

	private void Start()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		_activeSize = rectTransform.rect.size;
	}

	public void InitGrid(int horiCount, int vertCount)
	{
		_horiNodeCount = horiCount;
		_vertNodeCount = vertCount;
		int num = ((horiCount <= vertCount) ? vertCount : horiCount);
		_nodeWidth = _activeSize.x / (float)num;
		if (_nodeWidth > 150f)
		{
			_nodeWidth = 150f;
		}
		float num2 = _activeSize.x - _nodeWidth * (float)horiCount;
		float num3 = _activeSize.y - _nodeWidth * (float)vertCount;
		Vector3 startPos = new Vector3((0f - _activeSize.x) / 2f + _nodeWidth / 2f + num2 / 2f, _activeSize.y / 2f - _nodeWidth / 2f - num3 / 2f);
		_grid = new AlphabetGrid(startPos, horiCount, vertCount, _nodeWidth, _nodeWidth);
	}

	public void OprationAllNodes(UnityAction<AlphabetTableItem> action)
	{
		if (_grid == null)
		{
			return;
		}
		List<AlphabetTableItem> nodes = _grid.nodes;
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] != null)
			{
				action.MyInvoke(nodes[i]);
			}
		}
	}

	public AlphabetTableItem GetItemByPosIndex(int posIndex)
	{
		if (_grid == null)
		{
			return null;
		}
		List<AlphabetTableItem> nodes = _grid.nodes;
		if (posIndex >= nodes.Count)
		{
			return null;
		}
		return nodes[posIndex];
	}

	public List<AlphabetTableItem> GetAllNodes(bool isContanUnUse = false)
	{
		if (_grid == null)
		{
			return null;
		}
		if (isContanUnUse)
		{
			return _grid.nodes;
		}
		List<AlphabetTableItem> list = new List<AlphabetTableItem>();
		for (int i = 0; i < _grid.nodes.Count; i++)
		{
			if (!(_grid.nodes[i].obj == null))
			{
				list.Add(_grid.nodes[i]);
			}
		}
		return list;
	}
}
