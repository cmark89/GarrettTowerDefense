using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class BinaryMinHeap<T>
    {
        public List<HeapNode<T>> Heap { get; private set; }

        public BinaryMinHeap()
        {
            Heap = new List<HeapNode<T>>();
        }

        public void Add(T data, int value)
        {
            Heap.Add(new HeapNode<T>(data, value));
            Heapify(Heap.Count - 1);
        }

        public void Heapify(int index)
        {
            //Make this not dumb.
            while (Heap[index].Value < Heap[(int)Math.Floor((float)(index / 2))].Value)
            {
                SwapNodes(index, (int)Math.Floor((float)(index / 2)));
            }
        }

        public void SwapNodes(int index1, int index2)
        {
            HeapNode<T> temp = Heap[index1];
            Heap[index1] = Heap[index2];
            Heap[index2] = temp;
        }
    }

    public class HeapNode<T>
    {
        public T Data;
        public int Value;

        public HeapNode(T data, int value)
        {
            Data = data;
            Value = value;
        }
    }
}
