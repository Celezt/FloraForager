using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum Heap
{
    MinHeap = -1,
    MaxHeap = 1
}

[System.Serializable]
public class PriorityQueue<T>
{
    [System.Serializable]
    private struct Element
    {
        public T Item;
        public float Priority;
    }

    [SerializeField, HideInInspector]
    private readonly List<Element> priorityQueue;
    [SerializeField, HideInInspector]
    private readonly Heap heap = Heap.MinHeap;

    private int Parent(int pos) => (pos - 1) / 2;
    private int Left(int pos) => (2 * pos) + 1;
    private int Right(int pos) => (2 * pos) + 2;

    public T this[int i] => priorityQueue[i].Item;
    public int Count => priorityQueue.Count;
    public float Priority(int i) => priorityQueue[i].Priority;

    public PriorityQueue(Heap heap)
    {
        if (System.Enum.IsDefined(typeof(Heap), heap))
            this.heap = heap;

        priorityQueue = new List<Element>();
    }

    public void Enqueue(T element, float priority)
    {
        priorityQueue.Add(new Element { Item = element, Priority = priority }); // Add to end of list
        MoveUp(Count - 1);
    }
    public T Dequeue()
    {
        if (Count <= 0) // No data available
            return default;

        T root = priorityQueue.First().Item;

        priorityQueue[0] = priorityQueue[Count - 1]; // Replace root with element at end
        priorityQueue.RemoveAt(Count - 1);           // Remove element at end

        MoveDown(0); // After previous root is removed, move the current one down

        return root;
    }

    public void Remove(int pos)
    {
        if (pos < 0 || pos >= Count)
            return;

        priorityQueue[pos] = priorityQueue[Count - 1];
        priorityQueue.RemoveAt(Count - 1);

        MoveDown(pos);
    }
    public void Remove(T element)
    {
        Remove(Find(element));
    }

    public bool Contains(T element)
    {
        for (int i = 0; i < priorityQueue.Count; i++)
        {
            if (priorityQueue[i].Item.Equals(element))
                return true;
        }
        return false;
    }
    public int Find(T element)
    {
        for (int i = 0; i < priorityQueue.Count; i++)
        {
            if (priorityQueue[i].Item.Equals(element))
                return i;
        }
        return -1;
    }

    private void MoveUp(int pos)
    {
        if (pos <= 0) // We have reached root
            return;

        int parent = Parent(pos);
        if (Compare(priorityQueue[pos].Priority, priorityQueue[parent].Priority) == (int)heap)
        {
            Swap(pos, parent);
            MoveUp(parent);
        }
    }

    private void MoveDown(int pos)
    {
        int left = Left(pos);
        int right = Right(pos);

        int smallest = pos;
        if (left < Count && Compare(priorityQueue[left].Priority, priorityQueue[smallest].Priority) == (int)heap)
        {
            smallest = left;
        }
        if (right < Count && Compare(priorityQueue[right].Priority, priorityQueue[smallest].Priority) == (int)heap)
        {
            smallest = right;
        }

        if (smallest != pos)
        {
            Swap(pos, smallest);
            MoveDown(smallest);
        }
    }

    private void Swap(int first, int second)
    {
        Element tmp = priorityQueue[first];
        priorityQueue[first] = priorityQueue[second];
        priorityQueue[second] = tmp;
    }

    private int Compare(float x, float y)
    {
        return Comparer<float>.Default.Compare(x, y);
    }
}
