﻿using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.Users.Policy
{
    /// <summary>
    /// Houses the role data populated by the role created handler.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// The role ID.
        /// </summary>
        public Guid RoleId { get; private set; }
        /// <summary>
        /// The role name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The Id of the policy defining the roles.
        /// </summary>
        public Guid PolicyId { get; internal set; }

        private readonly HashSet<Permission> _permissions = new HashSet<Permission>();
        private readonly HashSet<Permission> _effectivePermissions = new HashSet<Permission>();
        public IReadOnlyList<Permission> DirectPermissions => _permissions.ToList().AsReadOnly();

        public IReadOnlyList<Permission> Permissions => _effectivePermissions.ToList().AsReadOnly();
        private readonly HashSet<Role> _parentRoles = new HashSet<Role>();
        private readonly HashSet<Role> _childRoles = new HashSet<Role>();
        public IReadOnlyList<Role> ChildRoles => _childRoles.ToList().AsReadOnly();
        /// <summary>
        /// Houses the role data populated by the role created handler.
        /// </summary>
        public Role(
            Guid roleId,
            string name,
            Guid policyId)
        {
            RoleId = roleId;
            Name = name;
            PolicyId = policyId;
        }

        public Permission AddPermission<T>() where T : ICommand => AddPermission(typeof(T));

        public Permission AddPermission(Type t)
        {
            var p = new Permission(t);
            
            _permissions.Add(p);
            _effectivePermissions.Add(p);
            foreach (var parentRole in _parentRoles)
            {
                parentRole.AddInherited(p);
            }

            return p;
        }
        
        public void AddInherited(Permission p)
        {
            _effectivePermissions.Add(p);
            foreach (var parentRole in _parentRoles)
            {
                parentRole.AddInherited(p);
            }
        }
        public void AddChildRole(Role role)
        {
            _childRoles.Add(role);
            _effectivePermissions.UnionWith(role._permissions);
            role._parentRoles.Add(this);
        }

        internal void SetRoleId(Guid id)
        {
            if (id == RoleId) return;
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id), "Cannot set roleId to guid.empty");
            if (RoleId != Guid.Empty) throw new InvalidOperationException("cannot change RoleID ");
            RoleId = id;
        }
    }
}