using System;
using System.Collections.Generic;
using System.Reflection;

namespace SteamUtil
{
    public struct User
    {
        public String SteamUsername;
        public ulong Steam64ID;
        public Boolean VacBan;
        public Boolean CommBan;
        public Boolean EcoBan;
        public String LeagacyID;
        public String ProfileURL;
        public String PossibleSteamUsername;
        public DateTime TimeCreated;
        public DateTime LastOnline;
        public Boolean Exists;
        public Boolean ProfileSet;

        public Dictionary<string, object> ToDict()
        {
            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();
            User user = this;

            Dictionary<string, object> values = new Dictionary<string, object>();
            Array.ForEach(fields, (field) => values.Add(field.Name, field.GetValue(user)));
            Array.ForEach(properties, (property) =>
            {
                if (property.CanRead)
                    values.Add(property.Name, property.GetValue(user, null));
            });

            return values;
        }
    }


}
