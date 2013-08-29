using Communication.Logic;
using Data.Enums;
using Data.Structures.Player;
using Network.Server;
using System;
using System.Text;
using Tera.Services;
using Utils;

namespace Network.Client
{
    public class CpCreateCharacter : ARecvPacket
    {
        protected PlayerData PlayerData = new PlayerData();

        public override void Read()
        {
            //Shifts
            short nameShift = (short)ReadH();
            short detailsShift1 = (short)ReadH();
            short detailsLength1 = (short)ReadH();
            short detailsShift2 = (short)ReadH();
            short detailsLength2 = (short)ReadH();

            PlayerData.Gender = (Gender)ReadD();
            PlayerData.Race = (Race)ReadD();
            PlayerData.Class = (PlayerClass)ReadD();
            PlayerData.Data = ReadB(8);

            ReadD();
            ReadC();

            PlayerData.Name = ReadS();

            PlayerData.Details1 = ReadB(detailsLength1);
            PlayerData.Details2 = ReadB(detailsLength2);

        }

        public override void Process()
        {
            PlayerLogic.CreateCharacter(Connection, PlayerData);
        }
    }
}