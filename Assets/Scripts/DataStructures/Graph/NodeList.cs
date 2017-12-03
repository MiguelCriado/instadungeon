using System;
using System.Collections.ObjectModel;

public class NodeList<T> : Collection<Node<T>>
{
    public NodeList() : base() { }

    public NodeList(int initialSize)
    {
        for (int i = 0; i < initialSize; i++)
		{
			base.Items.Add(default(Node<T>));
		}
    }

    public Node<T> FindByValue(T value)
    {
		Node<T> node;

		for (int i = 0; i < Items.Count; i++)
		{
			node = Items[i];

			if (node.Value.Equals(value))
			{
				return node;
			}
		}

        // if we reached here, we didn't find a matching node
        return null;
    }

	public T FindValue(Predicate<T> match)
	{
		T result = default(T);
		int i = 0;

		while (result.Equals(default(T)) && i < Items.Count)
		{
			if (match(Items[i].Value) == true)
			{
				result = Items[i].Value;
			}

			i++;
		}

		return result;
	}
}