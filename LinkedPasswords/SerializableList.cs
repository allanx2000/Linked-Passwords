using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedPasswords
{
    public interface Serializer<T>
    {
        T Deserialize(string obj);
        string Serialize(T obj);
    }

    public class SerializableList<T>
    {
        private readonly char Delimiter;
        private readonly int maxLength;
        private readonly Serializer<T> serializer;

        public SerializableList(Serializer<T> serializer, char delimiter = '\t', int maxLength = 5)
        {
            this.serializer = serializer;
            this.Delimiter = delimiter;
            this.maxLength = maxLength;
        }

        private List<T> items = new List<T>();

        public IReadOnlyList<T> Items
        {
            get { return items.AsReadOnly(); }
        }

        public void AddItem(T item)
        {
            if (Items.Contains(item))
            {
                items.Remove(item);
            }

            items.Insert(0, item);

            if (items.Count > maxLength)
            {
                items.RemoveAt(items.Count - 1);
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public void LoadFromString(string values)
        {
            Clear();

            if (string.IsNullOrEmpty(values))
                return;

            string[] strings = values.Split(Delimiter);

            foreach (var s in strings)
            {
                T item = serializer.Deserialize(s);
                items.Add(item);
            }
        }

        public string SerializeToString()
        {
            List<string> strings = new List<string>();

            foreach (var i in items)
            {
                strings.Add(serializer.Serialize(i));
            }

            return string.Join(Delimiter.ToString(), strings);
        }
    }
}
