using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;

namespace SeeSharpSoft
{
    public static class SeeSharpSoftCoreExtensions
    {
        /// <summary>
        /// Checks whether value is null or DBNull.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value</param>
        /// <returns>True if value is null or DBNull, false else.</returns>
        public static bool IsNull<T>(this T value)
        {
            return value == null || value is DBNull;
        }

        /// <summary>
        /// Checks whether value is null or DBNull or its toString representation is an emtpy string.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value</param>
        /// <returns>True if value is null or DBNull or its toString representation is an emtpy string, false else.</returns>
        public static bool IsNullOrEmpty<T>(this T value)
        {
            return value.IsNull() || String.IsNullOrEmpty(value.ToString());
        }

        /// <summary>
        /// Returns value or the first non-null element of the valueList.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value</param>
        /// <param name="valueList">List of alternatives.</param>
        /// <returns>Value or the first non-null element of the valueList.</returns>
        public static T Coalesce<T>(this T value, params T[] valueList)
        {
            return Coalesce(value, IsNull, valueList);
        }

        /// <summary>
        /// Returns value or the first non-null or -empty element of the valueList.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value</param>
        /// <param name="valueList">List of alternatives.</param>
        /// <returns>Value or the first non-null or -empty element of the valueList.</returns>
        public static T CoalesceEmpty<T>(this T value, params T[] valueList)
        {
            return Coalesce(value, IsNullOrEmpty, valueList);
        }

        /// <summary>
        /// Returns value or the first element of the valueList which shouldn't be skipped.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="value">Value</param>
        /// <param name="skip">Function to determine whether to skip an element.</param>
        /// <param name="valueList">List of alternatives.</param>
        /// <returns>Value or the first element of the valueList which shouldn't be skipped.</returns>
        public static T Coalesce<T>(this T value, Func<T, bool> skip, params T[] valueList)
        {
            if (!skip.Invoke(value)) return value;

            return valueList.FirstOrDefault(elem => !skip.Invoke(elem));
        }

        public static void ResetValues(this object component)
        {
            foreach (PropertyDescriptor desc in TypeDescriptor.GetProperties(component))
            {
                if (desc.CanResetValue(component))
                {
                    desc.ResetValue(component);
                }
            }
        }

        #region IDataReader

        public static T GetValue<T>(this IDataReader reader, int index)
        {
            return (T)Convert.ChangeType(reader[index], typeof(T));
        }

        public static T GetValue<T>(this IDataReader reader, String name)
        {
            return (T)Convert.ChangeType(reader[name], typeof(T));
        }

        public static IEnumerable<IDataRecord> AsEnumerable(this IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader;
            }
        }

        public static DataTable ToDataTable(this IDataReader dr)
        {
            DataTable result = new DataTable();
            result.Load(dr);
            return result;
        }

        public static DataTable Select(this DataTable dt, string whereExpression, string orderByExpression)
        {
            DataView view = new DataView(dt, whereExpression, orderByExpression, DataViewRowState.CurrentRows);
            return view.ToTable();
        }

        #endregion

        #region Collection

