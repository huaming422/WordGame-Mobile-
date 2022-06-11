using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisignGridCtrl : MonoBehaviour
{
	public static DisignGridCtrl instance;

	private DesignGrid _grid;

	private GameObject _nodePrefab;

	private Vector2 _activeSize = Vector2.zero;

	private RectTransform _rectTran;

	private Color _selectColor = Color.yellow;

	private DesignGridNode _nowOpreationNode;

	private int _gridSize = 12;

	private List<GridState> _cacheStstes = new List<GridState>();

	private AlpHabetPos _bounsMin = AlpHabetPos.zero;

	private AlpHabetPos _bounsMax = AlpHabetPos.zero;

	private void Start()
	{
		instance = this;
		_nodePrefab = base.transform.Find("Item").gameObject;
		_rectTran = base.transform as RectTransform;
		_activeSize = _rectTran.rect.size;
		CreateGrid();
		CreateGridNodeItems();
	}

	private void CreateGrid()
	{
		int gridSize = _gridSize;
		int gridSize2 = _gridSize;
		float num = _activeSize.x / (float)gridSize;
		float num2 = _activeSize.y / (float)gridSize2;
		_grid = new DesignGrid(new Vector3((0f - _activeSize.x) / 2f + num / 2f, _activeSize.y / 2f - num2 / 2f, 0f), gridSize, gridSize2, num, num2);
	}

	public void CreateGridNodeItems()
	{
		List<DesignGridNode> nodes = _grid.nodes;
		for (int i = 0; i < nodes.Count; i++)
		{
			int index = i;
			GameObject gameObject = Object.Instantiate(_nodePrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.SetActive(true);
			nodes[i].obj = gameObject.transform;
			gameObject.transform.localPosition = nodes[i].pos;
			nodes[i].Init();
			Button component = gameObject.GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				OnClickItem(index);
			});
		}
	}

	private void OnClickItem(int index)
	{
		List<DesignGridNode> nodes = _grid.nodes;
		_nowOpreationNode = nodes[index];
		for (int i = 0; i < nodes.Count; i++)
		{
			Color imageColor = ((i != index) ? Color.white : _selectColor);
			nodes[i].SetImageColor(imageColor);
		}
	}

	public void SetTextContent(int hori, int vert, string text)
	{
		SaveSate();
		List<DesignGridNode> nodes = _grid.nodes;
		int num = vert * _gridSize + hori;
		if (num < nodes.Count)
		{
			nodes[num].SetText(text);
			UpdateLevelInfo();
		}
	}

	public void Clearn()
	{
		SaveSate();
		List<DesignGridNode> nodes = _grid.nodes;
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].SetText(string.Empty);
		}
		UpdateLevelInfo();
	}

	public void SetNowText(string text)
	{
		SaveSate();
		if (_nowOpreationNode != null)
		{
			_nowOpreationNode.SetText(text);
			UpdateLevelInfo();
		}
	}

	public void SetTextByWord(string word, bool isHori)
	{
		if (_nowOpreationNode == null)
		{
			DisignInfoCtrl.instance.LogError("没有选择开始格子!!!!!!");
			return;
		}
		SaveSate();
		word = word.ToUpper();
		int horiPosIndex = _nowOpreationNode.horiPosIndex;
		int vertPosIndex = _nowOpreationNode.vertPosIndex;
		List<DesignGridNode> nodes = _grid.nodes;
		int num = horiPosIndex;
		int num2 = vertPosIndex;
		for (int i = 0; i < word.Length; i++)
		{
			if (isHori)
			{
				num = horiPosIndex + i;
			}
			else
			{
				num2 = vertPosIndex + i;
			}
			if (num >= _gridSize || num2 >= _gridSize)
			{
				return;
			}
			int index = num2 * _gridSize + num;
			nodes[index].SetText(word[i].ToString());
		}
		UpdateLevelInfo();
	}

	private void SaveSate()
	{
		GridState state = GridState.GetState(_grid.nodes);
		if (_cacheStstes.Count > 200)
		{
			_cacheStstes.RemoveAt(0);
		}
		_cacheStstes.Add(state);
	}

	public void ShowState()
	{
		if (!_cacheStstes.IsEmpty())
		{
			int index = _cacheStstes.Count - 1;
			GridState state = _cacheStstes[index];
			GridState.ShowState(_grid.nodes, state);
			_cacheStstes.RemoveAt(index);
		}
	}

	public void UpdateLevelInfo()
	{
		WorldDesignUICtrl.UpdateLeveData();
		WorldDesignUICtrl.UpdateInfo();
	}

	public string[] GetUseAlphabets()
	{
		List<DesignGridNode> nodes = _grid.nodes;
		List<string> list = new List<string>();
		for (int i = 0; i < nodes.Count; i++)
		{
			string data = nodes[i].data;
			if (!string.IsNullOrEmpty(data) && list.IndexOf(data) == -1)
			{
				list.Add(data);
			}
		}
		return list.ToArray();
	}

	public Word[] GetUseWords()
	{
		List<DesignGridNode> nodes = _grid.nodes;
		List<Word> list = new List<Word>();
		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				int index = i * _gridSize + j;
				if (!string.IsNullOrEmpty(nodes[index].data))
				{
					list.AddRange(GetWord(nodes, j, i));
				}
			}
		}
		return list.ToArray();
	}

	public string[] GetUseAlphabets(Word[] words)
	{
		List<string>[] array = new List<string>[26];
		for (int i = 0; i < words.Length; i++)
		{
			string text = words[i].word.ToLower();
			int[] array2 = new int[26];
			for (int j = 0; j < text.Length; j++)
			{
				int num = text[j] - 97;
				array2[num]++;
			}
			for (int k = 0; k < array2.Length; k++)
			{
				List<string> list = array[k];
				if (list == null)
				{
					list = new List<string>();
				}
				while (list.Count < array2[k])
				{
					list.Add(((char)(k + 97)).ToString().ToUpper());
				}
				array[k] = list;
			}
		}
		List<string> list2 = new List<string>();
		for (int l = 0; l < array.Length; l++)
		{
			if (!array[l].IsEmpty())
			{
				list2.AddRange(array[l]);
			}
		}
		return list2.ToArray();
	}

	private List<Word> GetWord(List<DesignGridNode> nodes, int hori, int vert)
	{
		DesignGridNode designGridNode = null;
		DesignGridNode designGridNode2 = null;
		bool flag = false;
		bool flag2 = false;
		if (hori > 0)
		{
			designGridNode = nodes[vert * _gridSize + hori - 1];
		}
		if (hori < _gridSize - 1)
		{
			designGridNode2 = nodes[vert * _gridSize + hori + 1];
		}
		if ((designGridNode == null || string.IsNullOrEmpty(designGridNode.data)) && designGridNode2 != null && !string.IsNullOrEmpty(designGridNode2.data))
		{
			flag = true;
		}
		designGridNode = null;
		designGridNode2 = null;
		if (vert > 0)
		{
			designGridNode = nodes[(vert - 1) * _gridSize + hori];
		}
		if (vert < _gridSize - 1)
		{
			designGridNode2 = nodes[(vert + 1) * _gridSize + hori];
		}
		if ((designGridNode == null || string.IsNullOrEmpty(designGridNode.data)) && designGridNode2 != null && !string.IsNullOrEmpty(designGridNode2.data))
		{
			flag2 = true;
		}
		List<Word> list = new List<Word>();
		UpdateBouns();
		if (flag)
		{
			Word word = new Word();
			string text = string.Empty;
			List<AlpHabetPos> list2 = new List<AlpHabetPos>();
			for (int i = hori; i < _gridSize; i++)
			{
				int index = vert * _gridSize + i;
				string data = nodes[index].data;
				if (string.IsNullOrEmpty(data))
				{
					break;
				}
				text += data;
				AlpHabetPos item = new AlpHabetPos(i - _bounsMin.h, vert - _bounsMin.v);
				list2.Add(item);
			}
			word.word = text;
			word.alphabetPos = list2.ToArray();
			list.Add(word);
		}
		if (flag2)
		{
			Word word2 = new Word();
			string text2 = string.Empty;
			List<AlpHabetPos> list3 = new List<AlpHabetPos>();
			for (int j = vert; j < _gridSize; j++)
			{
				int index2 = j * _gridSize + hori;
				string data2 = nodes[index2].data;
				if (string.IsNullOrEmpty(data2))
				{
					break;
				}
				text2 += data2;
				AlpHabetPos item2 = new AlpHabetPos(hori - _bounsMin.h, j - _bounsMin.v);
				list3.Add(item2);
			}
			word2.word = text2;
			word2.alphabetPos = list3.ToArray();
			list.Add(word2);
		}
		return list;
	}

	private void UpdateBouns()
	{
		int num = 999;
		int num2 = -1;
		int num3 = 999;
		int num4 = -1;
		List<DesignGridNode> nodes = _grid.nodes;
		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				int index = i * _gridSize + j;
				string data = nodes[index].data;
				if (!string.IsNullOrEmpty(data))
				{
					if (j < num)
					{
						num = j;
					}
					if (i < num3)
					{
						num3 = i;
					}
					if (j > num2)
					{
						num2 = j;
					}
					if (i > num4)
					{
						num4 = i;
					}
				}
			}
		}
		_bounsMin = new AlpHabetPos(num, num3);
		_bounsMax = new AlpHabetPos(num2, num4);
	}

	public AlpHabetPos GetSize()
	{
		UpdateBouns();
		return _bounsMax - _bounsMin + new AlpHabetPos(1, 1);
	}

	public void ShowLevelData(LevelData levelData)
	{
		if (levelData == null)
		{
			return;
		}
		ClearnNoSate();
		List<DesignGridNode> nodes = _grid.nodes;
		int num = (_gridSize - levelData.gridHoriCount) / 2;
		int num2 = (_gridSize - levelData.gridVertCount) / 2;
		Word[] words = levelData.words;
		foreach (Word word in words)
		{
			AlpHabetPos[] alphabetPos = word.alphabetPos;
			for (int j = 0; j < alphabetPos.Length; j++)
			{
				AlpHabetPos alpHabetPos = alphabetPos[j];
				int index = (alpHabetPos.v + num2) * _gridSize + (alpHabetPos.h + num);
				nodes[index].SetText(word.word[j].ToString().ToUpper());
			}
		}
	}

	public void ClearnNoSate()
	{
		List<DesignGridNode> nodes = _grid.nodes;
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].SetText(string.Empty);
		}
	}
}
