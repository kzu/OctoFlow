/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow.Diagnostics
{
    /// <summary>
    /// Manages <see cref="ITracer"/> instances. Provides the implementation 
    /// for the <see cref="Tracer"/> static facade class.
    /// </summary>
    ///	<nuget id="Tracer.Interfaces" />
    partial interface ITracerManager
    {
        /// <summary>
        /// Gets a tracer instance with the specified name.
        /// </summary>
        ITracer Get(string name);
    }
}
