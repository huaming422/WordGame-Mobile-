public struct AlpHabetPos
{
	public int h;

	public int v;

	public static AlpHabetPos zero = new AlpHabetPos(0, 0);

	public AlpHabetPos(int h, int v)
	{
		this.h = h;
		this.v = v;
	}

	public override string ToString()
	{
		return h + "_" + v;
	}

	public static AlpHabetPos ToAlpHabetPos(string data)
	{
		string[] array = data.Split('_');
		return new AlpHabetPos(int.Parse(array[0]), int.Parse(array[1]));
	}

	public static AlpHabetPos operator +(AlpHabetPos a, AlpHabetPos b)
	{
		return new AlpHabetPos(a.h + b.h, a.v + b.v);
	}

	public static AlpHabetPos operator -(AlpHabetPos a, AlpHabetPos b)
	{
		return new AlpHabetPos(a.h - b.h, a.v - b.v);
	}
}
