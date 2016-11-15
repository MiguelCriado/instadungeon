public interface ILayoutGenerator 
{
	Layout NewLayout();
	Layout Iterate(Layout layout);
	bool IsDone();
}
