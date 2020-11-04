using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps
{

    public enum Category
    {
        Sport,
        Meet,
        Event,
        Party
    }

    public class Meet
    {
        #region PinTypes
        public static string PinType(Category cat)
        {
            if (cat == Category.Party)
            {
                return "Party";
            }
            else if (cat == Category.Meet)
            {
                return "Meet";
            }
            else if (cat == Category.Sport)
            {
                return "Sport";
            }
            return "Event";
        }

        public static Category RevPinType(string cat)
        {
            if (cat == "Party")
            {
                return Category.Party;
            }
            else if (cat == "Meet")
            {
                return Category.Meet;
            }
            else if (cat == "Sport")
            {
                return Category.Sport;
            }
            return Category.Event;
        }
        #endregion

        private string desc;
        private string creator;
        private Category categ;
        private string participants;
        private string date;
        private Pushpin pin;
        private string id;


        public Meet(string d, string c, Category ca, string p, string da, string i)
        {
            desc = d;
            creator = c;
            categ = ca;
            participants = p;
            date = da;
            id = i;
        }

        

        public Pushpin Pin { get => pin; set => pin = value; }
        public string GetDescription { get => desc;  }
        public string GetDate { get => date;  }
        public string GetCreator { get => creator; }
        public Category GetCategory { get => categ; }
        public string GetParticipants { get => participants; }
        public string ID { get => id; set => id = value; }
    }
}
