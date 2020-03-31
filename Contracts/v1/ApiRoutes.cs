using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string root = "api";
        public const string version = "v1";
        public const string Base = root + "/" + version;
        public static class Posts
        {
            public const string GetAll = Base + "/posts";
            public const string Create = Base + "/posts";
            public const string Get = Base + "/post/{postId}";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";

        }


        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
            public const string Refresh = Base + "/identity/refresh";
        }
    }
}
