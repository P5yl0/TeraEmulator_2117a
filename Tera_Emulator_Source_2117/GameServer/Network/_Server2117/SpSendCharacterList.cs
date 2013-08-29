using Data.Enums;
using Data.Structures.Account;
using Data.Structures.Player;
using System.IO;
using Utils;

namespace Network.Server
{
    public class SpSendCharacterList : ASendPacket
    {
        protected Account Account;

        public SpSendCharacterList(Account account)
        {
            Account = account;
        }

        public override void Write(BinaryWriter writer)
        {
            //Packet    C.Count 2700        [9byte]         Max.Count   ???         ???
            //E894      0000    0000    000000000000000000  08000000    01000000    0000  
            WriteH(writer, (short)Account.Players.Count); //Character Count
            WriteH(writer, (short)(Account.Players.Count == 0 ? 0 : 39));
            WriteB(writer, new byte[9]); //000000000000000000  9bytes to go?!?  8chars= 0100000000 maybe char create allowed yes/no?!?
            WriteD(writer, 8); // 08000000 Max character count
            WriteD(writer, 1); // 01000000 ???
            WriteH(writer, 0); // 0000

            //280000000000000018000000
            WriteB(writer, "280000000000000018000000"); //2117 seems new!!!

            for (int i = 0; i < Account.Players.Count; i++)
            {
                //read needed infos
                Player player = Account.Players[i];

                while ((player.PlayerLevel + 1) != Data.Data.PlayerExperience.Count - 1 && player.PlayerExp >= Data.Data.PlayerExperience[player.PlayerLevel])
                {
                    player.PlayerLevel++;
                }

                short check1 = (short)writer.BaseStream.Position;
                WriteH(writer, check1); //Check1
                WriteH(writer, 0); //Check2 next char posi?

                WriteD(writer, (short)0);  //???  good one!!!

                WriteH(writer, 0); //Name shift
                WriteH(writer, 0); //Details shift
                WriteH(writer, (short)player.PlayerData.Details1.Length); //Details length
                WriteH(writer, 0); //Details shift2
                WriteH(writer, (short)player.PlayerData.Details2.Length); //Details length2

                WriteD(writer, player.PlayerId); //PlayerId
                WriteD(writer, player.PlayerData.Gender.GetHashCode()); //Gender
                WriteD(writer, player.PlayerData.Race.GetHashCode()); //Race
                WriteD(writer, player.PlayerData.Class.GetHashCode()); //Class
                WriteD(writer, player.GetLevel()); //Level

                WriteB(writer, "3B49000067110000"); //A0860100A0860100 Unknown?!?

                WriteB(writer, player.ZoneDatas ?? new byte[12]);
                WriteD(writer, player.PlayerLastOnlineUtc);

                WriteB(writer, "00000000008F5F0100000000007CDBE4AD"); //00000000008F5F0100000000007CDBE4AD  Unknown! 2117

                WriteD(writer, player.Inventory.GetItemId(1) ?? 0); //Item (WeaponSlot)
                WriteD(writer, 0); //Item (earing1 left?)
                WriteD(writer, 0); //Item (earing2 right?)
                WriteD(writer, player.Inventory.GetItemId(3) ?? 0); //Item (ArmorSlot)
                WriteD(writer, player.Inventory.GetItemId(4) ?? 0); //Item (GlovesSlot)
                WriteD(writer, player.Inventory.GetItemId(5) ?? 0); //Item (ShoesSlot)
                WriteD(writer, 0); //Item (unknownSlot)?
                WriteD(writer, 0); //Item (ring1 left?)
                WriteD(writer, 0); //Item (ring2 right?)
                WriteD(writer, 0); //Item (HairSlot ?)
                WriteD(writer, 0); //Item (FaceSlot ?)

                //WriteD(writer, 0); //Item ? 2117 one slot too much 

                WriteB(writer, player.PlayerData.Data);
                WriteC(writer, 0); //Offline?
                WriteB(writer, "00000000000000000000000000ED7BE3AD"); //34bytes Unk 2117

                WriteB(writer, new byte[48]); //Unk

                //Item Color?
                WriteD(writer, player.Inventory.GetItem(1) != null ? player.Inventory.GetItem(1).Color : 0);
                WriteD(writer, player.Inventory.GetItem(3) != null ? player.Inventory.GetItem(3).Color : 0);
                WriteD(writer, player.Inventory.GetItem(4) != null ? player.Inventory.GetItem(4).Color : 0);
                WriteD(writer, player.Inventory.GetItem(5) != null ? player.Inventory.GetItem(5).Color : 0);

                WriteB(writer, new byte[12]); //16 bytes possible colors
                
                WriteD(writer, 0); //Item (Skin Head)
                WriteD(writer, 0); //Item (Skin Face)
                WriteD(writer, 0); //Item (Skin Backpack)?
                WriteD(writer, 0); //Item (Skin Weapon)
                WriteD(writer, 0); //Item (Skin Armor)
                WriteD(writer, 0); //Unk possible Item?


                WriteD(writer, 0); //unk -followed classes
                //WriteB(writer, "0C000000"); // unk berz =12
                //WriteB(writer, "06000000"); // unk sorc =6
                //WriteB(writer, "09000000"); // unk lance =9
                //WriteB(writer, "09000000"); // unk heal =9
                //WriteB(writer, "05000000"); // unk warr = 5
                //WriteB(writer, "00000000"); // unk slay = 0
                //WriteB(writer, "00000000"); // unk arch =0
                //WriteB(writer, "06000000"); // unk myst = 6

                WriteD(writer, 0); //Rested (current)
                WriteD(writer, 10000); //Rested (max)
                
                WriteB(writer, "0100000000000100000000"); //unk 2117 prolog/video?
                                
                /* 1725 removed
                WriteC(writer, 1);
                WriteC(writer, (byte)(player.Exp == 0 ? 1 : 0)); //Intro video flag

                WriteD(writer, 0); //Now start only in Island of Dawn
                //WriteD(writer, player.Exp == 0 ? 1 : 0); //Prolog or IslandOfDawn dialog window
                */

                writer.Seek(check1 + 8, SeekOrigin.Begin);
                WriteH(writer, (short)writer.BaseStream.Length); //Name shift
                writer.Seek(0, SeekOrigin.End);
                WriteS(writer, player.PlayerData.Name);

                writer.Seek(check1 + 10, SeekOrigin.Begin);
                WriteH(writer, (short)writer.BaseStream.Length); //Details shift
                writer.Seek(0, SeekOrigin.End);

                WriteB(writer, player.PlayerData.Details1);
                WriteB(writer, player.PlayerData.Details2);

                if (i != Account.Players.Count - 1)
                {
                    writer.Seek(check1 + 2, SeekOrigin.Begin);
                    WriteH(writer, (short)writer.BaseStream.Length); //Check2
                    writer.Seek(0, SeekOrigin.End);
                }
            }
        }
    }
}