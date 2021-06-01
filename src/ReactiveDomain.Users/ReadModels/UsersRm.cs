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
        public readonly Dictionary<string, Guid> UserIdsBySubjectAtDomain = new Dictionary<string, Guid>();
        public readonly Dictionary<Guid, UserDTO> UsersById = new Dictionary<Guid, UserDTO>();

        public UsersRm(IConfiguredConnection conn) : base(nameof(UsersRm), () => conn.GetListener(nameof(UsersRm)))
        {
            long position;
            using (var reader = conn.GetReader(nameof(UsersRm)))
            {
                reader.EventStream.Subscribe<UserMsgs.UserEvent>(this);
                reader.EventStream.Subscribe<UserMsgs.AuthDomainMapped>(this);
                reader.Read<User>();
                position = reader.Position ?? StreamPosition.Start;
            }
            EventStream.Subscribe<UserMsgs.UserEvent>(this);
            EventStream.Subscribe<UserMsgs.AuthDomainMapped>(this);
            Start<User>(checkpoint: position);
        }

        void IHandle<UserMsgs.AuthDomainMapped>.Handle(UserMsgs.AuthDomainMapped @event)
        {
            var subject = $"{@event.SubjectId}@{@event.AuthDomain}";
            if (UserIdsBySubjectAtDomain.ContainsKey(subject))
            {
                UserIdsBySubjectAtDomain[subject] = @event.UserId;
            }
            else
            {
                UserIdsBySubjectAtDomain.Add(subject, @event.UserId);
            }
        }
        public bool HasUser(string subjectId, string authDomain, out Guid userId)
        {
            var subject = $"{subjectId}@{authDomain}";
            return UserIdsBySubjectAtDomain.TryGetValue(subject, out userId);
        }

        public void Handle(UserMsgs.UserEvent @event)
        {
            if (@event is UserMsgs.UserCreated)
            {
                UsersById.Add(@event.UserId, new UserDTO(@event.UserId));
            }
            else if (UsersById.TryGetValue(@event.UserId, out var user))
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
