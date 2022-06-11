using UnityEngine;

public class PrefabBase : MonoBehaviour
{
	public GameObject prefab;

	public bool isInstanceAwake = true;

	public bool isSingleBundle;

	[SerializeField]
	private string _prefabsPath;

	[SerializeField]
	private GameObject prefabInstance;

	private void Awake()
	{
		if (isInstanceAwake)
		{
			InstancePrefabs();
		}
	}

	public void InstancePrefabs()
	{
		if (prefabInstance != null)
		{
			Object.DestroyImmediate(prefabInstance);
		}
		if (string.IsNullOrEmpty(_prefabsPath))
		{
			return;
		}
		GameObject gameObject = LoadPrefab(_prefabsPath);
		if (gameObject == null)
		{
			return;
		}
		prefabInstance = gameObject;
		prefabInstance.transform.SetParent(base.transform, false);
		if (!Application.isPlaying)
		{
			PrefabChild component = prefabInstance.GetComponent<PrefabChild>();
			if (component == null)
			{
				prefabInstance.AddComponent<PrefabChild>();
			}
		}
	}

	private string[] GetAbLoadBundleNameAndPrefabName(string path)
	{
		int num = path.LastIndexOf('/');
		string text;
		if (isSingleBundle)
		{
			if (num == -1)
			{
				return new string[2] { path, path };
			}
			text = path;
		}
		else
		{
			if (num == -1)
			{
				return null;
			}
			text = path.Substring(0, num + 1);
		}
		string text2 = path.Substring(num + 1);
		return new string[2] { text, text2 };
	}

	private GameObject LoadPrefab(string path)
	{
		path = path.Replace("\\", "/");
		if (Application.isPlaying)
		{
			string[] abLoadBundleNameAndPrefabName = GetAbLoadBundleNameAndPrefabName(path);
			if (abLoadBundleNameAndPrefabName == null)
			{
				return null;
			}
			return SingleObject<ResourceManager>.instance.LoadAsset<GameObject>(abLoadBundleNameAndPrefabName[0], abLoadBundleNameAndPrefabName[1]);
		}
		GameObject gameObject = Resources.Load<GameObject>(path);
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject);
		gameObject2.name = gameObject.name;
		return gameObject2;
	}
}
