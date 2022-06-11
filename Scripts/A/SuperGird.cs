using System.Collections.Generic;
using UnityEngine;

public class SuperGird<T> where T : GridNode, new()
{
	protected float _gridNodeWidth;

	protected float _gridNodeHeight;

	protected int _horiNodeCount;

	protected int _vertNodeCount;

	protected int _dataHoriNodeCount;

	protected int _dataVertNodeCount;

	protected Vector3 _startPos = Vector3.zero;

	protected int _startDataIndex;

	protected Vector4 _activeSize = Vector4.zero;

	protected List<List<T>> _gridNodes;

	protected ISupuerGridDataCtrl _dataCtrl;

	public int vertiCount
	{
		get
		{
			return _vertNodeCount;
		}
	}

	public int horiCount
	{
		get
		{
			return _horiNodeCount;
		}
	}

	public float gridNodeWidth
	{
		get
		{
			return _gridNodeWidth;
		}
	}

	public float gridNodeHeight
	{
		get
		{
			return _gridNodeHeight;
		}
	}

	public T this[int vert, int hori]
	{
		get
		{
			if (_gridNodes != null)
			{
				if (hori >= _horiNodeCount || vert >= _vertNodeCount)
				{
					return (T)null;
				}
				return _gridNodes[vert][hori];
			}
			return (T)null;
		}
		set
		{
			if (_gridNodes != null && hori < _horiNodeCount && vert < _vertNodeCount)
			{
				_gridNodes[vert][hori] = value;
			}
		}
	}

	public T head
	{
		get
		{
			if (_gridNodes == null)
			{
				return (T)null;
			}
			return _gridNodes[0][0];
		}
	}

	public T tail
	{
		get
		{
			if (_gridNodes == null)
			{
				return (T)null;
			}
			return _gridNodes[_vertNodeCount - 1][_horiNodeCount - 1];
		}
	}

	public SuperGird()
	{
	}

	public SuperGird(ISupuerGridDataCtrl dataCtrl, int dataHoriCount, int datavertCount)
	{
		_dataCtrl = dataCtrl;
		_dataHoriNodeCount = dataHoriCount;
		_dataVertNodeCount = datavertCount;
	}

	public void SetGridData(Vector4 activeSize, float width, float height, Vector3 startPos, int startDataIndex = 0)
	{
		_activeSize = activeSize;
		_horiNodeCount = Mathf.CeilToInt((activeSize.w - activeSize.z) / width);
		_vertNodeCount = Mathf.CeilToInt((activeSize.x - activeSize.y) / height);
		_gridNodeWidth = width;
		_gridNodeHeight = height;
		_startPos = startPos;
		_startDataIndex = startDataIndex;
		_gridNodes = null;
	}

	public void CreateGridNodes()
	{
		_gridNodes = new List<List<T>>();
		for (int i = 0; i < _vertNodeCount; i++)
		{
			List<T> list = new List<T>();
			for (int j = 0; j < _horiNodeCount; j++)
			{
				T val = new T
				{
					horiPosIndex = j,
					vertPosIndex = i,
					width = _gridNodeWidth,
					height = _gridNodeHeight,
					pos = new Vector3((float)j * _gridNodeWidth + _startPos.x, _startPos.y - (float)i * _gridNodeHeight, _startPos.z),
					dataIndex = _startDataIndex + (i * _dataHoriNodeCount + j)
				};
				if (_dataCtrl != null)
				{
					_dataCtrl.DoCreateNode(val);
				}
				list.Add(val);
			}
			_gridNodes.Add(list);
		}
	}

	public T GetNodeByDataIndex(int dataIndex)
	{
		if (_gridNodes == null)
		{
			return (T)null;
		}
		for (int i = 0; i < _vertNodeCount; i++)
		{
			for (int j = 0; j < _horiNodeCount; j++)
			{
				if (_gridNodes[i][j].dataIndex == dataIndex)
				{
					return _gridNodes[i][j];
				}
			}
		}
		return (T)null;
	}

