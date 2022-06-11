using System;
using System.Collections.Generic;
using UnityEngine;

public class FixCountScorll : MonoBehaviour
{
	public enum FixCountScorllAspect
	{
		horizontal,
		vertical
	}

	public bool isAutoInit;

	public GameObject childPrefab;

	public Vector2 spacing = Vector2.zero;

	public FixCountScorllAspect aspect = FixCountScorllAspect.vertical;

	public IFixCountScrollDataCtrl dataCtrl;

	public bool canMove = true;

	protected Vector2 _activeSize = Vector2.one;

	protected Vector2 _childSize = Vector2.one;

	protected List<FixedCountChildItem> _allChildItems = new List<FixedCountChildItem>();

	protected Vector4 _scorllSize = Vector4.one;

	protected int _targetChildCount;

	protected RectTransform _viewRectTransform;

	protected bool _isInited;

	public bool isInited
	{
		get
		{
			return _isInited;
		}
	}

	public List<FixedCountChildItem> childItems
	{
		get
		{
			return _allChildItems;
		}
	}

	protected FixedCountChildItem firstItem
	{
		get
		{
			if (_allChildItems == null)
			{
				return null;
			}
			return _allChildItems[0];
		}
	}

	protected FixedCountChildItem lastItem
	{
		get
		{
			if (_allChildItems == null)
			{
				return null;
			}
			return _allChildItems[_allChildItems.Count - 1];
		}
	}

	private void Awake()
	{
		_viewRectTransform = GetComponent<RectTransform>();
		ChildAwake();
	}

	protected virtual void ChildAwake()
	{
	}

	private void Start()
	{
		if (isAutoInit)
		{
			InitFixCountScorll();
		}
		ChildStart();
	}

	protected virtual void ChildStart()
	{
	}

	public void RegistDataCtrl(IFixCountScrollDataCtrl dataCtrl)
	{
		this.dataCtrl = dataCtrl;
	}

	public virtual void InitBounds()
	{
		if (_viewRectTransform == null)
		{
			_viewRectTransform = GetComponent<RectTransform>();
		}
		RectTransform viewRectTransform = _viewRectTransform;
		if (viewRectTransform == null)
		{
			return;
		}
		_activeSize = new Vector2(viewRectTransform.rect.size.x, viewRectTransform.rect.size.y);
		if (!(childPrefab == null))
		{
			RectTransform component = childPrefab.GetComponent<RectTransform>();
			if (!(component == null))
			{
				_childSize = new Vector2(component.rect.size.x, component.rect.size.y);
			}
		}
	}

	public virtual void InitFixCountScorll()
	{
		InitBounds();
		InitChildCount();
		InitScorllSize();
		InitChild();
		_isInited = true;
	}

	public virtual void InitScorllSize()
	{
		_scorllSize.x = _activeSize.y * 0.5f;
		_scorllSize.y = (0f - _activeSize.y) * 0.5f;
		_scorllSize.z = (0f - _activeSize.x) * 0.5f;
		_scorllSize.w = _activeSize.x * 0.5f;
	}

	protected virtual void InitChildCount()
	{
		if (aspect == FixCountScorllAspect.horizontal)
		{
			_targetChildCount = Mathf.CeilToInt(_activeSize.x / _childSize.x) + 1;
		}
		if (aspect == FixCountScorllAspect.vertical)
		{
			_targetChildCount = Mathf.CeilToInt(_activeSize.y / _childSize.y) + 1;
		}
	}

	protected virtual void InitChild()
	{
		if (!(childPrefab == null) && dataCtrl != null)
		{
			for (int i = 0; i < _targetChildCount && dataCtrl.CanCreateChild(i); i++)
			{
				GameObject gameObject = CreateChild();
				SetChildPos(i, gameObject);
				FixedCountChildItem item = new FixedCountChildItem(i, -i, gameObject);
				_allChildItems.Add(item);
				dataCtrl.CreateChildItem(i, gameObject);
			}
		}
	}

