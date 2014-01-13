using System;
using System.IO;
using System.Reflection;

namespace Vre.Server
{
	public static class Configuration
	{
		private static readonly ConfigurationBase _config = new AppFileConfiguration();

		static Configuration()
		{
			var path = Assembly.GetExecutingAssembly().GetName().CodeBase;
			if (path.StartsWith("file:///")) 
				path = path.Substring(8).Replace('/', Path.DirectorySeparatorChar);
			ConfigurationFilesPath = Path.GetDirectoryName(path);

			_config.OnModified += new EventHandler((o, e) => 
				{ if (OnModified != null) OnModified(o, e); });
		}

		public static EventHandler OnModified;

		public static readonly string ConfigurationFilesPath;

		public static class Debug
		{
			public static readonly BooleanConfigurationParam AllowExtendedLogging =
				new BooleanConfigurationParam(_config, "DebugAllowExtendedLogging", false);

			public static readonly BooleanConfigurationParam AllowReallyExtendedLogging =
				new BooleanConfigurationParam(_config, "DebugAllowReallyExtendedLogging", false);

			public static readonly IntegerConfigurationParam RandomObjectUpdateTimeSec =
				new IntegerConfigurationParam(_config, "DebugRandomObjectUpdateTimeSec", 0);

			public static readonly BooleanConfigurationParam ConvertRelativeTemplateUrls =
				new BooleanConfigurationParam(_config, "DebugConvertRelativeTemplateUrls", true);

			public static readonly BooleanConfigurationParam ConvertRelativeUrlsFFCSpeedTest =
				new BooleanConfigurationParam(_config, "DebugConvertRelativeUrlsFFCSpeedTest", false);

			public static readonly StringConfigurationParam DebugClientIpRangeInclude =
				new StringConfigurationParam(_config, "DebugClientIpRangeInclude", string.Empty);

			public static readonly StringConfigurationParam DebugClientIpRangeExclude =
				new StringConfigurationParam(_config, "DebugClientIpRangeExclude", string.Empty);
		}

		public static class Database
		{
			public static readonly StringConfigurationParam ConnectionString =
				new StringConfigurationParam(_config, "connectionString", string.Empty);

			public static readonly StringConfigurationParam TransactionIsolationLevel =
				new StringConfigurationParam(_config, "DbTransactionIsolationLevel", "serializable");
		}

		public static class Messaging
		{
			public static readonly IntegerConfigurationParam ViewOrderExpiryWarningDays =
				new IntegerConfigurationParam(_config, "ViewOrderExpiryWarningDays", 3, 1, 30);


			public static readonly StringConfigurationParam MessageTemplateRoot =
				new StringConfigurationParam(_config, "MessageTemplateRoot", ".");

			public static readonly StringConfigurationParam AdminMessageRecipients =
				new StringConfigurationParam(_config, "AdminMessageRecipients", "admin@3dcondox.com");

			public static readonly StringConfigurationParam SalesMessageRecipients =
				new StringConfigurationParam(_config, "SalesMessageRecipients", "sales@3dcondox.com");

			public static class Email
			{
				public static readonly StringConfigurationParam SmtpServerHost =
					new StringConfigurationParam(_config, "SmtpServerHost", string.Empty);

				public static readonly IntegerConfigurationParam SmtpServerPort =
					new IntegerConfigurationParam(_config, "SmtpServerPort", 25, 1, 65535);

				public static readonly BooleanConfigurationParam SmtpServerUseSsl =
					new BooleanConfigurationParam(_config, "SmtpServerUseSsl", false);

				public static class Credentials
				{
					public static class Server
					{
						public static readonly StringConfigurationParam Login =
							new StringConfigurationParam(_config, "SmtpServerLogin", string.Empty);
						public static readonly StringConfigurationParam Password =
							new StringConfigurationParam(_config, "SmtpServerPassword", string.Empty);
					}
					public static class ECommerce
					{
						public static readonly StringConfigurationParam Login =
							new StringConfigurationParam(_config, "SmtpECommerceLogin", string.Empty);
						public static readonly StringConfigurationParam Password =
							new StringConfigurationParam(_config, "SmtpECommercePassword", string.Empty);
					}
				}
			}
		}

		public static class PublicFileStore
		{
			public static readonly StringConfigurationParam AccessRoot =
				new StringConfigurationParam(_config, "FileStoreAccessRoot", string.Empty);

