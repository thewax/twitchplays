using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using Microsoft.DirectX.DirectInput;
using System.Diagnostics;
//using IrcDotNet;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
//using System.Reflection;

namespace TwitchPlays
{

    //Map Buttons to Emu
    public class BTN
    {
        public const char start = '1';
        public const char select = '2';

        public const char up = 'w';
        public const char down = 's';
        public const char left = 'a';
        public const char right = 'd';

        public const char a = 'q';
        public const char b = 'e';

    }


    class Program
    {

        // ---------  Enter in name of process for emulator here ---------------
        public const string Process_Name = "visualboyadvance";

        public WindowsAPI WindowsAPI = new WindowsAPI();

        public static Output Window = new Output();


        public static void PressKey(char ch, bool press)
        {
            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort scanCode = (ushort)WindowsAPI.MapVirtualKey(vk, 0);

            if (press)
                KeyDown(scanCode);
            else
                KeyUp(scanCode);
        }

        public static void KeyDown(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
            inputs[0].ki.dwFlags = 0;
            inputs[0].ki.wScan = (ushort)(scanCode & 0xff);

            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        public static void KeyUp(ushort scanCode)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
            uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
            if (intReturn != 1)
            {
                throw new Exception("Could not send key: " + scanCode);
            }
        }

        static void FocusWindow()
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName(Process_Name);
            WindowsAPI.SwitchWindow(p[0].MainWindowHandle);
        }

        static void StartWindow()
        {
            Window.ShowDialog();
        }

        static void Main(string[] args)
        {
            // --- FOR LOCAL TESTING -----

            //Console.Write("Enter test command : ");
            //string btn = Console.ReadLine();

            //FocusWindow();
            //MapKeys(btn);

            //Main(args);
            // --- END LOCAL TESTING ----

            // ---  START PROGRAM ----
            //Popup the log window for commands
            Thread formThread = new Thread(StartWindow);
            formThread.Start();

            //Get your oauth info for IRC chat.  http://twitchapps.com/tmi/
            //TODO: Add OAuth automatically
            Console.Write("OAuth: ");
            string pass = Console.ReadLine();
            
            //Start IRC process
            RunIRC(pass);
        }

        static void Sleep(int length = 25)
        {
            System.Threading.Thread.Sleep(length);
        }


        public static void RunIRC(string pass)
        {
            int port;
            string buf, nick, owner, server, chan;
            System.Net.Sockets.TcpClient sock = new System.Net.Sockets.TcpClient();
            System.IO.TextReader input;
            System.IO.TextWriter output;

            nick = "mwax1231";
            owner = "mwax1231";
            server = "irc.twitch.tv";
            port = 6667;
            chan = "#mwax321";
      
            Console.Clear();

            //Connect to irc server and get input and output text streams from TcpClient.
            sock.Connect(server, port);
            if (!sock.Connected)
            {
                Console.WriteLine("Failed to connect!");
                return;
            }
            input = new System.IO.StreamReader(sock.GetStream());
            output = new System.IO.StreamWriter(sock.GetStream());

            //Starting USER and NICK login commands 
            output.Write(
               "USER " + nick + "\n" +
               "PASS " + pass + "\r\n" +
               "NICK " + nick + "\r\n"
            );
            output.Flush();

            //Process each line received from irc server
            while ((buf = input.ReadLine()) != null)
            {

                //Display received irc message
                //
                if (buf != null)
                {
                    Console.WriteLine(buf);
                    checkCommands(buf);

                    //Send pong reply to any ping messages
                    if (buf.StartsWith("PING ") || (buf.Contains("PING") && buf.Contains("mwax321"))) { output.Write(buf.Replace("PING", "PONG") + "\r\n"); output.Flush(); }

                    if (buf.Contains("mwax321") && buf.Contains("clear"))
                    {
                        Console.Clear();
                    }

                    if (buf[0] != ':') continue;

                    /* IRC commands come in one of these formats:
                     * :NICK!USER@HOST COMMAND ARGS ... :DATA\r\n
                     * :SERVER COMAND ARGS ... :DATA\r\n
                     */

                    //After server sends 001 command, we can set mode to bot and join a channel
                    if (buf.Split(' ')[1] == "001")
                    {
                        output.Write(
                           "MODE " + nick + " +B\r\n" +
                           "JOIN " + chan + "\r\n"
                        );
                        output.Flush();
                    }
                }
                else
                {
                    Console.WriteLine("Null detected... ");
                    RunIRC(pass);
                }
            }
            Console.WriteLine("Null detected... ");
            RunIRC(pass);
        }

        public static void Button(char btn, int sleep = 50)
        {
            FocusWindow();
            PressKey(btn, true);
            Sleep(sleep);
            PressKey(btn, false);
        }

        public static void ButtonCombo(char[] btn)
        {
            foreach (var b in btn)
            {
                PressKey(b, true);
            }
            Sleep(25);
            foreach (var b in btn)
            {
                PressKey(b, false);
            }
        }

        //Map keys from IRC chat. If you want to use a different emulator or system, you will need to change this section to accept new commands from IRC
        private static bool MapKeys(string command)
        {
            bool commandReal = true;
            switch (command.ToLower())
            {
                case "a":
                    Button(BTN.a);
                    break;
                case "b":
                    Button(BTN.b);
                    break;
                case "start":
                    Button(BTN.start);
                    break; 
                case "select":
                    Button(BTN.select);
                    break;
                case "pickup":
                    ButtonCombo(new char[] { BTN.down, BTN.a });
                    break;
                case "pickup-up":
                    ButtonCombo(new char[] { BTN.up, BTN.a });
                    break;
                case "pickup-down":
                    ButtonCombo(new char[] { BTN.down, BTN.a });
                    break;
                case "pickup-left":
                    ButtonCombo(new char[] { BTN.left, BTN.a });
                    break;
                case "pickup-right":
                    ButtonCombo(new char[] { BTN.right, BTN.a });
                    break;
                case "up":
                    Button(BTN.up, 200);
                    break;
                case "down":
                    Button(BTN.down, 200);
                    break;
                case "left":
                    Button(BTN.left, 200);
                    break;
                case "right":
                    Button(BTN.right,200);
                    break;
                default:
                    commandReal = false;
                    break;
            }

            return commandReal;
        }


        private static void checkCommands(string input, int Player = 0)
        {
            //TODO: Refactor this into one regex. I got lazy and just piled crap upon crap

            //Look for messages from users and then get the user and command
            string regex = @":.* PRIVMSG.* :";
            string regexFullMatch = @"^:(\w+)(?:!(\w+)@([\w\.]+))? PRIVMSG (#\w+) :(.+)$";

            if (Regex.IsMatch(input, regex))
            {
                //User doesn't have a userID
                string user = "god";
                if (Regex.IsMatch(input, regexFullMatch))
                {
                    var m = Regex.Match(input, regexFullMatch);
                    user = m.Groups[1].Value;
                }
                string command = Regex.Replace(input, regex, "");
                bool commandReal = MapKeys(command);

                //Send command to log window and console
                if (commandReal)
                {
                    Window.AddText(user + ": " + command);
                    Console.WriteLine(user + ": " + command);
                }
            }
        }
    }


}
