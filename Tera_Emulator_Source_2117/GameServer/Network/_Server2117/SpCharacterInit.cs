using System.IO;
using Data.Enums;
using Data.Structures.Player;

namespace Network.Server
{
    public class SpCharacterInit : ASendPacket // Only for owner!!!
    {
        protected Player Player;

        public SpCharacterInit(Player player)
        {
            Player = player;
        }

        public override void Write(BinaryWriter writer)
        {
            WriteH(writer, 0); //Name shift

            WriteH(writer, 0); //Details1 shift
            WriteH(writer, (short) Player.PlayerData.Details1.Length); //Details1 length
            WriteH(writer, 0); //Details2 2117
            WriteH(writer, (short)Player.PlayerData.Details2.Length); //Details2 length 2117

            WriteD(writer, Player.PlayerData.SexRaceClass);
           
            //FC2A00001F2A0D0000800000
            WriteUid(writer, Player);
            
            //0D000000
            WriteD(writer, 13); //??? 2117 
            
            //68130000
            WriteD(writer, Player.PlayerId);

            //0000000001000000004600000069000000
            WriteB(writer, "0000000001000000004600000069000000");

            WriteB(writer, Player.PlayerData.Data); //PlayerData

            WriteC(writer, 1); //Online?
            WriteC(writer, 0); //??
            WriteD(writer, Player.GetLevel()); //Level

            WriteB(writer, "00000000000001000000000000000000");
            WriteQ(writer, Player.PlayerExp);
            WriteQ(writer, Player.GetExpShown());
            WriteQ(writer, Player.GetExpNeed());

            WriteB(writer, "00000000");  //unk

            //Player rested ToDo! now 100%
            WriteB(writer, "901B9800");//rested current
            WriteB(writer, "901B9800");//rested max

            WriteB(writer, "0000803F00000000"); //unk


            WriteD(writer, Player.Inventory.GetItemId(1) ?? 0); //Item (Weapon)
            WriteD(writer, Player.Inventory.GetItemId(3) ?? 0); //Item (Armor)
            WriteD(writer, Player.Inventory.GetItemId(4) ?? 0); //Item (Gloves)
            WriteD(writer, Player.Inventory.GetItemId(5) ?? 0); //Item (Shoes)
            WriteD(writer, 0); //Item (HairSlot)
            WriteD(writer, 0); //Item (FaceSlot)

            WriteB(writer, "5E10C101000000000100000000000000000000000000000000000000000000000000000000");

            WriteD(writer, Player.Inventory.GetItem(1) != null ? Player.Inventory.GetItem(1).Color : 0);
            WriteD(writer, Player.Inventory.GetItem(3) != null ? Player.Inventory.GetItem(3).Color : 0);
            WriteD(writer, Player.Inventory.GetItem(4) != null ? Player.Inventory.GetItem(4).Color : 0);
            WriteD(writer, Player.Inventory.GetItem(5) != null ? Player.Inventory.GetItem(5).Color : 0);
            
            WriteD(writer, 0); //Item ???
            WriteD(writer, 0); //Item ???

            WriteB(writer, "0001");//On/offline?

            WriteD(writer, 0); //Item (Skin Head)
            WriteD(writer, 0); //Item (Skin Face)
            WriteD(writer, 0); //Item (Skin Backpack)?
            WriteD(writer, 0); //Item (Skin Weapon)
            WriteD(writer, 0); //Item (Skin Armor)
            WriteD(writer, 0); //Unk possible Item?

            WriteB(writer, "00000000000000000000000001000000000000000064000000"); //unk

            writer.Seek(4, SeekOrigin.Begin);
            WriteH(writer, (short) (writer.BaseStream.Length)); //Name shift
            writer.Seek(0, SeekOrigin.End);
            WriteS(writer, Player.PlayerData.Name);

            writer.Seek(6, SeekOrigin.Begin);
            WriteH(writer, (short) (writer.BaseStream.Length)); //Details1 shift
            writer.Seek(0, SeekOrigin.End);
            WriteB(writer, Player.PlayerData.Details1);

            writer.Seek(10, SeekOrigin.Begin);
            WriteH(writer, (short)(writer.BaseStream.Length)); //Details2 shift
            writer.Seek(0, SeekOrigin.End);
            WriteB(writer, Player.PlayerData.Details2);//2117 Add


        }
    }
}