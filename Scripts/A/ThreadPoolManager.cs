using System.Threading;

public class ThreadPoolManager : SingleSysObj<ThreadPoolManager>
{
	protected override void Init()
	{
		base.Init();
		ThreadPool.SetMaxThreads(5, 5);
	}

	public void DoThing(WaitCallback callBack, object state)
	{
		ThreadPool.QueueUserWorkItem(callBack, state);
	}
}
