using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public static class Reflection
    {
        public static void MapValue<TEntity, TValue>(TEntity entity, string propName, TValue value)
        {
            Type examType = typeof(TEntity);    // Get a type object that represents the Example type.
                                                // Change the static property value.
            PropertyInfo piShared = examType.GetProperty(propName);
            try
            {
                if (value == null)
                    piShared.SetValue(entity, default(TValue), null);
                else if (value.GetType() == typeof(string))
                    piShared.SetValue(entity, value.ToString().Trim('\"'), null);
                else
                {
                    if (Nullable.GetUnderlyingType(piShared.PropertyType) != null)
                        // It's nullable
                        SetReferenceTypeValueFromNonreference(entity, value, piShared);
                    else
                        piShared.SetValue(entity, value, null);
                }
            }
            catch (NullReferenceException)
            {
                piShared.SetValue(entity, default(TValue), null);
            }
            catch (System.ArgumentException ex)
            {
                try
                {
                    SetReferenceTypeValueFromNonreference(entity, value, piShared);
                }
                catch (InvalidCastException)
                {
                    // the input string could not be converted to the target type - abort
                    return;
                }
            }
        }

        private static void SetReferenceTypeValueFromNonreference<TEntity, TValue>(TEntity entity, TValue value, PropertyInfo piShared)
        {
            var convertedValue = System.Convert.ChangeType(value,
                Nullable.GetUnderlyingType(piShared.PropertyType));
            piShared.SetValue(entity, convertedValue, null);
        }
    }
}
