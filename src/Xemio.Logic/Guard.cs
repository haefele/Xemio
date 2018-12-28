using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xemio.Logic
{
    internal class Guard
    {
        [DebuggerStepThrough]
        public static void NotNullOrWhiteSpace(string argument, string argumentName, string message = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentNullException(argumentName, message);
        }
        [DebuggerStepThrough]
        public static void NotNull(object argument, string argumentName, string message = null)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName, message);
        }
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(IEnumerable argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (argument.GetEnumerator().MoveNext() == false)
                throw new ArgumentException("List is empty.", argumentName);
        }
        [DebuggerStepThrough]
        public static void NotInvalidEnum(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            if (argument.GetType().IsEnum == false)
                throw new InvalidOperationException("The NotInvalidEnum only works with enum values.");

            if (Enum.IsDefined(argument.GetType(), argument) == false)
                throw new ArgumentException("Unknown enum value.", argumentName);
        }
        [DebuggerStepThrough]
        public static void NotInvalidGuid(Guid guid, string argumentName, string message = null)
        {
            if (guid == Guid.Empty)
                throw new ArgumentException(message, argumentName);
        }
    }
}