        public static bool IsSubsetOf<T>(this IEnumerable<T> collection, params T[] list)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            return collection.All(elem => list.Contains(elem));
        }

        public static void Remove<T>(this HashSet<T> hashSet, int hashCode)
        {
            hashSet.RemoveWhere(elem => elem.GetHashCode() == hashCode);
        }

        public static void AddRange<T>(this ICollection<T> target, params T[] collection)
        {
            target.AddRange(collection.AsEnumerable());
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> collection)
        {
            foreach (T elem in collection) target.Add(elem);
        }

        //public static void AddRange<K, V>(this IDictionary<K, V> target, IDictionary<K, V> source)
        //{
        //    target.AddRange(source.AsEnumerable());
        //}

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> target)
        {
            return new SeeSharpSoft.Utils.Permutations<T>(target);
        }

        //public static IEnumerable<IList<T>> GetCombinations<T>(this IEnumerable<T> target, int startIndex, int length)
        //{
        //    ICollection<IList<T>> result = new List<IList<T>>();

        //    if (startIndex == length)
        //    {
        //        result.Add(new List<T>(target.Take(length)));
        //    }
        //    else
        //    {
        //        T[] elements = target.ToArray();
        //        for (int i = startIndex; i < elements.Length; i++)
        //        {
        //            T tmp = elements[i];

        //            elements[i] = elements[startIndex];
        //            elements[startIndex] = tmp;

        //            result.AddRange(elements.GetCombinations(startIndex + 1, length));

        //            elements[startIndex] = elements[i];
        //            elements[i] = tmp;
        //        }
        //    }

        //    return result;
        //}

        public static T GetElement<T>(this HashSet<T> hashSet, int hashCode)
        {
            return hashSet.FirstOrDefault(elem => elem.GetHashCode() == hashCode);
        }

        /// <summary>
        /// Found at http://dpatrickcaldwell.blogspot.com/search/label/Extension%20Methods
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static String Join(this IEnumerable<string> list, string separator)
        {
            return string.Join(separator, list.ToArray());
        }

        public static String Join<T>(this IEnumerable<T> list, string separator)
        {
            return list.Select(elem => "{" + (Score.Convert.IsEmpty(elem) ? "" : elem.ToString()) + "}").Join(separator);
        }

        public static String Join<T>(this IEnumerable<IEnumerable<T>> list, string separator)
        {
            return list.Select(elem => "{" + elem.Join(separator) + "}").Join(separator);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable result = new DataTable();

            // column names 
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor pi in properties)
            {
                Type colType = pi.PropertyType;

                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }

                result.Columns.Add(new DataColumn(pi.Name, colType));
            }

            if (collection == null) return result;

            foreach (T entry in collection)
            {
                DataRow dr = result.NewRow();

                foreach (PropertyDescriptor pi in properties)
                {
                    dr[pi.Name] = pi.GetValue(entry) ?? DBNull.Value;
                }

                result.Rows.Add(dr);
            }
            return result;
        }

        #endregion


        public static IEnumerable<int> UpTo(this int start, int end)
        {
            return Enumerable.Range(start, end - start + 1);
        }

        #region String

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="s">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(this string s, int count, string endings)
        {
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");
            int sLength = s.Length;
            int len = sLength;
            if (endings != null)
                len += endings.Length;
            if (count > sLength)
                return s; //it's too short to reduce
            s = s.Substring(0, sLength - len + count);
            if (endings != null)
                s += endings;
            return s;
        }

        public static bool IsNullOrEmpty(this String value)
        {
            return String.IsNullOrEmpty(value);
        }

        public static String Format(this String value, params object[] args)
        {
            return String.Format(value, args);
        }

        public static String[] Between(this String input, String openString, String openEscape, String closeString, String closeEscape)
        {
            Regex.Match(input, @"(?<!" + openEscape + ")" + openString + ".*?(?<!(?:(?<!" + openEscape + ")" + openString + "|(?<!" + closeEscape + ")" + closeString + "))(?<!" + closeEscape + ")" + closeString);
            return null;
        }

        #endregion String

        #region Regular Expression

        public static MatchCollection RegexMatches(this String value, String regex, RegexOptions regexOptions)
        {
            return Regex.Matches(value, regex, regexOptions);
        }

        public static MatchCollection RegexMatches(this String value, String regex)
        {
            return value.RegexMatches(regex, RegexOptions.None);
        }

        public static IEnumerable<Group> RegexGroups(this String value, String regex, RegexOptions regexOptions)
        {
            return value.RegexMatches(regex, regexOptions).OfType<Match>().SelectMany(elem => elem.Groups.OfType<Group>());
        }

        public static IEnumerable<Group> RegexGroups(this String value, String regex)
        {
            return value.RegexGroups(regex, RegexOptions.None);
        }
        //TODO
        public static IEnumerable<Group> RegexAllGroups(this String value, String regex, RegexOptions regexOptions)
        {
            Queue<Group> unhandled = new Queue<Group>();
            //value.RegexAllGroups(regex, regexOptions).First().
            //return value.RegexMatches(regex).OfType<Match>().SelectMany(elem => elem.Groups.OfType<Group>());
            return null;
        }

        private static bool EqualsGroup(this Group groupA, Group groupB)
        {
            return groupA.Length == groupB.Length && groupA.Value == groupB.Value && groupA.Captures.OfType<Capture>().SequenceEqual(groupB.Captures.OfType<Capture>());
        }

        #endregion
    }
}