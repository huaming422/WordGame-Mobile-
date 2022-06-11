namespace TapjoyUnity.Internal.UnityCompat
{
	public struct SceneCompat
	{
		internal static readonly SceneCompat NONE = default(SceneCompat);

		private readonly int _hashCode;

		private readonly bool _valid;

		private readonly int? _buildIndex;

		private readonly string _name;

		private readonly string _path;

		internal int buildIndex
		{
			get
			{
				int? num = _buildIndex;
				return (!num.HasValue) ? (-1) : num.Value;
			}
		}

		internal string name
		{
			get
			{
				return _name;
			}
		}

		internal string path
		{
			get
			{
				return _path;
			}
		}

		public SceneCompat(object scene, bool valid, int buildIndex, string name, string path)
		{
			_hashCode = scene.GetHashCode();
			_valid = valid;
			_buildIndex = buildIndex;
			_name = name;
			_path = path;
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}

		internal bool IsValid()
		{
			return _valid;
		}
	}
}
