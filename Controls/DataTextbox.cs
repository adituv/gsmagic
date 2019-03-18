using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //Control
using System.Drawing; //Point, Size

namespace gsmagic
{
	class DataTextbox
	{
		Control pnl;
		public byte[] txt;
		public int baseIndex;
		public int theIndex;
		public TextBox tbx = new TextBox();
		public Button bn = new Button();
		public void doTextbox(Control panel, int x, int y, byte[] textBuf, int index)
		{
			pnl = panel;

			tbx.Location = new Point(x, y);
			tbx.Width = 120;
			tbx.Font = Globals.font;
			pnl.Controls.Add(tbx);
			//ctrls.Add(tbx);

			txt = textBuf;
			baseIndex = index;
			theIndex = index;

			//Attach Edit button!
			bn.Location = new Point(x + tbx.Width, y);
			bn.Width = 50;
			bn.Height = 20;
			bn.Font = Globals.font;
			bn.Text = "Edit";
			bn.Click += button_Click;
			pnl.Controls.Add(bn);
		}

		private void button_Click(object sender, EventArgs e)
		{ //TODO Move decomptext to Form1, compress text on save.
		  // * Function may need refactoring, but it works.
			String str = tbx.Text; //textBox1.Text;
			byte[] bytes = new byte[0x200];
			int a = 0, b = 0;
			while (a < str.Length)
			{
				if (str[a] == '[')
				{
					int num = 0;
					while (str[++a] != ']')
					{
						num = (num * 10) + (byte)(str[a]) - 0x30;
					}
					a++;
					bytes[b++] = (byte)num;
				}
				else if (((byte)str[a] == 13) && ((byte)str[a + 1] == 10))
				{
					a += 2;
				}
				else
				{
					bytes[b++] = (byte)str[a++];
				}
			}
			b++; //B/c 00 character at end.
				 //byte[] bytes = toRawStrData(textBox1.Text);
				 //int b = bytes.Length + 1; //=0x200 + 1 (NEEDS FIXING)
			//if (listView1.SelectedIndices.Count != 1) { return; }
			int srcEntry = theIndex * 4;
			//int srcEntry = listBox2.SelectedIndex * 4;
			int neaddr = 0xC300 + Bits.getInt32(txt, srcEntry + 4);
			int lendif = Bits.getInt32(txt, srcEntry) - Bits.getInt32(txt, srcEntry + 4) + b;
			int c = srcEntry + 4;
			while ((Bits.getInt32(txt, c) != 0))
			{
				Bits.setInt32(txt, c, Bits.getInt32(txt, c) + lendif);
				c += 4;
			}
			c = 0xC300 + Bits.getInt32(txt, c - 4) - lendif;
			while (txt[c++] != 0) { }
			if (Bits.getInt32(txt, srcEntry + 4) != 0) { Array.Copy(txt, neaddr, txt, 0xC300 + Bits.getInt32(txt, srcEntry + 4), c - neaddr); }
			int d = 0xC300 + Bits.getInt32(txt, srcEntry);
			while (b-- > 0)
			{
				txt[d] = bytes[d++ - (0xC300 + Bits.getInt32(txt, srcEntry))];
			}
			Comp.comptext(txt, Globals.mainForm.rom);
			//b=length needed. ; small - big + length
			//listView1.Invalidate();
		}
	}
}
