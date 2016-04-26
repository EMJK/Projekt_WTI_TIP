using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarChatApi
{
    class Connection
    {
        public string ConnectionID { get; set; }
        public string UserID { get; set; }

        public override bool Equals(object obj)
        {
            var obj2 = obj as Connection;
            if (obj2 == null) return false;
            return obj2.UserID == UserID && obj2.ConnectionID == ConnectionID;
        }

        public override int GetHashCode()
        {
            return (ConnectionID?.GetHashCode() ?? 0) ^ (UserID?.GetHashCode() ?? 0);
        }
    }
}
