using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vre.Server.BusinessLogic;

namespace CoreClasses
{
    public class SellingAgent
    {
        protected User _user;

        protected SellingAgent(User user) { _user = user; }

        public string ID { get { return _user.AutoID.ToString(); } }
        public string Nickname { get { return _user.NickName; } }
        public string Email { get { return _user.PrimaryEmailAddress; } }

        static public SellingAgent Create(ClientData cd)
        {
            User user = new User(cd);
            if (user.UserRole != User.Role.SellingAgent)
                return null;

            SellingAgent agent = new SellingAgent(user);

            return agent;
        }        
    }
}
