/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Provides access to tracer instances.
    /// </summary>
    ///	<nuget id="Tracer.Interfaces" />
    static partial class Tracer
    {
        private static ITracerManager manager = new DefaultManager();

        /// <summary>
        /// Provides the implementation for managing tracers.
        /// </summary>
        internal static void Initialize(ITracerManager manager)
        {
            Tracer.manager = manager;
        }

        /// <summary>
        /// Gets a tracer instance with the full type name of <typeparamref name="T"/>.
        /// </summary>
        public static ITracer Get<T>()
        {
            return manager.Get(NameFor(typeof(T)));
        }

        /// <summary>
        /// Gets a tracer instance with the full type name of the <paramref name="type"/>.
        /// </summary>
        public static ITracer Get(Type type)
        {
            return manager.Get(NameFor(type));
        }

        /// <summary>
        /// Gets a tracer instance with the given name.
        /// </summary>
        public static ITracer Get(string name)
        {
            return manager.Get(name);
        }


        /// <summary>
        /// Gets the tracer name for the given type.
        /// </summary>
        public static string NameFor<T>()
        {
            return NameFor(typeof(T));
        }

        /// <summary>
        /// Gets the tracer name for the given type.
        /// </summary>
        public static string NameFor(Type type)
        {
            if (type.IsGenericType)
            {
                var genericName = type.GetGenericTypeDefinition().FullName;

                return genericName.Substring(0, genericName.IndexOf('`')) +
                    "<" +
                    string.Join(",", type.GetGenericArguments().Select(t => NameFor(t)).ToArray()) +
                    ">";
            }

            return type.FullName;
        }

        private partial class DefaultManager : ITracerManager
        {
            public ITracer Get(string name)
            {
                return new DefaultTracer(name);
            }

            private class DefaultTracer : ITracer
            {
                private string name;

                public DefaultTracer(string name)
                {
                    this.name = name;
                }

                public void Trace(TraceEventType type, object message)
                {
                    Trace(type, message.ToString());
                }

                public void Trace(TraceEventType type, string format, params object[] args)
                {
                    Trace(type, Format(format, args));
                }

                public void Trace(TraceEventType type, Exception exception, object message)
                {
                    Trace(type, message + Environment.NewLine + exception.ToString());
                }

                public void Trace(TraceEventType type, Exception exception, string format, params object[] args)
                {
                    Trace(type, Format(format, args) + Environment.NewLine + exception.ToString());
                }

                private void Trace(TraceEventType type, string message)
                {
                    System.Diagnostics.Debug.Write(string.Format(
                        "[{0}::{1}] ",
                        this.name, type, message));
                    System.Diagnostics.Debug.WriteLine(message);
                }

                private string Format(string format, object[] args)
                {
                    if (args != null && args.Length != 0)
                        return string.Format(format, args);

                    return format;
                }
            }
        }
    }
}
