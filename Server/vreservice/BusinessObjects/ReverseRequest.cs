﻿using System;

namespace Vre.Server.BusinessLogic
{
    internal class ReverseRequest
    {
        public enum RequestType : int
        {
            /// <summary>
            /// Primary account registration procedure; access results in UI with password setting.
            /// If succeeds, account and authentication records are generated.
            /// <para>EXPIRABLE</para>
            /// </summary>
            /// <remarks>
            /// <para>The login is in <see cref="ReverseRequest.Login"/>; there should be no more than one record of this type per login.</para>
            /// <para>The account information is stored as JSON in <see cref="ReverseRequest.Subject"/>.</para>
            /// <para>The password setting form should be generated (and should return) 
            /// <see cref="ReverseRequest.ReferenceParamName"/> = ''; "rid" = <see cref="ReverseRequest.ReferenceParamValue"/>
            /// as a verification of proper form submission.</para>
            /// </remarks>
            AccountRegistration = 0,
            /// <summary>
            /// Change of login for accounts with email as login; access results in confirmation UI (static).
            /// If accessed, account's email address and authentication record are updated.
            /// <para>EXPIRABLE</para>
            /// </summary>
            /// <remarks>
            /// <para>The requester is in <see cref="ReverseRequest.UserId"/>; there should be no more than one record of this type per user.</para>
            /// <para>The new login is in <see cref="ReverseRequest.Subject"/>.</para>
            /// </remarks>
            LoginChange = 1,
            /// <summary>
            /// Brokerage registration; access results in confirmation UI (static).
            /// If accessed, brokerage record is created.
            /// <para>EXPIRABLE</para>
            /// </summary>
            /// <remarks>
            /// <para>The requester is in <see cref="ReverseRequest.UserId"/> or null if no user was involved (guest).</para>
            /// <para>The Brokerage information is stored as JSON in <see cref="ReverseRequest.Subject"/>.</para>
            /// </remarks>
            BrokerageRegistration = 2,
            /// <summary>
            /// Password reset; access results in UI with password setting.
            /// If succeeds, authentication record is created.
            /// <para>EXPIRABLE</para>
            /// </summary>
            /// <remarks>
            /// <para>The requester is in <see cref="ReverseRequest.UserId"/>; there should be no more than one record of this type per user.</para>
            /// <para>The password recovery form should be generated (and should return) 
            /// <see cref="ReverseRequest.ReferenceParamName"/> = ''; "rid" = <see cref="ReverseRequest.ReferenceParamValue"/>
            /// as a verification of proper form submission.</para>
            /// </remarks>
            PasswordRecover = 3,
            /// <summary>
            /// MLS Registered listing; access results in forwarding to specific building or suite.
            /// <para>EXPIRABLE</para>
            /// </summary>
            /// <remarks>
            /// <para>The link to redirect to is in <see cref="ReverseRequest.Subject"/></para>
            /// <para>The listing owner is in <see cref="ReverseRequest.UserId"/>; this can be used to list user's listings.</para>
            /// </remarks>
            //Listing = 4,
            /// <summary>
            /// Cancelling Building Availability Notification; access resuls in confirmation UI (static).
            /// If accessed, related notification is removed from system.
            /// <para>PERMANENT until used</para>
            /// </summary>
            /// <remarks>
            /// <para>TODO: ???</para>
            /// <para>The notification owner is in <see cref="ReverseRequest.UserId"/>; this can be used to list user's view orders.</para>
            /// </remarks>
            NotificationCancel = 5,
            /// <summary>
            /// Control View Order
            /// <para>PERMANENT while target exists</para>
            /// </summary>
            /// <remarks>
            /// <para>ViewOrder ID is in <see cref="ReverseRequest.Subject"/></para>
            /// </remarks>
            ViewOrderControl = 6,
			/// <summary>
			/// Activate ViewOrder via Payment System Notification
			/// <para>EXPIRABLE</para>
			/// </summary>
			/// <remarks>
			/// <para>The request # is used as a unique product ID or product number in Payment System</para>
			/// <para>ViewOrder ID is in <see cref="ReverseRequest.Subject"/></para>
			/// <para>ViewOrder contains temporary expiration time (very short; for preview only); 
			/// final bought ViewOrder expiration time is stored in <see cref="ReverseRequest.ReferenceParamName"/>
			/// in ticks (yeah, not best place).</para>
			/// <para>The requested net amount to be paid is stored in <see cref="ReverseRequest.ReferenceParamValue"/></para>
			/// </remarks>
			ViewOrderActivation = 7,
        }

        public virtual Guid Id { get; private set; }
        public virtual RequestType Request { get; private set; }
        public virtual DateTime ExpiresOn { get; private set; }
        public virtual int RequestCounter { get; private set; }
        public virtual DateTime LastRequestTime { get; private set; }

        public virtual int? UserId { get; private set; }
        public virtual string Login { get; private set; }
        public virtual string Subject { get; private set; }

        public virtual string ReferenceParamName { get; set; }
        public virtual string ReferenceParamValue { get; set; }

