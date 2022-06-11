public interface ISupuerGridDataCtrl
{
	void DoCreateNode(GridNode node);

	void DoChangeDataIndex(GridNode node);

	void DoMoveNode(GridNode node);

	bool NeedDrawGrid(GridNode node);

	void DoRebuildGird(GridNode node);
}
