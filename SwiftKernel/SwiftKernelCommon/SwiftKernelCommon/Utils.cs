using ProtoBuf;
using System;
using System.IO;

namespace Common {
    public static class Utils {
        public static T[] SubArray<T> (T[] data,int index,int length) {
            T[] result = new T[length];
            Array.Copy(data,index,result,0,length);
            return result;
        }

        public static T FromBytesJSON<T> (byte[] bytes) {
            using (var stream = new MemoryStream(bytes)) {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public static byte[] ToBytesJSON (object obj) {
            using (var stream = new MemoryStream()) {
                Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
    }
}