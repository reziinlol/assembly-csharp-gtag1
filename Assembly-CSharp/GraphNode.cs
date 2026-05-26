using System;
using System.Collections.Generic;

// Token: 0x0200012C RID: 300
public class GraphNode<T>
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600075F RID: 1887 RVA: 0x0002969C File Offset: 0x0002789C
	// (set) Token: 0x06000760 RID: 1888 RVA: 0x000296A4 File Offset: 0x000278A4
	public T Value { get; set; }

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000761 RID: 1889 RVA: 0x000296AD File Offset: 0x000278AD
	public List<GraphNode<T>> Parents { get; } = new List<GraphNode<T>>();

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000762 RID: 1890 RVA: 0x000296B5 File Offset: 0x000278B5
	public List<GraphNode<T>> Children { get; } = new List<GraphNode<T>>();

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000763 RID: 1891 RVA: 0x000296BD File Offset: 0x000278BD
	public int ChildCount
	{
		get
		{
			return this.Children.Count;
		}
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x000296CA File Offset: 0x000278CA
	public GraphNode(T value)
	{
		this.Value = value;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000296EF File Offset: 0x000278EF
	public GraphNode(T value, GraphNode<T> parent)
	{
		this.Value = value;
		this.Parents.Add(parent);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00029720 File Offset: 0x00027920
	public int GetSubtreeWidth(int depthLimit = 2147483647)
	{
		if (this.ChildCount == 0 || depthLimit == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num += graphNode.GetSubtreeWidth(depthLimit - 1);
		}
		return num;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00029788 File Offset: 0x00027988
	public GraphNode<T> AddChild(T value)
	{
		return this.AddChild(new GraphNode<T>(value));
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00029796 File Offset: 0x00027996
	public GraphNode<T> AddChild(GraphNode<T> child)
	{
		if (child.Parents.Contains(this))
		{
			throw new InvalidOperationException("Cannot add child more than once");
		}
		this.Children.Add(child);
		child.Parents.Add(this);
		return child;
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x000297CA File Offset: 0x000279CA
	public void RemoveChild(GraphNode<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parents.Remove(this);
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000297E7 File Offset: 0x000279E7
	public IEnumerable<GraphNode<T>> TraversePreOrder()
	{
		yield return this;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrder())
			{
				yield return graphNode2;
			}
			IEnumerator<GraphNode<T>> enumerator2 = null;
		}
		List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x000297F7 File Offset: 0x000279F7
	public IEnumerable<GraphNode<T>> TraversePreOrderDistinct(HashSet<GraphNode<T>> visited = null)
	{
		if (visited == null)
		{
			visited = new HashSet<GraphNode<T>>();
		}
		if (!visited.Contains(this))
		{
			yield return this;
			visited.Add(this);
			foreach (GraphNode<T> graphNode in this.Children)
			{
				foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrderDistinct(visited))
				{
					yield return graphNode2;
				}
				IEnumerator<GraphNode<T>> enumerator2 = null;
			}
			List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		}
		yield break;
		yield break;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0002980E File Offset: 0x00027A0E
	public IEnumerable<GraphNode<T>> TraverseBreadthFirst()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			yield return current;
			foreach (GraphNode<T> item in current.Children)
			{
				queue.Enqueue(item);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0002981E File Offset: 0x00027A1E
	public IEnumerable<GraphNode<T>> TraverseBreadthFirstDistinct()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			if (!visited.Contains(current))
			{
				visited.Add(current);
				yield return current;
				foreach (GraphNode<T> item in current.Children)
				{
					queue.Enqueue(item);
				}
				current = null;
			}
		}
		yield break;
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00029830 File Offset: 0x00027A30
	public int GetGraphDepth()
	{
		if (this.Children.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num = Math.Max(num, graphNode.GetGraphDepth());
		}
		return num + 1;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000298A0 File Offset: 0x00027AA0
	public int GetNodeDepth()
	{
		if (this.Parents.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Parents)
		{
			num = Math.Max(num, graphNode.GetNodeDepth());
		}
		return num + 1;
	}
}
