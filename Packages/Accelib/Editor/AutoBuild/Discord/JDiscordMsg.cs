using System.Collections.Generic;

namespace Accelib.Editor.Utility.Discord
{
    [System.Serializable]
    public class JDiscordMsg
    {
        public string content;
        public JDiscordEmbed[] embeds;
    }
}