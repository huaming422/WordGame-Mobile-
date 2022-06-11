using System;

namespace DG.Tweening
{
	[Flags]
	public enum AxisConstraint
	{
		None = 0x0,
		X = 0x2,
		Y = 0x4,
		Z = 0x8,
		W = 0x10
	}
}
