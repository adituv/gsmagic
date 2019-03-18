using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gsmagic {
    public partial class Editors : Form {
        public Editors() {
            InitializeComponent();
        }
        int[] list = null; //Call save with this.
        public Form1 test;
        //public byte[] rom;// = Form1.rom;
        public byte[] txt;
		private System.Windows.Forms.TabControl tabControl01;
		subeditor[] subEditor = null;//{ new eClass() };
		private void Editors_Load(object sender, EventArgs e) {
			Globals.editorsForm = this;
			System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
			txt = Comp.decompText(test.rom);
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			//this.tabControl1.Controls.Add(this.tabPage1);
			//this.tabControl1.Controls.Add(this.tabPage2);
			//this.tabControl1.Controls.Add(this.tabPage3);
			//this.tabControl1.Controls.Add(this.tabPage4);
			//this.tabControl1.Controls.Add(this.tabPage5);
			//this.tabControl1.Controls.Add(this.tabPage6);
			//this.tabControl1.Controls.Add(this.tabPage7);
			//this.tabControl1.Controls.Add(this.tabPage8);
			//this.tabControl1.Controls.Add(this.tabPage9);
			//this.tabControl1.Controls.Add(this.tabPage10);
			this.tabControl1.Location = new System.Drawing.Point(1, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(784, 558);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabIndexChange);
			// 
			// tabPage1
			// 
			//this.tabPage1.Controls.Add(this.checkedListBox2);
			//this.tabPage1.Controls.Add(this.checkedListBox1);
			//this.tabPage1.Controls.Add(this.listBox1);
			subeditor[] subEditor = { new eClass() };//subEditor = new eClass();
			tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(776, 532);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = subEditor[0].text();
			this.tabPage1.UseVisualStyleBackColor = true;
			
			//tabPage8.SuspendLayout();
//			initRandEnctrs(); //Make sure TextEditorLoad stays before this... (for now)
//			initEleTable(); //Quick test for now!
//			initForgeTable();
//			initClassTable();
			//tabControl1.SelectedIndex = 1;
			//tabPage8.ResumeLayout();
			//DateTime a = DateTime.Now;
			//byte[] des = new byte[0x8000]; int b = 0;
			//for (int i = 0x680000 + (1609 * 4); i < 0x680000 + (1723 * 4); i += 4) {
			//    Comp.decompress(test.rom, Bits.getInt32(test.rom, i) & 0x1FFFFFF, des, 0); b++;
			//}
			//Console.WriteLine(DateTime.Now - a + "   " + b); //~0.018-0.020
			sw.Stop();

			Console.WriteLine(sw.Elapsed + " (Editors Load)"); //Slow because of comboboxes AddRange?
		}
		int tabFlags = 0;
		
		private void tabIndexChange(object sender, EventArgs e)
		{
			//Next three lines b/c I don't want to dispose anything, or renew anything? :P (Also keeps selected items where they are until you close the form.)
			if ((tabFlags & (1 << ((TabControl)sender).SelectedIndex)) != 0)
				return;
			tabFlags |= (1 << ((TabControl)sender).SelectedIndex);
			switch (((TabControl)sender).SelectedIndex)
			{
				case 7:
					initRandEnctrs();
					break;
				case 6:
					initEleTable();
					break;
				case 5:
					initForgeTable();
					break;
				case 4:
					//initClassTable();
					subeditor a = new eClass();
					a.load(tabControl1);
					break;
				case 1:
					texteditor();
					break;
			}
		}
		public void initForgeTable() {
			Table_Manager tm = new Table_Manager();
			tm.setTable(test.rom, 0x10CC34, 0x24);
			tm.setPanel(tabControl1.SelectedTab);//tabPage6);
			tabPage6.Text = "Forge";
			
			//ListBox lstb = tm.doTableListbox();
			////607
			int ind3 = 0;
			List<int> items = new List<int>();
			while (true) //for (int ind3 = 0; ind3 < 0x14; ind3++)
			{
				//int ind2 = Bits.getInt16(test.rom, 0xEDACC + 4 + ind3 * 0x1C);
				int ind = Bits.getInt16(test.rom, 0x10CC34 + ind3 * 0x24);
				if (ind == 0xFFFF) { break; }
				//StringBuilder str = new StringBuilder(0x200);
				//str.Append((ind3).ToString().PadLeft(3, ' ') + "|");
				//items.Add((ind3).ToString().PadLeft(3, ' ') + "|" + getTextStrShort(607 + ind));
				items.Add(607 + ind);
				ind3++;
			}
			tm.doTableListbox(txt, items);// items);
			String[] itemNames = new String[461];
			for (int ind2 = 0; ind2 < 461; ind2++)
			{
				itemNames[ind2] = ind2.ToString().PadLeft(3, ' ') + "|" + Bits.getTextShort(txt, 607 + ind2);
			}
			int pnlx = tabPage8.Width / 2 - 150 + 100 + 25;
			int pnly = tabPage8.Height / 2 - 125;
			Label fi = tm.doLabel(pnlx, pnly, "Forgeable Item");
			//fi.Width = 200;
			tm.doCombo(pnlx, pnly + 20, Bits.textToBytes(itemNames), Bits.numList(461), 0, 16);
			tm.doLabel(pnlx + 200, pnly, "Just text?");
			String[] text = { "Processed", "Rusted" };
			tm.doCombo(pnlx + 200, pnly + 20, Bits.textToBytes(text), Bits.numList(2), 2, 16);
			//fi.Width = 200;
			//tm.doNud(pnlx+ 200, pnly + 20, 2, 16);

			tm.doLabel(pnlx, pnly+60, "Item");
			tm.doLabel(pnlx + 200, pnly+60, "Rate");

			tm.doCombo(pnlx, pnly + 80, Bits.textToBytes(itemNames), Bits.numList(461), 4, 16);
			tm.doCombo(pnlx, pnly + 100, Bits.textToBytes(itemNames), Bits.numList(461), 6, 16);
			tm.doCombo(pnlx, pnly + 120, Bits.textToBytes(itemNames), Bits.numList(461), 8, 16);
			tm.doCombo(pnlx, pnly + 140, Bits.textToBytes(itemNames), Bits.numList(461), 10, 16);
			tm.doCombo(pnlx, pnly + 160, Bits.textToBytes(itemNames), Bits.numList(461), 12, 16);
			tm.doCombo(pnlx, pnly + 180, Bits.textToBytes(itemNames), Bits.numList(461), 14, 16);
			tm.doCombo(pnlx, pnly + 200, Bits.textToBytes(itemNames), Bits.numList(461), 16, 16);
			tm.doCombo(pnlx, pnly + 220, Bits.textToBytes(itemNames), Bits.numList(461), 18, 16);

			tm.doNud(pnlx + 200, pnly + 80, 20, 16);
			tm.doNud(pnlx + 200, pnly + 100, 22, 16);
			tm.doNud(pnlx + 200, pnly + 120, 24, 16);
			tm.doNud(pnlx + 200, pnly + 140, 26, 16);
			tm.doNud(pnlx + 200, pnly + 160, 28, 16);
			tm.doNud(pnlx + 200, pnly + 180, 30, 16);
			tm.doNud(pnlx + 200, pnly + 200, 32, 16);
			tm.doNud(pnlx + 200, pnly + 220, 34, 16);

			//lstb.SelectedIndex = 0;
		}
		private void initEleTable() {
			Table_Manager tm = new Table_Manager();
			tm.setTable(test.rom, 0xC6684, 0x18);
			tm.setPanel(tabControl1.SelectedTab);//tabPage7);
			tabPage7.Text = "Elemental Data";

			//ListBox lstb = tm.doTableListbox();
			List<int> items = new List<int>();
			//Determine what should go in table's Listbox
			int[] elni = new int[48];
			for (int ind3 = 0; ind3 < 379; ind3++)
			{
				if (elni[test.rom[0xB9E7C + ind3 * 0x4C + 0x2A]] == 0)
				{
					elni[test.rom[0xB9E7C + ind3 * 0x4C + 0x2A]] = ind3+1;
				}
			}
			//Populate the Listbox
			for (int ind3 = 0; ind3 < 48; ind3++) {

				if (elni[ind3] == 0) {
					//items.Add((ind3).ToString().PadLeft(3, ' ') + "|");
					items.Add(-1);
					continue;
				}
				//items.Add((ind3).ToString().PadLeft(3, ' ') + "|" + getTextStrShort(1068 + elni[ind3]-1));
				items.Add(1068 + elni[ind3] - 1);
			}
			tm.doTableListbox(txt, items);

			int pnlx = tabPage8.Width / 2 - 150 + 25;// + 100;
			int pnly = tabPage8.Height / 2 - 125;

			Label ae = tm.doLabel(pnlx, pnly, "Attack Element");
			//ae.Width = 200;
			String[] elements = { "Venus", "Mercury", "Mars", "Jupiter", "Neutral" };
			tm.doCombo(pnlx, pnly + 20, Bits.textToBytes(elements), Bits.numList(5), 0, 32);

			tm.doLabel(pnlx+100, pnly+60, "Venus");
			tm.doLabel(pnlx+200, pnly+60, "Mercury");
			tm.doLabel(pnlx+300, pnly+60, "Mars");
			tm.doLabel(pnlx+400, pnly+60, "Jupiter");

			tm.doLabel(pnlx, pnly+80, "Level");
			tm.doLabel(pnlx, pnly+100, "Power");
			tm.doLabel(pnlx, pnly+120, "Resist");

			tm.doNud(pnlx + 100, pnly + 80, 4, 8);
			tm.doNud(pnlx + 200, pnly + 80, 5, 8);
			tm.doNud(pnlx + 300, pnly + 80, 6, 8);
			tm.doNud(pnlx + 400, pnly + 80, 7, 8);

			tm.doNud(pnlx + 100, pnly + 100, 8, 16);
			tm.doNud(pnlx + 100, pnly + 120, 10, 16);
			tm.doNud(pnlx + 200, pnly + 100, 12, 16);
			tm.doNud(pnlx + 200, pnly + 120, 14, 16);
			tm.doNud(pnlx + 300, pnly + 100, 16, 16);
			tm.doNud(pnlx + 300, pnly + 120, 18, 16);
			tm.doNud(pnlx + 400, pnly + 100, 20, 16);
			tm.doNud(pnlx + 400, pnly + 120, 22, 16);

			//lstb.SelectedIndex = 0;
		}
		//Random Encounters
		private void initRandEnctrs() {
			Table_Manager tm = new Table_Manager();
			tm.setTable(test.rom, 0xEDACC, 0x1C);
			tm.setPanel(tabControl1.SelectedTab);//tabPage8);
			tabPage8.Text = "Encounters";

			List<int> items = new List<int>();
			//ListBox lstb = tm.doTableListbox();
			for (int ind3 = 0; ind3 < 0x6E; ind3++)
			{
				int ind2 = Bits.getInt16(test.rom, 0xEDACC + 4 + ind3 * 0x1C);
				int ind = Bits.getInt16(test.rom, 0x12CE7C + ind2 * 0x18);
				//StringBuilder str = new StringBuilder(0x200);
				//str.Append((ind3).ToString().PadLeft(3, ' ') + "|");
				//items.Add((ind3).ToString().PadLeft(3, ' ') + "|" + getTextStrShort(1068 + ind));
				items.Add(1068 + ind);
			}
			tm.doTableListbox(txt, items); //items);

			int pnlx = tabPage8.Width / 2 - 150 + 100 + 25;
			int pnly = tabPage8.Height / 2 - 125;

			tm.doLabel(pnlx, pnly, "Frequency");
			tm.doNud(pnlx, pnly + 20, 0, 16);
			tm.doLabel(pnlx + 200, pnly, "Target Level");
			tm.doNud(pnlx + 200, pnly + 20, 2, 16);
			

			tm.doLabel(pnlx, pnly + 60, "Group");
			tm.doLabel(pnlx + 200, pnly + 60, "Rate");
			String[] enGroups = new String[660];
			for (int ind2 = 0; ind2 < 660; ind2++)
			{
				enGroups[ind2] = ind2.ToString().PadLeft(3, ' ') + "|" + Bits.getTextShort(txt, 1068 + Bits.getInt16(test.rom, 0x12CE7C + ind2 * 0x18));
			}
			tm.doCombo(pnlx, pnly + 80, Bits.textToBytes(enGroups), Bits.numList(660), 4,16);
			tm.doCombo(pnlx, pnly + 100, Bits.textToBytes(enGroups), Bits.numList(660), 6,16);
			tm.doCombo(pnlx, pnly + 120, Bits.textToBytes(enGroups), Bits.numList(660), 8,16);
			tm.doCombo(pnlx, pnly + 140, Bits.textToBytes(enGroups), Bits.numList(660), 10,16);
			tm.doCombo(pnlx, pnly + 160, Bits.textToBytes(enGroups), Bits.numList(660), 12,16);
			tm.doCombo(pnlx, pnly + 180, Bits.textToBytes(enGroups), Bits.numList(660), 14,16);
			tm.doCombo(pnlx, pnly + 200, Bits.textToBytes(enGroups), Bits.numList(660), 16,16);
			tm.doCombo(pnlx, pnly + 220, Bits.textToBytes(enGroups), Bits.numList(660), 18,16);

			tm.doNud(pnlx + 200, pnly + 80,20,8);
			tm.doNud(pnlx + 200, pnly + 100,21,8);
			tm.doNud(pnlx + 200, pnly + 120,22,8);
			tm.doNud(pnlx + 200, pnly + 140,23,8);
			tm.doNud(pnlx + 200, pnly + 160,24,8);
			tm.doNud(pnlx + 200, pnly + 180,25,8);
			tm.doNud(pnlx + 200, pnly + 200,26,8);
			tm.doNud(pnlx + 200, pnly + 220,27,8);

			//lstb.SelectedIndex = 0;
		}
		private void dispAllIcons() {
            DateTime a = DateTime.Now;
            //this.Show();
            //byte[] img = Comp.decompress16All(test.rom, 0x4eb58);//Comp.decompress16(test.rom, 0x0804F128 & 0x1FFFFFF, 0);
            //byte[] img = Comp.decompress16All(test.rom, 0x54A14);
            //byte[] img = Comp.decompress16All(test.rom, 0x58ff4);
            //Will load pal from ROM later.
            //int[] pal = { 0x0000, 0x0000, 0x001F, 0x01DF, 0x03FF, 0x03E0, 0x1DC7, 0x7FE0, 0x7C00, 0x59DC, 0x3ADF, 0x01D6, 0x000E, 0x1CE7, 0x4E73, 0x7FFF };
            int[] pal = { 0x000000, 0x000000, 0xf80000, 0xf87000, 0xf8f800, 0x00f800, 0x387038, 0x00f8f8, 0x0000f8, 0xE070B0, 0xF8B070, 0xb07000, 0x700000, 0x383838, 0x989898, 0xf8f8f8 };
            //int[] pal = null;//{ 0x00ffffff }; //, 0x7fffffff,0x7fffffff, 0x7fffffff };
            int tilePal = 0;//(tile >> 8) & 0xF0;
            int scnw = 16*32, scnh = 16*32, t2 = 0, row=0;
            int[] bmpdata = new int[scnw*scnh];//img.Length];
            int[] ilist = { 0x4eb58, 0x54A14, 0x58ff4 };
            for (int i = 0; i < 3; i++) {
                byte[] img = Comp.decompress16All(test.rom, ilist[i]);
                for (int r = row; r < scnh; r += 16) {
                    row += 16;
                    for (int c = 0; c < scnw; c += 16) {
                        if (t2 >= img.Length) { goto nextList; }
                        for (int y = r; y <= r + 15; y++) {
                            int ry = y;// if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
                            //int pix = ram.getInt32(pos); pos += 4;
                            for (int x = c; x <= c + 15; x++) { //x += 2) {
                                int rx = x; //if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
                                int pix = img[t2++];
                                int pix2 = pix & 0xF;
                                if (pix2 != 0) {
                                    bmpdata[(ry * scnw) + rx] = pal[tilePal | (pix2)];
                                    //priomap[(ry * bitmapWidth) + rx] = priority;
                                }
                                //pix >>= 4;
                                //rx = x + 1; //if ((tile & 0x400) != 0) { rx = c + ((7 - (x + 1)) & 7); } //Horr. flip
                                //pix2 = pix >> 4;
                                //if (pix2 != 0) {
                                //    bmpdata[(ry * scnw) + rx] = pal[tilePal | (pix2)];
                                //    //priomap[(ry * bitmapWidth) + rx] = priority;
                                //}
                                //pix >>= 4;
                            }
                        }
                    }
                }
            nextList: t2 = 0; row += 16;
            }
            //Add image to picbox.
            //System.IO.File.WriteAllBytes("C:/Users/Tea/Desktop/imgtest.dmp", img);
            pictureBox1.Image = Form1.PixelsToImage(bmpdata, scnw, scnh);
            Console.WriteLine(DateTime.Now - a);
        }
		
		private void texteditor() {
			TabPage tp = tabControl1.SelectedTab;
			int w = tp.Width;
			int h = tp.Height;
			Table_Manager tm = new Table_Manager();
			tm.setPanel(tp);
			tp.Text = "Text Editor";
			tm.doTableListbox(txt, Bits.numList(12461));
			tm.sl.tb.Bounds = new Rectangle(0, 120, w, 20);
			tm.sl.lv.Bounds = new Rectangle(0, 140, w, h - 140);

			DataTextbox dtb = tm.doNamebox(0, 0, txt, 0);
			dtb.tbx.Bounds = new Rectangle(0, 0, w - 100, 120);
			dtb.tbx.Multiline = true;
			dtb.bn.Bounds = new Rectangle(w - 100, 0, 100, 120);
		}
    }
}
