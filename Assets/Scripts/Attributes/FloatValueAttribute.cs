using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class FloatValuesAttribute : Attribute
{
    public float[] FloatArray;

    public static FloatValuesAttribute GetStoredData(Type EnumType, Enum EnumValue)
    {
        MemberInfo[] MemberInfos = EnumType.GetMember(EnumValue.ToString());
        MemberInfo EnumValueMemberInfo = MemberInfos.FirstOrDefault(m => m.DeclaringType == EnumType);
        object[] ValueAttributes = EnumValueMemberInfo.GetCustomAttributes(typeof(FloatValuesAttribute), true);
        return (FloatValuesAttribute)ValueAttributes[0];
    }

    public FloatValuesAttribute(params float[] Values)
    {
        FloatArray = Values;
    }
}
