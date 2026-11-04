using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace tounsiYoyo.Model
{
    public class Business
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public Special special { get; set; }

        public Business(string ID, string Name, string Logo, Special special)
        {
            this.ID = ID;
            this.Name = Name;
            this.Logo = Logo;
            this.special = special;
        }

        public void Export()
        {
            string json = JsonSerializer.Serialize(this);

            File.WriteAllText("tounsi.json", json);
        }
    }
}
