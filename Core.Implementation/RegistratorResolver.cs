using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OrlemSoftware.Basics.Core.Attributes;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Basics.Core.Implementation
{
    class RegistratorResolver : Registrator, IResolver
    {
        public Action<IShutdown> OnShutdownCreated { get; set; } = x => { };

        private readonly ILoggingService _loggingService;
        private readonly string _dynamicAssemblyName;

        public RegistratorResolver(ILoggingService loggingService, string dynamicAssemblyName)
            : base(dynamicAssemblyName)
        {
            _loggingService = loggingService;
            _dynamicAssemblyName = dynamicAssemblyName;
        }

        public RegistratorResolver(ILoggingService loggingService) : this(loggingService, null)
        {

        }

        public T Resolve<T>()
        {
            var retv = Resolve(typeof(T));
            if (retv is T variable)
                return variable;

            return default(T);
        }

        public T Resolve<T>(params object[] args)
        {
            var retv = Resolve(typeof(T), args);
            if (retv is T variable)
                return variable;

            return default(T);
        }

        public T Resolve<T>(params ResolveParameter[] parameters)
        {
            var retv = Resolve(typeof(T), parameters);
            if (retv is T variable)
                return variable;

            return default(T);
        }

        public object Resolve(Type type)
        {
            var retv = GetSingletone(type);
            if (retv != null)
                return retv;

            var dependenciesDictionary = GetDependenciesDictionary();
            ConstructorInfo ctor;
            if (dependenciesDictionary.ContainsKey(type))
                ctor = getConstructor(dependenciesDictionary[type], new object[0]);
            else if (tryMakeGenericType(type, out var genericType))
                ctor = getConstructor(genericType, new object[0]);
            else
                ctor = getConstructor(type, new object[0]);

            retv = getInstance(ctor, new object[0]);
            if (retv == null)
                return null;

            registerIfSingletone(retv, type);

            if (retv is IShutdown shutdown)
                OnShutdownCreated(shutdown);

            injectDependencies(retv);
            if (retv is IInitializable initializable)
                initializable.Initialize();

            return retv;
        }

        public object Resolve(Type type, params object[] args)
        {
            var dependenciesDictionary = GetDependenciesDictionary();
            ConstructorInfo ctor;
            if (dependenciesDictionary.ContainsKey(type))
                ctor = getConstructor(dependenciesDictionary[type], args);
            else if (tryMakeGenericType(type, out var genericType))
                ctor = getConstructor(genericType, args);
            else
                ctor = getConstructor(type, args);

            var retv = getInstance(ctor, args);
            if (retv == null)
                return null;

            registerIfSingletone(retv, type);

            if (retv is IShutdown shutdown)
                OnShutdownCreated(shutdown);

            injectDependencies(retv);
            if (retv is IInitializable initializable)
                initializable.Initialize();

            return retv;
        }

        public object Resolve(Type type, params ResolveParameter[] parameters)
        {
            var dependenciesDictionary = GetDependenciesDictionary();
            ConstructorInfo ctor;
            if (dependenciesDictionary.ContainsKey(type))
                ctor = getConstructor(dependenciesDictionary[type], parameters);
            else if (tryMakeGenericType(type, out var genericType))
                ctor = getConstructor(genericType, parameters);
            else
                ctor = getConstructor(type, parameters);

            var retv = getInstance(ctor, parameters);
            if (retv == null)
                return null;

            registerIfSingletone(retv, type);

            if (retv is IShutdown shutdown)
                OnShutdownCreated(shutdown);

            injectDependencies(retv);
            if (retv is IInitializable initializable)
                initializable.Initialize();

            return retv;
        }

        private object getInstance(ConstructorInfo ctorInfo, ResolveParameter[] parameters)
        {
            if (ctorInfo == null)
                return null;

            var ctor = ctorInfo;
            var resolvedParams = getCtorParams(ctor).ToList();
            var customParams = parameters.ToList();
            var orderedParams = new List<object>();
            var requiredParams = ctor.GetParameters();

            foreach (var requiredParam in requiredParams)
            {
                resolveParameter(requiredParam, ref resolvedParams, ref customParams, ref orderedParams);
            }

            return ctor?.Invoke(orderedParams.ToArray());
        }

        private object getInstance(ConstructorInfo ctorInfo, object[] parameters)
        {
            if (ctorInfo == null)
                return null;

            var ctor = ctorInfo;
            var resolvedParams = getCtorParams(ctor).ToList();
            var customParams = parameters.ToList();
            var orderedParams = new List<object>();
            var requiredParams = ctor.GetParameters();

            foreach (var requiredParam in requiredParams)
            {
                resolveParameter(requiredParam, ref resolvedParams, ref customParams, ref orderedParams);
            }

            return ctor.Invoke(orderedParams.ToArray());
        }

        private void resolveParameter(ParameterInfo requiredParam, ref List<object> resolvedParams,
            ref List<object> resolvedCustomParams, ref List<object> resultParameters)
        {
            var p = resolvedParams.FirstOrDefault(x => requiredParam.ParameterType.IsInstanceOfType(x));
            if (p != null)
            {
                resolvedParams.Remove(p);
                resultParameters.Add(p);
                return;
            }

            p = resolvedCustomParams.FirstOrDefault(x => requiredParam.ParameterType.IsInstanceOfType(x));
            if (p != null)
            {
                resolvedCustomParams.Remove(p);
                resultParameters.Add(p);
                return;
            }

            var nullIndex = resolvedCustomParams.IndexOf(null);
            resolvedCustomParams.RemoveAt(nullIndex);
            resultParameters.Add(null);
        }

        private void resolveParameter(ParameterInfo requiredParam, ref List<object> resolvedParams,
            ref List<ResolveParameter> resolvedCustomParams, ref List<object> resultParameters)
        {
            var cp = resolvedCustomParams.FirstOrDefault(x => requiredParam.Name == x.ParameterName);
            var p = cp?.Parameter;
            if (p != null)
            {
                resolvedCustomParams.Remove(cp);
                resultParameters.Add(p);
                return;
            }

            p = resolvedParams.FirstOrDefault(x => requiredParam.ParameterType.IsInstanceOfType(x));
            if (p != null)
            {
                resolvedParams.Remove(p);
                resultParameters.Add(p);
                return;
            }

            var nullIndex = resolvedCustomParams.IndexOf(null);
            resolvedCustomParams.RemoveAt(nullIndex);
            resultParameters.Add(null);
        }

        private ConstructorInfo getConstructor(Type type, params ResolveParameter[] customArgs)
        {
            if (!customArgs.Any())
                return type.GetConstructors()
                    .FirstOrDefault(x => !x.GetParameters().Any() || x.GetParameters().All(canResolve));

            return type.GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Where(p => !canResolve(p)).All(p => customArgs.Any(arg => p.Name == arg.ParameterName)));
        }

        private ConstructorInfo getConstructor(Type type, params object[] customArgs)
        {
            if (!customArgs.Any())
                return type.GetConstructors()
                    .FirstOrDefault(x => !x.GetParameters().Any() || x.GetParameters().All(canResolve));

            return type.GetConstructors()
                .FirstOrDefault(x => x.GetParameters().Where(p => !canResolve(p)).All(p => customArgs.Any(arg => p.ParameterType.IsInstanceOfType(arg))));
        }

        private object[] getCtorParams(ConstructorInfo ctorInfo)
        {
            var @params = new List<object>();
            var requiredParams = ctorInfo.GetParameters();
            var dict = GetDependenciesDictionary();
            foreach (var requiredParam in requiredParams)
            {
                var p = GetSingletone(requiredParam.ParameterType);
                if (p != null)
                {
                    @params.Add(p);
                    continue;
                }

                if (!dict.ContainsKey(requiredParam.ParameterType))
                    continue;

                var type = dict[requiredParam.ParameterType];
                var ctor = getConstructor(type, new object[0]);
                p = getInstance(ctor, new object[0]);
                registerIfSingletone(p, requiredParam.ParameterType);
                @params.Add(p);
            }

            return @params.ToArray();
        }

        private void registerIfSingletone(object instance, Type interfaceType)
        {
            if (instance?.GetType().GetCustomAttribute<SingletoneAttribute>() != null)
                Register(interfaceType, instance);
        }

        private bool canResolve(ParameterInfo parameter)
        {
            var pType = parameter.ParameterType;
            return GetSingletone(pType) != null
                   || GetDependenciesDictionary().ContainsKey(pType)
                       && GetDependenciesDictionary()[pType].GetConstructors()
                           .Any(x => x.GetParameters().All(canResolve));
        }

        private void injectDependencies(object instance)
        {
            var fieldsToInject = getInjectableFields(instance.GetType());
            foreach (var fieldInfo in fieldsToInject)
            {
                var type = fieldInfo.FieldType;
                var injectValue = Resolve(type);
                fieldInfo.SetValue(instance, injectValue);
            }
        }

        private IReadOnlyCollection<FieldInfo> getInjectableFields(Type type)
        {
            var retv = new List<FieldInfo>();

            var injFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<InjectableAttribute>() != null);

            retv.AddRange(injFields);

            if (type.BaseType == null || type.BaseType == typeof(object))
                return retv;

            var parentFields = getInjectableFields(type.BaseType);
            retv.AddRange(parentFields);

            return retv;
        }

        private bool tryMakeGenericType(Type incomingType, out Type resultType)
        {
            if (!incomingType.IsGenericType)
            {
                resultType = null;
                return false;
            }

            var pureInterface = incomingType.GetGenericTypeDefinition();
            var genericArgs = incomingType.GetGenericArguments();
            var dict = GetDependenciesDictionary();
            if (!dict.TryGetValue(pureInterface, out var genericImplementation))
            {
                resultType = null;
                return false;
            }

            resultType = genericImplementation.MakeGenericType(genericArgs);
            return true;
        }

        public RegistratorResolver Clone(ILoggingService loggingService)
        {
            var retv = new RegistratorResolver(loggingService, _dynamicAssemblyName);
            foreach (var dependency in GetDependenciesDictionary())
            {
                retv.Register(dependency.Key, dependency.Value);
            }

            foreach (var singletone in GetRunningSingletones())
            {
                retv.Register(singletone.Key, singletone.Value);
            }

            return retv;
        }
    }
}