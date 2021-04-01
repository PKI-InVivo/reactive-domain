﻿using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.Users.Messages
{
    /// <summary>
    /// Messages for the Role domain.
    /// </summary>
    public class RoleMsgs
    {
        /// <summary>
        /// Create a new Role.
        /// </summary>
        public class CreateRole : Command
        {
            /// <summary>The unique ID of the new role.</summary>
            public readonly Guid RoleId;
            /// <summary>The name of the role.</summary>
            public readonly string Name;
            /// <summary>The policy this role applies to.</summary>
            public readonly Guid PolicyId;


            /// <summary>
            /// Create a new Role.
            /// </summary>
            public CreateRole(
                Guid roleId,
                string name,
                Guid policyId)
            {
                RoleId = roleId;
                Name = name;
                PolicyId = policyId;
            }

        }

        /// <summary>
        /// A new role was created.
        /// </summary>
        public class RoleCreated : Event
        {
            /// <summary>The unique ID of the new role.</summary>
            public readonly Guid RoleId;
            /// <summary>The name of the role.</summary>
            public readonly string Name;
            /// <summary>The policy this role applies to.</summary>
            public readonly Guid PolicyId;

            /// <summary>
            /// A new role was created.
            /// </summary>
            public RoleCreated(
                Guid roleId,
                string name,
                Guid policyId)
            {
                RoleId = roleId;
                Name = name;
                PolicyId = policyId;
            }

        }

        //todo: fix migration to match the new model
        /// <summary>
        /// Role data was migrated.
        /// </summary>
        public class RoleMigrated : Event
        {
            /// <summary>The unique ID of the new role.</summary>
            public readonly Guid RoleId;
            /// <summary>The name of the role.</summary>
            public readonly string Name;
            /// <summary>The application this role applies to.</summary>
            public readonly Guid PolicyId;
            /// <summary>The source stream.</summary>
            public readonly string Source;
            /// <summary> The number of Events migrated.</summary>
            public readonly int EventCount;

            /// <summary>
            /// Role data was migrated.
            /// </summary>
            public RoleMigrated( 
                Guid roleId,
                string name,
                Guid policyId,
                string source, 
                int eventCount)
            {
                RoleId = roleId;
                Name = name;
                PolicyId = policyId;
                Source = source;
                EventCount = eventCount;
            }

        }
      
        /// <summary>
        /// Role data migrated.
        /// </summary>
        public class RoleDataMigrated : Event
        {
            /// <summary>The unique ID of the role.</summary>
            public readonly Guid RoleId;
            /// <summary>The stream data migrated to.</summary>
            public readonly string TargetStream;

            /// <summary>
            /// Role data migrated.
            /// </summary>
            public RoleDataMigrated(Guid roleId, string targetStream)
            {
                RoleId = roleId;
                TargetStream = targetStream;
            }
        }
    }
}