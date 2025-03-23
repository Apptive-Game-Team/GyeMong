using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace System.Event.Interpreter
{
    public class EventInterpreter : IEventInterpreter
    {
#if UNITY_EDITOR
        private readonly string _rootPath = Application.dataPath;
#else
        private readonly string _rootPath = Application.persistentDataPath;
#endif
        private readonly string _scriptPath = Path.Combine(Application.dataPath, "Events", "Scripts");
        private readonly string _extension = ".es";
        
        private static List<Type> _eventTypes = null;
        private static List<Type> _conditionTypes = null;
        public static List<Type> Events
        {
            get
            {
                if (_eventTypes == null)
                {
                    _eventTypes = new List<Type>();
                    Type[] types = typeof(global::Event).GetNestedTypes();
                    foreach (Type type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            _eventTypes.Add(type);
                        }
                    }
                }
                return _eventTypes;
            }
        }
        public static List<Type> Conditions
        {
            get
            {
                if (_conditionTypes == null)
                {
                    _conditionTypes = new List<Type>();
                    Type[] types = typeof(Condition).GetNestedTypes();
                    foreach (Type type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            _conditionTypes.Add(type);
                        }
                    }
                }
                return _conditionTypes;
            }
        }
        
        public static Type[] GetArgumentTypes(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            List<Type> types = new List<Type>();
            foreach (FieldInfo field in fields)
            {
                if (Attribute.IsDefined(field, typeof(SerializeField)) || 
                    Attribute.IsDefined(field, typeof(SerializeReference)) || 
                    field.IsPublic)
                {
                    types.Add(field.FieldType);
                }
            }
            return types.ToArray();
        }

        public static Type[] GetArgumentTypes(Type type, bool primaryType)
        {
            Type[] types = GetArgumentTypes(type);
            if (!primaryType)
                return types;
            
            List<Type> primaryTypes = new List<Type>();
            foreach (Type t in types)
            {
                if (t.IsPrimitive || t == typeof(string))
                {
                    primaryTypes.Add(t);
                }
            }
            return primaryTypes.ToArray();
        }
        
        public List<global::Event> Interpret(string path)
        {
            string script = File.ReadAllText(path);


            return null;
        }
    }
}