			public static readonly StringConfigurationParam RootPath =
				new StringConfigurationParam(_config, "FileStoreRoot", string.Empty);
		}

		public static class InternalFileStore
		{
			public static readonly StringConfigurationParam RootPath =
				new StringConfigurationParam(_config, "InternalFileStoreRoot", string.Empty);
		}

		public static class PaymentSystem
		{
			public static readonly BooleanConfigurationParam SandboxMode = 
				new BooleanConfigurationParam(_config, "PaymentSystemSandboxMode", false);

			public static readonly IntegerConfigurationParam UnpaidViewOrderLifespanMinutes =
				new IntegerConfigurationParam(_config, "UnpaidViewOrderLifespanMinutes", 15, 10, 1440);

			public static class PayPal
			{
				public static readonly StringConfigurationParam MerchantId =
					new StringConfigurationParam(_config, "PayPalMerchantId", "ecommerce@3dcondox.com"); 

				public static readonly StringConfigurationParam BusinessId =
					new StringConfigurationParam(_config, "PayPalBusinessId", MerchantId.Value);

				public static readonly StringConfigurationParam AccountId =
					new StringConfigurationParam(_config, "PayPalAccountId", "???");
			}
		}

		public static class FloodPrevention
		{
			public static readonly IntegerConfigurationParam MinAccessPeriodSec =
				new IntegerConfigurationParam(_config, "MinAccessPeriodSec", 30, 1, 450);

			public static readonly IntegerConfigurationParam AccessCleanupTimeoutSec =
				new IntegerConfigurationParam(_config, "AccessCleanupTimeoutSec", 600, 2, 900);

			public static readonly IntegerConfigurationParam HttpRequestHoldOffMs =
				new IntegerConfigurationParam(_config, "FloodingHttpRequestHoldOffMs", 1000, 0, 3600000);
		}

		public static class HttpService
		{
			public static readonly StringConfigurationParam ListenerUri =
				new StringConfigurationParam(_config, "HttpListenerUri", string.Empty);

			public static readonly StringConfigurationParam ListeningHostAliasList =
				new StringConfigurationParam(_config, "ListeningHostAliasList",
				"localhost:8026,168.144.195.160,ref.3dcondox.com,vrt.3dcondox.com,order.3dcondox.com,static.3dcondox.com,models.3dcondox.com");

			public static readonly StringConfigurationParam ReferringHostAliasList =
				new StringConfigurationParam(_config, "ReferringHostAliasList",
				"vrt.3dcondox.com,order.3dcondox.com,static.3dcondox.com,models.3dcondox.com");

			public static class FileService
			{
				public static readonly StringConfigurationParam RootPath =
					new StringConfigurationParam(_config, "FilesRoot", ConfigurationFilesPath);

				public static readonly StringConfigurationParam AllowedExtensions =
					new StringConfigurationParam(_config, "AllowedServedFileExtensions", string.Empty);

				public static readonly IntegerConfigurationParam StreamingBufferSize =
					new IntegerConfigurationParam(_config, "FileStreamingBufferSize", 16384, 1024, int.MaxValue);
			}

			public static readonly StringConfigurationParam ViewClientPath =
				new StringConfigurationParam(_config, "ViewClientPath", "VREstate.html");

			public static readonly IntegerConfigurationParam MaxRequestBodyLengthBytes =
				new IntegerConfigurationParam(_config, "MaxHttpRequestBodyLengthBytes", 10240, 0, int.MaxValue);

			public static readonly IntegerConfigurationParam ReverseRequestLinkExpirationSec =
				new IntegerConfigurationParam(_config, "ReverseRequestLinkExpirationSec", 3600, 0, 86400);
		}

		public static class Security
		{
			public static readonly BooleanConfigurationParam AllowSensitiveDataOverNonSecureConnection =
				new BooleanConfigurationParam(_config, "AllowSensitiveDataOverNonSecureConnection", false);

			public static readonly StringConfigurationParam PasswordHashType =
				new StringConfigurationParam(_config, "HashType", "SHA512");

			public static readonly IntegerConfigurationParam PasswordSaltSizeBytes =
				new IntegerConfigurationParam(_config, "SaltSizeBytes", 64, 0, 1024);
		}

		public static class ClientSession
		{
			public static readonly IntegerConfigurationParam TimeoutSec =
				new IntegerConfigurationParam(_config, "ClientSessionTimeoutSec", 600, 60, 3600);
		}

