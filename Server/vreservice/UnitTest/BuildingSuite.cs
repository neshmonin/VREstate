using NUnit.Framework;
using Vre.Server.BusinessLogic;
using System;
using System.Collections.Generic;
using Vre.Server.Dao;
using NHibernate;
using NHibernate.Criterion;

namespace Vre.Server.UnitTest
{
#if DEBUG_

    [TestFixture]
    internal class BuildingSuite
    {
        const int LevelCount = 24;
        const int AptsPerFloor = 12;

        private int estateDevId;

        //[TestFixtureSetUp]
        //public void Setup()
        //{
        //    using (EstateDeveloperDao dao = new EstateDeveloperDao())
        //    {
        //        EstateDeveloper ed = new EstateDeveloper(EstateDeveloper.Configuration.Kiosk_SingleScreen);
        //        dao.Create(ed);
        //        estateDevId = ed.AutoID;
        //    }
        //}

        [Test]
        public void T000BuildBuilding()
        {
            using (ISession session = NHibernateHelper.GetSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    EstateDeveloper ed = new EstateDeveloper(EstateDeveloper.Configuration.Kiosk_SingleScreen);
                    using (EstateDeveloperDao dao = new EstateDeveloperDao(session))
                    {
                        dao.Create(ed);
                    }

                    Site site1;
                    using (SiteDao sdao = new SiteDao(session)) site1 = sdao.GetById(19);

                    Site site = new Site(ed, "site one");
                    using (SiteDao sdao = new SiteDao(session)) sdao.Create(site);

                    Building b = new Building(site, "bldg one");
                    using (BuildingDao bdao = new BuildingDao(session)) bdao.Create(b);


                    SuiteType stype = new SuiteType(site, "type one");
                    using (SuiteTypeDao stdao = new SuiteTypeDao(session)) stdao.Create(stype);

                    SuiteLevel slevel = new SuiteLevel(stype, "Main Floor");
                    using (SuiteLevelDao sldao = new SuiteLevelDao(session)) sldao.Create(slevel);

                    RoomCategory rc1, rc2, rc3, rc4, rc5;
                    using (RoomCategoryDao rcdao = new RoomCategoryDao(session))
                    {
                        rc1 = rcdao.CreateOrGetByName("Entrance");
                        rc2 = rcdao.CreateOrGetByName("Main");
                        rc3 = rcdao.CreateOrGetByName("Washroom");
                        rc4 = rcdao.CreateOrGetByName("Dining");
                        rc5 = rcdao.CreateOrGetByName("Kitchen");
                    }

                    Room r1 = new Room(slevel, rc1);
                        r1.FloorArea.SetValue(185.0, ValueWithUM.Unit.SqFeet);
                        r1.PlinthLength.SetValue(49.0, ValueWithUM.Unit.Feet);
                        r1.WallsArea.SetValue(37.0, ValueWithUM.Unit.SqMeters);
                        r1.Location.Longitude = -79.0;
                        r1.Location.Latitude = 47.0;
                        r1.Location.Altitude = 183.4;
                        r1.Location.HorizontalHeading = 138.0;

                    Room r2 = new Room(slevel, rc2);
                        r2.FloorArea.SetValue(254.0, ValueWithUM.Unit.SqFeet);
                        r2.PlinthLength.SetValue(64.0, ValueWithUM.Unit.Feet);
                        r2.WallsArea.SetValue(51.0, ValueWithUM.Unit.SqMeters);
                        r2.Location.Longitude = -79.0;
                        r2.Location.Latitude = 47.0;
                        r2.Location.Altitude = 183.43;
                        r2.Location.HorizontalHeading = 98.0;

                    using (RoomDao rdao = new RoomDao(session))
                    {
                        rdao.Create(r1);
                        rdao.Create(r2);
                    }

                    Suite s = new Suite(b, 1, "1", "101");
                    //s.SuiteType = stype;
                    using (SuiteDao sdao = new SuiteDao(session)) sdao.Create(s);


                    tran.Commit();
                }
            }

            //Building b = new Building(estateDevId);

            //using (BuildingDao bdao = new BuildingDao())
            //{
            //    bdao.Create(b);
            //    Assert.Greater(b.AutoID, 0, "Building record creation fault!");
            //}

            //using (SuiteDao dao = new SuiteDao())
            //{
            //    for (int level = 1; level <= LevelCount; level++)
            //    {
            //        int floor = level;
            //        if (floor >= 13) floor++;

            //        for (int apt = 1; apt <= AptsPerFloor; apt++)
            //        {
            //            Suite s = new Suite(b, level, floor, (floor * 100 + apt).ToString());
            //            dao.Create(s);
            //        }
            //    }
            //}

            //using (SuiteDao dao = new SuiteDao())
            //{
            //    IList<Suite> ls =  dao.GetAll(b.AutoID);

            //    Assert.AreEqual(ls.Count, LevelCount * AptsPerFloor, "Some suited are missing!");

            //    foreach (Suite s in ls) dao.Delete(s);
            //}

            //using (BuildingDao dao = new BuildingDao())
            //{
            //    IList<Building> bl = dao.GetAll(estateDevId);
            //    Assert.AreEqual(bl.Count, 1, "Building count is invalid!");
            //    dao.Delete(bl[0]);
            //}
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            using (EstateDeveloperDao dao = new EstateDeveloperDao())
            {
                EstateDeveloper ed = dao.GetById(estateDevId);
                dao.Delete(ed);
            }
        }
    }

#endif
}