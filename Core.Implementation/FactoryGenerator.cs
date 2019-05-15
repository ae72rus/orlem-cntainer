using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using OrlemSoftware.Basics.Core.Attributes;

namespace OrlemSoftware.Basics.Core.Implementation
{
    class FactoryGenerator
    {
        private static readonly Dictionary<string, ModuleBuilder> _moduleBuildersBuildersDict = new Dictionary<string, ModuleBuilder>();
        private static readonly List<string> _createdTypeNames = new List<string>();
        public Type GenerateFactory<TFactory, TInterface, TImplementation>(string dynamicAssemblyName)
            where TFactory : IFactory
            where TImplementation : TInterface
        {
            var asmName = string.IsNullOrWhiteSpace(dynamicAssemblyName) 
                ? "OrlemDynamicAssembly" 
                : dynamicAssemblyName;

            return CreateFactoryType(typeof(TFactory), typeof(TInterface), typeof(TImplementation), asmName);
        }

        private ModuleBuilder getModuleBuilder(string assemblyName)
        {
            if (_moduleBuildersBuildersDict.ContainsKey(assemblyName))
                return _moduleBuildersBuildersDict[assemblyName];

            var asmName = new AssemblyName(assemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            _moduleBuildersBuildersDict[assemblyName] = assemblyBuilder.DefineDynamicModule("MainModule");

            return _moduleBuildersBuildersDict[assemblyName];
        }

        private Type CreateFactoryType(Type tFactory, Type tInterface, Type tImplementation, string dynamicAssemblyName)
        {
            var moduleBuilder = getModuleBuilder(dynamicAssemblyName);
            var createMethods = tFactory.GetMethods().Where(x => x.Name == "Create").ToArray();

            var factoryInterfaceType = tFactory;

            var singletoneAttrType = typeof(SingletoneAttribute);//created factory will be a singletone
            var singletonAttrCtor = singletoneAttrType.GetConstructor(new Type[0]);
            if (singletonAttrCtor == null)
                throw new ApplicationException("Unable to find SingletoneAttribute constructor");

            var singletoneAttrBuilder = new CustomAttributeBuilder(singletonAttrCtor, new object[0]);

            var typeNameBase = $"{tImplementation.Name}Factory";
            var typeName = typeNameBase;
            var idx = 0;
            while (_createdTypeNames.Contains(typeName))//make sure that name is unique
            {
                typeName = $"{typeNameBase}{++idx}";
            }

            _createdTypeNames.Add(typeName);

            var typeBuilder = moduleBuilder.DefineType(typeName,
                                                        TypeAttributes.Public
                                                        | TypeAttributes.Class
                                                        | TypeAttributes.AutoClass
                                                        | TypeAttributes.AnsiClass
                                                        | TypeAttributes.BeforeFieldInit
                                                        | TypeAttributes.AutoLayout,
                                                        null);

            typeBuilder.AddInterfaceImplementation(factoryInterfaceType);// make created factory implement interface
            typeBuilder.SetCustomAttribute(singletoneAttrBuilder);//add attribute

            var resolverField = typeBuilder.DefineField("_resolver", typeof(IResolver), FieldAttributes.Private | FieldAttributes.InitOnly);//create private field _resolver
            var tField = typeBuilder.DefineField("_t", typeof(Type), FieldAttributes.Private);//create private field _t of type Type
            var getTypeMethod = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)) ?? throw new ApplicationException();

            var resolveMethod = typeof(IResolver).GetMethod(nameof(IResolver.Resolve), new[] { typeof(Type), typeof(object[]) });

