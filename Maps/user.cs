
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;

namespace Maps
{

    public class User
    {
        private string name;
        private Geoposition position;
        private string id;

        public User(string n, string i)
        {
            name = n;
            id = i;
        }

        public string GetName { get => name; }
        public Geoposition GetPosition { get => position; }
        public Geoposition SetPosition { set => position = value; }
        public string GetId { get => id; }
    }
}
