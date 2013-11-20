﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Plant.Core.Exceptions;
using Plant.Core.Impl.Dictionaries;

namespace Plant.Core.Impl
{

    #region Events

    public class BluePrintEventArgs : EventArgs
    {
        private readonly object _objectConstructed;

        public BluePrintEventArgs(object objectConstructed)
        {
            _objectConstructed = objectConstructed;
        }

        public object ObjectConstructed
        {
            get { return _objectConstructed; }
        }
    }

    public delegate void BluePrintCreatedEventHandler(object sender, BluePrintEventArgs e);

    #endregion

    internal class BasePlant : IPlant
    {



        private readonly ConstructorDictionary _costructors = new ConstructorDictionary();
        private readonly PropertyDictionary _properties = new PropertyDictionary();
        private readonly SequenceDictionary _sequenceValues = new SequenceDictionary();
        private readonly PostCreationActionDictionary _postCreationActions = new PostCreationActionDictionary();
        private readonly CreatedBlueprintsDictionary _createdBluePrints = new CreatedBlueprintsDictionary();

        #region BluePrintCreated Event

        public event BluePrintCreatedEventHandler BluePrintCreated;

        protected virtual void OnBluePrintCreated(BluePrintEventArgs e)
        {
            if (BluePrintCreated != null)
                BluePrintCreated(this, e);
        }

        #endregion

        public virtual T CreateForChild<T>()
        {
            T constructedObject;

            if (_createdBluePrints.ContainsKey<T>(string.Empty))
                constructedObject = _createdBluePrints.Get<T>(string.Empty);
            else
                constructedObject = Create<T>();

            return constructedObject;
        }

        public virtual T Build<T>()
        {
            return Build<T>(string.Empty, null);
        }

        public virtual T Build<T>(string variation)
        {
            return Build<T>(variation, null);
        }

        public virtual T Build<T>(Action<T> userSpecifiedProperties)
        {
            return Build(string.Empty, userSpecifiedProperties);
        }

        public virtual T Build<T>(string variation, Action<T> userSpecifiedProperties)
        {
            var constructedObject = _costructors.CreateIstance<T>(variation);

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
                if (!_costructors.ContainsType(variation, prop.PropertyType) || prop.GetValue(constructedObject, null) != null)
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

        public virtual T Create<T>()
        {
            return Create<T>(string.Empty, null);
        }

        public virtual T Create<T>(string variation)
        {
            return Create<T>(variation, null);
        }

        public virtual T Create<T>(Action<T> userSpecifiedProperties)
        {
            return Create(string.Empty, userSpecifiedProperties);
        }

        public virtual T Create<T>(string variation, Action<T> userSpecifiedProperties)
        {
            T constructedObject = Build(variation, userSpecifiedProperties);

            OnBluePrintCreated(new BluePrintEventArgs(constructedObject));

            if (!_createdBluePrints.ContainsKey<T>(variation))
                _createdBluePrints.Add(variation, constructedObject);

            if (_postCreationActions.ContainsKey<T>(variation))
                _postCreationActions.ExecuteAction(variation, constructedObject);

            return constructedObject;
        }

        public virtual void Define<T>(Expression<Func<T>> definition)
        {
            Define(string.Empty, definition);
        }

        public virtual void Define<T>(string variation, Expression<Func<T>> definition)
        {
            Define(variation, definition, null);
        }

        public virtual void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation)
        {
            Define(string.Empty, definition, afterCreation);
        }

        public virtual void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation)
        {
            if (_costructors.ContainsType<T>(variation))
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
    }
}