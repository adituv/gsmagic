using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //Control
using System.Drawing; //Point, Size
namespace gsmagic
{
	class DataNud : NumericUpDown
	{
		//public NumericUpDown nud;
		public byte[] buf;
		public int addr;
		public int bits;
		public NumericUpDown doNud(Control pnl, int x, int y, byte[] buffer, int address, int numOfBits)
		{
			//nud = new NumericUpDown();
			//nud1.Location = new Point(pnl.Width / 2 - 50, pnl.Height / 2 - 100);
			Location = new Point(x, y);
			Width = 100;//nud1.Size = new Size(200, 100);
			Font = Globals.font; //new Font("Lucida Console", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
			pnl.Controls.Add(this);
			buf = buffer;
			addr = address; //offsets.Add(offset);
			bits = numOfBits; //bits.Add(numOfBits);
			ValueChanged += new EventHandler(nudChanged);
			//nud.MouseClick += new MouseEventHandler(nudrc);
			Maximum = ((long)1 << numOfBits) - 1; //if not long, 32+ will be 0.... so -1
			return this;
		}
		private void nudChanged(object sender, EventArgs e)
		{
			//int i = ctrls.IndexOf((NumericUpDown)sender);
			int value = (int)((NumericUpDown)sender).Value;
			for (int j = 0; j < bits; j += 8)
			{
				buf[addr + (j >> 3)] = (byte)value;
				value >>= 8;
			}
		}
		//public void getData()
		//{
		//	Value = Bits.getBits(buf, addr, bits);
		//}
	}
}
