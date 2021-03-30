﻿using ReactiveDomain.Messaging;
using System;

namespace ReactiveDomain.Users.Messages
{
    public class UserAuthMsgs
    {
        /// <summary>
        /// A new user login tracker was created.
        /// </summary>
        public class UserAuthHistoryCreated : Event
        {
            /// <summary>The unique Id of the tracked user.</summary>
            public readonly Guid UserId;
            /// <summary>The unique ID from the auth provider (e.g. Sub Claim) of the authenticated user.</summary>
            public readonly string SubjectId;
            /// <summary>The identity provider.</summary>
            public readonly string AuthProvider;
            /// <summary>The user's domain.</summary>
            public readonly string AuthDomain;
            /// <summary>
            /// A new user was created.
            /// </summary>
            /// <param name="id">The unique ID of the new user.</param>
            ///<param name="subjectId">The unique ID from the auth provider (e.g. Sub Claim) of the authenticated user.</param>
            /// <param name="authProvider">The identity provider.</param>
            /// <param name="authDomain">The user's domain.</param>
            public UserAuthHistoryCreated(
                Guid userId,
                string subjectId,
                string authProvider,
                string authDomain)
            {
                UserId = userId;
                SubjectId = subjectId;
                AuthProvider = authProvider;
                AuthDomain = authDomain;
            }
        }
        /// <summary>
        /// A user was successfully authenticated.
        /// </summary>
        public class Authenticated : Event
        {
            /// <summary>The ID of the authenticated user.</summary>
            public readonly Guid UserId;
            /// <summary>The date and time in UTC on the Elbe Server when the authentication was logged.</summary>
            public readonly DateTime TimeStamp;
            /// <summary>The IP address of the host asking for authentication.</summary>
            public readonly string HostIPAddress;

            /// <summary>
            /// A user was successfully authenticated.
            /// </summary>
            /// <param name="id">The ID of the authenticated user.</param>
            /// <param name="timeStamp">The date and time in UTC on the Elbe Server when the authentication was logged.</param>
            /// <param name="hostIPAddress">The IP address of the host asking for authentication.</param>
            public Authenticated(
                Guid userId,
                DateTime timeStamp,
                string hostIPAddress)
            {
                UserId = userId;
                TimeStamp = timeStamp;
                HostIPAddress = hostIPAddress;
            }
        }
        /// <summary>
        /// A user was not successfully authenticated.
        /// </summary>
        public class AuthenticationFailed : Event
        {
            /// <summary>The ID of the not authenticated user.</summary>
            public readonly Guid UserId;
            /// <summary>The date and time in UTC on the Elbe Server when the authentication was logged.</summary>
            public readonly DateTime TimeStamp;
            /// <summary>The IP address of the host asking for authentication.</summary>
            public readonly string HostIPAddress;

            /// <summary>
            /// A user was successfully authenticated.
            /// </summary>
            /// <param name="id">The ID of the not authenticated user.</param>
            /// <param name="timeStamp">The date and time in UTC on the Elbe Server when the authentication attempt was logged.</param>
            /// <param name="hostIPAddress">The IP address of the host asking for authentication.</param>
            public AuthenticationFailed(
                Guid userId,
                DateTime timeStamp,
                string hostIPAddress)
            {
                UserId = userId;
                TimeStamp = timeStamp;
                HostIPAddress = hostIPAddress;
            }
        }
        /// <summary>
        /// A user was not successfully authenticated because account is locked.
        /// </summary>
        public class AuthenticationFailedAccountLocked : Event
        {
            /// <summary>The ID of the not authenticated user.</summary>
            public readonly Guid UserId;
            /// <summary>The date and time in UTC on the Elbe Server when the authentication was logged.</summary>
            public readonly DateTime TimeStamp;
            /// <summary>The IP address of the host asking for authentication.</summary>
            public readonly string HostIPAddress;

            /// <summary>
            /// A user was not successfully authenticated because account is locked.
            /// </summary>
            /// <param name="id">The ID of the not authenticated user.</param>
            /// <param name="timeStamp">The date and time in UTC on the Elbe Server when the authentication attempt was logged.</param>
            /// <param name="hostIPAddress">The IP address of the host asking for authentication.</param>
            public AuthenticationFailedAccountLocked(
                Guid userId,
                DateTime timeStamp,
                string hostIPAddress)
            {
                UserId = userId;
                TimeStamp = timeStamp;
                HostIPAddress = hostIPAddress;
            }
        }

        /// <summary>
        /// A user was not successfully authenticated because account is disabled.
        /// </summary>
        public class AuthenticationFailedAccountDisabled : Event
        {
            /// <summary>The ID of the not authenticated user.</summary>
            public readonly Guid UserId;
            /// <summary>The date and time in UTC on the Elbe Server when the authentication was logged.</summary>
            public readonly DateTime TimeStamp;
            /// <summary>The IP address of the host asking for authentication.</summary>
            public readonly string HostIPAddress;

            /// <summary>
            /// A user was not successfully authenticated because account is disabled.
            /// </summary>
            /// <param name="id">The ID of the not authenticated user.</param>
            /// <param name="timeStamp">The date and time in UTC on the Elbe Server when the authentication attempt was logged.</param>
            /// <param name="hostIPAddress">The IP address of the host asking for authentication.</param>
            public AuthenticationFailedAccountDisabled(
                Guid userId,
                DateTime timeStamp,
                string hostIPAddress)
            {
                UserId = userId;
                TimeStamp = timeStamp;
                HostIPAddress = hostIPAddress;
            }
        }

        /// <summary>
        /// A user was not successfully authenticated because invalid credentials were supplied.
        /// </summary>
        public class AuthenticationFailedInvalidCredentials : Event
        {
            /// <summary>The ID of the not authenticated user.</summary>
            public readonly Guid UserId;
            /// <summary>The date and time in UTC on the Elbe Server when the authentication was logged.</summary>
            public readonly DateTime TimeStamp;
            /// <summary>The IP address of the host asking for authentication.</summary>
            public readonly string HostIPAddress;

            /// <summary>
            /// A user was not successfully authenticated because invalid credentials were supplied.
            /// </summary>
            /// <param name="id">The ID of the not authenticated user.</param>
            /// <param name="timeStamp">The date and time in UTC on the Elbe Server when the authentication attempt was logged.</param>
            /// <param name="hostIPAddress">The IP address of the host asking for authentication.</param>
            public AuthenticationFailedInvalidCredentials(
                Guid userId,
                DateTime timeStamp,
                string hostIPAddress)
            {
                UserId = userId;
                TimeStamp = timeStamp;
                HostIPAddress = hostIPAddress;
            }
        }

    }
}