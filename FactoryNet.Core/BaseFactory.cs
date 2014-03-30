using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FactoryNet.Core.Dictionaries;
using FactoryNet.Core.Exceptions;
using FactoryNet.Core.Helpers;
using Ninject;

namespace FactoryNet.Core
{
    #region Events

    public class ObjectEventArgs : EventArgs
    {
        private readonly object _object;

        public ObjectEventArgs(object currentObject)
        {
            _object = currentObject;
        }

        public object Object
        {
            get { return _object; }
        }
    }

    public delegate void ObjectCreatedEventHandler(object sender, ObjectEventArgs e);
    public delegate void ObjectDeletedEventHandler(object sender, ObjectEventArgs e);

    #endregion

    public class BaseFactory : IFactory
    {
        private readonly IConstructorHelper _constructorHelper;
        private readonly ILoaderHelper _loader;

        private readonly IConstructors _costructors;
        private readonly IProperties _properties;
        private readonly ISequences _sequenceValues;
        private readonly IPostCreationActions _postCreationActions;
        private readonly ICreatedBlueprints _createdBluePrints;

        private readonly List<object> _createdObjects;

        public BaseFactory()
        {
            _constructorHelper = new ConstructorHelper();
            _costructors = new Constructors();
            _properties = new Properties();
            _sequenceValues = new Sequences();
            _postCreationActions = new PostCreationActions();
            _createdBluePrints = new CreatedBlueprints();

            _createdObjects = new List<object>();
            _loader = new LoaderHelper(this);
        }

        #region Events

        public event ObjectCreatedEventHandler ObjectCreated;
        public event ObjectDeletedEventHandler ObjectDeleted;

        protected virtual void OnBluePrintCreated(ObjectEventArgs e)
        {
            if (ObjectCreated != null)
                ObjectCreated(this, e);
        }

        protected virtual void OnObjectDeleted(ObjectEventArgs e)
        {
            if (ObjectDeleted != null)
                ObjectDeleted(this, e);
        }


        #endregion

        public virtual T CreateForChild<T>()
        {
            T constructedObject;

            if (_createdBluePrints.ContainsKey<T>(string.Empty))
            {
                constructedObject = _createdBluePrints.Get<T>(string.Empty);
            }
            else
            {
                constructedObject = Create<T>();
            }
                
            return constructedObject;
        }

        public IList<object> CreatedObjects { get { return _createdObjects; } }

        public virtual T Build<T>(Action<T> userSpecifiedProperties)
        {
            return Build(string.Empty, userSpecifiedProperties);
        }

        public virtual T Build<T>(string variation = "", Action<T> userSpecifiedProperties = null)
        {
            var constructedObject = _constructorHelper.CreateInstance<T>(_costructors.Get<T>(variation));

            if (_properties.ContainsKey<T>(variation))
                SetProperties(_properties.Get<T>(variation), constructedObject);

            if (userSpecifiedProperties != null)
            {
                userSpecifiedProperties.Invoke(constructedObject);
            }

            // We should check if for the object properties we have a creation strategy and call create on that one.
            // Also if the property has a value, don't override.
            foreach (PropertyInfo prop in constructedObject.GetType().GetProperties())
            {
                if (!_costructors.ContainsKey(variation, prop.PropertyType) || prop.GetValue(constructedObject, null) != null)
                    continue;

                // check if property has a setter
                if (prop.GetSetMethod() == null)
                    continue;

                object value = GetType().
                    GetMethod("CreateForChild").
                    MakeGenericMethod(prop.PropertyType).
                    Invoke(this, null);

                prop.SetValue(constructedObject, value, null);
            }

            return constructedObject;
        }

        //public T Create<T>(Expression<Func<T>> definition)
        //{
        //    //throw new NotImplementedException();
        //    NewExpression newExpression;

        //    switch (definition.Body.NodeType)
        //    {
        //        case ExpressionType.MemberInit:
        //            {
        //                //var memberInitExpression = ((MemberInitExpression)definition.Body);
        //                newExpression = ((MemberInitExpression) definition.Body).NewExpression;
        //                //_properties.Add<T>(variation, memberInitExpression.Bindings);
        //            }
        //            break;
        //        case ExpressionType.New:
        //            {
        //                newExpression = (NewExpression)definition.Body;
        //            }
        //            break;
        //        default:
        //            throw new WrongDefinitionTypeException();
        //    }

