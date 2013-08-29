using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Crypt;
using Microsoft.Win32;
using PacketViewer.Macros;
using Utils;

namespace PacketViewer
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Packet.Init();

            foreach (var packetName in Packet.ClientPacketNames)
                PacketNamesList.Items.Add(packetName.Value);

            PacketNamesList.Items.Add(new Separator
                                          {
                                              HorizontalAlignment = HorizontalAlignment.Stretch,
                                              IsEnabled = false
                                          });

            foreach (var packetName in Packet.ServerPacketNames)
                PacketNamesList.Items.Add(packetName.Value);

            PacketNamesList.SelectedIndex = 0;

            OpenFile(null, null);

            // ReSharper disable ObjectCreationAsStatement
            new FindAllNpcs(this);
            new FindAllTraidlists(this);
            new FindAllGatherables(this);
            new FindAllClimbs(this);
            new FindAllCampfires(this);
            // ReSharper restore ObjectCreationAsStatement
        }

        protected Session Session;
        protected int State;

        protected byte[] ServerBuffer;
        protected byte[] ClientBuffer;

        public List<Packet> Packets;

        public void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog {Filter = "*.hex|*.hex"};

            if (openFileDialog.ShowDialog() == false)
                return;

            Session = new Session();
            State = -1;

            ServerBuffer = new byte[0];
            ClientBuffer = new byte[0];

            Packets = new List<Packet>();

            PacketsList.Items.Clear();

            using (FileStream fileStream = File.OpenRead(openFileDialog.FileName))
            {
                using (TextReader stream = new StreamReader(fileStream))
                {
                    while (true)
                    {
                        string line = stream.ReadLine();
                        if (line == null)
                            break;
                        if (line.Length == 0)
                            continue;
                        if (State == -1)
                        {
                            State = 0;
                            continue;
                        }

                        bool isServer = line[0] == ' ';

                        string hex = line.Substring(isServer ? 14 : 10, 49).Replace(" ", "");
                        byte[] data = hex.ToBytes();

                        if (isServer)
                        {
                            AppendServerData(data);
                            // ReSharper disable CSharpWarnings::CS0642
                            while (ProcessServerData()) ;
                            // ReSharper restore CSharpWarnings::CS0642
                        }
                        else
                        {
                            AppendClientData(data);
                            // ReSharper disable CSharpWarnings::CS0642
                            while (ProcessClientData()) ;
                            // ReSharper restore CSharpWarnings::CS0642
                        }
                    }
                }
            }

            SetText("Loaded " + Packets.Count + " packets...");
        }

        protected void AppendServerData(byte[] data)
        {
            if (State == 2)
                Session.Encrypt(ref data);

            Array.Resize(ref ServerBuffer, ServerBuffer.Length + data.Length);
            Array.Copy(data, 0, ServerBuffer, ServerBuffer.Length - data.Length, data.Length);
        }

        protected byte[] GetServerData(int length)
        {
            byte[] result = new byte[length];
            Array.Copy(ServerBuffer, result, length);

            byte[] reserve = (byte[]) ServerBuffer.Clone();
            ServerBuffer = new byte[ServerBuffer.Length - length];
            Array.Copy(reserve, length, ServerBuffer, 0, ServerBuffer.Length);

            return result;
        }

        protected void AppendClientData(byte[] data)
        {
            if (State == 2)
                Session.Decrypt(ref data);

            Array.Resize(ref ClientBuffer, ClientBuffer.Length + data.Length);
            Array.Copy(data, 0, ClientBuffer, ClientBuffer.Length - data.Length, data.Length);
        }

        protected byte[] GetClientData(int length)
        {
            byte[] result = new byte[length];
            Array.Copy(ClientBuffer, result, length);

            byte[] reserve = (byte[]) ClientBuffer.Clone();
            ClientBuffer = new byte[ClientBuffer.Length - length];
            Array.Copy(reserve, length, ClientBuffer, 0, ClientBuffer.Length);

            return result;
        }

        protected bool ProcessServerData()
        {
            switch (State)
            {
                case 0:
                    if (ServerBuffer.Length < 128)
                        return false;
                    Session.ServerKey1 = GetServerData(128);
                    State++;
                    return true;
                case 1:
                    if (ServerBuffer.Length < 128)
                        return false;
                    Session.ServerKey2 = GetServerData(128);
                    Session.Init();
                    State++;
                    return true;
            }

            if (ServerBuffer.Length < 4)
                return false;

            int length = BitConverter.ToUInt16(ServerBuffer, 0);

            if (ServerBuffer.Length < length)
                return false;

            short opCode = BitConverter.ToInt16(ServerBuffer, 2);

            Packets.Add(new Packet(true, opCode, GetServerData(length)));

            string itemText = string.Format("[S] {0} [{1}]"
                                            , Packets[Packets.Count - 1].Name
                                            , Packets[Packets.Count - 1].Data.Length
                );

            ListBoxItem item = new ListBoxItem
                                   {
                                       Content = itemText,
                                       Background = new SolidColorBrush(Colors.LightBlue)
                                   };

            PacketsList.Items.Add(item);

            return false;
        }

        protected bool ProcessClientData()
        {
            switch (State)
            {
                case 0:
                    if (ClientBuffer.Length < 128)
                        return false;
                    Session.ClientKey1 = GetClientData(128);
                    return true;
                case 1:
                    if (ClientBuffer.Length < 128)
                        return false;
                    Session.ClientKey2 = GetClientData(128);
                    return true;
            }

            if (ClientBuffer.Length < 4)
                return false;

            int length = BitConverter.ToUInt16(ClientBuffer, 0);

            if (ClientBuffer.Length < length)
                return false;

            short opCode = BitConverter.ToInt16(ClientBuffer, 2);

            Packets.Add(new Packet(false, opCode, GetClientData(length)));

            string itemText = string.Format("[C] {0} [{1}]"
                                            , Packets[Packets.Count - 1].Name
                                            , Packets[Packets.Count - 1].Data.Length
                );

            ListBoxItem item = new ListBoxItem
                                   {
                                       Content = itemText,
                                       Background = new SolidColorBrush(Colors.WhiteSmoke)
                                   };

            PacketsList.Items.Add(item);

            return false;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetHex(string text)
        {
            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            HexTextBox.Document.Blocks.Clear();
                            HexTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
                        }));
        }

        public void SetText(string text)
        {
            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            TextBox.Document.Blocks.Clear();
                            TextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
                        }));
        }

        private void OnPacketSelect(object sender, SelectionChangedEventArgs e)
        {
            if (PacketsList.SelectedIndex == -1)
                return;
            
            SetHex(Packets[PacketsList.SelectedIndex].Hex);
            SetText(Packets[PacketsList.SelectedIndex].Text);

            OpCodeBox.Text = Packets[PacketsList.SelectedIndex].Hex.Substring(0, 4);
        }

        private void FindByName(object sender, RoutedEventArgs e)
        {
            if (Packets == null)
                return;

            string name = PacketNamesList.SelectedItem.ToString();

            for (int i = PacketsList.SelectedIndex + 1; i < Packets.Count; i++)
            {
                if (Packets[i].Name == name)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }

            if (MessageBox.Show("Find from start?", "Not found", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            for (int i = 0; i < PacketsList.SelectedIndex; i++)
            {
                if (Packets[i].Name == name)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }
        }

        private void FindByHex(object sender, RoutedEventArgs e)
        {
            if (Packets == null)
                return;

            string hex = HexBox.Text.Replace(" ", "");

            for (int i = PacketsList.SelectedIndex + 1; i < Packets.Count; i++)
            {
                if (Packets[i].Hex.IndexOf(hex, 4, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }

            if (MessageBox.Show("Find from start?", "Not found", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            for (int i = 0; i < PacketsList.SelectedIndex; i++)
            {
                if (Packets[i].Hex.IndexOf(hex, 4, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }
        }

        private void FindByOpCode(object sender, RoutedEventArgs e)
        {
            if (Packets == null)
                return;

            string hex = OpCodeBox.Text.Replace(" ", "");

            for (int i = PacketsList.SelectedIndex + 1; i < Packets.Count; i++)
            {
                if (Packets[i].Hex.IndexOf(hex, 0, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }

            if (MessageBox.Show("Find from start?", "Not found", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            for (int i = 0; i < PacketsList.SelectedIndex; i++)
            {
                if (Packets[i].Hex.IndexOf(hex, 0, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    PacketsList.SelectedIndex = i;
                    PacketsList.ScrollIntoView(PacketsList.SelectedItem);
                    return;
                }
            }
        }
    }
}
