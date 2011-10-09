using NUnit.Framework;
using Vre.Server.BusinessLogic;
using System;
using Vre.Server.Dao;

namespace Vre.Server.UnitTest
{
#if DEBUG_

    [TestFixture]
    internal class UserManagement
    {
        [Test]
        public void T000_LoginCreate()
        {
            int id;

            using (UserDao dao = new UserDao())
            {
                User sa = new User(null, User.Role.Buyer);
                dao.Create(sa);
                Assert.Greater(sa.AutoID, 0, "User record creation fault!");
                id = sa.AutoID;

                string errorText;
                using (IAuthentication auth = new Authentication(null))
                {
                    if (!auth.CreateLogin(LoginType.Plain, "test", "test", sa.AutoID, out errorText))
                    {
                        dao.Delete(sa);
                        throw new Exception("Unable to create superadmin credentials: " + errorText);
                    }
                }
            }

            using (UserDao dao = new UserDao())
            {
                int uid;

                using (IAuthentication auth = new Authentication(null))
                {
                    if (!auth.AuthenticateUser(LoginType.Plain, "test", "test", out uid))
                    {
                        throw new Exception("Authentication failed!");
                    }
                }

                Assert.AreEqual(uid, id, "Authentication returns wrong ID!");

                User sa = dao.GetById(uid);
                Assert.IsNotNull(sa, "User record cannot be retrieved!");
                Assert.AreEqual(sa.AutoID, uid, "User record retrieved is wrong!");

                Assert.IsNotNull(sa.PersonalInfo, "Personal info is null!");

                dao.Delete(sa);

                using (IAuthentication auth = new Authentication(null))
                {
                    if (!auth.DropLogin(LoginType.Plain, "test"))
                    {
                        throw new Exception("Login drop failed!");
                    }
                }
            }

            using (UserDao dao = new UserDao())
            {
                int uid;

                using (IAuthentication auth = new Authentication(null))
                {
                    if (auth.AuthenticateUser(LoginType.Plain, "test", "test", out uid))
                    {
                        throw new Exception("Login drop failed (2)!");
                    }
                }

                User sa = dao.GetById(id);
                Assert.IsNull(sa, "User record not deleted!");
            }
        }

        User adminUser;
        EstateDeveloper ed;
        int referenceOne = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            using (UserManager um = new UserManager(null))
            {
                adminUser = um.Login(LoginType.Plain, "admin", "admin");

                ed = um.CreateEstateDeveloper(adminUser, EstateDeveloper.Configuration.Kiosk_SingleScreen);
            }

            Assert.IsNotNull(ed);
            Assert.Greater(ed.AutoID, 0);
        }

        [Test]
        public void T001_LoginCreate()
        {
            using (UserManager um = new UserManager(null))
            {
                string errorText;
                Assert.IsTrue(um.CreateUser(adminUser, User.Role.Buyer, ed.AutoID, LoginType.Plain, "buyer_t001", "pass", out errorText));

                User u = um.Login(LoginType.Plain, "buyer_t001", "pass");
                Assert.IsNotNull(u);
                Assert.AreEqual(u.UserRole, User.Role.Buyer);
                Assert.AreEqual(u.EstateDeveloperID, ed.AutoID);
            }
        }

        [Test]
        public void T002_LoginVerify()
        {
            using (UserManager um = new UserManager(null))
            {
                string errorReason;
                User u = um.Login(LoginType.Plain, "buyer_t001", "pass");
                Assert.IsNotNull(u);

                int eid = u.EstateDeveloperID.Value;

                int count = um.ListUsers(User.Role.Buyer, eid, null, false).Length;
                Assert.AreEqual(count, referenceOne);
                Assert.AreEqual(um.ListUsers(User.Role.Buyer, eid, "buy", false).Length, 0);

                u.PersonalInfo.FirstName = "t001_buyer";
                um.UpdateUser(adminUser, u, out errorReason);

                count = um.ListUsers(User.Role.Buyer, eid, "buy", false).Length;
                Assert.AreEqual(count, referenceOne);
            }
        }

        [Test]
        public void T003_LoginDelete()
        {
            using (UserManager um = new UserManager(null))
            {
                User u = um.Login(LoginType.Plain, "buyer_t001", "pass");
                Assert.IsNotNull(u);

                int eid = u.EstateDeveloperID.Value;

                string errorText;
                Assert.IsTrue(um.DeleteUser(adminUser, u, out errorText));

                Assert.AreEqual(um.ListUsers(User.Role.Buyer, eid, "buy", false).Length, 0);
                int count = um.ListUsers(User.Role.Buyer, eid, "buy", true).Length;
                Assert.AreEqual(count, referenceOne);
            }
        }

        [Test]
        public void T999_Cleanup()
        {
            using (UserManager um = new UserManager(null)) um.DeleteEstateDeveloper(adminUser, ed);
        }
    }

#endif
}