		public static class FileCache
		{
			public static readonly IntegerConfigurationParam SpaceLimitMb =
				new IntegerConfigurationParam(_config, "CacheSpaceLimitMb", 10, 0, 10240);

			public static readonly StringConfigurationParam RootPath =
				new StringConfigurationParam(_config, "CacheRoot", ".");
		}

		public static class GeneratedImageCache
		{
			public static readonly IntegerConfigurationParam MinCachedImagekPx =
				new IntegerConfigurationParam(_config, "MinCachedImagekPx", 10, 0, 10240);

			public static readonly IntegerConfigurationParam MaxGeneratedImagekPx =
				new IntegerConfigurationParam(_config, "MaxGeneratedImagekPx", 1000, 0, 10240);

			public static readonly StringConfigurationParam RootPath =
				new StringConfigurationParam(_config, "CacheRoot", ".");
		}

		public static class ModelCache
		{
			public static readonly BooleanConfigurationParam Enabled =
				new BooleanConfigurationParam(_config, "UseModelCache", true);

			public static readonly StringConfigurationParam RootPath =
				new StringConfigurationParam(_config, "ModelFileStore", string.Empty);
		}

		public static class Service
		{
			public static readonly StringConfigurationParam ServerRole =
				new StringConfigurationParam(_config, "ServerRole", "VRT");
		}

		public static class Urls
		{
			public static readonly StringConfigurationParam DisabledViewOrderRecover =
				new StringConfigurationParam(_config, "DisabledViewOrderRecoverUrl", "http://3dcondox.com/order?recoverId={0}");

			public static readonly StringConfigurationParam StaticLinkTemplate =
				new StringConfigurationParam(_config, "StaticLinkUrlTemplate", "http://ref.3dcondox.com/go?id={0}");
		}

		public static class Redirector
		{
			public static readonly StringConfigurationParam ButtonStorePath =
				new StringConfigurationParam(_config, "RedirectorButtonsStore", string.Empty);

			public static class Urls
			{
				public static readonly StringConfigurationParam DefaultUrl =
					new StringConfigurationParam(_config, "RedirectorDefaultUri", "http://3dcondox.com");

				public static readonly StringConfigurationParam ProductionClientViewOrderTemplate =
					new StringConfigurationParam(_config, "RedirectionClientViewOrderTemplate", "https://vrt.3dcondox.com/VREstate.html?viewOrderId={0}");

				public static readonly StringConfigurationParam TestingClientViewOrderTemplate =
					new StringConfigurationParam(_config, "RedirectionTestClientViewOrderTemplate", "https://vrt.3dcondox.com/vre/VREstate.html?viewOrderId={0}");

				public static readonly StringConfigurationParam ProductionReverseRequestTemplate =
					new StringConfigurationParam(_config, "RedirectionRevReqTemplate", "https://vrt.3dcondox.com/go/{0}");

				public static readonly StringConfigurationParam TestingReverseRequestTemplate =
					new StringConfigurationParam(_config, "RedirectionTestRevReqTemplate", "https://vrt.3dcondox.com/vre/go/{0}");
			}

			public static class Aliases
			{
				public static readonly StringConfigurationParam ProductionMapFile =
					new StringConfigurationParam(_config, "RedirectionAliasMapFile", "aliases.config");

				public static readonly StringConfigurationParam TestingMapFile =
					new StringConfigurationParam(_config, "RedirectionTestAliasMapFile", "aliases.test.config");
			}
		}

		public static class Mls
		{
			public static readonly StringConfigurationParam ImportUserNickNameOwner =
				new StringConfigurationParam(_config, "MlsImportUserNickNameOwner", "mlsImport");

			public static class Treb
			{
				public static class Status
				{
					public static readonly StringConfigurationParam ConfigString =
						new StringConfigurationParam(_config, "MLS-STATUS-TREB-Config", string.Empty);
				}
				public static class Info
				{
					public static readonly StringConfigurationParam ConfigString =
						new StringConfigurationParam(_config, "MLS-INFO-TREB-Config", string.Empty);
				}
			}
		}

		public static class User
		{
			public static readonly IntegerConfigurationParam UserPhotoWidthPx =
				new IntegerConfigurationParam(_config, "UserPhotoWidthPx", 128);

			public static readonly IntegerConfigurationParam UserPhotoHeightPx =
				new IntegerConfigurationParam(_config, "UserPhotoHeightPx", 128);
		}
	}
}