using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gsmagic
{
	abstract class subeditor
	{
		//public string text;
		public abstract string text();
		public abstract void load(TabControl tabControl1);
	}
}