	protected virtual GameObject CreateChild()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(childPrefab);
		gameObject.SetActive(true);
		gameObject.transform.SetParent(base.transform, false);
		return gameObject;
	}

	public void UpdateTargetPos()
	{
		for (int i = 0; i < _allChildItems.Count; i++)
		{
			if (aspect == FixCountScorllAspect.horizontal)
			{
				_allChildItems[i].targetPos = new Vector3(_scorllSize.z + _childSize.x / 2f + (float)i * _childSize.x, childPrefab.transform.localPosition.y, childPrefab.transform.localPosition.z);
			}
			if (aspect == FixCountScorllAspect.vertical)
			{
				_allChildItems[i].targetPos = new Vector3(childPrefab.transform.localPosition.x, _scorllSize.x - _childSize.y / 2f - (float)i * _childSize.y, childPrefab.transform.localPosition.z);
			}
		}
	}

	protected virtual void SetChildPos(int posIndex, GameObject temp)
	{
		if (aspect == FixCountScorllAspect.horizontal)
		{
			temp.transform.localPosition = new Vector3(_scorllSize.z + _childSize.x / 2f + (float)posIndex * _childSize.x, childPrefab.transform.localPosition.y, childPrefab.transform.localPosition.z);
		}
		if (aspect == FixCountScorllAspect.vertical)
		{
			temp.transform.localPosition = new Vector3(childPrefab.transform.localPosition.x, _scorllSize.x - _childSize.y / 2f + (float)posIndex * _childSize.y, childPrefab.transform.localPosition.z);
		}
	}

	public virtual void ReInit()
	{
		if (!(childPrefab == null) && dataCtrl != null)
		{
			for (int i = 0; i < _allChildItems.Count; i++)
			{
				UnityEngine.Object.Destroy(_allChildItems[i].item);
			}
			_allChildItems.Clear();
			InitChild();
		}
	}

	public virtual void Move(Vector3 distance)
	{
		if (!canMove || (aspect == FixCountScorllAspect.vertical && distance.y == 0f) || (aspect == FixCountScorllAspect.horizontal && distance.x == 0f) || _allChildItems == null || _allChildItems.Count < 1)
		{
			return;
		}
		for (int i = 0; i < _allChildItems.Count; i++)
		{
			if (aspect == FixCountScorllAspect.horizontal)
			{
				_allChildItems[i].MoveHori(distance.x);
			}
			if (aspect == FixCountScorllAspect.vertical)
			{
				_allChildItems[i].MoveVert(distance.y);
			}
		}
		CheckOverSize();
	}

	protected virtual void CheckOverSize()
	{
		bool flag = false;
		bool isAdd = true;
		if (aspect == FixCountScorllAspect.horizontal)
		{
			if (_allChildItems[0].position.x - _childSize.x / 2f > _scorllSize.z)
			{
				flag = true;
				isAdd = false;
			}
			if (_allChildItems[_allChildItems.Count - 1].position.x + _childSize.x / 2f < _scorllSize.w)
			{
				flag = true;
			}
		}
		if (aspect == FixCountScorllAspect.vertical)
		{
			if (lastItem.position.y + _childSize.y / 2f < _scorllSize.x)
			{
				flag = true;
				isAdd = false;
			}
			if (firstItem.position.y - _childSize.y / 2f > _scorllSize.y)
			{
				flag = true;
			}
		}
		if (flag)
		{
			DoChange(isAdd);
		}
	}

	protected virtual void DoChange(bool isAdd)
	{
		if (_allChildItems.Count < _targetChildCount)
		{
			OnMoveStateChange(isAdd, true);
			return;
		}
		FixedCountChildItem fixedCountChildItem = ((!isAdd) ? firstItem : lastItem);
		int index = ((!isAdd) ? (lastItem.dataIndex - 1) : (firstItem.dataIndex + 1));
		if (!dataCtrl.CanUpdateChildItemDataIndex(index))
		{
			OnMoveStateChange(isAdd, true);
			return;
		}
		OnMoveStateChange(isAdd, false);
		if (isAdd)
		{
			_allChildItems.RemoveAt(_allChildItems.Count - 1);
			if (aspect == FixCountScorllAspect.horizontal)
			{
				fixedCountChildItem.position = _allChildItems[_allChildItems.Count - 1].position + new Vector3(_childSize.x, 0f, 0f);
			}
			if (aspect == FixCountScorllAspect.vertical)
			{
				fixedCountChildItem.position = firstItem.position - new Vector3(0f, _childSize.y, 0f);
			}
			fixedCountChildItem.dataIndex = firstItem.dataIndex + 1;
			_allChildItems.Insert(0, fixedCountChildItem);
		}
		else
		{
			_allChildItems.RemoveAt(0);
			if (aspect == FixCountScorllAspect.horizontal)
			{
				fixedCountChildItem.position = _allChildItems[0].position + new Vector3(_childSize.x, 0f, 0f);
			}
			if (aspect == FixCountScorllAspect.vertical)
			{
				fixedCountChildItem.position = lastItem.position + new Vector3(0f, _childSize.y, 0f);
			}
			fixedCountChildItem.dataIndex = lastItem.dataIndex - 1;
			_allChildItems.Add(fixedCountChildItem);
		}
		UpdatePosIndex();
		dataCtrl.UpdateChildItemDataIndex(index, fixedCountChildItem.item);
		UpdateTargetPos();
	}

	protected virtual void OnMoveStateChange(bool isAdd, bool isEnd)
	{
	}

	protected virtual void UpdatePosIndex()
	{
		if (_allChildItems != null && _allChildItems.Count >= 1)
		{
			for (int i = 0; i < _allChildItems.Count; i++)
			{
				_allChildItems[i].posIndex = i;
				dataCtrl.UpdateChildItemPosIndex(_allChildItems[i].posIndex, _allChildItems[i].item);
			}
		}
	}

	public virtual void FocusPos()
	{
		if (_allChildItems == null || _allChildItems.Count < 1)
		{
			return;
		}
		for (int i = 0; i < _allChildItems.Count; i++)
		{
			if (aspect == FixCountScorllAspect.horizontal)
			{
				_allChildItems[i].item.transform.localPosition = new Vector3(_scorllSize.z + _childSize.x / 2f + (float)i * _childSize.x, base.transform.localPosition.y, base.transform.localPosition.z);
			}
			if (aspect == FixCountScorllAspect.vertical)
			{
				_allChildItems[i].item.transform.localPosition = new Vector3(base.transform.localPosition.x, _scorllSize.x - _childSize.y / 2f - (float)i * _childSize.y, base.transform.localPosition.z);
			}
		}
	}

	private void LateUpdate()
	{
		ChildLateUpdate();
	}

	protected virtual void ChildLateUpdate()
	{
	}

	public virtual void RemoveByIndex(int index, Func<int, GameObject, bool> removeLastItem = null)
	{
		int num = -1;
		for (int i = 0; i < _allChildItems.Count; i++)
		{
			if (_allChildItems[i].dataIndex == index)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		if (num < _targetChildCount)
		{
			for (int j = num; j < _allChildItems.Count; j++)
			{
				int nowDataIndex = index + (j - num);
				DoUpdateItem(nowDataIndex, j);
			}
			bool flag = true;
			int index2 = _allChildItems.Count - 1;
			if (removeLastItem != null)
			{
				flag = removeLastItem(_allChildItems[index2].dataIndex, _allChildItems[index2].item);
			}
			if (flag)
			{
				UnityEngine.Object.Destroy(_allChildItems[index2].item);
				_allChildItems.RemoveAt(index2);
			}
		}
		else
		{
			for (int num2 = num; num2 >= 0; num2--)
			{
				int nowDataIndex2 = index - (num - num2);
				DoUpdateItem(nowDataIndex2, num2);
			}
		}
	}

	private void DoUpdateItem(int nowDataIndex, int i)
	{
		_allChildItems[i].dataIndex = nowDataIndex;
		dataCtrl.UpdateChildItemDataIndex(nowDataIndex, _allChildItems[i].item);
	}

	public bool ItemCountIsEnough()
	{
		return _allChildItems.Count >= _targetChildCount;
	}
}
