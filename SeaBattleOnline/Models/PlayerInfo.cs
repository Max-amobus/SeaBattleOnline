using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class PlayerInfo
    {
        public string Name { get; set; } = "Unknown";   
        public string IP { get; set; } = "";          
        public bool IsReady { get; set; } = false;      
        public bool IsConnected { get; set; } = true;   

        public PlayerInfo(string name, string ip)
        {
            Name = name;
            IP = ip;
        }

        public override string ToString()
        {
            return $"{Name} ({IP})";
        }
    }
}
