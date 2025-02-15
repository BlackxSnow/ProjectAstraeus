﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class TypeAttribute : Attribute
{
    public Type Type;

    public static TypeAttribute GetStoredData(Type EnumType, Enum EnumValue)
    {
        MemberInfo[] MemberInfos = EnumType.GetMember(EnumValue.ToString());
        MemberInfo EnumValueMemberInfo = MemberInfos.FirstOrDefault(m => m.DeclaringType == EnumType);
        object[] ValueAttributes = EnumValueMemberInfo.GetCustomAttributes(typeof(TypeAttribute), true);
        return (TypeAttribute)ValueAttributes[0];
    }

    public TypeAttribute(Type _Module)
    {
        Type = _Module;
    }
}
