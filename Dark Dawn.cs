using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //MessageBox

using System.IO;
namespace gsmagic
{
	class Dark_Dawn
	{
		string path = "";
		byte[] header;// = new byte[0x200];
		byte[] fnt; //File Name Table
		byte[] fat; //File Allocation Table
		byte[] ovr; //Overlay entries
		byte[] ram = new byte[0x400000];
		public void init(string aPath)
		{
			path = aPath;
			header = Bits.openFilePart(path, 0, 0x200);
			fnt = Bits.openFilePart(path, Bits.getInt32(header, 0x40), Bits.getInt32(header, 0x44));
			fat = Bits.openFilePart(path, Bits.getInt32(header, 0x48), Bits.getInt32(header, 0x4C));
			ovr = Bits.openFilePart(path, Bits.getInt32(header, 0x50), Bits.getInt32(header, 0x54));
			//Title check?
			loadARM9();
			loadFile(0x154);
			//Overlay File 0x151 / Battle Stuff
			//loadFile(0x181);//0x11AD+1+0x16E);// 0x1383);//0xFC7 + 8); //0x1134);// 0xD07 + 48*8);
			loadFile(0xD07);
			//doTextFile();
			doTextFiles();
			Form dd = new Form();
			dd.Text = "Dark Dawn WIP - " + path;
			dd.Width = 800;
			dd.Height = 600;
			dd.Show();
			dd.Activate();//dd.Focus();
						  //dd.TopMost = true;
						  //dd.ShowDialog();

			TabControl a = new TabControl();
			a.Width = dd.Width;
			a.Height = dd.Height;

			a.Controls.Add(new TabPage("Enemies"));
			//loadEnemies(a);

			TabPage d = new TabPage();
			d.Text = "Djinn";
			a.Controls.Add(d);

			dd.Controls.Add(a);
			loadEnemies(a);
			//File.WriteAllBytes(path + "ram.bin", ram);

			Button s = new Button();
			s.Left = a.Width;// - 100;
			s.Top = 0;
			dd.Controls.Add(s);
			s.MouseClick += new MouseEventHandler(saveClick);
		}
		void saveClick(object sender, MouseEventArgs e)
		{
			loadFile(0); //TEST
			saveFile(0);
		}
		void loadEnemies(TabControl a)
		{
			TabPage d = a.SelectedTab;
			Table_Manager tm = new Table_Manager();
			tm.setTable(ram, 0x17726C, 0x58);
			tm.setPanel(a.SelectedTab);//tabPage6);
			int pnlx = 250; //d.Width / 2 - 150 + 50;
			int pnly = 20; // d.Height / 2 - 125;
						   //d.Text = "Forge";
			List<int> items = new List<int>();// = {5, 6, 7 };
			items.Add(5);
			String[] text = { "" };// "Processed", "Rusted" };

			//tm.doCombo(pnlx + 200, pnly + 20, Bits.textToBytes(text)
			//tm.doTableListbox2(Bits.textToBytes(text), Bits.numList(83));
			tm.doTableListbox2(ram, Bits.numList(366, 221));
			String[] lbl = { "ID", "Level", "Type", "HP", "PP", "Attack", "Defense", "Agility", "Luck", "Turns", "HP Regen", "PP Regen" };
			for (int i = 0; i < 12; i++)
				tm.doLabel(pnlx, pnly + 0 + i * 20, lbl[i]);
			tm.doNud(pnlx + 100, pnly + 0, 0, 32);
			tm.doNud(pnlx + 100, pnly + 20, 4, 8); //Level
			tm.doNud(pnlx + 100, pnly + 40, 5, 8);
			tm.doNud(pnlx + 100, pnly + 60, 6, 16); //HP
			tm.doNud(pnlx + 100, pnly + 80, 8, 16); //PP
			tm.doNud(pnlx + 100, pnly + 100, 10, 16); //Atk
			tm.doNud(pnlx + 100, pnly + 120, 12, 16); //Def
			tm.doNud(pnlx + 100, pnly + 140, 14, 16); //Agl
			tm.doNud(pnlx + 100, pnly + 160, 16, 8); //Lck
			tm.doNud(pnlx + 100, pnly + 180, 17, 8); //Turns
			tm.doNud(pnlx + 100, pnly + 200, 18, 8);
			tm.doNud(pnlx + 100, pnly + 220, 19, 8);

			//Items
			tm.doLabel(pnlx + 220, pnly, "Item");
			tm.doNud(pnlx + 220, pnly + 20, 20, 16);
			tm.doNud(pnlx + 220, pnly + 40, 22, 16);
			tm.doNud(pnlx + 220, pnly + 60, 24, 16);
			tm.doNud(pnlx + 220, pnly + 80, 26, 16);
			tm.doLabel(pnlx + 340, pnly, "Quantity");
			tm.doNud(pnlx + 340, pnly + 20, 28, 8);
			tm.doNud(pnlx + 340, pnly + 40, 29, 8);
			tm.doNud(pnlx + 340, pnly + 60, 30, 8);
			tm.doNud(pnlx + 340, pnly + 80, 31, 8);

			//Element
			//pnlx = ?
			pnly = 280;
			Label ae = tm.doLabel(pnlx, pnly, "Attack Element");
			//ae.Width = 200;
			String[] elements = { "Venus", "Mercury", "Mars", "Jupiter", "Neutral" };
			tm.doCombo(pnlx, pnly + 20, Bits.textToBytes(elements), Bits.numList(5), 0x20, 8);

			tm.doLabel(pnlx + 100, pnly + 60, "Venus");
			tm.doLabel(pnlx + 200, pnly + 60, "Mercury");
			tm.doLabel(pnlx + 300, pnly + 60, "Mars");
			tm.doLabel(pnlx + 400, pnly + 60, "Jupiter");

			tm.doLabel(pnlx, pnly + 80, "Level");
			tm.doLabel(pnlx, pnly + 100, "Power");
			tm.doLabel(pnlx, pnly + 120, "Resist");

			tm.doNud(pnlx + 100, pnly + 80, 0x21, 8);
			tm.doNud(pnlx + 200, pnly + 80, 0x22, 8);
			tm.doNud(pnlx + 300, pnly + 80, 0x23, 8);
			tm.doNud(pnlx + 400, pnly + 80, 0x24, 8);
			//0x25 = Unused?
			tm.doNud(pnlx + 100, pnly + 100, 0x26, 16);
			tm.doNud(pnlx + 100, pnly + 120, 0x28, 16);
			tm.doNud(pnlx + 200, pnly + 100, 0x2A, 16);
			tm.doNud(pnlx + 200, pnly + 120, 0x2C, 16);
			tm.doNud(pnlx + 300, pnly + 100, 0x2E, 16);
			tm.doNud(pnlx + 300, pnly + 120, 0x30, 16);
			tm.doNud(pnlx + 400, pnly + 100, 0x32, 16);
			tm.doNud(pnlx + 400, pnly + 120, 0x34, 16);

		}
		void loadDjinn(TabControl a)//, TabPage d)
		{
			TabPage d = a.SelectedTab;
			Table_Manager tm = new Table_Manager();
			tm.setTable(ram, 0x61AFC, 0x8);
			tm.setPanel(a.SelectedTab);//tabPage6);
			int pnlx = d.Width / 2 - 150 + 100 + 25;
			int pnly = d.Height / 2 - 125;
			//d.Text = "Forge";
			List<int> items = new List<int>();// = {5, 6, 7 };
			items.Add(5);
			String[] text = { "" };// "Processed", "Rusted" };
								   //tm.doCombo(pnlx + 200, pnly + 20, Bits.textToBytes(text)
			tm.doTableListbox(Bits.textToBytes(text), Bits.numList(83));
			tm.doNud(pnlx + 200, pnly + 80, 0, 16);
			tm.doNud(pnlx + 200, pnly + 100, 2, 8);
			tm.doNud(pnlx + 200, pnly + 120, 3, 8);
			tm.doNud(pnlx + 200, pnly + 140, 4, 8);
			tm.doNud(pnlx + 200, pnly + 160, 5, 8);
			tm.doNud(pnlx + 200, pnly + 180, 6, 8);
			tm.doNud(pnlx + 200, pnly + 200, 7, 8);
			//tm.loadEntry();
		}
		void loadARM9()
		{
			int romOffset = Bits.getInt32(header, 0x20);
			//int entryAddr = Bits.getInt32(header, 0x24); //Where execution starts?
			int ramAddr = Bits.getInt32(header, 0x28) & 0x1FFFFFF;
			int size = Bits.getInt32(header, 0x2C);
			using (FileStream a = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				a.Seek(romOffset, SeekOrigin.Begin);
				a.Read(ram, ramAddr, size);
			}
			int endAddr = Bits.getInt32(ram, 0xB88 + 0x14) & 0x1FFFFFF;
			dec1(endAddr);// ramAddr + size);
						  //return null;
		}
		void loadFile(int fileID)
		{
			//if ((fileID < 0) || (fileID >= 0xF000))
			//	return;
			int ovrA = fileID << 5;
			int fatA = fileID << 3; //One of Overlay ID or File ID in ovr should determine this? So update later?
			int fat1 = Bits.getInt32(fat, fatA);
			int fat2 = Bits.getInt32(fat, fatA + 4);
			int size = fat2 - fat1;
			byte[] src = Bits.openFilePart(path, fat1, size); //new byte[size];
			
			//if (fileID == 0)
			//	Bits.saveFile(@"C:\Users\Tea\Desktop\original.bin", src);
			//Bits.saveFilePart(path, fat1, size, des);
			int srcPos = 0;
			byte[] des = ram;
			//int desPos = Bits.getInt32(ovr, ovrA + 4) & 0x1FFFFFF;
			//if (fileID > 0x16D) //Quick hack:  If not overlay file.... then I dunno where to put it yet, so...
			//	desPos = 0x220B80; //Update to base on slot list at 020840D4?
			int desPos = 0x220B80;
			if (fileID <= 0x16D)
				desPos = Bits.getInt32(ovr, ovrA + 4) & 0x1FFFFFF;
			//Sample/example - Some empty text files look like this:
			//40 04 00 00 A0 FF 13 00 00 00
			//In easier to read format: 40 000004 60 FF 001/3 000/0
			//(Note:  -A0 = 60)
			//which decompresses to FFFFFFFF.
			if (src[srcPos++] != 0x40)
				return;
			int decSize = src[srcPos++] | (src[srcPos++] << 8) | (src[srcPos++] << 16);
			if (decSize == 0) { decSize = Bits.getInt32(src, srcPos); srcPos += 4; }
			while (decSize-- > 0)
			{
				int flags = 0x800000 - (src[srcPos++] << 0x18);
				while ((flags << 1) != 0)
				{
					if (flags > 0)
					{
						des[desPos++] = src[srcPos++];
					}
					else
					{
						int len = src[srcPos] & 0xF;
						int dist = (src[srcPos++] >> 4) | (src[srcPos++] << 4);
						if (len == 0)
						{
							if (dist == 0) //Used by default.
								return;
							len = 0x10 + src[srcPos++];
						}
						else if (len == 1)
						{
							if (dist == 0) //May not ever be used, but here for functionality.
								return;
							len = 0x110 + src[srcPos++] + (src[srcPos++] << 8);
						}
						dist = desPos - dist;
						while (len-- > 0)
							des[desPos++] = des[dist++];
					}
					flags <<= 1;
				}
			}
		}
		void saveFile(int fileID)
		{
			if (fileID > 0x16D)
				return;
			int ovrA = fileID << 5;
			int srcStart = Bits.getInt32(ovr, ovrA + 4) & 0x1FFFFFF;
			int srcSize = Bits.getInt32(ovr, ovrA + 8); // & 0x1FFFFFF;
			int srcEnd = srcStart + srcSize;
			int desSize = srcSize + (srcSize >> 3) + 7; //Absolute maximum possible size... (Including header and end of data command.)
			desSize = (desSize + 0x1FF) & -0x200;
			byte[] src = ram;// new byte[srcSize];
			int srcPos = srcStart;
			byte[] des = new byte[desSize];
			int desPos = 0;
			des[desPos++] = 0x40;
			des[desPos++] = (byte)srcSize;
			des[desPos++] = (byte)(srcSize >> 8);
			des[desPos++] = (byte)(srcSize >> 16);
			int fCur = 0x80;
			int fAddr = desPos++;
			int flags = 0;
			while (srcPos < srcEnd)
			{
				int dist = 0;
				int len = 0;
				int winStart = Math.Max(srcStart, srcPos - 0xFFF);
				for (int i = winStart; i < srcPos; i++)
				{
					for (int j = 1; j < 0x10110; j++)
					{
						if (src[srcPos + j] == src[i + j])
						{
							//if ((j + 1) >= len)
							if (j >= len)
							{
								dist = i;
								//len = j + 1;
								len = j;
							}
						}
						else
							break;
					}
					if (src[srcPos] == src[i])
					{
						len += 1;
					}
					else
					{
						dist += 1;
					}
				}
				if (src[srcPos] != src[dist]) //Insert byte
				{
					des[desPos++] = src[srcPos++];
					fCur >>= 1;
					if (fCur == 0)
					{
						des[fAddr] = (byte)-flags;
						fAddr = desPos++;
						fCur = 0x80;
						flags = 0;
					}
				}
				dist = srcPos - dist;
				if (len < 2)
				{
					des[desPos++] = src[srcPos]; len = 1;
				}
				else if (len < 0x10)
				{
					des[desPos++] = (byte)((dist << 4) | len);
					des[desPos++] = (byte)(dist >> 4);
				}
				else if (len < 0x110)
				{
					des[desPos++] = (byte)((dist << 4) | 0);
					des[desPos++] = (byte)(dist >> 4);
					des[desPos++] = (byte)(len - 0x10);
				}
				else // if (len < 0x10110)
				{
					des[desPos++] = (byte)((dist << 4) | 1);
					des[desPos++] = (byte)(dist >> 4);
					des[desPos++] = (byte)(len - 0x110);
					des[desPos++] = (byte)((len - 0x110) >> 8);
				}
				srcPos += len;
				if (len > 1)
					flags |= fCur;

				fCur >>= 1;
				if (fCur == 0)
				{
					des[fAddr] = (byte)-flags;
					fAddr = desPos++;
					fCur = 0x80;
					flags = 0;
				}
			}
			des[desPos++] = 0;
			des[desPos++] = 0;
			flags |= fCur;
			des[fAddr] = (byte)-flags;

			//Now to save to ROM.
			Bits.saveFile(@"C:\Users\Tea\Desktop\notoriginal.bin", des);
			return;
			int fatA = fileID << 3;
			int fat1 = Bits.getInt32(fat, fatA);
			Bits.setInt32(fat, fatA + 4, fat1 + desPos);
			//int fat2 = Bits.getInt32(fat, fatA + 4);
			int fat2 = Bits.getInt32(fat, fatA + 8);
			int size = fat2 - fat1;
			//For neatness....
			while (desPos < size)
			{
				des[desPos++] = 0xFF;
			}
			if (desPos != size)
			{
				MessageBox.Show("About to lose data... Have fun!");
			}
			Bits.saveFilePart(path, fat1, size, des);
		}
		void dec1(int addr)//, int size) //02000950.
		{
			//if (addr == 0)
			//	return;
			//addr &= 0x1FFFFFF;

			int h1 = Bits.getInt32(ram, addr - 8);
			int h2 = Bits.getInt32(ram, addr - 4);
			h2 += addr; //h2=destAddr
			int r3 = addr - (h1 >> 0x18); //r3=srcAddr
			h1 &= 0xFFFFFF;
			h1 = addr - h1; //h1=Where to stop decompressing.

			newset:
			if (r3 <= h1)
				return;
			int r5 = ram[--r3]; //r5=flags
			int r6 = 0x8;

			nextb:
			if (--r6 < 0)
				goto newset;
			
			if ((r5 & 0x80) == 0) //Constant / Distance/Length pair
			{
				ram[--h2] = ram[--r3];
			}
			else
			{
				int r12 = ram[--r3];
				int r7 = ram[--r3];
				r7 = (((r12 << 8) | r7) & 0xFFF) + 2;
				r12 += 0x20;
				do
				{
					ram[h2 - 1] = ram[h2 + r7]; h2--;
					r12 -= 0x10;
				} while (r12 >= 0);
			}
			r5 <<= 1;
			if (r3 > h1)
				goto nextb;
		}

