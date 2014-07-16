/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow
{
    using CLAP;
    using OctoFlow.Diagnostics;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Tracer.Initialize(new TracerManager());

            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            var parser = new Parser<App>();
            parser.Register.ErrorHandler(e => Console.WriteLine(e.Exception.Message));

            parser.Run(args, new App());

            //Parser.Run(args, new App());

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Tracer.Get(typeof(Program)).Error(e.Exception.Flatten(), "Unexpected exception");
            Console.WriteLine(e.Exception.Flatten().ToString());
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
            Process.GetCurrentProcess().Kill();
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tracer.Get(typeof(Program)).Error((Exception)e.ExceptionObject, "Unexpected exception");
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
            Process.GetCurrentProcess().Kill();
        }
    }
}
