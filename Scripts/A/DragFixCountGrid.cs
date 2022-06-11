using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragFixCountGrid : MonoBehaviour, ISupuerGridDataCtrl, IScrollHandler, IEventSystemHandler
{
	public SuperGird<ColorGridNode> grid;

	private bool _gridIsCreated;

	private RectTransform _viewRectTransform;

	private Vector4 _activeSize = Vector4.zero;

	private GridOpreationType _opreationType;

	private GameObject nodePrefab;

	private int dataHoriCount;

	private int dataVertCount;

	public float nodeStartWidth;

	public float nodeStartHeight;

	private float nowScale = 1f;

	public UnityAction<Vector3> onGridOpreationStart;

	public UnityAction<ColorGridNode> onGridNodeCreate;

	public UnityAction<ColorGridNode> onGridNodeUpdateDataIndex;

	public UnityAction<Vector3> onGridDrag;

	public UnityAction<Vector3> onClickGrid;

	public UnityAction<Vector3> onLongTochGrid;

	public UnityAction<float> onScaleGrid;

	public UnityAction<float> onScroll;

	public UnityAction<GridOpreationType> onGridOpretionOver;

	private Vector3 _dragendSpeed = Vector3.zero;

	private Vector3 _nowSpeed = Vector3.zero;

	private Vector2 _pointerStartPos = Vector2.zero;

	private Vector3 _pointerLastFrmaePos = Vector3.zero;

	private bool _isWaitPointerUp;

	private Camera _uiCamera;

	private float _pointerDownTime;

	private float _scaleStartDistance;

	private float _pointerDownTimeIsLongToch = 0.3f;

	private float _scrennWidthMiss;

	private float _gridShowMinNodeWidth;

	public RectTransform panleRect
	{
		get
		{
			return _viewRectTransform;
		}
	}

	public Vector4 activeSize
	{
		get
		{
			return _activeSize;
		}
	}

	public bool gridIsCreated
	{
		get
		{
			return _gridIsCreated;
		}
	}

	public Vector3 rectLeftUpPos
	{
		get
		{
			Vector2 sizeDelta = _viewRectTransform.sizeDelta;
			sizeDelta.Set((0f - sizeDelta.x) / 2f, sizeDelta.y / 2f);
			return sizeDelta;
		}
	}

	private void Start()
	{
		_uiCamera = Camera.main;
		_scrennWidthMiss = (float)Screen.width * 0.01f;
		_viewRectTransform = GetComponent<RectTransform>();
		Vector2 sizeDelta = _viewRectTransform.sizeDelta;
		_activeSize = new Vector4(sizeDelta.y / 2f, (0f - sizeDelta.y) / 2f, (0f - sizeDelta.x) / 2f, sizeDelta.x / 2f);
	}

	public void SetGridShowMinNodeWidth(float nodeWidth)
	{
		_gridShowMinNodeWidth = nodeWidth;
	}

	public void DoScale(float value)
	{
		nowScale = value;
		grid.DoScale(value, nodeStartWidth, nodeStartWidth);
	}

	public void CreateGrid(GameObject nodePrefab, int dataHoriCount, int dataVertCount, float nodeWith, float nodeHeight, int startDataIndex, Vector3 startPos)
	{
		this.nodePrefab = nodePrefab;
		this.dataHoriCount = dataHoriCount;
		this.dataVertCount = dataVertCount;
		nodeStartWidth = nodeWith;
		nodeStartHeight = nodeHeight;
		grid = new SuperGird<ColorGridNode>(this, dataHoriCount, dataVertCount);
		grid.SetGridData(_activeSize, nodeWith, nodeHeight, startPos, startDataIndex);
		grid.CreateGridNodes();
		_gridIsCreated = true;
	}

	public void UpdateGrid(Vector3 startPos, int startDataIndex, float gridNodeWidth, float gridNodeHeight)
	{
		grid.RebuildGrid(startPos, startDataIndex, gridNodeWidth, gridNodeHeight);
	}

	public void DoChangeDataIndex(GridNode node)
	{
		ColorGridNode colorGridNode = node as ColorGridNode;
		colorGridNode.UpdateTouchObjPos();
		colorGridNode.UpdateText();
		onGridNodeUpdateDataIndex.MyInvoke(colorGridNode);
	}

	public void DoMoveNode(GridNode node)
	{
		ColorGridNode colorGridNode = node as ColorGridNode;
		colorGridNode.UpdateTouchObjPos();
	}

	public void DoRebuildGird(GridNode node)
	{
		ColorGridNode colorGridNode = node as ColorGridNode;
		RectTransform rectTransform = colorGridNode.obj as RectTransform;
		rectTransform.sizeDelta = new Vector2(node.width, node.height);
		rectTransform.localPosition = node.pos;
		colorGridNode.ScaleText(nowScale);
		onGridNodeUpdateDataIndex.MyInvoke(colorGridNode);
	}

	public void DoCreateNode(GridNode node)
	{
		if (!(nodePrefab == null))
		{
			ColorGridNode colorGridNode = node as ColorGridNode;
			GameObject gameObject = UnityEngine.Object.Instantiate(nodePrefab);
			gameObject.transform.SetParent(base.transform, false);
			colorGridNode.Init(gameObject.transform);
			RectTransform rectTransform = colorGridNode.obj as RectTransform;
			rectTransform.sizeDelta = new Vector2(node.width, node.height);
			colorGridNode.ScaleText(nowScale);
			onGridNodeCreate.MyInvoke(colorGridNode);
		}
	}

	public bool NeedDrawGrid(GridNode node)
	{
		return true;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && !_isWaitPointerUp)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			EventSystem.current.RaycastAll(pointerEventData, list);
			if (list.Count != 0 && !(list[0].gameObject != base.gameObject))
			{
				_isWaitPointerUp = true;
				_pointerStartPos = Input.mousePosition;
				_pointerDownTime = Time.realtimeSinceStartup;
				onGridOpreationStart.MyInvoke(Input.mousePosition);
			}
		}
		else
		{
			if (!_isWaitPointerUp)
			{
				return;
			}
			if (_opreationType == GridOpreationType.None)
			{
				if (Input.touchCount > 1)
				{
					if (_opreationType == GridOpreationType.None)
					{
						_opreationType = GridOpreationType.Scale;
						Vector3 b = Input.GetTouch(0).position;
						Vector3 a = Input.GetTouch(1).position;
						_scaleStartDistance = Vector3.Distance(a, b);
						return;
					}
				}
				else
				{
					float num = Time.realtimeSinceStartup - _pointerDownTime;
					float num2 = Vector3.Distance(Input.mousePosition, _pointerStartPos);
					if (num2 > _scrennWidthMiss)
					{
						RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRectTransform, Input.mousePosition, _uiCamera, out _pointerStartPos);
						_opreationType = GridOpreationType.Drag;
					}
					if (num > _pointerDownTimeIsLongToch && num2 < _scrennWidthMiss)
					{
						_opreationType = GridOpreationType.LongTouch;
					}
				}
			}
			else
			{
				if (_opreationType == GridOpreationType.Drag)
				{
					DoDrag(Input.mousePosition);
				}
				if (_opreationType == GridOpreationType.LongTouch)
				{
					DoLongTouch(Input.mousePosition);
				}
				if (_opreationType == GridOpreationType.Scale && Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
				{
					Vector3 b2 = Input.GetTouch(0).position;
					Vector3 a2 = Input.GetTouch(1).position;
					float num3 = Vector3.Distance(a2, b2);
					float scale = num3 / _scaleStartDistance;
					DoOpretionScale(scale);
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				if (_opreationType == GridOpreationType.None)
				{
					_opreationType = GridOpreationType.Click;
					DoClick(Input.mousePosition);
				}
				_isWaitPointerUp = false;
				onGridOpretionOver.MyInvoke(_opreationType);
				_opreationType = GridOpreationType.None;
			}
		}
	}

	private void DoDrag(Vector3 pointerPos)
	{
		Vector2 localPoint = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRectTransform, pointerPos, _uiCamera, out localPoint))
		{
			Vector2 vector = localPoint - _pointerStartPos;
			_pointerStartPos = localPoint;
			_nowSpeed.Set(vector.x, vector.y, _nowSpeed.z);
			onGridDrag.MyInvoke(_nowSpeed);
		}
	}

	private void DoOpretionScale(float scale)
	{
		onScaleGrid.MyInvoke(scale);
	}

	private void DoLongTouch(Vector3 pos)
	{
		if (!(Vector3.Distance(pos, _pointerLastFrmaePos) < _scrennWidthMiss))
		{
			_pointerLastFrmaePos = pos;
			Vector3 position = _uiCamera.ScreenToWorldPoint(pos);
			Vector3 obj = _viewRectTransform.InverseTransformPoint(position);
			onLongTochGrid.MyInvoke(obj);
		}
	}

	private void DoClick(Vector3 pos)
	{
		Vector3 position = _uiCamera.ScreenToWorldPoint(pos);
		Vector3 obj = _viewRectTransform.InverseTransformPoint(position);
		onClickGrid.MyInvoke(obj);
	}

	public ColorGridNode GetNodeByPos(Vector3 pos)
	{
		if (grid == null)
		{
			return null;
		}
		return grid.GetNodeByPos(pos) as ColorGridNode;
	}

	public Vector3 GetPosByDataIndex(int dataIndex)
	{
		ColorGridNode nodeByDataIndex = grid.GetNodeByDataIndex(dataIndex);
		if (nodeByDataIndex == null)
		{
			return Vector3.zero;
		}
		return nodeByDataIndex.obj.position;
	}

	public void OnScroll(PointerEventData eventData)
	{
		float y = eventData.scrollDelta.y;
		if (y > 0f)
		{
			onScroll.MyInvoke(1f + y / 5f);
			return;
		}
		y = Math.Abs(y);
		onScroll.MyInvoke(1f - y / (y + 4f));
	}
}