        //    return _constructorHelper.CreateInstance<T>(newExpression);
        //}

        public virtual T Create<T>(Action<T> userSpecifiedProperties)
        {
            return Create(string.Empty, userSpecifiedProperties);
        }

        public virtual T Create<T>(string variation = "", Action<T> userSpecifiedProperties = null)
        {
            T constructedObject = Build(variation, userSpecifiedProperties);

            OnBluePrintCreated(new ObjectEventArgs(constructedObject));

            if (!_createdBluePrints.ContainsKey<T>(variation))
                _createdBluePrints.Add(variation, constructedObject);

            _createdObjects.Add(constructedObject);

            if (_postCreationActions.ContainsKey<T>(variation))
                _postCreationActions.ExecuteAction(variation, constructedObject);

            return constructedObject;
        }

        public virtual void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation = null)
        {
            Define(string.Empty, definition, afterCreation);
        }

        public virtual void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation = null)
        {
            if (_costructors.ContainsKey<T>(variation))
                throw new DuplicateBlueprintException(typeof(T), variation);

            switch (definition.Body.NodeType)
            {
                case ExpressionType.MemberInit:
                    {
                        var memberInitExpression = ((MemberInitExpression)definition.Body);
                        _costructors.Add<T>(variation, memberInitExpression.NewExpression);
                        _properties.Add<T>(variation, memberInitExpression.Bindings);
                    }
                    break;
                case ExpressionType.New:
                    {
                        _costructors.Add<T>(variation, (NewExpression)definition.Body);
                    }
                    break;
                default:
                    throw new WrongDefinitionTypeException();
            }

            if (afterCreation != null)
                _postCreationActions.Add(variation, afterCreation);
        }

        public void ClearCreatedObjects()
        {
            for (int i = _createdObjects.Count - 1; i >= 0; i--)
            {
                OnObjectDeleted(new ObjectEventArgs(_createdObjects[i]));
            }

            _createdObjects.Clear();
        }

        private void SetProperties<T>(IDictionary<PropertyData, Expression> properties, T instance)
        {
            properties.Keys.ToList().ForEach(property =>
                {
                    PropertyInfo propertyInfo = instance.GetType().GetProperties().FirstOrDefault(prop => prop.Name == property.Name);
                    if (propertyInfo == null) throw new PropertyNotFoundException(property.Name, properties[property]);

                    var expression = properties[property];

                    MethodCallExpression callExpression;

                    var unaryExpression = expression as UnaryExpression;

                    if (unaryExpression != null)
                    {
                        callExpression = unaryExpression.Operand as MethodCallExpression;
                    }
                    else
                    {
                        callExpression = expression as MethodCallExpression;
                    }

                    if (callExpression != null && callExpression.Method.DeclaringType == typeof(Sequence))
                    {
                        int sequenceNumber = _sequenceValues.GetSequenceValue<T>(propertyInfo);

                        Delegate compiled = ((LambdaExpression)callExpression.Arguments[0]).Compile();

                        var value = Convert.ChangeType(compiled.DynamicInvoke(sequenceNumber), propertyInfo.PropertyType);

                        propertyInfo.SetValue(instance, value, null);
                    }
                    else
                    {
                        LambdaExpression lambda = Expression.Lambda(expression);
                        Delegate compiled = lambda.Compile();

                        propertyInfo.SetValue(instance, compiled.DynamicInvoke(null), null);
                    }
                });
        }

        public IFactory LoadBlueprintsFromAssembly(Assembly assembly)
        {
            return _loader.LoadBlueprintsFromAssembly(assembly);
        }

        public IFactory LoadBlueprintsFromAssemblies()
        {
            return _loader.LoadBlueprintsFromAssemblies();
        }

        public IFactory LoadBlueprintsFromCurrentAssembly()
        {
            return _loader.LoadBlueprintsFromCurrentAssembly(Assembly.GetCallingAssembly());
        }
    }
}