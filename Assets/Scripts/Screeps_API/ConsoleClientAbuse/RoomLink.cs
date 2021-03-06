﻿using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Screeps_API.ConsoleClientAbuse
{
    public class RoomLink : IConsoleClientAbuse
    {
        public void Abuse(ScreepsConsole.ConsoleMessage message)
        {
            message.Message = ParseAndReplace(message);
        }

        public static string ParseAndReplace(string message)
        {
            // find room link
            // <a href=\"#!/room//W2N2\">[W2N2 17,24]</a>
            string roomLinkPattern = @"<a href=\""#!\/room\/(?<shard>.*?)\/(?<room>.+?)\"">(?<text>.+?)<\/a>";
            foreach (Match m in Regex.Matches(message, roomLinkPattern))
            {
                // #!/room/botarena/W2N2
                var shard = m.Groups["shard"].Value; // botarena
                var room = m.Groups["room"].Value; // W2N2
                var text = m.Groups["text"].Value; // [W2N2 17,24]

                // TODO: extract coordinates?
                // TODO: add links to ConsoleMessage

                message = message.Replace(m.Value, FormatTMPLink(shard, room, text));
            }

            return message;
        }

        public static string FormatTMPLink(string shard, string room, string text = null)
        {
            return string.Format(@"<link=""{0}/{1}"">{2}</link>", shard, room, text);
        }
    }
}
