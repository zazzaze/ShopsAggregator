using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShopsAggregator
{
    public static class HelpMethods<T>
    {
        public static T GetFullCopy(T obj)
        {
            return Deserialize(Serialize(obj));
        }
        
        private static MemoryStream Serialize(T obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms;
        }

        private static T Deserialize(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            BinaryFormatter bf = new BinaryFormatter();
            return (T) bf.Deserialize(ms);
        }
    }
}