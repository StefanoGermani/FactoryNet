using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly IDictionary<Type, object> _postBuildActions = new Dictionary<Type, object>();
        private readonly IDictionary<string, object> _postBuildVariationActions = new Dictionary<string, object>();
        private readonly IDictionary<Type, int> _sequenceValues = new Dictionary<Type, int>();

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

        private T CreateViaProperties<T>(IDictionary<PropertyData, object> userProperties)
        {
            var instance = CreateInstanceWithEmptyConstructor<T>();
            SetProperties(_properties.Get<T>(), instance);
            return instance;
        }

        private object GetPropertyValue<T>(object property)
        {
            if (property is ILazyProperty)
                return ((ILazyProperty)property).Func.DynamicInvoke();
            if (property is ISequence)
                return ((ISequence)property).Func.DynamicInvoke(_sequenceValues[typeof(T)]++);
            return property;
        }

        private T CreateInstanceWithEmptyConstructor<T>()
        {
            return Activator.CreateInstance<T>();
        }

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
            return Create<T>(null, null, false);
        }

        public virtual T Build<T>(string variation)
        {
            return Create<T>(null, variation, false);
        }

        public virtual T Build<T>(Action<T> userSpecifiedProperties)
        {
            return Create<T>(userSpecifiedProperties, null, false);
        }

        public virtual T Create<T>()
        {
            return Create<T>(null, null, true);
        }

        public virtual T Create<T>(string variation)
        {
            return Create<T>(null, variation, true);
        }

        public virtual T Create<T>(Action<T> userSpecifiedProperties)
        {
            return Create<T>(userSpecifiedProperties, null, true);
        }

        public virtual T Create<T>(Action<T> userSpecifiedProperties, string variation, bool created)
        {
            var instance = _costructors.CreateIstance<T>();

            if (_properties.ContainsKey<T>())
                SetProperties(_properties.Get<T>(), instance);

            if (userSpecifiedProperties != null)
            {
                userSpecifiedProperties.Invoke(instance);
            }

            return instance;

            //T constructedObject = default(T);
            //if (StrategyFor<T>() == CreationStrategy.Constructor)
            //    constructedObject = CreateViaConstructor<T>(userSpecifiedPropertyList);
            //else
            //constructedObject = CreateViaProperties<T>(userSpecifiedPropertyList);

            // We should check if for the object properties we have a creation strategy and call create on that one.
            // Also if the property has a value, don't override.
            //foreach (var prop in constructedObject.GetType().GetProperties())
            //{
            //    if (/*StrategyFor(prop.PropertyType) == null ||*/ prop.GetValue(constructedObject, null) != null)
            //        continue;

            //    // check if property has a setter
            //    if (prop.GetSetMethod() == null)
            //        continue;

            //    var value = this.GetType().
            //        GetMethod("CreateForChild").
            //        MakeGenericMethod(prop.PropertyType).
            //        Invoke(this, null);

            //    prop.SetValue(constructedObject, value, null);
            //}

            //UpdateProperties(constructedObject, variation);

            //string bluePrintKey = BluePrintKey<T>(variation);

            //if (created)
            //    OnBluePrintCreated(new BluePrintEventArgs(constructedObject));

            //if (!_createdBluePrints.ContainsKey(bluePrintKey))
            //    _createdBluePrints.Add(bluePrintKey, constructedObject);

            //if (_postBuildActions.ContainsKey(typeof(T)))
            //    ((Action<T>)_postBuildActions[typeof(T)])(constructedObject);

            //if (_postBuildVariationActions.ContainsKey(bluePrintKey))
            //    ((Action<T>)_postBuildVariationActions[bluePrintKey])(constructedObject);

            //return constructedObject;
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
                  var instanceProperty = instance.GetType().GetProperties().FirstOrDefault(prop => prop.Name == property.Name);
                  if (instanceProperty == null) throw new PropertyNotFoundException(property.Name, properties[property]);

                  var value = properties[property];
                  if (value == null)
                      return;
                  if (value is ILazyProperty)
                      AssignLazyPropertyResult(instance, instanceProperty, value);
                  else if (value is ISequence)
                      AssignSequenceResult(instance, instanceProperty, value, _sequenceValues[typeof(T)]);
                  else if (value.NodeType == ExpressionType.Constant)
                      instanceProperty.SetValue(instance, ((ConstantExpression)value).Value, null);
              });
            //_sequenceValues[typeof(T)]++;
        }

        private void AssignSequenceResult<T>(T instance, PropertyInfo instanceProperty, object value, int sequenceValue)
        {
            var sequence = (ISequence)value;

            if (sequence.Func.Method.ReturnType != instanceProperty.PropertyType)
                throw new LazyPropertyHasWrongTypeException(string.Format("Cannot assign type {0} to property {1} of type {2}",
                  sequence.Func.Method.ReturnType,
                  instanceProperty.Name,
                  instanceProperty.PropertyType));
            // I can pass in the instance as a parameter to this function, but only if I'm using property-setters
            instanceProperty.SetValue(instance, sequence.Func.DynamicInvoke(sequenceValue), null);
        }

        private void AssignLazyPropertyResult<T>(T instance, PropertyInfo instanceProperty, object value)
        {
            var lazyProperty = (ILazyProperty)value;

            if (lazyProperty.Func.Method.ReturnType != instanceProperty.PropertyType)
                throw new LazyPropertyHasWrongTypeException(string.Format("Cannot assign type {0} to property {1} of type {2}",
                  lazyProperty.Func.Method.ReturnType,
                  instanceProperty.Name,
                  instanceProperty.PropertyType));
            // I can pass in the instance as a parameter to this function, but only if I'm using property-setters
            instanceProperty.SetValue(instance, lazyProperty.Func.DynamicInvoke(), null);
        }

        public virtual void Define<T>(Expression<Func<T>> definition)
        {
            if (_costructors.ContainsKey<T>()) throw new DuplicateBlueprintException(typeof(T).Name + " is already registered. You can only register one factory per type.");

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


    }
}
