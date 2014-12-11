namespace Raygun.Owin.Samples.NancyFX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.Security;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration
            {
                RedirectUrl = "~/login",
                UserMapper = new UserMapper()
            });
        }
    }

    public class User
    {
        public User()
        {
        }

        public User(Guid id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
    }

    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }

        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public IEnumerable<string> Claims { get; private set; }
    }

    public class UserMapper : IUserMapper
    {
        public static readonly List<User> Users = new List<User>
        {
            new User(new Guid("d793fca7-f16a-4b0f-ac32-03981718133f"), "test1"),
            new User(new Guid("d1f35daa-75a4-4cf3-9d99-d95301a6c701"), "test2"),
            new User(new Guid("7497f610-9711-4322-b1c5-69e1b990bcd1"), "test3"),
            new User(new Guid("a4d76c58-95ed-4305-ba8c-518c323632d7"), "test4")
        };

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var user = Users.First(u => u.Id == identifier);

            return new UserIdentity(user);
        }
    }
}