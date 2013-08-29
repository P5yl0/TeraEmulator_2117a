using System.IO;

namespace Network.Server
{
    public class SpCharacterCheckNameResult : ASendPacket
    {
        protected int Result;
        protected string Name;
        protected int Type;

        public SpCharacterCheckNameResult(int result, string name, int type = 1)
        {
            Result = result;
            Name = name;
            Type = type;
        }

        public override void Write(BinaryWriter writer)
        {           
            //1DC8 010008000800000016000 
            WriteB(writer, "01000800080000001600");
            WriteD(writer, Type);
            WriteD(writer, Result);
            WriteS(writer, Name);
        }
    }
}