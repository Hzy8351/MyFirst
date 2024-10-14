using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T>:IDisposable where T : new()//范型
{
	private static T _instance;
	public  static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}

	public virtual void Dispose()
	{
		
	}
}
