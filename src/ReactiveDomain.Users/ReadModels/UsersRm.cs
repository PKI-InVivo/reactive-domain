using System;
using System.Collections.Generic;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Users.Domain;
using ReactiveDomain.Users.Messages;

namespace ReactiveDomain.Users.ReadModels
{
    public class UsersRm :
        ReadModelBase,
        IHandle<UserMsgs.AuthDomainMapped>,
        IHandle<UserMsgs.UserEvent>
    {
        private readonly Dictionary<string, Guid> _userIds = new Dictionary<string, Guid>();
        public Dictionary<Guid, UserDTO> Users = new Dictionary<Guid, UserDTO>();

        public UsersRm(IConfiguredConnection conn) : base(nameof(UsersRm), () => conn.GetListener(nameof(UsersRm)))
        {
            long position;
            using (var reader = conn.GetReader(nameof(UsersRm)))
            {
                reader.EventStream.Subscribe<UserMsgs.UserEvent>(this);
                reader.Read<User>();
                position = reader.Position ?? StreamPosition.Start;
            }
            EventStream.Subscribe<UserMsgs.UserEvent>(this);
            Start<User>(checkpoint: position);
        }

        void IHandle<UserMsgs.AuthDomainMapped>.Handle(UserMsgs.AuthDomainMapped @event)
        {
            var subject = $"{@event.SubjectId}@{@event.AuthDomain}";
            if (_userIds.ContainsKey(subject))
            {
                _userIds[subject] = @event.UserId;
            }
            else
            {
                _userIds.Add(subject, @event.UserId);
            }
        }
        public bool HasUser(string subjectId, string authDomain, out Guid userId)
        {
            var subject = $"{subjectId}@{authDomain}";
            return _userIds.TryGetValue(subject, out userId);
        }

        public void Handle(UserMsgs.UserEvent @event)
        {
            if (@event is UserMsgs.UserCreated)
            {
                Users.Add(@event.UserId, new UserDTO(@event.UserId));
            }
            else if (Users.TryGetValue(@event.UserId, out var user))
            {
                user.Handle((dynamic)@event);
            }
            if (@event is UserMsgs.AuthDomainMapped mapped)
            {
                this.Handle(mapped);
            }
        }
    }
}