	public Vector3 MoveGird(Vector3 distance)
	{
		if (_gridNodes == null)
		{
			return distance;
		}
		distance = GetCanMoveDistance(distance);
		for (int i = 0; i < _vertNodeCount; i++)
		{
			for (int j = 0; j < _horiNodeCount; j++)
			{
				_gridNodes[i][j].pos += distance;
				if (_dataCtrl != null)
				{
					_dataCtrl.DoMoveNode(_gridNodes[i][j]);
				}
			}
		}
		if (distance.x == 0f && distance.y == 0f)
		{
			return distance;
		}
		CheckAndChangePos(distance);
		return distance;
	}

	private Vector3 GetCanMoveDistance(Vector3 distance)
	{
		T val = head;
		T val2 = tail;
		int dataIndex = val.dataIndex;
		int dataIndex2 = val2.dataIndex;
		if (Mathf.Abs(distance.x) > _gridNodeWidth)
		{
			distance.x = Mathf.Sign(distance.x) * _gridNodeWidth;
		}
		if (Mathf.Abs(distance.y) > _gridNodeHeight)
		{
			distance.y = Mathf.Sign(distance.y) * _gridNodeHeight;
		}
		if (dataIndex % _dataHoriNodeCount == 0 && val.pos.x - _gridNodeWidth / 2f + distance.x >= _activeSize.z)
		{
			distance.x = _activeSize.z - (val.pos.x - _gridNodeWidth / 2f);
		}
		if (dataIndex2 % _dataHoriNodeCount == _dataHoriNodeCount - 1 && val2.pos.x + _gridNodeWidth / 2f + distance.x <= _activeSize.w && distance.x < 0f)
		{
			distance.x = _activeSize.w - (val2.pos.x + _gridNodeWidth / 2f);
		}
		if (dataIndex / _dataHoriNodeCount == 0 && val.pos.y + _gridNodeWidth / 2f + distance.y <= _activeSize.x && distance.y < 0f)
		{
			distance.y = _activeSize.x - (val.pos.y + _gridNodeWidth / 2f);
		}
		if (dataIndex2 / _dataHoriNodeCount == _dataVertNodeCount - 1 && val2.pos.y - _gridNodeHeight / 2f + distance.y >= _activeSize.y && distance.y > 0f)
		{
			distance.y = _activeSize.y - (val2.pos.y - _gridNodeHeight / 2f);
		}
		return distance;
	}

	public void CheckAndChangePos(Vector3 distance)
	{
		T val = head;
		T val2 = tail;
		int dataIndex = val.dataIndex;
		int dataIndex2 = val2.dataIndex;
		if (dataIndex % _dataHoriNodeCount > 0 && val.pos.x - _gridNodeWidth / 2f > _activeSize.z && distance.x > 0f)
		{
			DoChangeHori(_horiNodeCount - 1, 0);
		}
		if (dataIndex2 % _dataHoriNodeCount < _dataHoriNodeCount - 1 && val2.pos.x + _gridNodeWidth / 2f < _activeSize.w && distance.x < 0f)
		{
			DoChangeHori(0, _horiNodeCount - 1);
		}
		if (dataIndex / _dataHoriNodeCount > 0 && val.pos.y + _gridNodeWidth / 2f < _activeSize.x && distance.y < 0f)
		{
			DoChangeVert(_vertNodeCount - 1, 0);
		}
		if (dataIndex2 / _dataHoriNodeCount < _dataVertNodeCount - 1 && val2.pos.y - _gridNodeHeight / 2f > _activeSize.y && distance.y > 0f)
		{
			DoChangeVert(0, _vertNodeCount - 1);
		}
	}

