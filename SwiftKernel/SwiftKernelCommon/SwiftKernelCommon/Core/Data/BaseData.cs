using ProtoBuf;
using System;

namespace Common.Core.Data {
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class BaseData {
        public enum Types { RequestData, ResponseData, EventData }

        [ProtoMember(1)]
        public Types Type;
        [ProtoMember(2)]
        public byte[] Data;
        [ProtoMember(3)]
        public string NetworkID;

        public BaseData (Types type, byte[] data, string networkID) {
            Type = type;
            Data = data;
            NetworkID = networkID;
        }
    }
}
