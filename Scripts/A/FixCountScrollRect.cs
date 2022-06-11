using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixCountScrollRect : FixCountScorll, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	private bool _isMoveEnd;

	private Vector3 _nowSpeed = Vector3.zero;

	private Vector2 _curStartPos = Vector2.zero;

	private Camera _mainCamera;

	private bool _isDraging;

	private Vector2 _preCurPost = Vector2.zero;

	private Vector3 _dragendSpeed = Vector3.zero;

	private int aixs;

	protected override void ChildAwake()
	{
		base.ChildAwake();
		_mainCamera = Camera.main;
		_viewRectTransform = GetComponent<RectTransform>();
		aixs = ((aspect != 0) ? 1 : 0);
	}

	public override void ReInit()
	{
		base.ReInit();
		_nowSpeed = Vector3.zero;
	}

	public void JumpToPos(int itemIndex)
	{
		_nowSpeed = Vector3.zero;
		List<FixedCountChildItem> list = _allChildItems.MyClone();
		_allChildItems.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			int num = 0;
			if (dataCtrl.CanUpdateChildItemDataIndex(itemIndex - i))
			{
				num = itemIndex - i;
				SetChildPos(i, list[i].item);
				_allChildItems.Add(list[i]);
			}
			else
			{
				num = itemIndex + i;
				SetChildPos(-i, list[i].item);
				_allChildItems.Insert(0, list[i]);
			}
			list[i].dataIndex = num;
			dataCtrl.CreateChildItem(num, list[i].item);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && CanMove())
		{
			_curStartPos = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRectTransform, eventData.position, eventData.pressEventCamera, out _curStartPos);
			_isDraging = true;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			_isMoveEnd = false;
			_isDraging = false;
			_dragendSpeed = _nowSpeed;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != 0 || !CanMove())
		{
			return;
		}
		Vector2 localPoint = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewRectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
		{
			Vector2 offset = localPoint - _curStartPos;
			_curStartPos = localPoint;
			if (_isMoveEnd)
			{
				offset = GetMoveEndSpeed(offset);
			}
			_nowSpeed.Set(offset.x, offset.y, _nowSpeed.z);
			Move(_nowSpeed);
		}
	}

	protected override void OnMoveStateChange(bool isAdd, bool isEnd)
	{
		base.OnMoveStateChange(isAdd, isEnd);
		_isMoveEnd = isEnd;
	}

	public bool CanMove()
	{
		return base.isActiveAndEnabled && _allChildItems.Count > 0;
	}

	private Vector2 GetMoveEndSpeed(Vector2 offset)
	{
		float num = 0f;
		if (aspect == FixCountScorllAspect.vertical)
		{
			num = ((_allChildItems.Count < _targetChildCount) ? Mathf.Abs(_scorllSize.x - (base.lastItem.position.y + _childSize.y / 2f)) : ((!(offset.y > 0f)) ? (_scorllSize.x - (base.lastItem.position.y + _childSize.y / 2f)) : (base.firstItem.position.y - _childSize.y / 2f - _scorllSize.y)));
			float num2 = RubberDelta(num, _activeSize.y);
			offset.Set(offset.x, offset.y - offset.y * num2);
		}
		return offset;
	}

	private static float RubberDelta(float overStretching, float viewSize)
	{
		return Mathf.Clamp(overStretching * 2f / viewSize, -1f, 1f);
	}

	protected override void ChildLateUpdate()
	{
		base.ChildLateUpdate();
		if (!CanMove() || _isDraging)
		{
			return;
		}
		if (Mathf.Abs(_nowSpeed[aixs]) < 0.1f)
		{
			if (CheckNeedLateMove())
			{
				Vector3 b = GetLateUpdateStretching();
				b = Vector3.Lerp(Vector3.zero, b, Time.deltaTime * 10f);
				Move(b);
			}
		}
		else
		{
			_nowSpeed = GetOffset();
			Move(_nowSpeed);
		}
	}

	private bool CheckNeedLateMove()
	{
		if (aspect == FixCountScorllAspect.vertical)
		{
			float num = _scorllSize.x - (base.lastItem.position.y + _childSize.y / 2f);
			if (num > 0.2f)
			{
				return true;
			}
			num = base.firstItem.position.y - _childSize.y / 2f - _scorllSize.y;
			if (num > 0.2f)
			{
				if (_allChildItems.Count >= _targetChildCount)
				{
					return true;
				}
				num = base.lastItem.position.y + _childSize.y / 2f - _scorllSize.x;
				if (num > 0.2f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private Vector2 GetLateUpdateStretching()
	{
		Vector2 zero = Vector2.zero;
		if (base.lastItem.position.y + _childSize.y / 2f < _scorllSize.x)
		{
			float value = _scorllSize.x - (base.lastItem.position.y + _childSize.y / 2f);
			zero[aixs] = value;
		}
		if (base.firstItem.position.y - _childSize.y / 2f > _scorllSize.y)
		{
			if (_allChildItems.Count < _targetChildCount)
			{
				if (base.lastItem.position.y + _childSize.y / 2f > _scorllSize.x)
				{
					float value2 = _scorllSize.x - (base.lastItem.position.y + _childSize.y / 2f);
					zero[aixs] = value2;
				}
			}
			else
			{
				float value3 = _scorllSize.y - (base.firstItem.position.y - _childSize.y / 2f);
				zero[aixs] = value3;
			}
		}
		return zero;
	}

	private Vector2 GetOffset()
	{
		if (_isMoveEnd)
		{
			_nowSpeed[aixs] = 0f;
			return _nowSpeed;
		}
		_nowSpeed[aixs] *= 0.9f;
		if (Mathf.Abs(_nowSpeed[aixs]) < 1f)
		{
			_nowSpeed[aixs] = 0f;
		}
		return _nowSpeed;
	}
}
