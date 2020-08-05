using System;
using System.ComponentModel;

namespace LogStudio.Framework
{
    public class BindingListEx<T> : BindingList<T>
    {
        public event EventHandler<ItemRemovedEventArgs<T>> OnItemRemoved;

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];

            base.RemoveItem(index);

            OnItemRemoved?.Invoke(this, new ItemRemovedEventArgs<T>(removedItem));
        }
    }

    public class ItemRemovedEventArgs<T> : EventArgs
    {
        public T Item { get; }

        public ItemRemovedEventArgs(T item)
        {
            Item = item;
        }
    }
}
