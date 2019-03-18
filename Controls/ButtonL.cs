using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //Control
using System.Drawing; //Point, Size
namespace gsmagic
{
	class ButtonL : Button
	{
		Control pnl;
		public byte[] buf;
		public int addr;
		public int bits;
		//public List<string> itemsL;
		public byte[] txt;

		public List<int> items;

		//String[] items;
		//int index = 0;
		//protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		//{
		//	if (keyData == Keys.Down || keyData == Keys.Up)
		//	{
		//		// Process keys
		//		Text = "TEST";
		//		return true;
		//	}
		//	return false;
		//}
		//override = Since we are inheritting from Button, these are the functions we are replacing?
		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Up:
				case Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//	base.OnKeyDown(e);
		//	switch (e.KeyCode)
		//	{
		//		case Keys.Up:
		//			break;
		//		case Keys.Down:
		//			break;
		//	}
		//}
		public Control doCombo(Control panel, int x, int y, byte[] textBank, List<int> items, byte[] buffer, int address, int numOfBits)
		{
			pnl = panel;
			AutoEllipsis = true; //If false: If text string too long and lacking spaces, text can disappear.
			Location = new Point(x, y);
			//bn.Width = 180;
			Size = new Size(180, 20);
			Font = Globals.font; //new Font("Lucida Console", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
								 //bn.DropDownStyle = ComboBoxStyle.DropDownList;
								 //bn.Items.AddRange(items);
			pnl.Controls.Add(this);
			//ctrls.Add(this);
			//itemsL = items.ToList();
			buf = buffer;
			addr = address; //offsets.Add(offset);
			bits = numOfBits; //bits.Add(numOfBits);
			TextAlign = ContentAlignment.MiddleLeft;
			//bn.SelectedIndexChanged += new EventHandler(comboChanged);
			//cmb.DrawMode = DrawMode.OwnerDrawFixed;
			//return cmb;

			txt = textBank;
			this.items = items;

			PreviewKeyDown += new PreviewKeyDownEventHandler(comboKeyDown);
			//bn.MouseClick += new MouseEventHandler(nudrc);
			Click += test;
			return this;
		}
		//private void test(object sender, EventArgs e) {
		//	curList = (ButtonL)sender;
		//	int i = ctrls.IndexOf((Button)sender);
		//	//sl2.mItems = itemsL[i];
		//	//sl2.Tb_TextChanged(null, null);
		//	lpnl.Show();
		//	initHiddenList()
		//}
		ButtonL curList;
		Panel lpnl = new Panel();
		public SearchList sl2 = new SearchList();
		
		//private int tt() {
		//	return 0;
		//}
		//Action aa;// = tt;
				  //Object aaa = tt;
		//Func<int> aab;//b; = tt;//object tta = tt;
		private void test(object sender, EventArgs e) //(object sender, EventArgs e)
		{
			//Small code hack to support the sorting w/o opening pop-up list...
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				//aa = tt;//nudrc(sender, null);
				//aab = tt;
				return;
			}
			//lpnl.Hide();
			//pnl.BackColor = Color.Brown;
			lpnl = new Panel();
			curList = (ButtonL)sender;
			//sl2.mItems = itemsL[i];
			//sl2.Tb_TextChanged(null, null);
			lpnl.Show();

			sl2.doTableListbox(lpnl, txt, items);
			lpnl.Left = 300;
			lpnl.Width = 250;
			lpnl.Top = 20;
			lpnl.Height = pnl.Height - 40;
			sl2.lv.Height -= 20;

			Button ok = new Button();
			ok.Left = 0;
			ok.Top = lpnl.Height - 20;
			//cntAddr.Text = lpnl.Height.ToString();
			ok.Width = 120;
			ok.Height = 20;
			ok.Text = "Ok";
			lpnl.Controls.Add(ok);
			ok.Click += okClick;

			Button cancel = new Button();
			cancel.Left = 120;
			cancel.Top = lpnl.Height - 20;
			cancel.Width = 120;
			cancel.Height = 20;
			cancel.Text = "Cancel";
			lpnl.Controls.Add(cancel);
			cancel.Click += cancelClick;

			//pnl.Controls.Add(sl2);
			pnl.Controls.Add(lpnl);
			lpnl.BringToFront();
			//ok.BringToFront();
		}
		private void okClick(object sender, EventArgs e)
		{
			lpnl.Hide();
			curList.Focus();
			//int i = ctrls.IndexOf((ButtonL)curList);
			//int value = (int)((ListView)sl2.lv)..SelectedIndex;
			if (sl2.lv.SelectedIndices.Count != 1) { return; }
			int value = sl2.sItems[sl2.lv.SelectedIndices[0]];
			for (int j = 0; j < bits; j += 8)
			{
				buf[addr + (j >> 3)] = (byte)value;
				value >>= 8;
			}
			value = Bits.getBits(buf, addr, bits);
			Text = Bits.getTextShort(txt, items[value]);//itemsL[value].ToString();
		}
		private void cancelClick(object sender, EventArgs e)
		{
			lpnl.Hide();
			curList.Focus();
		}
		//private void comboChanged(object sender, EventArgs e)
		//{
		//	int i = ctrls.IndexOf((ComboBox)sender);
		//	int value = (int)((ComboBox)sender).SelectedIndex;
		//	for (int j = 0; j < bits[i]; j += 8)
		//	{
		//		buf[entryAddr + offsets[i] + (j >> 3)] = (byte)value;
		//		value >>= 8;
		//	}
		//}

		private void comboKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			//base.comboKeyDown(e);
			//base.OnKeyDown(e);
			//e..Handled = true;
			int value = Bits.getBits(buf, addr, bits);
			switch (e.KeyCode)
			{
				//case Keys.Left:
				//case Keys.Right:
				case Keys.Up:
					if (value <= 0) { break; }
					Bits.setBits(buf, addr, bits, --value);
					Text = Bits.getTextShort(txt, sl2.mItems[sl2.sItems[value]]); //itemsL[value].ToString();
					break;
				case Keys.Down:
					if (value >= items.Count() - 1) { break; }
					Bits.setBits(buf, addr, bits, ++value);
					Text = Bits.getTextShort(txt, sl2.mItems[sl2.sItems[value]]); //itemsL[value].ToString();
					break;
			}
			//int value = (int)((ComboBox)sender).SelectedIndex;
			//for (int j = 0; j < bits[i]; j += 8)
			//{
			//	buf[entryAddr + offsets[i] + (j >> 3)] = (byte)value;
			//	value >>= 8;
			//}
		}
	}
}