namespace TapjoyUnity
{
	public delegate TResult TapjoyFunc<out TResult>();
	public delegate TResult TapjoyFunc<in T1, out TResult>(T1 arg1);
}
