using NUnit.Framework;
using Vre.Server.BusinessLogic;
using System;
using System.Collections.Generic;
using Vre.Server.Dao;
using NHibernate;
using NHibernate.Criterion;

namespace Vre.Server.UnitTest
{
#if DEBUG

    [TestFixture]
    internal class JSON
    {
        [Test]
        public void T000ToFromConversionBasic()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsFalse(result);
        }

        [Test]
        public void T001ToFromConversionChanged()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            json = json.Replace("43.7", "44.1");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T002ToFromConversionChanged()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            json = json.Replace("201", "202");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T003ToFromConversionChanged()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            json = json.Replace("\"status\":0", "\"status\":1");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T004ToFromConversionUnchanged()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            json = json.Replace("\"status\":0", "\"status\":\"Available\"");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsFalse(result);
        }

        [Test]
        public void T005ToFromConversionChanged()
        {
            Suite s = new Suite(null, 1, "2", "201");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.Location.HorizontalHeading = 334.0;

            string json = JavaScriptHelper.BoToJson(s);

            json = json.Replace("\"status\":0", "\"status\":\"OnHold\"");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T006ToFromDateTime()
        {
            Building s = new Building(null, "MyName");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.OpeningDate = DateTime.Now;

            string json = JavaScriptHelper.BoToJson(s);

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsFalse(result);
        }

        [Test]
        public void T007ToFromDateTimeChanged()
        {
            Building s = new Building(null, "MyName");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.OpeningDate = DateTime.Parse("2011-05-25 12:23:34");

            string json = JavaScriptHelper.BoToJson(s);

            // "{\"id\":0,\"name\":\"MyName\",\"status\":0,\"openingDate\":\"\\/Date(1306340614000)\\/\",\"position\":{\"lon\":-79.5,\"lat\":43.7,\"alt\":0}}"
            json = json.Replace("\"openingDate\":\"\\/Date(1306340614000)\\/\"", "\"openingDate\":\"\\/Date(1306340614001)\\/\"");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T008ToFromDateTimeChanged()
        {
            Building s = new Building(null, "MyName");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;
            s.OpeningDate = DateTime.Parse("2011-05-25 12:23:34");

            string json = JavaScriptHelper.BoToJson(s);

            // "{\"id\":0,\"name\":\"MyName\",\"status\":0,\"openingDate\":\"\\/Date(1306340614000)\\/\",\"position\":{\"lon\":-79.5,\"lat\":43.7,\"alt\":0}}"
            json = json.Replace("\"openingDate\":\"\\/Date(1306340614000)\\/\"", "\"openingDate\":null");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T009ToFromDateTimeChanged()
        {
            Building s = new Building(null, "MyName");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;

            string json = JavaScriptHelper.BoToJson(s);

            // "{\"id\":0,\"name\":\"MyName\",\"status\":0,\"openingDate\":null,\"position\":{\"lon\":-79.5,\"lat\":43.7,\"alt\":0}}"
            json = json.Replace("\"openingDate\":null", "\"openingDate\":\"\\/Date(1306340614000)\\/\"");

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsTrue(result);
        }

        [Test]
        public void T010ToFromDateTime()
        {
            Building s = new Building(null, "MyName");
            s.Location.Longitude = -79.5;
            s.Location.Latitude = 43.7;

            string json = JavaScriptHelper.BoToJson(s);

            ClientData cd = JavaScriptHelper.JsonToClientData(json);

            bool result = s.UpdateFromClient(cd);

            Assert.IsFalse(result);
        }
    }

#endif
}