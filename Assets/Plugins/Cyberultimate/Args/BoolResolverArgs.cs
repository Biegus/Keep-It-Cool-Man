#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
namespace Cyberultimate
{
	public class BoolResolverArgs : EventArgs
	{
		public bool NoSignal { get; private set; } = true;
		public void SendSignal(bool when = true)
		{
			if (when == false)
				return;
			NoSignal = false;
		}
		
	}
}