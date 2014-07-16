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

    /// <summary>
    /// Interface used by the application components to log messages.
    /// </summary>
    ///	<nuget id="Tracer.Interfaces" />
    partial interface ITracer
    {
        /// <summary>
        /// Traces the specified message with the given <see cref="TraceEventType"/>.
        /// </summary>
        void Trace(TraceEventType type, object message);

        /// <summary>
        /// Traces the specified formatted message with the given <see cref="TraceEventType"/>.
        /// </summary>
        void Trace(TraceEventType type, string format, params object[] args);

        /// <summary>
        /// Traces an exception with the specified message and <see cref="TraceEventType"/>.
        /// </summary>
        void Trace(TraceEventType type, Exception exception, object message);

        /// <summary>
        /// Traces an exception with the specified formatted message and <see cref="TraceEventType"/>.
        /// </summary>
        void Trace(TraceEventType type, Exception exception, string format, params object[] args);
    }
}
