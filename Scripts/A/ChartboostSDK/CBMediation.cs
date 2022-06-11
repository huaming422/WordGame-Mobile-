namespace ChartboostSDK
{
	public sealed class CBMediation
	{
		public static readonly CBMediation AdMarvel = new CBMediation("AdMarvel");

		public static readonly CBMediation Fuse = new CBMediation("Fuse");

		public static readonly CBMediation Fyber = new CBMediation("Fyber");

		public static readonly CBMediation HeyZap = new CBMediation("HeyZap");

		public static readonly CBMediation MoPub = new CBMediation("MoPub");

		public static readonly CBMediation Supersonic = new CBMediation("Supersonic");

		public static readonly CBMediation AdMob = new CBMediation("AdMob");

		public static readonly CBMediation HyprMX = new CBMediation("HyprMX");

		public static readonly CBMediation AerServ = new CBMediation("AerServ");

		public static readonly CBMediation Other = new CBMediation("Other");

		private readonly string name;

		private CBMediation(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
