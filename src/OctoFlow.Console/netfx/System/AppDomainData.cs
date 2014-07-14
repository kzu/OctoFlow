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
/// Provides strong-typed access to AppDomain storage data.
/// </summary>
/// <nuget id="netfx-System.AppDomainData" />
internal static class AppDomainData
{
	/// <summary>
	/// Sets the given data in the <see cref="AppDomain"/> storage. The returned 
	/// <see cref="IDisposable"/> can be used to remove the state 
	/// when disposed.
	/// </summary>
	public static IDisposable SetData<T>(T data)
		where T : class
	{
		return AppDomain.CurrentDomain.SetData(data);
	}

	/// <summary>
	/// Gets the data from the <see cref="AppDomain"/> storage.
	/// </summary>
	public static T GetData<T>()
		where T : class
	{
		return AppDomain.CurrentDomain.GetData<T>();
	}

	/// <summary>
	/// Sets the given data in the <see cref="AppDomain"/> storage. The returned 
	/// <see cref="IDisposable"/> can be used to remove the state 
	/// when disposed.
	/// </summary>
	///	<param name="domain" this="true">The domain to set the data on.</param>
	///	<param name="data">The data to save.</param>
	public static IDisposable SetData<T>(this AppDomain domain, T data)
		where T : class
	{
		return new TransientData<T>(domain, data, domain.GetData<T>());
	}

	/// <summary>
	/// Gets the data from the <see cref="AppDomain"/> storage.
	/// </summary>
	/// <param name="domain" this="true">The domain to get the data.</param>
	public static T GetData<T>(this AppDomain domain)
		where T : class
	{
		return (T)domain.GetData(typeof(T).FullName);
	}

	private class TransientData<T> : IDisposable
		where T : class
	{
		private T oldData;
		private AppDomain domain;

		public TransientData(AppDomain domain, T newData, T oldData)
		{
			this.domain = domain;
			this.oldData = oldData;
			this.domain.SetData(typeof(T).FullName, newData);
		}

		public void Dispose()
		{
			this.domain.SetData(typeof(T).FullName, this.oldData);
		}
	}
}
