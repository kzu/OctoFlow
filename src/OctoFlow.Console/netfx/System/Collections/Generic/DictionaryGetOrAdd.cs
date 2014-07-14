/*
   Copyright 2014 Daniel Cazzulino

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Adds a key/value pair to the <see cref="IDictionary{TKey, TValue}"/> if the key does not already exist. 
/// </summary>
internal static partial class DictionaryGetOrAdd
{
	/// <summary>
	/// Adds a key/value pair to the <see cref="IDictionary{TKey, TValue}"/> if the key does not already exist. 
	/// No locking occurs, so the value may be calculated twice on concurrent scenarios. If you need 
	/// concurrency assurances, use a concurrent dictionary instead.
	/// </summary>
	/// <nuget id="netfx-System.Collections.Generic.DictionaryGetOrAdd" />
	/// <param name="dictionary" this="true">The dictionary where the key/value pair will be added</param>
	/// <param name="key">The key to be added to the dictionary</param>
	/// <param name="valueFactory">The value factory</param>
	public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
	{
		var value = default(TValue);
		if (!dictionary.TryGetValue(key, out value))
		{
			value = valueFactory(key);
			dictionary[key] = value;
		}

		return value;
	}
}