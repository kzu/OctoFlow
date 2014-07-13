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
    using System.Configuration;

    public class TokenProvider : DefaultProvider
    {
        public override object GetDefault(VerbExecutionContext context)
        {
            return ConfigurationManager.AppSettings["apiToken"];
        }

        public override string Description
        {
            get { return "apiToken in appSettings"; }
        }
    }
}
