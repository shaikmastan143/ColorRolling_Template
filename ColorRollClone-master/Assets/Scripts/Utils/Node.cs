using System;
using System.Collections;
using System.Collections.Generic;

public class Node<T>
{
    public T obj { private set; get; }
    public List<Node<T>> children { private set; get; }
    public Node(T obj)
    {
        this.obj = obj;
        children = new List<Node<T>>();
    }

    public void ChangeObj(T obj)
    {
        this.obj = obj;
    }
    public void AddChild(Node<T> child)
    {
        children.Add(child);
    }

    public void Traverse(Action<T> action)
    {
        action(obj);
        
        foreach (var child in children)
        {
            child.Traverse(action);
        }
    }

    public Node<T> TraverseWithTermination<U>(ref U data,Func<Node<T>,U, bool> terminatingCondition)
    {
        Node<T> found = null;

        foreach (var child in children)
        {
            found = child.TraverseWithTermination(ref data,terminatingCondition);

            if (found != null)
            {
                break;
            }
        }

        if (found == null)
        {
            if (terminatingCondition(this,data))
            {
                found = this;
            }
        }
        return found;
    }
    public Node<T> TraverseWithTermination(Func<Node<T>, bool> terminatingCondition)
    {
        Node<T> found = null;

        foreach (var child in children)
        {
            found = child.TraverseWithTermination(terminatingCondition);

            if (found != null)
            {
                break;
            }
        }

        if (found == null)
        {
            if (terminatingCondition(this))
            {
                found = this;
            }
        }
        return found;
    }
    public bool IsSubset(Node<T> root, IComparer<T>comparer)
    {
        if (comparer.Compare(obj, root.obj) != 0) return false;
        for(int i = 0; i<children.Count; i++)
        {
            if (i >= root.children.Count)
                return false;
            if (!children[i].IsSubset(root.children[i],comparer))
                return false;
        }
        return true;
    }
    public void Unset()
    {
        foreach (var child in children)
            child.Unset();

        children.Clear();
    }
}
