using System.Collections;

public class CanSaveEventItem
{
	public int id;

	public int type;

	public Hashtable data;

	public CanSaveEventItem(int id, int type, Hashtable data)
	{
		this.id = id;
		this.type = type;
		this.data = data;
	}
}
