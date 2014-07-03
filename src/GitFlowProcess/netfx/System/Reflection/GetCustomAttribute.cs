/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Allows retrieving custom attributes from assemblies, types, methods, properties, etc. using a generic method.
    /// </summary>
    ///	<nuget id="netfx-System.Reflection.GetCustomAttribute" />
    static partial class GetCustomAttributeExtension
    {
        /// <summary>
        /// Retrieves the first defined attribute of the given type from the provider if any.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
        /// <param name="provider" this="true">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
        /// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetCustomAttributes<TAttribute>(provider, inherit).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first defined attribute of the given type from the provider if any.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute, which must inherit from <see cref="Attribute"/>.</typeparam>
        /// <param name="provider" this="true">Any type implementing the interface, which can be an assembly, type, 
        /// property, method, etc.</param>
        /// <param name="inherit">Optionally, whether the attribute will be looked in base classes.</param>
        /// <returns>The attribute instance if defined; <see langword="null"/> otherwise.</returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
            where TAttribute : Attribute
        {
            Guard.NotNull(() => provider, provider);

            return provider
                .GetCustomAttributes(typeof(TAttribute), inherit)
                .Cast<TAttribute>();
        }
    }
}