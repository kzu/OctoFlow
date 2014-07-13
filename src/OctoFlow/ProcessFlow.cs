/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

namespace OctoFlow
{
    public class ProcessFlow
    {
        public ProcessFlow(ProcessType type)
        {
            this.Type = type;
        }

        public ProcessType Type { get; private set; }
    }
}
