using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SeeSharpSoft
{
    public class NameAttribute : Attribute
    {
        public NameAttribute(String name)
            : base()
        {
            Name = name;
        }

        public String Name { set; get; }
    }

    public class TypeAttribute : Attribute
    {
        public TypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { private set; get; }
    }
}