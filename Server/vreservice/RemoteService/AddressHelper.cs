﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;

namespace Vre.Server.RemoteService
{
    internal static class AddressHelper
    {
        private static readonly List<string> _streetSuffixDictionaryUri = new List<string>
        {
            "1_4", "1_2", "3_4",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };

        public static UpdateableBase ParseGeographicalAddressToModel(ServiceQuery query, ISession dbSession)
        {
            string suite = query["ad_ibn"];

            if (!string.IsNullOrWhiteSpace(suite)) suite = Utilities.NormalizeSuiteNumber(suite);
            else suite = null;

            IList<Building> searchResult = parseGeographicalAddress(query, dbSession);
            UpdateableBase result = null;

            if (searchResult.Count > 0)
            {
                if (searchResult.Count > 1)
                {
                    throw new ArgumentException("Address: non-unique result returned; please add details");
                }
                else
                {
                    Building b = searchResult[0];

                    if ((suite != null) && (b.Suites.Count > 0))
                    {
                        if (b.Suites.Count > 1)
                        {
                            //if (null == suite) throw new ArgumentException("Address: suite number is required");

                            foreach (Suite s in b.Suites)
                            {
                                if (s.SuiteName.Equals(suite))
                                {
                                    result = s;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            result = b.Suites[0];
                        }
                    }
                    else
                    {
                        result = b;
                    }
                }
            }

            return result;
        }

        public static IEnumerable<UpdateableBase> ParseGeographicalAddressToModel(ServiceQuery query, ISession dbSession,
            bool allowMultipleResults)
        {
            var suite = query["ad_ibn"];
            suite = string.IsNullOrWhiteSpace(suite) ? null : Utilities.NormalizeSuiteNumber(suite);

	        var result = new List<UpdateableBase>();

			foreach (var b in parseGeographicalAddress(query, dbSession))
	        {
		        if ((suite != null) && (b.Suites.Count > 0))
				    result.AddRange(b.Suites.Where(s => s.SuiteName.Equals(suite)));
		        else
					result.Add(b);
	        }

			if ((result.Count > 1) && !allowMultipleResults)
				throw new ArgumentException("Address: non-unique result returned; please add details");

            return result;
        }

        private static IList<Building> parseGeographicalAddress(ServiceQuery query, ISession dbSession)
        {
            string country = query["ad_co"];
            string postalCode = query["ad_po"];
            string state = query["ad_stpr"];
            string municipality = query["ad_mu"];
            string street = query["ad_stn"];
            string streetType = query["ad_stt"];
            string streetDirection = query["ad_std"];
            string streetSuffix = query["ad_sts"];
            string building = query["ad_bn"];
            StringBuilder freetextAddress = new StringBuilder();
            //TextInfo textInfo = System.Globalization.CultureInfo.InvariantCulture.TextInfo;

            if (!string.IsNullOrWhiteSpace(country)) //throw new ArgumentException("Address: country is not provided");
                country = country.Trim().ToUpperInvariant(); // textInfo.ToTitleCase(country.Trim().ToLowerInvariant());
            else
                country = null;

            if (!string.IsNullOrWhiteSpace(postalCode)) //throw new ArgumentException("Address: postal code is not provided");
                postalCode = postalCode.Trim().ToUpperInvariant();
            else
                postalCode = null;

            if (!string.IsNullOrWhiteSpace(state)) //throw new ArgumentException("Address: state (province) is not provided");
                state = state.Trim().ToUpperInvariant();
            else
                state = null;

            if (!string.IsNullOrWhiteSpace(municipality)) //throw new ArgumentException("Address: municipality is not provided");
                municipality = municipality.Trim().ToUpperInvariant(); // textInfo.ToTitleCase(municipality.Trim().ToLowerInvariant());
            else
                municipality = null;

            if (!string.IsNullOrWhiteSpace(street)) //throw new ArgumentException("Address: street is not provided");
            {
                street = street.Trim().ToUpperInvariant();

                if (!string.IsNullOrWhiteSpace(streetType))
                {
                    if (streetType.Equals("avenue")) streetType = "AVE";
                    else if (streetType.Equals("court")) streetType = "CT";
                    else if (streetType.Equals("crescent")) streetType = "CR";
                    else if (streetType.Equals("drive")) streetType = "DR";
                    else if (streetType.Equals("road")) streetType = "RD";
                    else if (streetType.Equals("street")) streetType = "ST";
					else if (streetType.Equals("terrace")) streetType = "TERR";
					else if (streetType.Equals("mews")) streetType = "MEWS";
					else if (streetType.Equals("way")) streetType = "WAY";
					else if (streetType.Equals("quay")) streetType = "QUAY";
					else if (streetType.Equals("boulevard")) streetType = "BLVD";
					else if (streetType.Equals("square")) streetType = "SQ";
					else if (streetType.Equals("lane")) streetType = "LANE";
					else if (streetType.Equals("trail")) streetType = "TR";
					else if (streetType.Equals("other")) streetType = "";
                    //else throw new ArgumentException("Address: street type is invalid");
					else streetType = streetType.Trim().ToUpperInvariant();  // assume already abbreviated name
                }
                else
                {
                    streetType = null;
                }

                if (!string.IsNullOrWhiteSpace(streetDirection))
                {
                    if (streetDirection.Equals("n")) streetDirection = "N";
                    else if (streetDirection.Equals("nw")) streetDirection = "NW";
                    else if (streetDirection.Equals("ne")) streetDirection = "NE";
                    else if (streetDirection.Equals("w")) streetDirection = "W";
                    else if (streetDirection.Equals("e")) streetDirection = "E";
                    else if (streetDirection.Equals("sw")) streetDirection = "SW";
                    else if (streetDirection.Equals("se")) streetDirection = "SE";
                    else if (streetDirection.Equals("s")) streetDirection = "S";
                    else throw new ArgumentException("Address: street direction is invalid");
                }
                else
                {
                    streetDirection = null;
                }

                if (!string.IsNullOrWhiteSpace(streetSuffix))
                {
                    if (!_streetSuffixDictionaryUri.Contains(streetSuffix))
                        throw new ArgumentException("Address: street suffix is invalid");

                    streetSuffix = streetSuffix.Replace('_', '/');
                }
                else
                {
                    streetSuffix = null;
                }

                if (!string.IsNullOrWhiteSpace(building)) // throw new ArgumentException("Address: building (house) number is not provided");
                    building = building.Trim().ToUpperInvariant();

                if (!string.IsNullOrWhiteSpace(building)) appendAddressData(ref freetextAddress, building);
				else appendAddressData(ref freetextAddress, "*");

                if (!string.IsNullOrWhiteSpace(street)) appendAddressData(ref freetextAddress, street);
                if (!string.IsNullOrWhiteSpace(streetSuffix)) appendAddressData(ref freetextAddress, streetSuffix);
                if (!string.IsNullOrWhiteSpace(streetType)) appendAddressData(ref freetextAddress, streetType);
                if (!string.IsNullOrWhiteSpace(streetDirection)) appendAddressData(ref freetextAddress, streetDirection);

				freetextAddress.Append('*');
            }

			ServiceInstances.Logger.Debug("Address to parse: c=<{0}>, pc=<{1}>, sp=<{2}>, m=<{3}>, a=<{4}>",
				country, postalCode, state, municipality, freetextAddress);

            using (BuildingDao dao = new BuildingDao(dbSession))
                return dao.SearchByAddress(country, null/*postalCode*/, state, municipality,
                    (freetextAddress.Length > 0) ? freetextAddress.ToString() : null);
        }

		private static void appendAddressData(ref StringBuilder freeTextAddress, string field)
		{
			if (freeTextAddress.Length > 0) freeTextAddress.Append(' ');
			freeTextAddress.Append(field);
		}

        public static ClientData ConvertToNormalizedAddress(Building building, Suite suite)
        {
            ClientData comp = new ClientData();

            // Suite part
            //
            if (suite != null)
            {
                comp.Add("ad_ibn", suite.SuiteName);
                comp.Add("ad_ibt", "apt");
            }

            // Fine-level elements
            // TODO: Needs revamping; currently need some fuzziness for proper processing
            string sta = building.AddressLine1 + " " + building.AddressLine2;

            // TODO: Dirty way of parsing free text address; should split it into elements
            string[] stae = sta.Trim().Split(' ');
            comp.Add("ad_bn", stae[0]);
            if (stae.Length > 1)
            {
                int parsedElements = 0;
                if (stae.Length > 2)
                {
                    string t = null;

                    string d = stae[stae.Length - 1].ToUpperInvariant();
                    if (d.Equals("N")) { comp.Add("ad_std", "n"); parsedElements++; }
                    else if (d.Equals("NW")) { comp.Add("ad_std", "nw"); parsedElements++; }
                    else if (d.Equals("NE")) { comp.Add("ad_std", "ne"); parsedElements++; }
                    else if (d.Equals("W")) { comp.Add("ad_std", "w"); parsedElements++; }
                    else if (d.Equals("E")) { comp.Add("ad_std", "e"); parsedElements++; }
                    else if (d.Equals("SW")) { comp.Add("ad_std", "sw"); parsedElements++; }
                    else if (d.Equals("SE")) { comp.Add("ad_std", "se"); parsedElements++; }
                    else if (d.Equals("E")) { comp.Add("ad_std", "e"); parsedElements++; }
                    else t = d;

                    if ((null == t) && (stae.Length > 3)) t = stae[stae.Length - 2].ToUpperInvariant();

                    if (t != null)
                    {
                        if (t.Equals("AVE")) { comp.Add("ad_stt", "avenue"); parsedElements++; }
                        else if (t.Equals("CT")) { comp.Add("ad_stt", "court"); parsedElements++; }
                        else if (t.Equals("CR")) { comp.Add("ad_stt", "crescent"); parsedElements++; }
                        else if (t.Equals("DR")) { comp.Add("ad_stt", "drive"); parsedElements++; }
                        else if (t.Equals("RD")) { comp.Add("ad_stt", "road"); parsedElements++; }
                        else if (t.Equals("ST")) { comp.Add("ad_stt", "street"); parsedElements++; }
                        else comp.Add("ad_stt", "other");
                    }
                    else comp.Add("ad_stt", "other");
                }

                // SLOW AND DIRTY
                sta = " ";
                int last = stae.Length - parsedElements;
                for (int idx = 1; idx < last; idx++) sta += stae[idx] + " ";
                comp.Add("ad_stn", sta.Trim());
            }
            // comp.Add("ad_sts", building.PostalCode.ToUpperInvariant()); - not supported

            // Coarse-level elements
            //
            comp.Add("ad_co", building.Country.ToUpperInvariant());
            comp.Add("ad_po", building.PostalCode.ToUpperInvariant());
            comp.Add("ad_stpr", building.StateProvince.ToUpperInvariant());
            comp.Add("ad_mu", building.City.ToUpperInvariant());

            return comp;
        }

        public static string ConvertToReadableAddress(Building building, Suite suite)
        {
            StringBuilder rd = new StringBuilder();
            TextInfo textInfo = System.Globalization.CultureInfo.InvariantCulture.TextInfo;

            // Suite part
            //
            if (suite != null) rd.AppendFormat("{0} - ", suite.SuiteName);

            // Fine-level elements
            if (!string.IsNullOrWhiteSpace(building.AddressLine1) || !string.IsNullOrWhiteSpace(building.AddressLine2))
            {
                string sta = building.AddressLine1 + " " + building.AddressLine2;
                sta = sta.Trim();
                if (sta.Length > 0) rd.AppendFormat("{0}, ", sta);
            }

            // Coarse-level elements
            //
            if (!string.IsNullOrWhiteSpace(building.City)) rd.AppendFormat("{0}, ", building.City);
            if (!string.IsNullOrWhiteSpace(building.StateProvince)) rd.AppendFormat("{0}, ", building.StateProvince.ToUpperInvariant());
            if (!string.IsNullOrWhiteSpace(building.PostalCode)) rd.AppendFormat("{0}, ", building.PostalCode.ToUpperInvariant());
            if (!string.IsNullOrWhiteSpace(building.Country)) rd.AppendFormat("{0}, ", building.Country);
            
            string result = rd.ToString();
            if (result.Length > 0) return result.Substring(0, result.Length - 2);  // trim trailing comma
            else return string.Empty;
        }

		public static string GetReadableAddress(this Building b)
		{
			return ConvertToReadableAddress(b, null);
		}

		public static string GetReadableAddress(this Suite s)
		{
			return ConvertToReadableAddress(s.Building, s);
		}

		public static bool IsSameAddress(this Building b, Building other)
		{
			bool aeq = addressElementLooseEqual(b.Country, other.Country)
				&& addressElementLooseEqual(b.StateProvince, other.StateProvince)
				&& addressElementLooseEqual(b.City, other.City)
				&& addressElementLooseEqual(b.AddressLine1, other.AddressLine1)
				&& addressElementLooseEqual(b.AddressLine2, other.AddressLine2);
			return aeq || b.Location.IsEqual(other.Location, 3.0);
		}

		/// <summary>
		/// Loose address element comparison. Assumes any element being null as missing information and thus potentially equal.
		/// </summary>
		private static bool addressElementLooseEqual(string left, string right)
		{
			if (!string.IsNullOrEmpty(left))
			{
				if (!string.IsNullOrEmpty(right)) 
					return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
			}
			return true;
		}

		public static bool IsSameAddress(this Suite s, Suite other)
		{
			return (s.Building.IsSameAddress(other.Building) 
				&& (s.SuiteName.Equals(other.SuiteName, StringComparison.InvariantCultureIgnoreCase)));
		}
	}
}
