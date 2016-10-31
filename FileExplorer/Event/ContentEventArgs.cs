using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Event
{
    public class ContentEventArgs<T> : EventArgs
    {
        public bool IsProperty { get; protected set; }
        public T Content { get; protected set; }
        public ContentEventArgs(T item, bool isProperty = true)
        {
            this.Content = item;
            this.IsProperty = isProperty;
        }
    }
}
