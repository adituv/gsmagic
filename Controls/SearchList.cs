using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //Control
using System.Drawing; //Point, Size
namespace gsmagic
{
	class SearchList
	{
		Control pnl;
		public List<int> mItems;
		public List<int> sItems;

		public TextBox tb;
		public ListView lv;

		byte[] txt;
		//int index;
		//int size;
		public void doTableListbox(Control panel, byte[] textBank, List<int> items)
		{
			pnl = panel;

			tb = new TextBox();
			//tb.Location = new Point(0, 0);
			tb.Width = 240; //lv.Size = new Size(250, pnl.Height);
			tb.Font = Globals.font;
			pnl.Controls.Add(tb);
			tb.TextChanged += Tb_TextChanged;

			txt = textBank;
			//this.index = index;
			//this.size = size;
			//sItems = mItems;
			mItems = items;

			lv = new ListView();
			lv.BackColor = Color.FromArgb(0x08, 0x70, 0x98); lv.ForeColor = Color.White;
			lv.HideSelection = false;
			lv.MultiSelect = false;

			ColumnHeader header = new ColumnHeader();
			header.Text = "";
			header.Name = "col1";
			//header.Width = 240 + 240 + 240;
			//header.Width = pnl.Width; //lv.Width - 4 - SystemInformation.VerticalScrollBarWidth; //Check if scrollba/turn it "always on"?
			lv.Columns.Add(header);
			//lv.Columns[lv.Columns.Count - 1].Width = -2;
			//listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

			//listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			var doubleBufferPropertyInfo = lv.GetType().GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			doubleBufferPropertyInfo.SetValue(lv, true, null);

			lv.Location = new Point(0, 20);
			lv.Size = new Size(240, pnl.Height - 20);
			pnl.Controls.Add(lv);

			lv.Columns[0].Width = 0x7FFF;//lv.Width - 4 - SystemInformation.VerticalScrollBarWidth; //0x7FFF = largest possible.

			// 
			lv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; //| AnchorStyles.Right);
			lv.FullRowSelect = true;
			lv.HeaderStyle = ColumnHeaderStyle.None;
			lv.Font = Globals.font;
			//lv.Alignment=ListViewAlignment.
			lv.View = View.Details;
			//lv.VirtualListSize = 0;
			lv.VirtualMode = true;
			lv.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(lv_RetrieveVirtualItem);
			//lv.SelectedIndexChanged += new EventHandler(loadEntry);

			//cntAddr = doLabel(250, 0, "");

			//lv.VirtualListSize = mItems.Count;
			Tb_TextChanged(null, null);
			//lv.Items[0].Focused = true;
			//lv.Items[0].Selected = true;
		}

		public void Tb_TextChanged(object sender, EventArgs e)
		{
			//if (mItems == null)
			//	return;
			//sItems = new List<int>();
			//for (int i = 0; i < mItems.Count; i++)
			//{
			//	//foreach (string a in mItems) {
			//	if (mItems[i].Contains(tb.Text))
			//	{
			//		sItems.Add(i);
			//	}
			//	//if tb.Text in a { }
			//}
			//lv.VirtualListSize = sItems.Count;
			//throw new NotImplementedException();
			sItems = Bits.getTextMatches(txt, tb.Text, mItems);
			lv.VirtualListSize = sItems.Count;
		}

		private void lv_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (sItems == null)
			{
				e.Item = new ListViewItem("null");
				return;
			}
			if (sItems[e.ItemIndex] == -1)
			{
				e.Item = new ListViewItem("null2");
				return;
			}
			//e.Item = new ListViewItem(mItems[sItems[e.ItemIndex]]);
			e.Item = new ListViewItem((sItems[e.ItemIndex]).ToString().PadLeft(5, ' ') + "|" + Bits.getTextShort(txt, mItems[sItems[e.ItemIndex]]));
		}


		//Dark Dawn compatibility
		public void doTableListbox2(Control panel, byte[] textBank, List<int> items)
		{
			pnl = panel;

			tb = new TextBox();
			tb.Width = 240;
			tb.Font = Globals.font;
			pnl.Controls.Add(tb);
			tb.TextChanged += Tb_TextChanged2;

			txt = textBank;
			mItems = items;

			lv = new ListView();
			lv.BackColor = Color.FromArgb(0x08, 0x70, 0x98); lv.ForeColor = Color.White;
			lv.HideSelection = false;
			lv.MultiSelect = false;

			ColumnHeader header = new ColumnHeader();
			header.Text = "";
			header.Name = "col1";
			lv.Columns.Add(header);
			var doubleBufferPropertyInfo = lv.GetType().GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			doubleBufferPropertyInfo.SetValue(lv, true, null);

			lv.Location = new Point(0, 20);
			lv.Size = new Size(240, pnl.Height - 20);
			pnl.Controls.Add(lv);

			lv.Columns[0].Width = 0x7FFF;//lv.Width - 4 - SystemInformation.VerticalScrollBarWidth; //0x7FFF = largest possible.

			lv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; //| AnchorStyles.Right);
			lv.FullRowSelect = true;
			lv.HeaderStyle = ColumnHeaderStyle.None;
			lv.Font = Globals.font;
			lv.View = View.Details;
			lv.VirtualMode = true;
			lv.RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(lv_RetrieveVirtualItem2);
			Tb_TextChanged2(null, null);
		}

		public void Tb_TextChanged2(object sender, EventArgs e)
		{
			sItems = Bits.getTextMatches2(txt, tb.Text, mItems);
			lv.VirtualListSize = sItems.Count;
		}

		private void lv_RetrieveVirtualItem2(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (sItems == null)
			{
				e.Item = new ListViewItem("null");
				return;
			}
			if (sItems[e.ItemIndex] == -1)
			{
				e.Item = new ListViewItem("null2");
				return;
			}
			//e.Item = new ListViewItem(mItems[sItems[e.ItemIndex]]);
			e.Item = new ListViewItem((sItems[e.ItemIndex]).ToString().PadLeft(5, ' ') + "|" + Bits.getText2(txt, mItems[sItems[e.ItemIndex]]));
		}
	}
}