            if (resolveMethod == null)
                throw new ApplicationException("Unable to find Resolver.Resolve(Type, object[])");

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IResolver) });//let's create a constructor
            var ctorIlGen = ctor.GetILGenerator();

            //create code
            ctorIlGen.Emit(OpCodes.Ldarg_0);//load this on stack
            ctorIlGen.Emit(OpCodes.Dup);//load this on stack again
            ctorIlGen.Emit(OpCodes.Ldtoken, tImplementation);//load Impl type on stack
            ctorIlGen.Emit(OpCodes.Call, getTypeMethod);//call .GetTypeFromHandle()
            ctorIlGen.Emit(OpCodes.Stfld, tField);//save type value to 

            ctorIlGen.Emit(OpCodes.Ldarg_1);//load arg1 on stack
            ctorIlGen.Emit(OpCodes.Stfld, resolverField);//save arg1 to field _resolver

            ctorIlGen.Emit(OpCodes.Ret);//return from ctor

            var implCtorInfos = tImplementation.GetConstructors().OrderBy(x => x.GetParameters().Length).ToList();
            var usedCtorInfos = new List<ConstructorInfo>();//put used ctors here

            foreach (var createMethodInfo in createMethods)
            {
                var parameterTypes = createMethodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

                var implCtor = implCtorInfos.FirstOrDefault(x => parameterTypes.Any() && parameterTypes.All(pt => x.GetParameters().Any(cp => cp.ParameterType == pt))
                                                                  || !parameterTypes.Any() && !usedCtorInfos.Contains(x));

                usedCtorInfos.Add(implCtor);

                var createMethodImpl = typeBuilder.DefineMethod("Create", MethodAttributes.Public | MethodAttributes.Virtual, tInterface, parameterTypes);//create Create method
                typeBuilder.DefineMethodOverride(createMethodImpl, createMethodInfo);//override interface method (difference between abstract class in interface? ;))

                var notImpEx = typeof(NotImplementedException);
                var notImpExCtor = notImpEx.GetConstructor(
                            new[]
                            {typeof(string)});

                var errorMessageBuilder = new StringBuilder("Unable to find a constructor with parameters: ");//create error message if matched implementation ctor not found

                var ilGen = createMethodImpl.GetILGenerator();

                if (parameterTypes.Length == 0)
                    errorMessageBuilder.Append("<no parameters>");

                var localArray = ilGen.DeclareLocal(typeof(object[]));//create local variable of type object[] (call it arr)

                var tmpObj = ilGen.DeclareLocal(typeof(object));//create local variable of type object (call it obj)

                ilGen.Emit(OpCodes.Ldc_I4, parameterTypes.Length);//put langth value on stack
                ilGen.Emit(OpCodes.Newarr, typeof(object));//create new instance of array (length will be taken from stack. see prevoius line)
                ilGen.Emit(OpCodes.Stloc, localArray);//save created array to arr

                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var currentType = parameterTypes[i];

                    ilGen.Emit(OpCodes.Ldarg, i + 1);//load arg by index (0 skipped because arg0 = "this")
                    ilGen.Emit(OpCodes.Stloc, tmpObj);//save loaded arg to obj

                    ilGen.Emit(OpCodes.Ldloc, localArray);//load on stack arr
                    ilGen.Emit(OpCodes.Ldc_I4, i);//load index on stack
                    ilGen.Emit(OpCodes.Ldloc, tmpObj);//load obj

                    if (currentType.IsValueType)
                        ilGen.Emit(OpCodes.Box, currentType);//box value typed item if necessary
                    else
                        ilGen.Emit(OpCodes.Castclass, typeof(object));//cast to object reference type

                    ilGen.EmitWriteLine($"cur: {i} total: {parameterTypes.Length}");//trace
                    ilGen.Emit(OpCodes.Stelem, typeof(object));//put obj to arr at specified index (which was loaded just before obj). obj will be boxed or casted to object in process

                    errorMessageBuilder.Append(currentType.Name);
                    if (i < parameterTypes.Length - 1)
                        errorMessageBuilder.Append(", ");
                }

                if (implCtor != null)//if implementation ctor was found
                {
                    ilGen.Emit(OpCodes.Ldarg_0);//load this
                    ilGen.Emit(OpCodes.Ldfld, resolverField);//load resolver field
                    ilGen.Emit(OpCodes.Ldarg_0);//load this again
                    ilGen.Emit(OpCodes.Ldfld, tField);//load _t field
                    ilGen.Emit(OpCodes.Ldloc, localArray);//load arr
                    ilGen.Emit(OpCodes.Call, resolveMethod);//load _resolver.Resolve(_t, arr)

                    ilGen.Emit(OpCodes.Castclass, tImplementation);//cast returned object to Implementation type

                    ilGen.Emit(OpCodes.Ret);//return
                    continue;
                }

                if (notImpExCtor == null)//if exception ctor was not found (hard to imagine why it could happen)
                    continue;

                ilGen.Emit(OpCodes.Ldstr, errorMessageBuilder.ToString());//load error message on stack
                ilGen.Emit(OpCodes.Newobj, notImpExCtor);// new NotImplementedException(error message);
                ilGen.Emit(OpCodes.Throw);// throw created exception
            }

            return typeBuilder.CreateTypeInfo();
        }
    }
}