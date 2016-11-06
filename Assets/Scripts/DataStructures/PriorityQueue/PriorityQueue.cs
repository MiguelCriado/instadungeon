﻿using System;
using System.Collections.Generic;

public class PriorityQueue<T, E> where E : IComparable<E>
{
    private struct Tuple
    {
        public T Item;
        public E Priority;

        public Tuple(T item, E priority) 
        {
            this.Item = item;
            this.Priority = priority;
        }
    }

    private List<Tuple> data;

    public PriorityQueue()
    {
        this.data = new List<Tuple>();
    }

    public void Enqueue(T item, E priority)
    {
        data.Add(new Tuple(item, priority));
        int ci = data.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (data[ci].Priority.CompareTo(data[pi].Priority) >= 0) break; // child item is larger than (or equal) parent so we're done
            Tuple tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        // assumes pq is not empty; up to calling code
        int li = data.Count - 1; // last index (before removal)
        Tuple frontItem = data[0];   // fetch the front
        data[0] = data[li];
        data.RemoveAt(li);

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        while (true)
        {
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && data[rc].Priority.CompareTo(data[ci].Priority) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                ci = rc;
            if (data[pi].Priority.CompareTo(data[ci].Priority) <= 0) break; // parent is smaller than (or equal to) smallest child so done
            Tuple tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
            pi = ci;
        }
        return frontItem.Item;
    }

    public T Peek()
    {
        T frontItem = data[0].Item;
        return frontItem;
    }

    public int Count()
    {
        return data.Count;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "count = " + data.Count;
        return s;
    }

    public bool IsConsistent()
    {
        // is the heap property true for all data?
        if (data.Count == 0) return true;
        int li = data.Count - 1; // last index
        for (int pi = 0; pi < data.Count; ++pi) // each parent index
        {
            int lci = 2 * pi + 1; // left child index
            int rci = 2 * pi + 2; // right child index

            if (lci <= li && data[pi].Priority.CompareTo(data[lci].Priority) > 0) return false; // if lc exists and it's greater than parent then bad.
            if (rci <= li && data[pi].Priority.CompareTo(data[rci].Priority) > 0) return false; // check the right child too.
        }
        return true; // passed all checks
    } 
} 
