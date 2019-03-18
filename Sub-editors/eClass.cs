using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace gsmagic//.Sub_editors
{
	class eClass : subeditor
	{
		Form1 test = Globals.mainForm;
		byte[] txt = Globals.editorsForm.txt;
		//new string text = "Classes";
		public override string text()
		{
			return "Classes";
		}
		public override void load(TabControl tabControl1)
		{
			Table_Manager tm = new Table_Manager();
			tm.setTable(test.rom, 0xC15F4, 0x54);
			tm.setPanel(tabControl1.SelectedTab);// tabPage5);
			int w = tabControl1.SelectedTab.Width;
			int h = tabControl1.SelectedTab.Height;

			//class Type Labels
			//List<int> ctlbl = new List<int>();
			//Shouldn't need more than 256 types if not doing code hacks to increase the number of classes. (e.g. Class index is saved in 8-bit.)
			//var ctlbl = Enumerable.Repeat(0, 256).ToList(); //If using a List.
			var ct = new int[0x100];
			for (int i = 2915 - 2915; i < 3159 - 2915; i++)
			{
				int n = Bits.getInt32(test.rom, 0xC15F4 + i * 0x54);
				if (n >= 0x100)
					continue;
				if (ct[n] == 0) //{ }
					ct[n] = 2915 + i;
			}
			var ctstr = new string[0x100];//new List<String>();//
			for (int i = 0; i < 0x100; i++) //{ }
				if (ct[i] != 0)
					ctstr[i] = (i.ToString().PadLeft(3, ' ') + "|" + Bits.getTextShort(txt, ct[i]));
				else
				{
					ct[i] = -1;
					ctstr[i] = i.ToString().PadLeft(3, ' ') + "|";
				}

			//2915
			//List<String> items = new List<String>();
			//for (int i = 2915; i < 3159; i++)
			//{
			//	items.Add((i - 2915).ToString().PadLeft(3, ' ') + "|" + getTextStrShort(i));
			//}
			List<int> items = new List<int>();
			for (int i = 0; i < 3159 - 2915; i++)
			{
				items.Add(2915 + i);
			}
			tm.doTableListbox(txt, items);// items);

			int pnlx = w / 2 - 200 + 100 + 25;
			int pnly = 20 + 100;// tabPage8.Height / 2 - 125;
			pnly += 20;
			tm.doNamebox(pnlx, pnly - 30, txt, 2915);

			String[] psyNames = new String[734];
			for (int ind2 = 0; ind2 < 734; ind2++)
			{
				psyNames[ind2] = ind2.ToString().PadLeft(3, ' ') + "|" + Bits.getTextShort(txt, 1447 + ind2);
			}

			Label fi = tm.doLabel(pnlx, pnly, "Class Type");
			//tm.doNud(pnlx + 100, pnly, 0, 32);
			tm.doCombo(pnlx + 100, pnly, txt, ct.ToList(), 0, 32);
			pnly -= 30;
			//fi = tm.doLabel(pnlx, pnly + 60, "Elemental Level Requirements");
			//fi.Width = 500;
			string[] eNames = { "Venus", "Mercury", "Mars", "Jupiter" };
			for (int i = 0; i < 4; i++)
			{
				tm.doLabel(pnlx + i * 100, pnly + 60, eNames[i] + " Lv.");
				tm.doNud(pnlx + i * 100, pnly + 80, 4 + i, 8);
			}

			string[] percents = new string[0x100];
			for (int i = 0; i < 0x100; i++)
			{
				percents[i] = (i * 10) + "%";
			}
			pnlx -= 50;
			tm.doLabel(pnlx, pnly + 200 - 80, "HP:");
			Control combo = tm.doCombo(pnlx + 80, pnly + 200 - 80, Bits.textToBytes(percents), Bits.numList(0x100), 0x8, 8);
			combo.Width = 70;
			tm.doLabel(pnlx, pnly + 200 - 60, "PP:");
			combo = tm.doCombo(pnlx + 80, pnly + 200 - 60, Bits.textToBytes(percents), Bits.numList(0x100), 0x9, 8);
			combo.Width = 70;

			tm.doLabel(pnlx + 150 + 10, pnly + 200 - 80, "Attack:");
			combo = tm.doCombo(pnlx + 150 + 80 + 10, pnly + 200 - 80, Bits.textToBytes(percents), Bits.numList(0x100), 0xA, 8);
			combo.Width = 70;
			tm.doLabel(pnlx + 150 + 10, pnly + 200 - 60, "Defense:");
			combo = tm.doCombo(pnlx + 150 + 80 + 10, pnly + 200 - 60, Bits.textToBytes(percents), Bits.numList(0x100), 0xB, 8);
			combo.Width = 70;

			tm.doLabel(pnlx + 300 + 20, pnly + 200 - 80, "Agility:");
			combo = tm.doCombo(pnlx + 300 + 80 + 20, pnly + 200 - 80, Bits.textToBytes(percents), Bits.numList(0x100), 0xC, 8);
			combo.Width = 70;
			tm.doLabel(pnlx + 300 + 20, pnly + 200 - 60, "Luck:");
			combo = tm.doCombo(pnlx + 300 + 80 + 20, pnly + 200 - 60, Bits.textToBytes(percents), Bits.numList(0x100), 0xD, 8);
			combo.Width = 70;

			pnlx += 50;
			tm.doLabel(pnlx - 50, pnly + 200 - 20, "Level");
			tm.doLabel(pnlx + 60 - 50, pnly + 200 - 20, "Ability");
			for (int i = 0; i < 8; i++)
			{
				Control lv = tm.doNud(pnlx - 50, pnly + 200 + i * 20, 0x10 + 2 + i * 4, 8);
				lv.Width = 60;
				tm.doCombo(pnlx + 60 - 50, pnly + 200 + i * 20, Bits.textToBytes(psyNames), Bits.numList(734), 0x10 + 0 + i * 4, 16);
			}
			tm.doLabel(pnlx + 200, pnly + 200 - 20, "Level");
			tm.doLabel(pnlx + 60 + 200, pnly + 200 - 20, "Ability");
			for (int i = 0; i < 8; i++)
			{
				Control lv = tm.doNud(pnlx + 200, pnly + 200 + i * 20, 0x10 + 2 + (8 + i) * 4, 8);
				lv.Width = 60;
				tm.doCombo(pnlx + 60 + 200, pnly + 200 + i * 20, Bits.textToBytes(psyNames), Bits.numList(734), 0x10 + 0 + (8 + i) * 4, 16);
			}
			Label lbl = tm.doLabel(pnlx, pnly + 370, "Effect Weaknesses (+25% chance for non-flat effects.)");
			lbl.Width = 400;
			for (int i = 0; i < 3; i++)
			{
				tm.doNud(pnlx + i * 100, pnly + 390, 0x50 + i, 8);
			}
			pnly = 0; //pnly -= 110;
					  //pnlx += 5;
					  //Class Type Chart
			Table_Manager tm2 = new Table_Manager();
			tm2.setTable(test.rom, 0xC6604, 0x40);
			tm2.setPanel(tabControl1.SelectedTab); //tabPage5);
			for (int i = 0; i < 4; i++)
				tm2.doLabel(pnlx + (i * 115), pnly, eNames[i]);
			for (int j = 0; j < 4; j++)
				tm2.doLabel(pnlx - 70, pnly + j * 20 + 20, eNames[j]);
			pnly += 20;
			//for (int j = 0; j < 4; j++)
			//	for (int i = 0; i < 4; i++)
			//		tm2.doNud(pnlx + (i * 100), pnly + j * 20, (j * 4 + i) * 4, 32);
			for (int j = 0; j < 4; j++)
				for (int i = 0; i < 4; i++)
				{
					Control ctrl = tm2.doCombo(pnlx + (i * 115), pnly + j * 20, txt, ct.ToList(), (j * 4 + i) * 4, 32);
					ctrl.Width = 115;
				}
			tm2.loadEntry(null, null);
		}
	}
}
