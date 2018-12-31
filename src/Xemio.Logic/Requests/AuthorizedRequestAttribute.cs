using System;

namespace Xemio.Logic.Requests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizedRequestAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UnauthorizedRequestAttribute : Attribute
    {
    }
}