using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Plant.Core.Exceptions;
using Plant.Core.Helpers;

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
            get
            {
                return _objectConstructed;
            }
        }
    }

    public delegate void BluePrintCreatedEventHandler(object sender, BluePrintEventArgs e);
    #endregion

    internal class BasePlant : IPlant
    {
        private readonly Dictionary<string, object> _createdBluePrints = new Dictionary<string, object>();
        private readonly IDictionary<Type, object> _postCreationActions = new Dictionary<Type, object>();

        private readonly SequenceDictionary _sequenceValues = new SequenceDictionary();
        private readonly ConstructorDictionary _costructors = new ConstructorDictionary();
        private readonly PropertyDictionary _properties = new PropertyDictionary();


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
            string bluePrintKey = BluePrintKey<T>(null);
            T constructedObject;

            if (_createdBluePrints.ContainsKey(bluePrintKey))
                constructedObject = (T)_createdBluePrints[bluePrintKey];
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
            var constructedObject = _costructors.CreateIstance<T>();

            if (_properties.ContainsKey<T>())
                SetProperties(_properties.Get<T>(), constructedObject);

            if (userSpecifiedProperties != null)
            {
                userSpecifiedProperties.Invoke(constructedObject);
            }

            // We should check if for the object properties we have a creation strategy and call create on that one.
            // Also if the property has a value, don't override.
            foreach (var prop in constructedObject.GetType().GetProperties())
            {
                if (!_costructors.ContainsType(prop.PropertyType) || prop.GetValue(constructedObject, null) != null)
                    continue;

                // check if property has a setter
                if (prop.GetSetMethod() == null)
                    continue;

                var value = this.GetType().
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
            return Create<T>(string.Empty, userSpecifiedProperties);
        }

        public virtual T Create<T>(string variation, Action<T> userSpecifiedProperties)
        {

            var constructedObject = Build(userSpecifiedProperties);


            var bluePrintKey = BlueprintKeyGenerator.BluePrintKey<T>();

            OnBluePrintCreated(new BluePrintEventArgs(constructedObject));

            if (!_createdBluePrints.ContainsKey(bluePrintKey))
                _createdBluePrints.Add(bluePrintKey, constructedObject);

            if (_postCreationActions.ContainsKey(typeof(T)))
                ((Action<T>)_postCreationActions[typeof(T)])(constructedObject);

            //if (_postBuildVariationActions.ContainsKey(bluePrintKey))
            //    ((Action<T>)_postBuildVariationActions[bluePrintKey])(constructedObject);

            return constructedObject;
        }

        protected string BluePrintKey<T>(string variation)
        {
            return string.Format("{0}-{1}", typeof(T), variation);
        }

        private void UpdateProperties<T>(T constructedObject, string variation)
        {
            if (string.IsNullOrEmpty(variation))
                return;

            SetProperties(_properties.Get<T>(variation), constructedObject);
        }

        private void SetProperties<T>(IDictionary<PropertyData, Expression> properties, T instance)
        {
            properties.Keys.ToList().ForEach(property =>
              {
                  var propertyInfo = instance.GetType().GetProperties().FirstOrDefault(prop => prop.Name == property.Name);
                  if (propertyInfo == null) throw new PropertyNotFoundException(property.Name, properties[property]);

                  var expression = properties[property];

                  var callExpression = expression as MethodCallExpression;

                  if (callExpression != null && callExpression.Method.DeclaringType == typeof(Sequence))
                  {
                      var sequenceNumber = _sequenceValues.GetSequenceValue<T>(propertyInfo);

                      var compiled = ((LambdaExpression) callExpression.Arguments[0]).Compile();

                      propertyInfo.SetValue(instance, compiled.DynamicInvoke(sequenceNumber), null);
                  }
                  else
                  {
                      var lambda = Expression.Lambda(expression);
                      var compiled = lambda.Compile();

                      propertyInfo.SetValue(instance, compiled.DynamicInvoke(null), null);
                  }
              });
        }


        public virtual void Define<T>(Expression<Func<T>> definition)
        {
            if (_costructors.ContainsType<T>()) throw new DuplicateBlueprintException(typeof(T).Name + " is already registered. You can only register one factory per type.");

            switch (definition.Body.NodeType)
            {
                case ExpressionType.MemberInit:
                    {
                        var memberInitExpression = ((MemberInitExpression)definition.Body);
                        _costructors.Add<T>(memberInitExpression.NewExpression);
                        _properties.Add<T>(memberInitExpression.Bindings);
                    }
                    break;
                case ExpressionType.New:
                    {
                        _costructors.Add<T>((NewExpression)definition.Body);
                    }
                    break;
                default:
                    throw new WrongDefinitionTypeException();
            }
        }

        public virtual void Define<T>(string variation, Expression<Func<T>> definition)
        {
            Define(definition);
        }

        public virtual void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation)
        {
            Define(definition);

            _postCreationActions.Add(typeof(T), afterCreation);
        }

        public virtual void Define<T>(string variation, Expression<Func<T>> definition, Action<T> afterCreation)
        {
            Define(definition, afterCreation);
        }

    }
}
