public class NetWorkProtocal
{
	public const int Connect = 100;

	public const int ConnectError = 101;

	public const int Exception = 103;

	public const int Disconnect = 104;

	public static bool isNetProtocal(int id)
	{
		if (id == 100 || id == 103 || id == 104 || id == 101)
		{
			return true;
		}
		return false;
	}
}