	private void DoChangeHori(int src, int des)
	{
		float num = ((src <= des) ? tail.pos.x : head.pos.x);
		int num2 = ((src <= des) ? 1 : (-1));
		for (int i = 0; i < _vertNodeCount; i++)
		{
			List<T> list = _gridNodes[i];
			int dataIndex = list[des].dataIndex;
			T val = list[src];
			list.RemoveAt(src);
			if (des >= list.Count)
			{
				list.Add(val);
			}
			if (des - 1 < 0)
			{
				list.Insert(0, val);
			}
			val.pos.x = num + (float)num2 * _gridNodeWidth;
			val.dataIndex = dataIndex + num2;
			if (_dataCtrl != null)
			{
				_dataCtrl.DoChangeDataIndex(val);
			}
		}
	}

	private void DoChangeVert(int src, int des)
	{
		float num = ((src <= des) ? tail.pos.y : head.pos.y);
		int num2 = ((src > des) ? 1 : (-1));
		List<T> list = _gridNodes[src];
		List<T> list2 = ((src <= des) ? _gridNodes[_gridNodes.Count - 1] : _gridNodes[0]);
		_gridNodes.RemoveAt(src);
		if (des >= _gridNodes.Count)
		{
			_gridNodes.Add(list);
		}
		if (des - 1 < 0)
		{
			_gridNodes.Insert(0, list);
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].pos.y = num + (float)num2 * _gridNodeHeight;
			list[i].dataIndex = list2[i].dataIndex - num2 * _dataHoriNodeCount;
			if (_dataCtrl != null)
			{
				_dataCtrl.DoChangeDataIndex(list[i]);
			}
		}
	}

	public virtual GridNode GetNodeByPos(Vector3 position)
	{
		if (position.x < _activeSize.z || position.x > _activeSize.w || position.y > _activeSize.x || position.y < _activeSize.y)
		{
			return null;
		}
		int num = Mathf.FloorToInt(Mathf.Abs(position.x + _gridNodeWidth / 2f - head.pos.x) / _gridNodeWidth);
		int num2 = Mathf.FloorToInt(Mathf.Abs(position.y - _gridNodeWidth / 2f - head.pos.y) / _gridNodeWidth);
		if (num > _horiNodeCount || num2 >= _vertNodeCount)
		{
			return null;
		}
		return _gridNodes[num2][num];
	}

	public virtual void DoScale(float scaleRate, float nodeStartWidth, float nodeStartHeight)
	{
		GridNode nodeByPos = GetNodeByPos(Vector3.zero);
		int num = Mathf.Abs(Mathf.RoundToInt((nodeByPos.pos.x - head.pos.x) / _gridNodeWidth));
		int num2 = Mathf.Abs(Mathf.RoundToInt((nodeByPos.pos.y - head.pos.y) / _gridNodeWidth));
		float num3 = (float)num * nodeStartWidth * scaleRate;
		float num4 = (float)num2 * nodeStartHeight * scaleRate;
		float newX = nodeByPos.pos.x - num3;
		float newY = nodeByPos.pos.y + num4;
		_gridNodeWidth = nodeStartWidth * scaleRate;
		_gridNodeHeight = nodeStartHeight * scaleRate;
		_startPos.Set(newX, newY, _startPos.z);
		SetRealStartPos();
		RebuildGrid(_startPos);
	}

	protected virtual void SetRealStartPos()
	{
		float num = (float)_horiNodeCount * _gridNodeWidth;
		float num2 = (float)_vertNodeCount * _gridNodeHeight;
		T val = head;
		T val2 = tail;
		int dataIndex = val.dataIndex;
		int dataIndex2 = val2.dataIndex;
		if (_startPos.x > _activeSize.z + _gridNodeWidth / 2f)
		{
			if (dataIndex % _dataHoriNodeCount > 0)
			{
				DoChangeHori(_horiNodeCount - 1, 0);
				_startPos[0] -= _gridNodeWidth;
			}
			else
			{
				_startPos[0] = _activeSize.z + _gridNodeWidth / 2f;
			}
		}
		if (_startPos.x + num < _activeSize.w + _gridNodeWidth / 2f)
		{
			if (dataIndex2 % _dataHoriNodeCount < _dataHoriNodeCount - 1)
			{
				DoChangeHori(0, _horiNodeCount - 1);
				_startPos[0] += _gridNodeWidth;
			}
			else
			{
				_startPos[0] = _activeSize.w + _gridNodeWidth / 2f - num;
			}
		}
		if (_startPos.y < _activeSize.x - _gridNodeHeight / 2f)
		{
			if (dataIndex / _dataHoriNodeCount > 0)
			{
				DoChangeVert(_vertNodeCount - 1, 0);
				_startPos[1] += _gridNodeHeight;
			}
			else
			{
				_startPos[1] = _activeSize.x - _gridNodeHeight / 2f;
			}
		}
		if (_startPos.y - num2 > _activeSize.y - _gridNodeHeight / 2f)
		{
			if (dataIndex2 / _dataHoriNodeCount < _dataVertNodeCount - 1)
			{
				DoChangeVert(0, _vertNodeCount - 1);
				_startPos[1] -= _gridNodeHeight;
			}
			else
			{
				_startPos[1] = _activeSize.y - _gridNodeHeight / 2f + num2;
			}
		}
	}

	public virtual void RebuildGrid(Vector3 startPos)
	{
		if (_gridNodes == null)
		{
			return;
		}
		_startPos = startPos;
		for (int i = 0; i < _vertNodeCount; i++)
		{
			for (int j = 0; j < _horiNodeCount; j++)
			{
				GridNode gridNode = _gridNodes[i][j];
				gridNode.width = _gridNodeWidth;
				gridNode.height = _gridNodeHeight;
				gridNode.horiPosIndex = j;
				gridNode.vertPosIndex = i;
				gridNode.pos = new Vector3((float)j * _gridNodeWidth + _startPos.x, _startPos.y - (float)i * _gridNodeHeight, _startPos.z);
				if (_dataCtrl != null)
				{
					_dataCtrl.DoRebuildGird(gridNode);
				}
			}
		}
	}

	public virtual void RebuildGrid(Vector3 startPos, int startDataIndex, float nodeWidth, float nodeHeight)
	{
		if (_gridNodes == null)
		{
			return;
		}
		_startPos = startPos;
		_gridNodeWidth = nodeWidth;
		_gridNodeHeight = nodeHeight;
		_startDataIndex = startDataIndex;
		for (int i = 0; i < _vertNodeCount; i++)
		{
			for (int j = 0; j < _horiNodeCount; j++)
			{
				GridNode gridNode = _gridNodes[i][j];
				gridNode.width = _gridNodeWidth;
				gridNode.height = _gridNodeHeight;
				gridNode.horiPosIndex = j;
				gridNode.vertPosIndex = i;
				gridNode.pos = new Vector3((float)j * _gridNodeWidth + _startPos.x, _startPos.y - (float)i * _gridNodeHeight, _startPos.z);
				gridNode.dataIndex = _startDataIndex + (i * _dataHoriNodeCount + j);
				if (_dataCtrl != null)
				{
					_dataCtrl.DoRebuildGird(gridNode);
				}
			}
		}
	}

	public virtual void DrawGrid(Transform parent, Material m_LineMaterial)
	{
		if (_gridNodes == null || _dataCtrl == null)
		{
			return;
		}
		m_LineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(parent.localToWorldMatrix);
		for (int i = 0; i < _vertNodeCount; i++)
		{
			for (int j = 0; j < _horiNodeCount; j++)
			{
				GridNode gridNode = _gridNodes[i][j];
				if (_dataCtrl.NeedDrawGrid(gridNode))
				{
					gridNode.UpdateBunods();
					GL.Begin(2);
					GL.Vertex(gridNode.leup);
					GL.Vertex(gridNode.reup);
					GL.Vertex(gridNode.redow);
					GL.Vertex(gridNode.ledow);
					GL.Vertex(gridNode.leup);
					GL.End();
				}
			}
		}
		GL.PopMatrix();
	}
}
