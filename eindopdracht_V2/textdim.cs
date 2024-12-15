using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace eindopdracht_V2
{
    internal class TEXTD:Window
    {
		private string text = "dim=100%";

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

	}
}
