﻿using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Policy.Messages;
using ReactiveDomain.Policy.ReadModels;

namespace ReactiveDomain.Policy.Application
{
    // for use in applications enforcing security policy
    /// <summary>
    /// A read model that contains a synchronized security policy for the application. 
    /// </summary>
    public class SecurityPolicySyncService :
        ReadModelBase,
        IHandle<ApplicationMsgs.ApplicationCreated>,
        IHandle<ApplicationMsgs.PolicyCreated>,
        IHandle<RoleMsgs.RoleCreated>//,
                                     //IHandle<UserMsgs.UserCreated>,
                                     //IHandle<UserMsgs.Deactivated>,
                                     //IHandle<UserMsgs.Activated>,
                                     //IHandle<UserMsgs.RoleAssigned>,
                                     //IHandle<UserMsgs.AuthDomainUpdated>,
                                     //IHandle<UserMsgs.UserNameUpdated>,
                                     //IHandle<UserMsgs.RoleUnassigned>
    {
        //public data
        public SecurityPolicy Policy;

        private readonly Dictionary<Guid, Role> _roles = new Dictionary<Guid, Role>();
        private readonly Dictionary<Guid, SecuredApplication> _applications = new Dictionary<Guid, SecuredApplication>();
        //private SubjectRM _subjectRM;
        private ICorrelatedRepository _repo;
        private ICorrelatedMessage _source;

        /// <summary>
        /// Create a read model for a synchronized Security Policy.
        /// </summary>
        public SecurityPolicySyncService(
            SecurityPolicy basePolicy,
            IConfiguredConnection conn)
            : base(nameof(ApplicationsRM), () => conn.GetListener(nameof(SecurityPolicySyncService)))
        {
            var dispatcher = new Dispatcher(nameof(SecurityPolicySyncService));
            _repo = conn.GetCorrelatedRepository();
            _source = new SyncServiceCorrelationSource { CorrelationId = Guid.NewGuid() };

            Policy = basePolicy ?? new SecurityPolicyBuilder().Build();

            using (var appReader = conn.GetReader("AppReader"))
            {
                appReader.EventStream.Subscribe<ApplicationMsgs.ApplicationCreated>(this);
                appReader.Read(typeof(ApplicationMsgs.ApplicationCreated));
            }
            Guid appId;
            Guid correlationId = Guid.NewGuid();
            var dbApp = _applications.Values.FirstOrDefault(
                app => string.CompareOrdinal(app.Name, Policy.ApplicationName) == 0 &&
                                string.CompareOrdinal(app.Version, Policy.ApplicationVersion) == 0);
            if (dbApp == null)
            {
                appId = Guid.NewGuid();
                var app = new Domain.SecuredApplication(appId, Policy.ApplicationName, Policy.ApplicationVersion, _source);
                _repo.Save(app);
            }
            else
            {
                appId = dbApp.Id;
            }




            using (var appReader = conn.GetReader("RoleReader"))
            {
                appReader.EventStream.Subscribe<ApplicationMsgs.ApplicationCreated>(this);
                appReader.EventStream.Subscribe<ApplicationMsgs.PolicyCreated>(this);
                appReader.EventStream.Subscribe<RoleMsgs.RoleCreated>(this);

                appReader.Read<Domain.SecuredApplication>(appId);
            }
            if (dbApp == null)
            {
                dbApp = _applications[appId];
            }
            Policy.OwningApplication.UpdateApplicationDetails(appId);

            Policy.PolicyId = dbApp.Policies.First().PolicyId; //todo:add multi policy support

            //enrich db with roles from the base policy, if any are missing
            foreach (var role in Policy.Roles)
            {
                if (_roles.Values.All(r => !r.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    var roleId = Guid.NewGuid();
                    var application = _repo.GetById<Domain.SecuredApplication>(appId, _source);
                    application.DefaultPolicy.AddRole(roleId, role.Name);
                    _repo.Save(application);
                    role.SetRoleId(roleId);
                    _roles.Add(roleId, new Role(roleId, role.Name, role.PolicyId));
                }
            }
            //sync all roles on the Policy with Ids from the DB
            //and add any missing roles from the db
            foreach (var role in _roles.Values)
            {
                var baseRole = Policy.Roles.FirstOrDefault(r => r.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase));
                if (baseRole == null)
                {
                    Policy.AddRole(role);
                }
                else
                {
                    baseRole.SetRoleId(role.RoleId);
                }
            }

            //_subjectRM = new SubjectRM(() => conn.GetQueuedListener(nameof(SubjectRM)));

            //todo: subscribe to user assignments

            //todo: subscribe to user stream???
            //Start<ApplicationRoot>(appId, blockUntilLive: true);
        }

        public void Handle(ApplicationMsgs.ApplicationCreated @event)
        {
            if (_applications.ContainsKey(@event.ApplicationId)) return;

            _applications.Add(
                @event.ApplicationId,
                new SecuredApplication(
                    @event.ApplicationId,
                    @event.Name,
                    @event.ApplicationVersion
                ));
        }

        public void Handle(ApplicationMsgs.PolicyCreated @event)
        {
            if (!_applications.ContainsKey(@event.ApplicationId)) return;
            var app = _applications[@event.ApplicationId];
            app.AddPolicy(
                new SecurityPolicy(
                    @event.ClientId,
                    @event.PolicyId, app,
                    principal => default
                )
            );
        }

        /// <summary>
        /// Given the role created event, adds a new role to the collection of roles.
        /// </summary>
        public void Handle(RoleMsgs.RoleCreated @event)
        {
            if (_roles.ContainsKey(@event.RoleId)) return;
            _roles.Add(
                @event.RoleId,
                new Role(
                    @event.RoleId,
                    @event.Name,
                    Policy.PolicyId));
        }


        /*
        /// <summary>
        /// Handle a UserMsgs.UserCreated event.
        /// </summary>
        public void Handle(UserMsgs.UserCreated @event) {
            if (_users.ContainsKey(@event.Id)) return;
            _users.Add(
                @event.Id,
                new UserModel(
                    @event.Id,
                    @event.UserName,
                    @event.SubjectId,
                    @event.AuthDomain));
        }
        public void Handle(UserMsgs.Deactivated @event)
        {
            if (!_users.ContainsKey(@event.UserId)) return;
            _users[@event.UserId].IsActivated = false;
        }

        public void Handle(UserMsgs.Activated @event)
        {
            if (!_users.ContainsKey(@event.UserId)) return;
            _users[@event.UserId].IsActivated = true;
        }

        public void Handle(UserMsgs.RoleAssigned @event)
        {
            if (!_users.ContainsKey(@event.UserId) || !_roles.ContainsKey(@event.RoleId)) return;
            var role = _users[@event.UserId].Roles.FirstOrDefault(r => r.RoleId == @event.RoleId);
            if( role != null) return;
            _users[@event.UserId].Roles.Add(_roles[@event.RoleId]);
        }

        public void Handle(UserMsgs.RoleUnassigned @event)
        {
            if (!_users.ContainsKey(@event.UserId)|| !_roles.ContainsKey(@event.RoleId)) return;
            var role = _users[@event.UserId].Roles.FirstOrDefault(r => r.RoleId == @event.RoleId);
            if( role == null) return;
            _users[@event.UserId].Roles.Remove(role);
        }
        public void Handle(UserMsgs.AuthDomainUpdated @event)
        {
            if (!_users.ContainsKey(@event.UserId)) return;
            _users[@event.UserId].AuthDomain = @event.AuthDomain;
        }
        public void Handle(UserMsgs.UserNameUpdated @event)
        {
            if (!_users.ContainsKey(@event.UserId)) return;
            _users[@event.UserId].UserName = @event.UserName;
        }
        public List<Role> RolesForUser(
            string subjectId,
            string userName,
            string authDomain,
            Application application)
        {
            var user = _users.Values.FirstOrDefault(x =>
                x.AuthDomain.Equals(authDomain, StringComparison.CurrentCultureIgnoreCase)
                && (string.IsNullOrEmpty(subjectId) ||
                    string.IsNullOrEmpty(x.SubjectId)
                    ? x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                    : x.SubjectId.Equals(subjectId, StringComparison.CurrentCultureIgnoreCase))

            );
            if (user == null)
            {
                throw new UserNotFoundException("UserId = " + subjectId + ", authDomain = " + authDomain);
            }
            if (!user.IsActivated)
            {
                throw new UserDeactivatedException("UserId = " + subjectId + ", authDomain = " + authDomain);
            }
            return user.Roles.FindAll(x => x.Application == application);
        }
        */
        class SyncServiceCorrelationSource : ICorrelatedMessage
        {
            public Guid MsgId => Guid.NewGuid();
            public Guid CorrelationId { get; set; }
            public Guid CausationId { get; set; }
        }
    }
}
