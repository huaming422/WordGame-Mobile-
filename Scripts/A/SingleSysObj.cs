public class SingleSysObj<T> where T : SingleSysObj<T>, new()
{
	private static T _instance;

	private static object temp = new object();

	public static T instance
	{
		get
		{
			lock (temp)
			{
				if (_instance == null)
				{
					_instance = new T();
					_instance.Init();
				}
			}
			return _instance;
		}
	}

	protected virtual void Init()
	{
	}
}
