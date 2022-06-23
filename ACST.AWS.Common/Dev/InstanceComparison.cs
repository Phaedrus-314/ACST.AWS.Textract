
namespace ACST.AWS.Common.Dev
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    //This structure represents the comparison of one member of an object to the corresponding member of another object.
    public struct MemberComparison
    {
        readonly MemberInfo Member; 

        readonly object Value1;
        
        readonly object Value2;
        
        public MemberComparison(MemberInfo member, object value1, object value2)
        {
            this.Member = member;
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public override string ToString()
        {
            string s;

            if (Value1 == null )
                s = Member?.Name + ": " + "null" + " != " + Value2?.ToString();
            else
                s = Member?.Name + ": " + Value1?.ToString() + (Value1.Equals(Value2) ? " == " : " != ") + Value2?.ToString();
            return s;
        }

        //public override bool Equals(object obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public override int GetHashCode()
        //{
        //    throw new NotImplementedException();
        //}

        //public static bool operator ==(MemberComparison left, MemberComparison right)
        //{
        //    return left.Equals(right);
        //}

        //public static bool operator !=(MemberComparison left, MemberComparison right)
        //{
        //    return !(left == right);
        //}
    }

    public sealed class InstanceComparison
    {

        public static List<MemberComparison> ReflectiveCompare<T>(T x, T y)
        {
            List<MemberComparison> list = new List<MemberComparison>();//The list to be returned

            foreach (MemberInfo m in typeof(T).GetMembers(BindingFlags.NonPublic | BindingFlags.Instance))
                
                if (m.MemberType == MemberTypes.Field)
                {
                    FieldInfo field = (FieldInfo)m;
                    
                    var xValue = field.GetValue(x);
                    var yValue = field.GetValue(y);

                    if (!object.Equals(xValue, yValue))
                        list.Add(new MemberComparison(field, yValue, xValue));
                }
                else if (m.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)m;
                    if (prop.CanRead && prop.GetGetMethod()?.GetParameters().Length == 0)
                    {
                        var xValue = prop.GetValue(x, null);
                        var yValue = prop.GetValue(y, null);
                        if (!object.Equals(xValue, yValue))
                            list.Add(new MemberComparison(prop, xValue, yValue));
                    }
                    else
                        continue;
                }

            return list;
        }
    }
}