		//void dec2()
		//0x16E
		void doTextFiles()
		{
			//0xBF7 <0x13D2
			for (int i = 0xBF7; i < 0x13D2; i++)
			{
				fileid = i;
				loadFile(i);
				doTextFile();
			}
		}
		int fileid = 0;
		void doTextFile()
		{
			int address = 0x220B80;// 0x23C310;//int address = 0x17726C;
			System.Text.StringBuilder strbuild = new System.Text.StringBuilder(0x1000);

			//int entries = 130;
			while (true)
			{
				int a = Bits.getInt32(ram, address);
				if (a == -1) break;
				getText(strbuild, ram, address);
				strbuild.AppendLine();
				address += 4;
			}
			//System.IO.File.WriteAllText(@"C:\Users\Tea\Desktop\text.txt", strbuild.ToString());
			string strbstr = strbuild.ToString();
			if (strbstr.Length < 1)
				return;
			//System.IO.File.WriteAllText("C:\\Users\\Tea\\Desktop\\msg\\" + strbstr.Substring(0, 8) + ".txt", strbstr.ToString());
			string fn = getFilename(fileid);
			System.IO.File.WriteAllText("C:\\Users\\Tea\\Desktop\\msg\\" + fn, strbstr.ToString());
		}
		string getFilename(int i)
		{
			string str = "";
			//int addr = Bits.getInt32(fnt, (i - 0x16D) * 4);
			int addr = Bits.getInt32(fnt, 0x140); //File 0xBF7+
			//int i2 = 0xBF7;
			for (int i2 = 0xBF7; i2 < i; i2++)
			{
				addr += (fnt[addr] & 0x7F) + 1;
			}
			for (int size = fnt[addr++] & 0x7F; size > 0; size--)
			{
				str += (char)fnt[addr++];
			}
			return str;
		}
		void getText(System.Text.StringBuilder strbuild, byte[] buffer, int address)
		{
			int addr2 = Bits.getInt32(buffer, address) + 0x220B80;
			int length = 2000; int pos = addr2 & 0xFFFFFF;
			//MessageBox.Show(addr2.ToString("x8"));
			while (length-- > 0)
			{
				int c = Bits.getInt16(buffer, pos); pos += 2;
				if (c == 0)
					break;
				if ((c < 0x100) && (c > 0xF))
					strbuild.Append((char)c);
				else
					strbuild.Append("["+c.ToString("X4")+"]");
			}
		}
}
}
