using UnityEngine;

public class MyWeakReference
{
	private object _target;

	public object Target
	{
		get
		{
			return _target;
		}
	}

	public bool isAlive
	{
		get
		{
			return _target as GameObject != null || _target as MonoBehaviour != null;
		}
	}

	public MyWeakReference(Object obj)
	{
		_target = obj;
	}
}
