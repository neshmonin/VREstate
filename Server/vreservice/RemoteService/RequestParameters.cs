using System;
using System.Web;
using Vre.Server.BusinessLogic;

namespace Vre.Server.RemoteService
{
	static class RequestParameters
	{
		public static bool HasPropertyType(this IRequestData request)
		{
			return request.Query.Contains("propertytype")
				|| request.Query.Contains("propertyType");  // OBSOLETE URI
		}

		public static ViewOrder.SubjectType GetPropertyType(this IRequestData request)
		{
			var arg = request.Query["propertytype"];
			if (string.IsNullOrEmpty(arg)) arg = request.Query["propertyType"];  // OBSOLETE URI

			ViewOrder.SubjectType result;
			if (arg.Equals("suite"))
				result = ViewOrder.SubjectType.Suite;
			else if (arg.Equals("building"))
				result = ViewOrder.SubjectType.Building;
			else
				throw new ArgumentException("Unknown property type");

			return result;
		}

		public static bool HasPropertyId(this IRequestData request)
		{
			return request.Query.Contains("propertyid")
				|| request.Query.Contains("propertyId");  // OBSOLETE URI
		}

		public static int GetPropertyId(this IRequestData request)
		{
			var arg = request.Query["propertyid"];
			if (string.IsNullOrEmpty(arg)) arg = request.Query["propertyId"];  // OBSOLETE URI

			int result;

			if (!int.TryParse(arg, out result))
				throw new ArgumentException("Property ID is not valid");

			return result;
		}

		public static bool HasMlsId(this IRequestData request)
		{
			return request.Query.Contains("mls_id");
		}

		public static string GetMlsId(this IRequestData request)
		{
			return request.Query["mls_id"];
		}

		public static bool HasMlsUrl(this IRequestData request)
		{
			return request.Query.Contains("mls_url");
		}

		public static string GetMlsUrl(this IRequestData request)
		{
			return HttpUtility.UrlDecode(request.Query["mls_url"]);
		}

		public static bool HasProductType(this IRequestData request)
		{
			return request.Query.Contains("product");
		}

		public static ViewOrder.ViewOrderProduct GetProductType(this IRequestData request)
		{
			var arg = request.Query["product"];

			ViewOrder.ViewOrderProduct result;
			if (arg.Equals("prl"))
				result = ViewOrder.ViewOrderProduct.PrivateListing;
			else if (arg.Equals("pul"))
				result = ViewOrder.ViewOrderProduct.PublicListing;
			else if (arg.Equals("b3dl"))
				result = ViewOrder.ViewOrderProduct.Building3DLayout;
			else
				throw new ArgumentException("Unknown product type");

			return result;
		}

		public static bool HasProductOptions(this IRequestData request)
		{
			return request.Query.Contains("options");
		}

		public static ViewOrder.ViewOrderOptions GetProductOptions(this IRequestData request)
		{
			var arg = request.Query["options"];

			ViewOrder.ViewOrderOptions result;
			if (string.IsNullOrEmpty(arg))
				result = ViewOrder.ViewOrderOptions.FloorPlan;
			else if (arg.Equals("fp"))
				result = ViewOrder.ViewOrderOptions.FloorPlan;
			else if (arg.Equals("evt"))
				result = ViewOrder.ViewOrderOptions.ExternalTour;
			else if (arg.Equals("3dt"))
				result = ViewOrder.ViewOrderOptions.VirtualTour3D;
			else
				throw new ArgumentException("Unknown product option");

			return result;
		}
	}
}