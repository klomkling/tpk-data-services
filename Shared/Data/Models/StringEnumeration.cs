using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tpk.DataServices.Shared.Data.Models
{
    public abstract class StringEnumeration : IComparable
    {
        protected StringEnumeration()
        {
        }

        protected StringEnumeration(string value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public string Value { get; }

        public string DisplayName { get; }

        public int CompareTo(object other)
        {
            return string.Compare(Value, ((StringEnumeration) other).Value,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static IEnumerable<T> GetAll<T>(string[] collection = null, bool isContain = false)
            where T : StringEnumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();

                if (!(info.GetValue(instance) is T locatedValue)) continue;

                if (collection != null)
                {
                    if (isContain)
                    {
                        if (collection.Contains(locatedValue.Value, StringComparer.InvariantCultureIgnoreCase))
                            yield return locatedValue;
                    }
                    else
                    {
                        if (collection.Contains(locatedValue.Value, StringComparer.InvariantCultureIgnoreCase) == false)
                            yield return locatedValue;
                    }
                }
                else
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StringEnumeration otherValue)) return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = string.Equals(Value, otherValue.Value, StringComparison.InvariantCultureIgnoreCase);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static T FromValue<T>(string value) where T : StringEnumeration, new()
        {
            var matchingItem = Parse<T, string>(value, "value", item => item.Value == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : StringEnumeration, new()
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
            return matchingItem;
        }

        private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate)
            where T : StringEnumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = $"'{value}' is not a valid {description} in {typeof(T)}";
                throw new ApplicationException(message);
            }

            return matchingItem;
        }
    }
}