using System;
using System.Collections.Generic;

// Token: 0x02000132 RID: 306
public class Node<T>
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600079C RID: 1948 RVA: 0x0002A13F File Offset: 0x0002833F
	// (set) Token: 0x0600079D RID: 1949 RVA: 0x0002A147 File Offset: 0x00028347
	public T Value { get; set; }

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x0600079E RID: 1950 RVA: 0x0002A150 File Offset: 0x00028350
	// (set) Token: 0x0600079F RID: 1951 RVA: 0x0002A158 File Offset: 0x00028358
	public Node<T> Parent { get; private set; }

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060007A0 RID: 1952 RVA: 0x0002A161 File Offset: 0x00028361
	public List<Node<T>> Children { get; } = new List<Node<T>>();

	// Token: 0x060007A1 RID: 1953 RVA: 0x0002A169 File Offset: 0x00028369
	public Node(T value)
	{
		this.Value = value;
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0002A184 File Offset: 0x00028384
	public Node<T> AddChild(T value)
	{
		Node<T> node = new Node<T>(value)
		{
			Parent = this
		};
		this.Children.Add(node);
		return node;
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0002A1AC File Offset: 0x000283AC
	public Node<T> AddChild(Node<T> child)
	{
		Node<T> parent = child.Parent;
		if (parent != null)
		{
			parent.RemoveChild(child);
		}
		this.Children.Add(child);
		child.Parent = this;
		return child;
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002A1D4 File Offset: 0x000283D4
	public void RemoveChild(Node<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parent = null;
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002A1EB File Offset: 0x000283EB
	public IEnumerable<Node<T>> TraversePreOrder()
	{
		yield return this;
		foreach (Node<T> node in this.Children)
		{
			foreach (Node<T> node2 in node.TraversePreOrder())
			{
				yield return node2;
			}
			IEnumerator<Node<T>> enumerator2 = null;
		}
		List<Node<T>>.Enumerator enumerator = default(List<Node<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002A1FB File Offset: 0x000283FB
	public IEnumerable<Node<T>> TraverseBreadthFirst()
	{
		Queue<Node<T>> queue = new Queue<Node<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			Node<T> current = queue.Dequeue();
			yield return current;
			foreach (Node<T> item in current.Children)
			{
				queue.Enqueue(item);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002A20C File Offset: 0x0002840C
	public List<Node<T>> GetPath()
	{
		List<Node<T>> list = new List<Node<T>>
		{
			this
		};
		for (Node<T> parent = this.Parent; parent != null; parent = parent.Parent)
		{
			list.Insert(0, parent);
		}
		return list;
	}
}
