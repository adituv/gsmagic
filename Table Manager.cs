using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //Control
using System.Drawing; //Point, Size
namespace gsmagic
{
	class Table_Manager
	{
		List<DataNud> nuds = new List<DataNud>();
		List<ButtonL> butls = new List<ButtonL>();
		List<DataTextbox> dtbs = new List<DataTextbox>();

		//List<Control> ctrls = new List<Control>();
		//List<int> offsets = new List<int>();
		//List<int> bits = new List<int>(); //Supports only multiples of 8 for now.
		//List<List<String>> itemsL = new List<List<String>>(); // Should probably move this to custom controls?

		byte[] buf;
		Control pnl;
		int baseAddr = 0;
		int entryLen = 0;
		int entryAddr = 0;
		Label cntAddr;
		public void setPanel(Control panel) //Maybe just init stuff?
		{
			pnl = panel;
			if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				combo2numeric = 1;
		}
		public void setTable(byte[] buffer, int baseA, int eLen)
		{
			buf = buffer;
			baseAddr = baseA;
			entryLen = eLen;
			entryAddr = baseAddr;
		}

		public SearchList sl = new SearchList();
		int combo2numeric = 0;
		public void doTableListbox(byte[] txt, List<int> items) //List<string> items)
		{
			sl.doTableListbox(pnl, txt, items);
			sl.lv.SelectedIndexChanged += new EventHandler(loadEntry);
			if (entryLen != 0) // Entry length is "0" for text editor at present.
				cntAddr = doLabel(240, 0, "        ");
		}
		public void doTableListbox2(byte[] txt, List<int> items) //List<string> items)
		{
			sl.doTableListbox2(pnl, txt, items);
			sl.lv.SelectedIndexChanged += new EventHandler(loadEntry);
			if (entryLen != 0) // Entry length is "0" for text editor at present.
				cntAddr = doLabel(240, 0, "        ");
		}
		public Label doLabel(int x, int y, String text)
		{
			Label lbl = new Label();
			lbl.Location = new Point(x, y);
			lbl.Size = new Size(text.Length * 8 + 8, 20);
			//lbl.Size = new Size(100, 20);
			lbl.Font = Globals.font;
			//lbl.BackColor = Color.Beige;
			lbl.Text = text;
			pnl.Controls.Add(lbl);
			//ctrls.Add(lbl);
			return lbl;
		}
		public void loadEntry(object sender, EventArgs e)
		{
			int srcEntry = 0;
			int entryAddr2 = entryAddr;
			if (sender != null)
			{
				ListView lv = ((ListView)sender);
				if (lv.SelectedIndices.Count != 1) { return; }
				srcEntry = sl.sItems[lv.SelectedIndices[0]];//sortList[lv.SelectedIndices[0]];
				entryAddr2 = baseAddr + srcEntry * entryLen;

				//I want copy/paste entire entries functionality! Yay!
				if (entryAddr != 0) //Ensuring that there is an item to copy. Also prevents CtrL+Shift form load bug because an item is selected on form load.
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
						if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
							for (int i = 0; i < entryLen; i++)
								buf[entryAddr2 + i] = buf[entryAddr + i];
				if (entryLen != 0) // Entry length is "0" for text editor at present.
					cntAddr.Text = (0x8000000 | entryAddr2).ToString("X8");
			}
			else { entryAddr = baseAddr; }
			//int ind = ((ListBox)pnl.Controls[0]).SelectedIndex;
			//int addr = 0xEDACC + ind * 0x1C;
			//pnl.Controls[1].Text = (0x8000000 | addr).ToString("X8");
			for (int i = 0; i < nuds.Count; i++)
			{
				nuds[i].addr = nuds[i].addr - entryAddr + entryAddr2;
				nuds[i].Value = Bits.getBits(nuds[i].buf, nuds[i].addr, nuds[i].bits); //nuds[i].getData();
			}
			for (int i = 0; i < butls.Count; i++)
			{
				butls[i].addr = butls[i].addr - entryAddr + entryAddr2;
				butls[i].Text = Bits.getTextShort(butls[i].txt, butls[i].items[Bits.getBits(butls[i].buf, butls[i].addr, butls[i].bits)]);
				//butls[i].itemsL[Bits.getBits(butls[i].buf, butls[i].addr, butls[i].bits)].ToString();
				//butls[i].Text = Bits.getTextLong(txt, Bits.getBits(butls[i].buf, butls[i].addr, butls[i].bits));
			}
			for (int i = 0; i < dtbs.Count; i++)
			{
				dtbs[i].theIndex = dtbs[i].baseIndex + srcEntry;
				dtbs[i].tbx.Text = Bits.getTextLong(dtbs[i].txt, dtbs[i].theIndex);
			}
			entryAddr = entryAddr2;
			//for (int i = 0; i < ctrls.Count; i++)
			//{
			//	if (ctrls[i].GetType() == typeof(ComboBox))
			//	{
			//		((ComboBox)ctrls[i]).SelectedIndex = Bits.getBits(buf, entryAddr + offsets[i], bits[i]);
			//	}
			//	if (ctrls[i].GetType() == typeof(ButtonL))
			//	{
			//		((ButtonL)ctrls[i]).Text = itemsL[i][Bits.getBits(buf, entryAddr + offsets[i], bits[i])].ToString();
			//	}
			//	if (ctrls[i].GetType() == typeof(TextBox))
			//	{
			//		((TextBox)ctrls[i]).Text = Bits.getTextLong(txt, theIndex + srcEntry);
			//	}
			//}
		}
		public NumericUpDown doNud(int x, int y, int offset, int numOfBits)
		{
			var dnud = new DataNud();
			NumericUpDown nud = dnud.doNud(pnl, x, y, buf, entryAddr + offset, numOfBits);
			nud.MouseClick += new MouseEventHandler(nudrc2);
			nuds.Add(dnud);
			return nud;
		}
		private void nudrc2(object sender, MouseEventArgs e)
		{
			var dnud = (DataNud)sender;
			nudrcMain(dnud.addr - entryAddr, dnud.bits);
		}
		private void nudrcMain(int offset, int bits)
		{ //Sorts main list based on data.
			if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
				return;
			if (sl == null) //Some error-proofing if you don't have a main list.
				return;
			//int k = nuds.IndexOf((DataNud)sender); //What we should sort by.
			//var dnud = ((DataNud)sender);
			for (int i = 0; i < sl.sItems.Count; i++)
			{
				int x = sl.sItems[i];
				int value = Bits.getBits(buf, baseAddr + x * entryLen + offset, bits);
				int j = i;
				while ((j > 0) && (Bits.getBits(buf, (baseAddr + sl.sItems[j - 1] * entryLen) + offset, bits) > value))
				{
					sl.sItems[j] = sl.sItems[j - 1];
					j = j - 1;
				}
				sl.sItems[j] = x;
			}
			sl.lv.Invalidate();
		}
		public Control doCombo(int x, int y, byte[] txt, List<int> items, int offset, int numOfBits)
		//public Control doCombo(int x, int y, string[] items, int offset, int numOfBits)
		{
			if (combo2numeric == 1)
			{
				return doNud(x, y, offset, numOfBits);
				//return;
			}
			ButtonL bn = new ButtonL();
			bn.doCombo(pnl, x, y, txt, items, buf, entryAddr + offset, numOfBits);
			bn.MouseClick += new MouseEventHandler(nudrc3);
			butls.Add(bn);
			return bn;
		}
		private void nudrc3(object sender, MouseEventArgs e)
		{
			var dnud = (ButtonL)sender;
			nudrcMain(dnud.addr - entryAddr, dnud.bits);
		}
		//Textbox stuff for name editing purposes! (So def. link to text function?)
		public DataTextbox doNamebox(int x, int y, byte[] textBuf, int index)
		{
			DataTextbox tbx = new DataTextbox();
			tbx.doTextbox(pnl, x, y, textBuf, index);
			//tbx.Width = 180;
			dtbs.Add(tbx);

			tbx.bn.Click += button_Click;
			return tbx;
		}
		private void button_Click(object sender, EventArgs e)
		{
			//Comp.comptext(((DataTextbox)sender).txt, buf); //Assuming buf is the rom when name boxes are used?
			sl.lv.Invalidate();
		}
	}
}