        public bool IsAttemptSensitive { get { return ((Request == RequestType.LoginChange) || (Request == RequestType.PasswordRecover)); } }

        private static readonly DateTime _minimalTime = new DateTime(2000, 1, 1);

        private ReverseRequest() { }

		public void ProlongUntil(DateTime newLimit)
		{
			ExpiresOn = newLimit;
		}

		public void ProlongBy(TimeSpan time)
		{
			ExpiresOn = ExpiresOn.Add(time);
		}

		public void Touch()
		{
			RequestCounter++;
			LastRequestTime = DateTime.UtcNow;
		}

        public static ReverseRequest CreateRegisterAccount(string login, string accountInformation, DateTime expiresOn,
            string refParamName, string refParamValue)
        {
            ReverseRequest result = new ReverseRequest();
            result.Id = Guid.NewGuid();
            result.Request = RequestType.AccountRegistration;
            result.ExpiresOn = expiresOn;
            result.RequestCounter = 0;
            result.LastRequestTime = _minimalTime;
            result.UserId = null;
            result.Login = login;
            result.Subject = accountInformation;
            result.ReferenceParamName = refParamName;
            result.ReferenceParamValue = refParamValue;
            return result;
        }

        public static ReverseRequest CreateLoginChange(int userId, string newLogin, DateTime expiresOn,
            string refParamName, string refParamValue)
        {
            ReverseRequest result = new ReverseRequest();
            result.Id = Guid.NewGuid();
            result.Request = RequestType.LoginChange;
            result.ExpiresOn = expiresOn;
            result.RequestCounter = 0;
            result.LastRequestTime = _minimalTime;
            result.UserId = userId;
            result.Login = null;
            result.Subject = newLogin;
            result.ReferenceParamName = refParamName;
            result.ReferenceParamValue = refParamValue;
            return result;
        }

        public static ReverseRequest CreateRegisterBrokerage(int? userId, string brokerageInformation, DateTime expiresOn)
        {
            ReverseRequest result = new ReverseRequest();
            result.Id = Guid.NewGuid();
            result.Request = RequestType.BrokerageRegistration;
            result.ExpiresOn = expiresOn;
            result.RequestCounter = 0;
            result.LastRequestTime = _minimalTime;
            result.UserId = userId;
            result.Login = null;
            result.Subject = brokerageInformation;
            result.ReferenceParamName = null;
            result.ReferenceParamValue = null;
            return result;
        }

        public static ReverseRequest CreatePasswordRecover(int userId, string login, DateTime expiresOn,
            string refParamName, string refParamValue)
        {
            ReverseRequest result = new ReverseRequest();
            result.Id = Guid.NewGuid();
            result.Request = RequestType.PasswordRecover;
            result.ExpiresOn = expiresOn;
            result.RequestCounter = 0;
            result.LastRequestTime = _minimalTime;
            result.UserId = userId;
            result.Login = login;
            result.Subject = null;
            result.ReferenceParamName = refParamName;
            result.ReferenceParamValue = refParamValue;
            return result;
        }

        public static ReverseRequest CreateViewOrderControl(int userId, ViewOrder vo, DateTime expiresOn,
            string refParamName, string refParamValue)
        {
            ReverseRequest result = new ReverseRequest();
            result.Id = Guid.NewGuid();
            result.Request = RequestType.ViewOrderControl;
            result.ExpiresOn = expiresOn;
            result.RequestCounter = 0;
            result.LastRequestTime = _minimalTime;
            result.UserId = userId;
            result.Login = null;
            result.Subject = vo.AutoID.ToString();
            result.ReferenceParamName = refParamName;
            result.ReferenceParamValue = refParamValue;
            return result;
        }

        //public static ReverseRequest CreateListing(int userId, DateTime expiresOn)
        //{
        //    ReverseRequest result = new ReverseRequest();
        //    result.Id = Guid.NewGuid();
        //    result.Request = RequestType.Listing;
        //    result.ExpiresOn = expiresOn;
        //    result.RequestCounter = 0;
        //    result.LastRequestTime = _minimalTime;
        //    result.UserId = userId;
        //    result.Login = null;
        //    result.Subject = null;
        //    result.ReferenceParamName = null;
        //    result.ReferenceParamValue = null;
        //    return result;
        //}

		public static ReverseRequest CreateViewOrderActivation(
			ViewOrder vo,
			Money amountToPay, DateTime expirationTime)
		{
			var result = new ReverseRequest();
			result.Id = Guid.NewGuid();
			result.Request = RequestType.ViewOrderActivation;
			// safety gap; allow paying and processing a ViewOrder within 30 minutes
			// after trial time expires.
			result.ExpiresOn = vo.ExpiresOn.AddMinutes(30.0);
			result.RequestCounter = 0;
			result.LastRequestTime = _minimalTime;
			result.UserId = vo.OwnerId;
			result.Login = null;
			result.Subject = vo.AutoID.ToString();
			result.ReferenceParamName = expirationTime.Ticks.ToString();
			result.ReferenceParamValue = amountToPay.ToFullString();
			return result;
		}
    }
}