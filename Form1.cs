using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; //Bitmap /May later need for Rectangle as well.
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using System.Drawing; //Bitmap/Rectangle
using System.Drawing.Imaging; //PixelFormat
using System.Runtime.InteropServices; //Marshal

namespace gsmagic {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}
		string sayHi() { //WIP - Can be used for alerts/etc. but only used for form's title bar at the moment.
			string[] greeting = {
				"Hi", "Hey", "Hello"
			};
			int rng = new Random().Next(3);
			return greeting[rng] + " " + Environment.UserName + "!";
		}
		void creativeTitle() {
			string[] greeting = {
				sayHi(), //Used for default message. (After the below messages are done.) And given the rng, it's not the same everytime!
				"Hello! Welcome to GSM! Nice to meet you!",
				"Hey there! What's your name again? Was it {0}? Welcome back!",
				"Hey {0}! I think I finally got your name now. What's up?",
				"Hey {0}! How's the hacking going?",

				"You sure do love to hack, don't you {0}?"

			};
			int runs = Properties.Settings.Default.Runs;
			if (runs < 20000) runs += 1;
			Properties.Settings.Default.Runs = runs;
			Properties.Settings.Default.Save();
			if (runs >= greeting.Length) { runs = 0; }//-= 1;
			Text = "GSM: " + string.Format(greeting[runs], Environment.UserName);
		}
		//Form load > Open/Save ROM
		private void Form1_Load(object sender, EventArgs e) {
			// An attempt to find out how many random seeds there are.... but it didn't work.
			//byte[] a = new byte[0x20000000];
			//ulong seed = 0;
			//int num = 0;
			////a[seed] = (uint)(a[seed] & ~((uint)1 << ((int)seed & 7)) | ((uint)1 << ((int)seed & 7));
			//while (((a[seed >> 3] >> (int)(seed & 7)) & 1) == 0)
			//{
			//	a[seed >> 3] |= (byte)(1 << ((int)(seed & 7)));
			//	seed = (uint)((ulong)1103515245 * seed + 12345);
			//	num++;
			//}
			//MessageBox.Show(num.ToString("X8") + "   " + seed.ToString("X8"));
			//return;
			Globals.mainForm = this; //Makes it easy to reference this form everywhere. (Not sure of best method?)
									 //Dark_Dawn test = new Dark_Dawn();
									 //test.extract();

			//Console.WriteLine("UserName: {0}", Environment.UserName);
			//Console.WriteLine(System.Windows.Forms.SystemInformation.UserName);
			//MessageBox.Show((unchecked((ulong)-1)<<0x40).ToString("X16"));
			//byte[] ba = {7,7,7,7};
			//System.Media.SoundPlayer test = new System.Media.SoundPlayer(new MemoryStream(ba));
			//test.
			if (System.IO.File.Exists(Properties.Settings.Default.LastRom)) { OpenROM(Properties.Settings.Default.LastRom); } else { OpenROMdialogue(); }
			creativeTitle();
			//DateTime a = DateTime.Now;//.Ticks;
			//byte[] t = decompress(rom, 0xA8B1B8, 0); //Format 2
			//byte[] t = decompress(rom, 0x888FE4 + 0x80, 0); //Format 2
			//byte[] t = decompress16(rom, 0x4F157, 0); //Format 2
			//byte[] t = decompText(rom, 0x111); // decompBtlBG(rom, 0x7765D8 + 0x100);
			//byte[] t = decompText(rom);
			//comptext(t, rom);;
			//Editors a = new Editors();
			//a.Show();
			//a.tabControl = 0;
			//a.rom = rom;
			//DateTime c = DateTime.Now;//.Ticks;
			//Console.WriteLine((c - a).ToString());
			//System.IO.File.WriteAllBytes("C:/Users/Tea/Desktop/gsdtest.dmp", t);
			//DateTime a = DateTime.Now;//.Ticks;
			//loadEntireMap();
			//DateTime c = DateTime.Now;//.Ticks;
			//Console.WriteLine((c - a).ToString());
			//System.IO.File.WriteAllBytes("C:/Users/Tea/Desktop/gsdtest2.dmp", decompText(rom));
		}
		private void OpenROMdialogue() {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open a GBA/NDS File";
			ofd.Filter = "GBA/NDS/3DS file(*.gba;*.nds;*.3ds)|*.gba;*.nds;*.3ds";
			if (ofd.ShowDialog() == DialogResult.OK) { OpenROM(ofd.FileName); }
		}
		//public static Bits rom = new Bits();
		//public static Bits ram = new Bits();
		//Public OpenFileDialog1 = ""
		public byte[] rom;
		string romfn;
		int version = -1;
		int filetable = -1;
		char language;
		private void OpenROM(string filename) { //TODO: If new filename is error/incompatible, revert to old file.
												//mainMem.setPath(filename)
												//string vstr = ""; //Dim vstr As String = ""
			if (filename.EndsWith(".nds", true, null))
			{
				//Dark_Dawn a = new Dark_Dawn();
				new Dark_Dawn().init(filename);
			}
			else if (filename.EndsWith(".gba", true, null)) { //Load entire ROM to buffer; GBA ROMs are maxed at 32 MB.
				rom = Bits.openFile(filename);
				if (rom == null) { this.Text = "Error reading ROM file."; return; }
				/*
				HexEditor hx = new HexEditor();
				hx.init();
				hx.disp();
				*/
				//rom.seek(0xA0);
				//vstr = Bits.getString(rom, 0xA0, 16);
				//ram.buffer = new byte[0x40000];
				switch (Bits.getString(rom, 0xA0, 15)) {
					case "Golden_Sun_AAGS": //(U) GS1
					case "GOLDEN_SUN_AAGS": //Italy GS1
					case "OugonTaiyo_AAGS": //(J) GS1
						version = 0; filetable = 0x08320000; break;
					case "GOLDEN_SUN_BAGF":
					case "OUGONTAIYO_BAGF":
						version = 1; filetable = 0x08680000; break;
					case "MARIOTENNISABTM":
						version = 11; filetable = 0x08C28000; break;
					case "MARIOGOLFGBABMG":
						version = 10; filetable = 0x08800000; break;
						//default:
						//    MessageBox.Show("Not a compatible ROM.");
						//    break;
				}
				language = (char)rom[0xAF];
				if (language == 'E' && version == 1) { //Chinese check
					if (Bits.getUInt32(rom, 0x08090000 & 0x01FFFFFF) == 0xEA00002E && //unchecked((int)0xEA00002E) && //Make UInt?
						Bits.getUInt32(rom, 0x08090004 & 0x01FFFFFF) == 0x51AEFF24 &&
						Bits.getUInt32(rom, 0x08090008 & 0x01FFFFFF) == 0x21A29A69 &&
						Bits.getUInt32(rom, 0x0809000C & 0x01FFFFFF) == 0x0A82843D) {
						language = 'C';
					}
				} else if (version == 0) {
					switch (language) {
						case 'E':
						case 'I': filetable = 0x08320000; break;
						case 'J': filetable = 0x08317000; break;
						case 'D': filetable = 0x0831FE00; break;
						case 'F':
						case 'S': filetable = 0x08321800; break;
					}
				}
				//File table verification.
				if (((Bits.getUInt32(rom, filetable & 0x01FFFFFF) >> 24) != 8) ||
					(Bits.getUInt32(rom, (filetable + 4) & 0x01FFFFFF) != filetable)) {
					//Incompatible
				}
				//TODO: Reorganize repointed data from Atrius's editor back into where they should belong.
				//Text data, filetable/map code data, Innate psys.
				//count_freespace()
				//init_globals
				//with (obj_MasterEditor) { sel=6*(global.version>=10); event_user(0); }
				loadEntireMap();
				//} else if (filename.EndsWith(".nds", true, null)) { //Dark Dawn?
			}
			romfn = filename;
			Properties.Settings.Default.LastRom = filename;
			Properties.Settings.Default.Save();
			if (palPath == null)
				palPath = System.IO.Path.GetDirectoryName(Properties.Settings.Default.LastRom) + @"\palette.bin";
		}
		string palPath;
		//Toolbar > File menu
		private void openToolStripMenuItem_Click(object sender, EventArgs e) {
			OpenROMdialogue();
		}
		private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
			Bits.saveFile(romfn, rom);
		}
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save ROM File";
			//sfd.Filter = "GBA/NDS/3DS file(*.gba;*.nds;*.3ds)|*.gba;*.nds;*.3ds";
			if (sfd.ShowDialog() == DialogResult.OK) {
				romfn = sfd.FileName;
				Bits.saveFile(romfn, rom);
				Properties.Settings.Default.LastRom = romfn;
				Properties.Settings.Default.Save();
			}
		}
		private void nextMapToolStripMenuItem_Click(object sender, EventArgs e) {
			if (sel == map_num) { return; }
			sel += 1;
			loadEntireMap();
		}

		private void previousMapToolStripMenuItem_Click(object sender, EventArgs e) {
			if (sel == 0) { return; }
			sel -= 1;
			loadEntireMap();
		}
		private void Form1_KeyDown(object sender, KeyEventArgs e) {
			//MessageBox.Show(e.KeyData.ToString());
			switch (e.KeyCode) {
				case (Keys)0x20:
					tileDisp += 1;
					if (tileDisp > 4) { tileDisp = 0; }
					tWin(lastPressedKey, 1);
					break;
				case (Keys)0x21:
					//if (sel == map_num) {return; }
					//sel += 1;
					int sel2 = (int)sel + 1;
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
						sel2 = (int)sel + 10;
					sel2 = Math.Min(sel2, map_num);
					if (sel2 == sel)
						return;
					sel = (uint)sel2;
					loadEntireMap();
					tWin(lastPressedKey, 0);
					pictureBox1.Image.Save(romfn + ".png");
					break;
				case (Keys)0x22:
					//if (sel == 0) { return; }
					//sel -= 1;
					sel2 = (int)sel - 1;
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
						sel2 = (int)sel - 10;
					sel2 = Math.Max(sel2, 0);
					if (sel2 == sel)
						return;
					sel = (uint)sel2;
					loadEntireMap();
					tWin(lastPressedKey, 0);
					break;
				case (Keys)0x31:
					if ((lastPressedKey == 0x31))
					{
						hideLayer ^= 1 << (0x31 - 0x31);
						//dispMap();
					}
					tWin(0x31, 1);
					//if (showhmap != 0)
						dispMap();
					break;
				case (Keys)0x32:
					if ((lastPressedKey == 0x32))
					{
						hideLayer ^= 1 << (0x32 - 0x31);
						//dispMap();
					}
					tWin(0x32, 1);
					//if (showhmap != 0)
						dispMap();
					break;
				case (Keys)0x33:
					if ((lastPressedKey == 0x33))
					{
						hideLayer ^= 1 << (0x33 - 0x31);
						//dispMap();
					}
					//loadEntireMap();
					tWin(0x33, 1);
					//if (showhmap != 0)
						dispMap();
					break;
				//default:
				//    Map.mapNum += 10;
				//    break;
				case (Keys)0x48: //'h'=heightmap
					showhmap ^= 1;
					//if (showhmap > 2)
					//	showhmap = 0;
					dispMap();
					break;
				case (Keys)0x46: //'f'=Flatten/Unflatten
					showhmap ^= 2;
					dispMap();
					break;
				case (Keys)0x47: //'g'=Cursor's y-based flattening.
					showhmap ^= 4;
					dispMap();
					break;
				case (Keys)0x57: //'W'=For resize how many tiles display per row based on selection box's width.s
					wr ^= 1;
					tWin(lastPressedKey, 0);
					break;
				case (Keys)0x4C: //"L"=Layered or side-by-side
					nlmap ^= 1;
					loadEntireMap(); //Only to update scnw & scnh for now.
					break;
				case (Keys)0xC0: //'`' Oemtilde = Grid
					if((Control.ModifierKeys & Keys.Control) != 0)
						grid ^= 3;
					else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
						grid ^= 2;
					else
						grid ^= 1;
					dispMap();
					break;
				case Keys.OemOpenBrackets: // [
					if (tilePal > 0)
						tilePal -= 0x10;
					tWin(lastPressedKey, 0); //dispMap();
					break;
				case Keys.Oem6: //]
					if (tilePal < 0xD0)
						tilePal += 0x10;
					tWin(lastPressedKey, 0); //dispMap();
					break;

				case Keys.E: //Export
					//MessageBox.Show(Properties.Settings.Default.LastRom +" " + System.IO.Path.GetDirectoryName(Properties.Settings.Default.LastRom));
					//string path = System.IO.Path.GetDirectoryName(Properties.Settings.Default.LastRom) + @"\palette.bin";
					//MessageBox.Show(path);
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
					{
						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Title = "Save palette file";
						//ofd.Filter = "GBA/NDS/3DS file(*.gba;*.nds;*.3ds)|*.gba;*.nds;*.3ds";
						if (sfd.ShowDialog() == DialogResult.OK) { palPath = sfd.FileName; } else { break; }
					}
					//openFile(path);
					//((PictureBox)ts.Controls[0]).Image.Save();///bmp/gif/jpg/png/tif
					switch (palPath.Substring(palPath.Length - 4))
					{
						case ".bmp":
						case ".png":
						case ".gif":
						case ".jpg":
						case ".tif":
							((PictureBox)ts.Controls[0]).Image.Save(palPath);
							break;
						default:
							Bits.saveFile(palPath, paldat);
							break;
					}
					//if ((new string[] { ".bmp", ".png", ".gif", ".jpg", ".tif" }).Contains(path.EndsWith()) { }
					break;
				case Keys.D: //Import
					//palPath = System.IO.Path.GetDirectoryName(Properties.Settings.Default.LastRom) + @"\palette.bin";
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
					{
						OpenFileDialog ofd = new OpenFileDialog();
						ofd.Title = "Load palette file";
						//ofd.Filter = "GBA/NDS/3DS file(*.gba;*.nds;*.3ds)|*.gba;*.nds;*.3ds";
						if (ofd.ShowDialog() == DialogResult.OK) { palPath = ofd.FileName; } else { break; }
					}
					
					switch (palPath.Substring(palPath.Length - 4))
					{
						case ".bmp":
						case ".png":
						case ".gif":
						case ".jpg":
						case ".tif":
							Bitmap palbmp = new Bitmap(palPath);
							int wi = palbmp.Width / 16;
							int hi = palbmp.Height / 16;
							for (int h = 0; h < 14; h++)
							{
								for (int w = 0; w < 16; w++)
								{
									int pix = palbmp.GetPixel(w * wi, h * hi).ToArgb();
									paldat[h * 0x20 + w * 2] = (byte)(((pix >> 6) & 0xE0) | ((pix >> 19) & 0x1F));
									paldat[h * 0x20 + w * 2 + 1] = (byte)(((pix >> 1) & 0x7C) | ((pix >> 14) & 0x03));
								}
							}
							break;
						default:
							paldat = Bits.openFile(palPath);
							break;
					}

					//Compress
					byte[] temp = new byte[0x10000];
					temp[0] = 1; //Compression Type 01
					int size = Comp.compressFormat1(paldat, 0, temp, 1)[1];
					size = (size + 3) & ~3;
					Array.Resize<byte>(ref temp, size);
					insertMFTfile(palfind, temp);

					initPal(paldat, 0, 0xE0);
					tWin(lastPressedKey, 0);
					dispMap(); //loadEntireMap();
					break;
			}
		}
		int grid = 0;
		int map_num = 0;
		uint sel = 5;//202;//5;// 174; //5;// 44;//10;//4;//102;//5;

		static void genPixPosTabel(int bitmapWidth) {
			int p = 0;
			//for (int h = 0; h < 4; h += 1) {
			for (short i = 0; i < 8 * bitmapWidth; i += (short)bitmapWidth) {//No flip table
				for (short j = i; j < i + 8; j++) {
					pixPosTable[p++] = j;
				}
			}
			for (short i = 0; i < 8 * bitmapWidth; i += (short)bitmapWidth) { //X-flip table
				for (short j = (short)(i + 7); i <= j; j--) {
					pixPosTable[p++] = j;
				}
			}
			for (short i = (short)(7 * bitmapWidth); 0 <= i; i -= (short)bitmapWidth) { //Y-flip table
				for (short j = i; j < i + 8; j++) {
					pixPosTable[p++] = j;
				}
			}
			for (short i = (short)(7 * bitmapWidth); 0 <= i; i -= (short)bitmapWidth) { //X/Y-flip table
				for (short j = (short)(i + 7); i <= j; j--) {
					pixPosTable[p++] = j;
				}
			}
		}
		static short[] pixPosTable = new short[0x100]; //Maybe most useful for 3DS pixel rendering? (@GBA/DS unnested tiles: Doesn't really have a noticable speed increase?)
		static byte[] blendflag = null;//Test.
		static int bldmod = 0x3F4F;//0x3F42;//0x3C42;
		static int eva = 8;//0x3C42;
		static int evb = 0x10;//0x3C42;
		byte[] paldat; byte[] tsetdat; byte[] stiles; byte[] tmap; byte[] hmap; byte[] visfx;
		byte[] htiles;
		int[] pos = new int[6];
		int scnw, scnh;
		int mapfind;
		int palfind;
		int world_map;
		void loadEntireMap() {
			//init_globals
			filetable &= 0x1FFFFFF;
			uint maps_pos1 = Bits.getUInt32(rom, filetable + 0x18) & 0x1FFFFFF;
			maps_pos1 = Bits.getUInt32(rom, unchecked((int)(maps_pos1 + 0xC))) & 0x1FFFFFF;
			maps_pos1 = Bits.getUInt32(rom, unchecked((int)((maps_pos1 & 0x1FFFFFC) - 4 - 0x3C * ((version == 11) ? 1 : 0)))) & 0x1FFFFFF;

			//ds_map_add(global.data_desc,global.maps_pos1,"Map Data Reference 1")
			uint maps_pos2 = Bits.getUInt32(rom, filetable + 0xC) & 0x1FFFFFF;
			maps_pos2 = Bits.getUInt32(rom, unchecked((int)(maps_pos2 + 0x10C + 0x20 * ((version == 0) ? 1 : 0)))) & 0x1FFFFFF;
			uint maps_off = Bits.getUInt32(rom, unchecked((int)((maps_pos2 & 0x1FFFFFC) - 4 - 0x160 * ((version == 0) ? 1 : 0)))) & 0x1FFFFFF;
			maps_pos2 = Bits.getUInt32(rom, unchecked((int)((maps_pos2 & 0x1FFFFFC) - 8 - 0x164 * ((version == 0) ? 1 : 0)))) & 0x1FFFFFF;

			//ds_map_add(global.data_desc,global.maps_pos2,"Map Data Reference 2")
			int mapid_str = 0;

			if (version == 0) {
				switch (language) {
					case 'J': mapid_str = 2624; break;
					case 'E': mapid_str = 2567; break;
					default: mapid_str = -1; break;
				}
				map_num = 201;
			} else if (version == 1) {
				switch (language) {
					case 'J': mapid_str = 3772; break;
					case 'D':
					case 'S':
					case 'F':
					case 'I': mapid_str = 4231; break;
					case 'E':
					case 'C': mapid_str = 3770; break;
					default: mapid_str = -1; break;
				}
				map_num = 325;
			} else if (version == 10) {
				mapid_str = 0;
				map_num = 55;
			} else if (version == 11) {
				mapid_str = 882;
				map_num = 69;
			}

			//byte[] mapc = new byte[0x8000];
			//for (int a = 0x649; a < 0x6BB; a++) {
			//    decompress(rom, Bits.getInt32(rom, filetable + a * 4) & 0x1FFFFFF, mapc, 0);
			//    uint val = (uint)Bits.getInt32(mapc, 0);
			//    int i = 4; int j = 0;
			//    while (true) {
			//        if ((i & 3) == 0) { //POINTERS
			//            if ((val) == 0x03001238) {
			//                //val2 = (int)val & 0x1FFFFFE;
			//                MessageBox.Show(j.ToString("X8") + " | " +  a.ToString("X8") + " --- " + i.ToString("X8") + ":" + val.ToString("X8")); j++;
			//            }
			//        }
			//        if (i >= mapc.Length) { break; }
			//        val = (val >> 8) | ((uint)mapc[i++] << 24);
			//    }
			//}

			//obj_eMaps > User Defined 1 //Swap all to Unsigned?
			//uint sel = 16;
			uint loc = (maps_pos1 + 8 * sel) & 0x1FFFFFF; //file_bin_seek(global.file,(global.maps_pos1+8*sel)&$1FFFFFF)
			int code = Bits.getInt16(rom, loc); //show_message(string_hex(READU8(global.file,READU32(global.file,(global.filetable+(code<<2)))),2)+', '+string_hex(dataresource_size(code),8))
												//rom(loc + 2) //Music/Name?
												//rom(loc + 3) //???
			if (version == 10) //Golf
			{
				//@3= See table at 080686C0 in (U) version. ([+0xC]=Tilemap)
				//@4= Likely uses a separate table as well.
				if ((rom[loc + 3] == 3) || (rom[loc + 3] == 4))
				{
					dMap = 0; // Don't display map.
					this.Text = sel.ToString() + " - Type " + rom[loc + 3] + " is not currently supported."; // + " - " + pos[0].ToString("X8");
					return;
				}
			}
			else if (version == 11) //Tennis
			{
				dMap = 0; // Don't display map.
				this.Text = sel.ToString() + " - No maps supported until I fix it.";
				return;
			}
			int map = Bits.getInt16(rom, loc + 4);
			loc = unchecked((uint)((maps_pos2 + 12 * map) & 0x1FFFFFF)); //file_bin_seek(global.file,(global.maps_pos2+12*map)&$1FFFFFF)
			pos[0] = Bits.getInt16(rom, loc); mapfind = pos[0] + (int)maps_off;
			pos[1] = Bits.getInt16(rom, loc + 2); palfind = pos[1] + (int)maps_off;
			pos[2] = Bits.getInt16(rom, loc + 4);
			pos[3] = Bits.getInt16(rom, loc + 6);
			pos[4] = Bits.getInt16(rom, loc + 8);
			pos[5] = Bits.getInt16(rom, loc + 10);

			for (int a = 0; a < 6; a++) {
				pos[a] = Bits.getInt32(rom, unchecked((int)(filetable + ((pos[a] + maps_off) << 2)) & 0x1FFFFFF)) & 0x1FFFFFF;
			}
			world_map = (map == 1) ? 1 : 0;
			if (world_map == 1) {
				if (version == 0) {
					dispWMGS1(); return;
				} else if (version == 1) {
					dispWMGS2(); return;
				}
			}
			/* Skip for now.
 file_bin_seek(global.file,(global.filetable+(code<<2))&$1FFFFFF)
 code=READ_UINT(global.file)
 code_f=decompressMapCode(global.file,code,temp_directory+'\code.bin')
 code=file_bin_open(code_f,2)
             */
			//byte[] t = decompress(rom, 0xA8B1B8, 0); //Format 2
			//byte[] t = decompress(rom, 0x888FE4 + 0x80, 0); //Format 2
			//byte[] t = decompress16(rom, 0x4F157, 0); //Format 2
			//byte[] t = decompText(rom, 0x111); // decompBtlBG(rom, 0x7765D8 + 0x100);
			paldat = new byte[0x1C0]; //0x200];
			Comp.decompress(rom, pos[1], paldat, 0); initPal(paldat, 0, 0xE0); //pictureBox1.Image = PixelsToImage(pal, 16, 14, 16);
			tsetdat = new byte[0x10000];
			Comp.decompress(rom, pos[2], tsetdat, 0x0000);
			Comp.decompress(rom, pos[3], tsetdat, 0x4000);
			Comp.decompress(rom, pos[4], tsetdat, 0x8000);
			Comp.decompress(rom, pos[5], tsetdat, 0xC000);

			scnw = rom[pos[0] + 0x2] * 8;
			scnh = rom[pos[0] + 0x3] * 8;

			stiles = new byte[0x10000]; tmap = new byte[0x10000]; visfx = new byte[0x200];
			htiles = new byte[0x1000]; hmap = new byte[0x4000];
			//int length =
			decompTileData(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x24)), stiles, 0); //8x8 tile table - Decompress & Deobfuscate.
			Comp.decompress(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x28)), htiles, 0); //Height Tile Data - Skip for now.
																						   //System.IO.File.WriteAllBytes("C:/Users/Tea/Desktop/htiles.dmp", htiles);
			decompMap2(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x2C)), tmap, 0); //16x16 tilemap - Decompress & Deobfuscate.

			if (version == 1) {
				Comp.decompress(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x30)), hmap, 0); //Height Tilemap
																							 //Animation
				if (Bits.getInt32(rom, pos[0] + 0x38) != 0) { Comp.decompress(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x38)), visfx, 0); } //catch { } //Visual Effects
			}
			//Coord data

			//shift[0]=READ_UBYTE(global.file)+(READ_UBYTE(global.file)<<7)
			this.Text = sel.ToString() + " - " + pos[0].ToString("X8") + "-" + (Bits.getInt32(rom, unchecked((int)(filetable + ((mapfind + 1) << 2)) & 0x1FFFFFF)) & 0x1FFFFFF).ToString("X8");
			//Temporary error avoidance:
			dMap = 0;
			if (scnw == 0) { this.Text += (" (Error - Width was 0)").ToString(); scnw = 0x80 * 16; scnh = 0x36 * 16; } //Anti-error for... was it sel 102(?)
			if (scnh == 0) { this.Text += (" (Error - Height was 0)").ToString(); scnh = 16; }
			//if ((sel == 175) || (sel == 176) || (sel == 202) || (sel == 229)) { this.Text += (" (Error for 175, 176, 202, and 229)").ToString(); return; }
			dMap = 1;
			dispMap();
			//if (sel == 124)
			//{
			//	MessageBox.Show(Bits.getInt32(stiles, 0x1A0).ToString("X8") + " " + Bits.getInt32(stiles, 0x1A4).ToString("X8"));
			//}
			//if (sel == 125)
			//{
			//	MessageBox.Show(Bits.getInt32(stiles, 0x838).ToString("X8") + " " + Bits.getInt32(stiles, 0x83C).ToString("X8"));
			//}
		}
		void dispWMGS1() {
			paldat = new byte[0x800]; tsetdat = new byte[0x20000];
			int wmflst = 0x132CC;// +(9 * 4 * 3);
			pal = new int[0x400];
			for (int j = 0; j < 2; j++) {
				int i = j * 0x8000;
				Comp.decompressf(rom, Bits.getInt32(rom, 0x320000 + 4 * Bits.getInt32(rom, wmflst)) & 0x1FFFFFF, paldat, j * 0x200, 1);
				//Comp.decompress(rom, 0x680000 + 4 * Bits.getInt32(rom, 0x2ED34 + 4), , 0); //Animation Data (Ignore)
				//Comp.decompress(rom, 0x680000 + 4 * Bits.getInt32(rom, 0x2ED34 + 8), , 0); //Terrain Tileset (Ignore)
				Comp.decompress(rom, Bits.getInt32(rom, 0x320000 + 4 * Bits.getInt32(rom, wmflst + 0x4)) & 0x1FFFFFF, tsetdat, i + 0x0000); //Tileset 1
				Comp.decompress(rom, Bits.getInt32(rom, 0x320000 + 4 * Bits.getInt32(rom, wmflst + 0x8)) & 0x1FFFFFF, tsetdat, i + 0x2000); //Tileset 2
				Comp.decompress(rom, Bits.getInt32(rom, 0x320000 + 4 * Bits.getInt32(rom, wmflst + 0xC)) & 0x1FFFFFF, tsetdat, i + 0x4000); //Tileset 3
				Comp.decompress(rom, Bits.getInt32(rom, 0x320000 + 4 * Bits.getInt32(rom, wmflst + 0x10)) & 0x1FFFFFF, tsetdat, i + 0x6000); //Tileset 4

				//Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x1C)) & 0x1FFFFFF, tsetdat, 0x8000); //Animated Tileset 1
				//Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x20)) & 0x1FFFFFF, tsetdat, 0xA000); //Animated Tileset 2
				wmflst += 0x18;
			}
			initPal(paldat, 0, 0x400);
			stiles = new byte[0x10000];
			Comp.decompress(rom, Bits.getInt32(rom, 0x320000 + 4 * 0xD5) & 0x1FFFFFF, stiles, 0x0000); //8x8 tiles

			//Comp.decompress(rom, 0x680000 + 4 * 0x199, , 0x0000); //Misc data (0x0202E000)

			scnw = 0x1000; scnh = 0x1400;
			//int[] bmpdata = new int[scnw * scnh];

			int wmpos = Bits.getInt32(rom, 0x320000 + 4 * 0xD4) & 0x1FFFFFF;
			//byte[] mapdata = new byte[0x400];
			byte[] mapdata = new byte[0x800];
			int iOffset = 0;
			int tspos = 0; int highind = 0;
			int[] bmpdata = new int[scnw * scnh];
			while (iOffset < 0x500) {
				for (int yMap = 0; yMap < scnh; yMap += 0x100) {
					for (int xMap = 0; xMap < scnw; xMap += 0x100) {
						int rptr = Bits.getInt32(rom, wmpos + iOffset); //iOffset += 4;
						int rptr2 = Bits.getInt32(rom, wmpos + iOffset + 0x500); iOffset += 4;
						if (rptr == 0) { continue; }
						Comp.decompressf(rom, wmpos + rptr, mapdata, 0x0000, 1); //Map data
																				 //int t = 0;
						if (rptr2 == 0) { continue; }
						Comp.decompressf(rom, wmpos + rptr2, mapdata, 0x0400, 1);
						int iOffset2 = 0;
						while (iOffset2 < 0x800)
						{
							for (int row = yMap; row < yMap + 0x100; row += 16)
							{
								for (int col = xMap; col < xMap + 0x100; col += 16)
								{
									int t = Bits.getInt16(mapdata, iOffset2) * 4;
									int tsNum = mapdata[(iOffset2 & 0x3FF) + 3] & 0x7F;
									//if (tsNum > highind) { highind = tsNum; }
									//if ((tsNum > 0) && (iOffset > 0x500)) { MessageBox.Show("Hahaha!"); }
									if (tsNum == 0x15)
									{
										tsNum = 1;
									}
									else { tsNum = 0; }
									iOffset2 += 4;
									for (int r = row; r < row + 16; r += 8)
									{
										for (int c = col; c < col + 16; c += 8)
										{
											int tile = stiles[t++];// tspos++;
											int pos = tsNum * 0x8000 + (tile << 6);
											if (iOffset2 > 0x400) { pos += 0x4000; } // > is intentional do to +4 above.
											for (int y = r; y <= r + 7; y++)
											{
												for (int x = c; x <= c + 7; x++)
												{
													if (tsetdat[pos] == 0) { pos++; continue; }
													bmpdata[(y * scnw) + x] = pal[tsNum * 0x100 + tsetdat[pos++]];
												}
											}
										}
									}
									//tsNum = mapdata[iOffset2 - 4 + 2];// &0x7F;
									//for (int r = row; r < row + 16; r += 8)
									//{
									//	for (int c = col; c < col + 16; c += 8)
									//	{
									//		//int tile = stiles[t++];// tspos++;
									//		//int pos = tsNum * 0x8000 + (tile << 6);
									//		if (tsNum == 0) { continue; }
									//		if (iOffset2 > 0x400) { continue; } // pos += 0x4000; } // > is intentional do to +4 above.
									//		for (int y = r; y <= r + 7; y++)
									//		{
									//			for (int x = c; x <= c + 7; x++)
									//			{
									//				//if (tsetdat[pos] == 0) { pos++; continue; }
									//				if ((tsNum & 1) == 1) { bmpdata[(y * scnw) + x] = 0xC0C0C0; } else { bmpdata[(y * scnw) + x] = 0x404040; } // pal[0];
									//			}
									//			tsNum >>= 1;
									//		}
									//	}
									//}
								}
							}
						}
					}
				}
			}
			//MessageBox.Show(highind.ToString("X4"));
			//for (int r = 0; r < 0x180; r += 8) {
			//    for (int c = 0; c < 0x80; c += 8) {
			//        int tile = tspos++;
			//        int pos = tile << 6; 
			//        for (int y = r; y <= r + 7; y++) {
			//            for (int x = c; x <= c + 7; x++) {
			//                bmpdata[(y * scnw) + x] = pal[tsetdat[pos++]];
			//            }
			//        }
			//    }
			//}
			pictureBox1.Width = scnw; pictureBox1.Height = scnh;
			pictureBox1.Image = PixelsToImage(bmpdata, scnw, scnh);
			//pictureBox1.Image.Save(@"C:\Users\Tea\Desktop\GS1WorldMap.png");
		}
		void dispWMGS2() {
			paldat = new byte[0x800]; tsetdat = new byte[0x20000];
			int wmflst = 0x2ED34;// +(9 * 4 * 3);
			pal = new int[0x400];
			for (int j = 0; j < 4; j++) {
				int i = j * 0x8000;
				Comp.decompressf(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst)) & 0x1FFFFFF, paldat, j * 0x200, 1);
				//Comp.decompress(rom, 0x680000 + 4 * Bits.getInt32(rom, 0x2ED34 + 4), , 0); //Animation Data (Ignore)
				//Comp.decompress(rom, 0x680000 + 4 * Bits.getInt32(rom, 0x2ED34 + 8), , 0); //Terrain Tileset (Ignore)
				Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0xC)) & 0x1FFFFFF, tsetdat, i + 0x0000); //Tileset 1
				Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x10)) & 0x1FFFFFF, tsetdat, i + 0x2000); //Tileset 2
				Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x14)) & 0x1FFFFFF, tsetdat, i + 0x4000); //Tileset 3
				Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x18)) & 0x1FFFFFF, tsetdat, i + 0x6000); //Tileset 4
				
				//Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x1C)) & 0x1FFFFFF, tsetdat, 0x8000); //Animated Tileset 1
				//Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * Bits.getInt32(rom, wmflst + 0x20)) & 0x1FFFFFF, tsetdat, 0xA000); //Animated Tileset 2
				wmflst += 0x24;
			}
			initPal(paldat, 0, 0x400);
			stiles = new byte[0x10000];
			Comp.decompress(rom, Bits.getInt32(rom, 0x680000 + 4 * 0x198) & 0x1FFFFFF, stiles, 0x0000); //8x8 tiles

			//Comp.decompress(rom, 0x680000 + 4 * 0x199, , 0x0000); //Misc data (0x0202E000)

			scnw = 0x2000; scnh = 0x2000;
			//int[] bmpdata = new int[scnw * scnh];

			int wmpos = Bits.getInt32(rom, 0x680000 + 4 * 0x197) & 0x1FFFFFF;
			//byte[] mapdata = new byte[0x400];
			byte[] mapdata = new byte[0x800];
			int iOffset = 0;
			int tspos = 0;
			int[] bmpdata = new int[scnw * scnh];
			while (iOffset < 0x1000) {
				for (int yMap = 0; yMap < scnh; yMap += 0x100) {
					for (int xMap = 0; xMap < scnw; xMap += 0x100) {
						int rptr = Bits.getInt32(rom, wmpos + iOffset); //iOffset += 4;
						int rptr2 = Bits.getInt32(rom, wmpos + 0x1000 + iOffset); iOffset += 4;
						//if (rptr == 0) { continue; }
						//Comp.decompressf(rom, wmpos + rptr, mapdata, 0x0000, 1); //Map data
						//if (rptr2 == 0) { continue; }
						//Comp.decompressf(rom, wmpos + rptr2, mapdata, 0x0400, 1); //WIP
						int iOffset2 = 0;
						while (iOffset2 < 0x800)
						{
							if (iOffset2 < 0x400)
							{
								if (rptr == 0) { iOffset2 = 0x400; continue; }
								Comp.decompressf(rom, wmpos + rptr, mapdata, 0x0000, 1); //Map data
							}
							else
							{
								if (rptr2 == 0) { iOffset2 = 0x800; continue; }
								Comp.decompressf(rom, wmpos + rptr2, mapdata, 0x0400, 1); //WIP
							}
							for (int row = yMap; row < yMap + 0x100; row += 16)
							{
								for (int col = xMap; col < xMap + 0x100; col += 16)
								{
									int t = Bits.getInt16(mapdata, iOffset2) * 4;
									int tsNum = mapdata[(iOffset2 & 0x3FF) + 3] & 0x3F;
									if (tsNum < 0x14)
									{
										tsNum = 0;
									}
									else if (tsNum < 0x1E)
									{
										tsNum = 2;
									}
									else if (tsNum < 0x28)
									{
										tsNum = 3;
									}
									else if (tsNum < 0x32)
									{
										tsNum = 1;
									}
									else { tsNum = 0; }
									iOffset2 += 4;
									//stiles[t]
									for (int r = row; r < row + 16; r += 8)
									{
										for (int c = col; c < col + 16; c += 8)
										{
											int tile = stiles[t++];// tspos++;
											int pos = tsNum * 0x8000 + (tile << 6);
											if (iOffset2 > 0x400) { pos += 0x4000; }
											for (int y = r; y <= r + 7; y++)
											{
												for (int x = c; x <= c + 7; x++)
												{
													if (tsetdat[pos] == 0) { pos++; continue; }
													bmpdata[(y * scnw) + x] = pal[tsNum * 0x100 + tsetdat[pos++]];
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			//for (int r = 0; r < 0x180; r += 8) {
			//    for (int c = 0; c < 0x80; c += 8) {
			//        int tile = tspos++;
			//        int pos = tile << 6; 
			//        for (int y = r; y <= r + 7; y++) {
			//            for (int x = c; x <= c + 7; x++) {
			//                bmpdata[(y * scnw) + x] = pal[tsetdat[pos++]];
			//            }
			//        }
			//    }
			//}
			pictureBox1.Width = scnw; pictureBox1.Height = scnh;
			pictureBox1.Image = PixelsToImage(bmpdata, scnw, scnh);
		}
		int dMap = 1;
		int showhmap = 0;
		int[] bmpdata = new int[1];
		int nlmap = 0; //nl = no layer / as in just 1 layer of everything.
		void dispMap() {
			if (dMap == 0) { return; }
			if (world_map == 1)
			{
				if (version == 0)
				{
					dispWMGS1(); return;
				}
				else if (version == 1)
				{
					dispWMGS2(); return;
				}
			}
			//if ((showhmap & 1) != 0)
			//{
			//	pictureBox1.Width = scnw; pictureBox1.Height = scnh;
			//	pictureBox1.Image = disphmap();
			//	//pictureBox1.Image = dispPrioMap();//
			//	return;
			//}
			//if (bmpdata.Length != scnw * scnh)
			if (nlmap == 1)
			{
				scnw = 0x800;
				scnh = 0x800;
			}
			bmpdata = new int[scnw * scnh];
			//for (int l = 0; l < 3; l += 1) {
			//    int shift = rom[pos[0] + 0xC + (l * 8)] + (rom[pos[0] + 0xD + (l * 8)] << 7);
			//    for (int dy = 0; dy < rom[pos[0] + 0x3]; dy += 2) {
			//        for (int dx = 0; dx < rom[pos[0] + 0x2]; dx += 2) {
			//            int t = Bits.getInt16(tmap, 0x8000 + dx + shift + (dy << 7)) & 0xFFF;
			//            for (int t8 = 0; t8 < 8; t8 += 2) {
			//                int tile = Bits.getInt16(stiles, t * 8);
			//                int t2 = ((tile & 0x3FF) + ((rom[pos[0] + 7 + l] - 1) * 0x200)) << 5; //+ tset
			//                int tilePal = (tile >> 8) & 0xF0;
			//                for (int n = 0; n < 0x40; n += 2) {
			//                    int ry = y; if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
			//                    int rx = x; if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
			//                    int pix = tsetdat[t2++];
			//                    int pix2 = pix & 0xF;
			//                    if (pix2 != 0) {
			//                        bmpdata[((dy*8 + (t8&4)*2 + (n>>3)) * width) + rx] = pal[tilePal | (pix2)];
			//                    }
			//                    int rx = x + 1; if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
			//                    pix2 = pix >> 4;
			//                    if (pix2 != 0) {
			//                        bmpdata[(ry * width) + rx] = pal[tilePal | (pix2)];
			//                    }
			//                }
			//            }
			//        }
			//    }
			//}
			//int mappos = 0;
			//return;
			//try {
			genPixPosTabel(scnw);
			//PREPARE BLENDING INFO!
			blendflag = new byte[(scnw * scnh) >> 3];// bldmod = 0;
			bldmod = Bits.getInt16(visfx, 0);// | 0xF;
			eva = visfx[2];
			evb = visfx[3];
			//int bldaddr = 0x3B78AC + (rom[0x3A78D4 + (mapNum * 0x18) + 0x9] << 3);
			//bldmod = Bits.getInt32(rom, bldaddr);
			//bldmod = ((bldmod & 0xFC0) << 2) | (bldmod & 0x3F);
			//int evaddr = Bits.getInt32(rom, bldaddr + 4);
			//if (evaddr != 0) {
			//    bldmod |= 0x40;
			//    int evdata = Bits.getInt32(rom, evaddr & 0x1FFFFFF);
			//    eva = evdata & 0x1F;
			//    evb = (evdata >> 5) & 0x1F;
			//}
			//Text += "  " +hideLayer.ToString();
			int m = 0; int l = 3;
			int layiOffset = 1;
			int shiftx = 0, shifty = 0;
			int shift = 0;
			while (m < 4) { //for (int m = 1; m <= 3; m++) {
				if (nlmap != 0) //Quick hack
				{
					m = 4;
					l = lastPressedKey - 0x31;
					l &= 3; if (l == 3) { l = 0; }
					goto jmp1;
				}
				l--; //int l = 2;
				while ((l >= 0) && (rom[pos[0] + 4 + l] != m)) {
					l--;
				}
				if (l < 0) { l = 3; m++; continue; }
				layiOffset = 1 << (3 - l);
				shiftx = rom[pos[0] + 0xC + (l * 8)]; shifty = rom[pos[0] + 0xD + (l * 8)];
				shift = (shiftx & 0xfe) + ((shifty & 0xfe) << 7);
				if ((hideLayer & (1 << l)) != 0)
					continue;
				jmp1:
				for (int row = 0; row < (scnh); row += 16) {
					for (int col = 0; col < (scnw); col += 16) {
						//int t = Bits.getInt16(tmap, 0x8000 + dx + shift + (dy << 7)) & 0xFFF;
						//Quick fix for when some layers are likely smaller than the map area? (e.g. slower scrolling)
						//This is only needed for a few maps. (At least 2? 175/176)
						if ((0x8000 + (col >> 3) + shift + (row << 4)) >= 0x10000)
							break;
						int t = (Bits.getInt16(tmap, 0x8000 + (col >> 3) + shift + (row << 4)) & 0xFFF) * 8;
						//try {
						for (int r = row; r < row + 16; r += 8) {
							//Quick fix for when the map height is an odd number.
							if (r >= scnh)
								break;
							for (int c = col; c < col + 16; c += 8) {
								//Quick fix for when the map width is an odd number.
								if (c >= scnw)
									break;

								int tile = Bits.getInt16(stiles, t); t += 2;

								int q = (tile >> 4) & 0xC0;
								int tilepos = ((r * scnw) + c);
								//int k = q + 0x40;
								//int t2 = ((tile & 0x3FF) + ((rom[pos[0] + 7 + l] - 1) * 0x200)) << 5; //+ tset
								int t2 = ((tile & 0x3FF) << 5) + ((rom[pos[0] + 7 + l] - 1) * 0x4000); //+ tset
								if (t2 >= 0) {
									int tilePal = (tile >> 8) & 0xF0;
									for (int y = r; y <= r + 7; y++) {
										int ry = y; if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
										ry -= (shifty & 1) * 8; if (ry < 0) { continue; }
										//int pix = ram.getInt32(pos); pos += 4;
										for (int x = c; x <= c + 7; x += 2) {
											int rx = x; if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
											rx -= (shiftx & 1) * 8; if (rx < 0) { continue; }
											int pix = tsetdat[t2++];
											int pix2 = pix & 0xF;
											if (pix2 != 0) {
												//bmpdata[(ry * scnw) + rx] = pal[tilePal | (pix2)];
												//priomap[(ry * bitmapWidth) + rx] = priority;
												int mappos = tilepos + pixPosTable[q];
												if (((blendflag[mappos >> 3] >> (mappos & 7)) & 1) == 1) {
													if ((bldmod & (0xC0 | (layiOffset << 8))) == (0x40 | (layiOffset << 8))) {
														bmpdata[mappos] = blendit(bmpdata[mappos], pal[tilePal | pix2], eva, evb);
													}
													blendflag[mappos >> 3] ^= (byte)(1 << (mappos & 7));
												} else if (bmpdata[mappos] == 0) {
													bmpdata[mappos] = pal[tilePal | pix2];
													//priomap[(ry * bitmapWidth) + rx] = priority;
													if ((bldmod & (0xC0 | layiOffset)) == (0x40 | layiOffset)) {
														blendflag[mappos >> 3] |= (byte)(1 << (mappos & 7));
													}
												}
											} q++;
											//pix >>= 4;
											rx = x + 1; if ((tile & 0x400) != 0) { rx = c + ((7 - (x + 1)) & 7); } //Horr. flip
											pix2 = pix >> 4;
											if (pix2 != 0) {
												//bmpdata[(ry * scnw) + rx] = pal[tilePal | (pix2)];
												//priomap[(ry * bitmapWidth) + rx] = priority;
												int mappos = tilepos + pixPosTable[q];
												if (((blendflag[mappos >> 3] >> (mappos & 7)) & 1) == 1) {
													if ((bldmod & (0xC0 | (layiOffset << 8))) == (0x40 | (layiOffset << 8))) {
														bmpdata[mappos] = blendit(bmpdata[mappos], pal[tilePal | pix2], eva, evb);
													}
													blendflag[mappos >> 3] ^= (byte)(1 << (mappos & 7));
												} else if (bmpdata[mappos] == 0) {
													bmpdata[mappos] = pal[tilePal | pix2];
													//priomap[(ry * bitmapWidth) + rx] = priority;
													if ((bldmod & (0xC0 | layiOffset)) == (0x40 | layiOffset)) {
														blendflag[mappos >> 3] |= (byte)(1 << (mappos & 7));
													}
												}
											} q++;
											//pix >>= 4;
										}
									}
								}
							}
						}
						//} catch { }
					}
				}

			}
			//} catch { }
			if ((grid & 1) != 0)
			{
				for (int row = 0; row < (scnh); row += 1)
				{
					for (int col = 0; col < (scnw); col += 1)
					{
						if (((col & 0xF) == 0xF) || ((row & 0xF) == 0xF))
						{
							bmpdata[row * scnw + col] = 0;
						}
					}
				}
			}
			if ((grid & 2) != 0)
			{
				for (int row = 0; row < (scnh); row += 1)
				{
					for (int col = 0; col < (scnw); col += 1)
					{
						if (((col & 0xF) == 0) || ((row & 0xF) == 0))
						//if ((((col & row) & 0xF) == 0)) //Nope... Random fun, though.
						{
							bmpdata[row * scnw + col] = 0;
						}
					}
				}
			}
			if (tileDisp == 4)
			{
				dispPrioMap();
			}
			else if ((showhmap & 1) != 0)
			{
				//Text += ">";
				disphmap();
				//Text += "<";

				//pictureBox1.Width = scnw; pictureBox1.Height = scnh;
				//pictureBox1.Image = disphmap();
				//pictureBox1.Image = dispPrioMap();//
				//return;
			}

			pictureBox1.Width = scnw; pictureBox1.Height = scnh;
			pictureBox1.Image = PixelsToImage(bmpdata, scnw, scnh);
			//System.IO.File.WriteAllBytes("C:/Users/Tea/Desktop/gsdtest.dmp", tsetdat); //stiles);
		}
		//void tile_add(int[] mapBmp, int mapPos, byte[] tsetBmp, int tsetPos, int[] pal, int tileInfo) {

		//}
		public static int blendit(int first, int second, int eva, int evb) {
			int blue = (((byte)first * eva) >> 4) + (((byte)second * evb) >> 4);
			if (blue > 0xF8) { blue = 0xF8; }
			int green = (((byte)(first >> 8) * eva) >> 4) + (((byte)(second >> 8) * evb) >> 4);
			if (green > 0xF8) { green = 0xF8; }
			int red = (((byte)(first >> 16) * eva) >> 4) + (((byte)(second >> 16) * evb) >> 4);
			if (red > 0xF8) { red = 0xF8; }
			return unchecked((int)0xFF000000) | ((red << 16) | (green << 8) | blue) & 0xF8F8F8;
		}
		public int[] pal = new int[0x100];
		public void initPal(byte[] src, int srcPos, int num) { // int[] des, int desPos, int num) { //Loads palette data into 32-bit array with reversed format.
															   //rom.seek(0x8C88C8);
			for (int i = 0; i < num; i++) {
				int palt = Bits.getInt16(src, srcPos + (i * 2));
				//pal[i] = (short)(((palt & 0x1F) << 10) | (palt & 0x3E0) | (palt >> 10));
				pal[i] = unchecked((int)0xFF000000) | ((palt & 0x1F) << 0x13) | ((palt & 0x3E0) << 6) | ((palt >> 7) & 0xF8);
			}
		}
		//Only needed to display zoomed in Palette bitmap,
		public static Bitmap PixelsToImage(int[] array, int width, int height, int zoom) {
			if (zoom <= 0) { return new Bitmap(1, 1); }
			int[] array2 = new int[width * height * zoom * zoom];
			//for (int i = 0; i < (width * height); i++) {
			int i = 0;
			for (int yt = 0; yt < (height * zoom) * (width * zoom); yt += (width * zoom) * zoom) {
				for (int xt = yt; xt < yt + (width * zoom); xt += zoom) {
					int palt = array[i++];
					for (int y = xt; y < xt + (width * zoom) * zoom; y += (width * zoom)) {
						for (int x = y; x < y + zoom; x++) {
							array2[x] = palt;
						}
					}
				}
			}
			return PixelsToImage(array2, width * zoom, height * zoom);
		}
		public static Bitmap PixelsToImage(int[] array, int width, int height) {
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
			IntPtr pNative = bitmapData.Scan0;
			Marshal.Copy(array, 0, pNative, width * height);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		void decompTileData(byte[] src, int srcPos, byte[] des, int desPos) {
			byte[] t = new byte[0x10000];
			int length = Comp.decompress(src, srcPos, t, desPos)[1];
			//MessageBox.Show( ""+t[0]);
			//byte[] stiles = new byte[0x8000];
			int i = 0, pos1, pos2, n = 0;
			switch (t[0]) {
				case 1:
					pos2 = (length >> 1) + 1;
					for (pos1 = 1; pos1 <= (length >> 1); i += 2) {
						n ^= ((t[pos1++] << 8) | (t[pos2++]));
						des[i] = (byte)n; des[i + 1] = (byte)(n >> 8);
					}
					break;
				case 2:
					for (i = 1; i < (length - 1); i += 2) {
						n ^= t[i] | (t[i + 1] << 8);
						des[i - 1] = (byte)n; des[i] = (byte)(n >> 8);
					}
					break;
				default: //case 0:
					for (i = 0; i < length; i++) {
						des[i] = t[i + 1];
					}
					break;
			}
			length--;
			t8amt = length >> 3;
		}
		void decompMap2(byte[] src, int srcPos, byte[] des, int desPos) {
			Comp.decompress(src, srcPos, des, desPos);
			uint b = 0xFFFFF001, pos1, num;
			for (pos1 = 0x8000; pos1 < 0x10000; pos1 += 2) {
				num = (uint)(des[pos1] | (des[pos1 + 1] << 8));
				if ((num << 20) == 0xFFF00000) {
					num += b;
					if (b != 0) { b++; }
				}
				des[pos1] = (byte)num; des[pos1 + 1] = (byte)(num >> 8);
			}
		}

		private void editorsToolStripMenuItem_Click(object sender, EventArgs e) {
			Editors eds = new Editors();
			eds.test = this;
			eds.Show();
		}
		//byte[] paldat; byte[] tsetdat; byte[] stiles;
		int tilePal = 0;
		public Bitmap dispTsetImg(int charBase)
		{ //, int tilePal) {
			int tspos = 0;
			int[] bmpdata = new int[0x80000];
			//for (int row = 0; row < 0x200; row += 16) {
			//for (int col = 0; col < 0x100; col += 16) {
			for (int r = 0; r < 0x280; r += 8)
			{
				for (int c = 0; c < 0x100; c += 8)
				{
					//int tile = rom.getInt16(tsetaddr[0] + tspos); tspos += 2;
					int tile = tspos;//Bits.getInt16(stiles, tspos);
					tspos += 1;
					//if (tsetaddr[tspos >> 12] == -1) { tile = 0; } else { tile = Bits.getInt16(rom, tsetaddr[tspos >> 12] + (tspos & 0xFFF)); tspos += 2; } //tspos &= 0xFFF;
					int pos = tile << 5; // (tile & 0x3FF) << 5;
										 //pos += charBase;
										 //int tilePal = 0;// (tile >> 8) & 0xF0;
					pos -= 0x4000; if (pos < 0) { continue; }
					//int x1, x2, xi, y1, y2, yi; // Not sure how I wanted flipping coded in?  Alternative below, though!
					//if ((tile & 0x400) == 0) { x1 = 0; x2 = 7; xi = 1; } else { x1 = 7; x2 = 0; xi = -1; };
					//if ((tile & 0x800) == 0) { y1 = 0; y2 = 7; yi = 1; } else { y1 = 7; y2 = 0; yi = -1; };
					for (int y = r; y <= r + 7; y++)
					{
						int ry = y; //if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
						int pix = Bits.getInt32(tsetdat, pos); pos += 4;
						for (int x = c; x <= c + 7; x++)
						{
							int rx = x; //if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
							bmpdata[(ry << 9) + rx] = pal[tilePal | (pix & 0xF)];
							pix >>= 4;
						}
					}
				}
			}
			//}
			//}
			//charBase indicator
			for (int y = 0; y < charBase; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0xF80000;
				bmpdata[(y << 9) + 0x101] = 0xF80000;
			}
			for (int y = charBase; y < charBase + 0x100; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0x00F800;
				bmpdata[(y << 9) + 0x101] = 0x00F800;
			}
			for (int y = charBase + 0x100; y < 0x200; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0xF80000;
				bmpdata[(y << 9) + 0x101] = 0xF80000;
			}
			return PixelsToImage(bmpdata, 0x200, 0x400);
		}
		public Bitmap dispTsetImgWM(int charBase)
		{ //, int tilePal) {
			int tspos = 0;
			int[] bmpdata = new int[0x80000];
			//for (int row = 0; row < 0x200; row += 16) {
			//for (int col = 0; col < 0x100; col += 16) {
			for (int r = 0; r < 0x100; r += 8)
			{
				for (int c = 0; c < 0x80; c += 8)
				{
					//int tile = rom.getInt16(tsetaddr[0] + tspos); tspos += 2;
					int tile = tspos;//Bits.getInt16(stiles, tspos);
					tspos += 1;
					//if (tsetaddr[tspos >> 12] == -1) { tile = 0; } else { tile = Bits.getInt16(rom, tsetaddr[tspos >> 12] + (tspos & 0xFFF)); tspos += 2; } //tspos &= 0xFFF;
					int pos = tile << 6; // (tile & 0x3FF) << 5;
										 //pos += charBase;
										 //int tilePal = 0;// (tile >> 8) & 0xF0;
					//pos -= 0x4000;
					if (pos < 0) { continue; }
					//int x1, x2, xi, y1, y2, yi; // Not sure how I wanted flipping coded in?  Alternative below, though!
					//if ((tile & 0x400) == 0) { x1 = 0; x2 = 7; xi = 1; } else { x1 = 7; x2 = 0; xi = -1; };
					//if ((tile & 0x800) == 0) { y1 = 0; y2 = 7; yi = 1; } else { y1 = 7; y2 = 0; yi = -1; };
					for (int y = r; y <= r + 7; y++)
					{
						int ry = y; //if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
						//int pix = Bits.getInt32(tsetdat, pos); pos += 4;
						for (int x = c; x <= c + 7; x++)
						{
							int rx = x; //if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
							bmpdata[(ry << 9) + rx] = pal[tsetdat[pos++]];
							//pix >>= 4;
						}
					}
				}
			}
			//}
			//}
			//charBase indicator
			for (int y = 0; y < charBase; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0xF80000;
				bmpdata[(y << 9) + 0x101] = 0xF80000;
			}
			for (int y = charBase; y < charBase + 0x100; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0x00F800;
				bmpdata[(y << 9) + 0x101] = 0x00F800;
			}
			for (int y = charBase + 0x100; y < 0x200; y++)
			{
				bmpdata[(y << 9) + 0x100] = 0xF80000;
				bmpdata[(y << 9) + 0x101] = 0xF80000;
			}
			return PixelsToImage(bmpdata, 0x200, 0x400);
		}
		int wr = 0; //width resize of tile table image.... (0 or 1)
		public Bitmap dispTileset(int charBase)
		{//(ByVal tsslot As Integer) {

			//tsetaddr[0]
			//int maxWidth = (ts.Width & ~0xF) - 17;
			int maxWidth = 0x200;
			if (wr != 0)
				maxWidth = (ts.Width - 33) & ~0xF; //0x200
												   //int maxHeight = 0x8000 / (maxWidth>>4);
			int tspos = 0;
			//int[] bmpdata = new int[0x20000];
			int[] bmpdata = new int[0x80000];
			//for (int row = 0; row < 16; row++) {
			//    for (int col = 0; col < 32; col++) {
			for (int row = 0; row < 0x400; row += 16)
			{
				for (int col = 0; col < maxWidth; col += 16)
				{
					for (int r = row; r < row + 16; r += 8)
					{
						for (int c = col; c < col + 16; c += 8)
						{
							//int tile = rom.getInt16(tsetaddr[0] + tspos); tspos += 2;
							int tile = Bits.getInt16(stiles, tspos); tspos += 2;
							//if (tsetaddr[tspos >> 12] == -1) { tile = 0; } else { tile = Bits.getInt16(rom, tsetaddr[tspos >> 12] + (tspos & 0xFFF)); tspos += 2; } //tspos &= 0xFFF;
							int pos = (tile & 0x3FF) << 5;
							pos += charBase;
							int tilePal = (tile >> 8) & 0xF0;
							pos -= 0x4000; if (pos < 0) { continue; }
							//int x1, x2, xi, y1, y2, yi; // Not sure how I wanted flipping coded in?  Alternative below, though!
							//if ((tile & 0x400) == 0) { x1 = 0; x2 = 7; xi = 1; } else { x1 = 7; x2 = 0; xi = -1; };
							//if ((tile & 0x800) == 0) { y1 = 0; y2 = 7; yi = 1; } else { y1 = 7; y2 = 0; yi = -1; };
							for (int y = r; y <= r + 7; y++)
							{
								int ry = y; if ((tile & 0x800) != 0) { ry = r + ((7 - y) & 7); } //Vert. flip
								int pix = Bits.getInt32(tsetdat, pos); pos += 4;
								for (int x = c; x <= c + 7; x++)
								{
									int rx = x; if ((tile & 0x400) != 0) { rx = c + ((7 - x) & 7); } //Horr. flip
									bmpdata[(ry << 9) + rx] = pal[tilePal | (pix & 0xF)];
									pix >>= 4;
								}
							}
						}
					}
				}
			}
			return PixelsToImage(bmpdata, 0x200, 0x400);// 0x100);
		}
		public Bitmap dispTilesetWM(int charBase)
		{//(ByVal tsslot As Integer) {

			//tsetaddr[0]
			//int maxWidth = (ts.Width & ~0xF) - 17;
			int maxWidth = 0x200;
			if (wr != 0)
				maxWidth = (ts.Width - 33) & ~0xF; //0x200
												   //int maxHeight = 0x8000 / (maxWidth>>4);
			int tspos = 0;
			//int[] bmpdata = new int[0x20000];
			int[] bmpdata = new int[0x80000];
			//for (int row = 0; row < 16; row++) {
			//    for (int col = 0; col < 32; col++) {
			for (int row = 0; row < 0x400; row += 16)
			{
				for (int col = 0; col < maxWidth; col += 16)
				{
					for (int r = row; r < row + 16; r += 8)
					{
						for (int c = col; c < col + 16; c += 8)
						{
							//int tile = rom.getInt16(tsetaddr[0] + tspos); tspos += 2;
							//int tile = Bits.getInt16(stiles, tspos); tspos += 2;
							int tile = stiles[tspos++];
							//if (tsetaddr[tspos >> 12] == -1) { tile = 0; } else { tile = Bits.getInt16(rom, tsetaddr[tspos >> 12] + (tspos & 0xFFF)); tspos += 2; } //tspos &= 0xFFF;
							int pos = (tile & 0x3FF) << 6;
							//pos += charBase;
							//int tilePal = (tile >> 8) & 0xF0;
							//pos -= 0x4000;
							if (pos < 0) { continue; }
							//int x1, x2, xi, y1, y2, yi; // Not sure how I wanted flipping coded in?  Alternative below, though!
							//if ((tile & 0x400) == 0) { x1 = 0; x2 = 7; xi = 1; } else { x1 = 7; x2 = 0; xi = -1; };
							//if ((tile & 0x800) == 0) { y1 = 0; y2 = 7; yi = 1; } else { y1 = 7; y2 = 0; yi = -1; };
							for (int y = r; y <= r + 7; y++)
							{
								int ry = y; if ((tile & 0x800) != 0) { ry ^= 7; } // r + ((7 - y) & 7); } //Vert. flip
								//int pix = Bits.getInt32(tsetdat, pos); pos += 4;
								for (int x = c; x <= c + 7; x++)
								{
									int rx = x; if ((tile & 0x400) != 0) { rx ^= 7; } // c + ((7 - x) & 7); } //Horr. flip
									bmpdata[(ry << 9) + rx] = pal[tsetdat[pos++]];
									//pix >>= 4;
								}
							}
						}
					}
				}
			}
			return PixelsToImage(bmpdata, 0x200, 0x400);// 0x100);
		}
		//private void tWin() {
		//    Form ts = new Form();
		//    //ts.FormBorderStyle = FormBorderStyle.FixedToolWindow;
		//    ts.FormBorderStyle = FormBorderStyle.SizableToolWindow;
		//    ts.ClientSize = new Size(512, 769); //ts.Width = 512; ts.Height = 512;
		//    //ts.StartPosition = FormStartPosition.Manual;// Screen.PrimaryScreen.Bounds.Width - 512; 
		//    //ts.SetDesktopLocation(Screen.PrimaryScreen.Bounds.Width - 525, 25);
		//    //ts.BackgroundImageLayout = ImageLayout.None;
		//    ts.AutoScroll = true; //Adds scrollbars to form.
		//    //ts.StartPosition
		//    ts.Show(this);
		//    PictureBox tspicBox1 = new PictureBox(); //System.Windows.Forms.
		//    tspicBox1.Size = new System.Drawing.Size(0x200, 0x400);
		//    ts.Controls.Add(tspicBox1);
		//    ((PictureBox)ts.Controls[0]).Image = dispTileset();
		//}
		int tileDisp = 0;
		Form ts = new Form(); int lastPressedKey = 0;
		int hideLayer = 0;
		void tWin(int controlSet, int showForm) { //showForm is temp arg??? //updateToolBox
												  //if (ts.Visible == true) { tspicBox1.Image = Map.dispTileset(); }
			if (lastPressedKey != controlSet)
			{
				ts.Controls.Clear();
				lastPressedKey = controlSet;
			}
			else
			{

			}
			if (ts.IsDisposed || !ts.Visible) {
				if (showForm == 0) { return; }
				ts = new Form();
				//ts.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				ts.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				ts.ClientSize = new Size(529, 769); //ts.Width = 512; ts.Height = 512; //512,769
													//ts.StartPosition = FormStartPosition.Manual;// Screen.PrimaryScreen.Bounds.Width - 512; 
													//ts.SetDesktopLocation(Screen.PrimaryScreen.Bounds.Width - 525, 25);
													//ts.BackgroundImageLayout = ImageLayout.None;
				ts.AutoScroll = true; //Adds scrollbars to form.
									  //ts.StartPosition
				ts.Show(this); //Setting the owner to "this" form makes windows stay in front of main form, but not in front of other apps like .TopMost.
				ts.KeyDown += new KeyEventHandler(this.Form1_KeyDown);
				//ts.MouseDown += new MouseEventHandler(this.tsDefocus);

			}
			switch (controlSet) {
				case 0x31:
				case 0x32:
				case 0x33:
					if (ts.Controls.Count == 0) { //Init
						PictureBox tspicBox1 = new PictureBox(); //System.Windows.Forms.
						tspicBox1.Size = new System.Drawing.Size(0x200, 0x400);
						ts.Controls.Add(tspicBox1);
						//tsClick += new System.EventHandler(this.tsClick);
						tspicBox1.MouseDown += new MouseEventHandler(this.tsClick);
						tspicBox1.MouseMove += new MouseEventHandler(this.tsClick);//Move);
						tspicBox1.Paint += new PaintEventHandler(this.tsPaint);
					}
					if (tileDisp == 0)
					{
						if (world_map == 1)
						{
							((PictureBox)ts.Controls[0]).Image = dispTsetImgWM(rom[pos[0] + 7 + (controlSet - 0x31)] * 0x80);
						}
						else
						{
							((PictureBox)ts.Controls[0]).Image = dispTsetImg(rom[pos[0] + 7 + (controlSet - 0x31)] * 0x80);
						}
						//if (showhmap == 1)
						//{
						//	showhmap = 0;
						//	dispMap();
						//}
					}
					else if (tileDisp == 1)
					{
						if (world_map == 1)
						{
							((PictureBox)ts.Controls[0]).Image = dispTilesetWM(rom[pos[0] + 7 + (controlSet - 0x31)] * 0x4000); //tspicBox1.Image = Map.dispTileset();
						}
						else
						{
							((PictureBox)ts.Controls[0]).Image = dispTileset(rom[pos[0] + 7 + (controlSet - 0x31)] * 0x4000); //tspicBox1.Image = Map.dispTileset();
						}
					}
					else if (tileDisp == 2)
					{
						((PictureBox)ts.Controls[0]).Image = PixelsToImage(pal, 16, 16, 32);
					}
					else if (tileDisp == 3)
					{
						((PictureBox)ts.Controls[0]).Image = disphtable();
						//if (showhmap == 0)
						//{
						//	showhmap = 1;
						//	dispMap();
						//}
					}
					else if (tileDisp == 4)
					{
						((PictureBox)ts.Controls[0]).Image = disptmedit();
						dispMap();
					}
					((PictureBox)ts.Controls[0]).Width = ((PictureBox)ts.Controls[0]).Image.Width;
					((PictureBox)ts.Controls[0]).Height = ((PictureBox)ts.Controls[0]).Image.Height;
					break;
				case 0x34: //Solidity
					break;
				case 0x37: //Area Scripts
					if (ts.Controls.Count == 0) {
					}
					//loadAreaData();
					break;
				case 0x38: //Warps
					if (ts.Controls.Count == 0) {
					}

					//if (((NumericUpDown)ts.Controls[1]).Value != 0) { //Commented because changing visibility of warp boxes will set to 0.
					//    ((NumericUpDown)ts.Controls[1]).Value = 0;
					//} else {
					// loadWarpData(); //Not sure of best way to trigger ValueChanged. :(
					//}
					break;
			}
		}
		//int getHeight(int x, int y)
		//{
		//	return getHeight(x, y, 0); //TODO: Change 0.
		//}
		//int[] hv = { 0, 0, 0 }; //init once was not noticeably faster?
		int getHeight(int x, int y, int htTile)
		{
			x &= 0xF; y &= 0xF;
			int h = 0;
			int[] hv = { // Height Values
				((htiles[htTile * 4 + 1] ^ 0x80) - 0x80) * 8,
				((htiles[htTile * 4 + 2] ^ 0x80) - 0x80) * 8,
				((htiles[htTile * 4 + 3] ^ 0x80) - 0x80) * 8
			};
			// Height Values
			//hv[0] = ((htiles[htTile * 4 + 1] ^ 0x80) - 0x80) * 8;
			//hv[1] = ((htiles[htTile * 4 + 2] ^ 0x80) - 0x80) * 8;
			//hv[2] = ((htiles[htTile * 4 + 3] ^ 0x80) - 0x80) * 8;

			switch (htiles[htTile * 4] & 0xF)
			{
				case 0: // Flat ground
					h = hv[0];
					break;
				case 1: // Left/Right stairs
					h = hv[0] + (((hv[1] - hv[0]) * x) >> 4);
					break;
				case 2: // North/South stairs
					h = hv[0] + (((hv[1] - hv[0]) * y) >> 4);
					break;
				case 3: // /
					h = hv[x + y >= 0x10 ? 1 : 0];
					break;
				case 4: // \
					h = hv[y >= x ? 1 : 0];
					break;
				case 5:
					break;
				case 6:
					break;
				case 7: // O
					if (version == 1)
						h = hv[0] + (hv[1] - hv[0]) * rom[0x2EDC4 + y * 0x10 + (x & 0xF)];
					break;
				case 8:
					h = hv[((x >> 3) & 1)];
					break;
				case 9:
					h = hv[(y >= 8 ? 1 : 0)];
					break;
				case 0xD:
					if (version == 1)
						h = hv[rom[0x2EEC4 + y * 0x10 - (x & 0xF) + 0xF]];
					break;
				case 0xE:
					break;
				case 0xF:
					h = hv[(y >= 8 ? 1 : 0) + ((x | y) >= 8 ? 1 : 0)];
					break;
			}
			return h;
		}
		Bitmap disphtable()
		{
			int w = 256 * 16;
			int h = 256 * 8 + 16 + 16 + 32;
			int[] bmpdata = new int[w * h];
			for (int x = 0; x < 0x10 * 0x100; x++)
			{
				int dh = h; //draw height
				for (int y = 15; y >= 0; y--)
				{
					//int fColor = 0x00f800;
					//if (((htiles[(x >> 4) * 4] & 0xF) == 1) || ((htiles[(x >> 4) * 4] & 0xF) == 2))
					//{
					//	fColor = 0xf8f800; //Yellow for stairs.
					//}
					int hy = h - 0x410 + y - getHeight(x, y, x >> 4);
					//while (hy < dh - 12)
					//{
					//	bmpdata[--dh * w + x] = 0x800000;
					//}
					while (hy < dh - 12)
					{
						if (dh > (h - 0x410 + y))
						{
							bmpdata[--dh * w + x] = 0xC00000;
						}
						else
						{
							if (dh <= 0) { break; }
							bmpdata[--dh * w + x] = 0xf80000;
						}
					}
					if (dh <= 0) { break; }
					while (hy < dh - 8)
					{
						//bmpdata[--dh * w + x] = 0xf84000;
						if (dh > (h - 0x410 + y))
						{
							bmpdata[--dh * w + x] = 0xC04000;
						}
						else
						{
							bmpdata[--dh * w + x] = 0xf84000;
						}
					}
					while (hy < dh)
					{
						//bmpdata[--dh * w + x] = 0xf88000;
						if (dh > (h - 0x410 + y))
						{
							bmpdata[--dh * w + x] = 0xC08000;
						}
						else
						{
							bmpdata[--dh * w + x] = 0xf88000;
						}
					}
					if (hy == dh)
					{
						int fColor = 0x00f800;
						if (((htiles[(x >> 4) * 4] & 0xF) == 1) || ((htiles[(x >> 4) * 4] & 0xF) == 2))
						{
							fColor = 0xf8f800; //Yellow for stairs.
						}
						if (dh > (h - 0x410 + y))
						{
							fColor &= 0xC0C000;
						}
						bmpdata[hy * w + x] = fColor; // 0x00f800;
					}
				}
			}
			//return disphmap();// (0);
			return PixelsToImage(bmpdata, w, h);
		}
		Bitmap disphmap()
		{
			int l = lastPressedKey - 0x31;
			if ((l < 0) || (l > 2)) { l = 0; } //{ return; }
			int shiftx = rom[pos[0] + 0xC + (l * 8)], shifty = rom[pos[0] + 0xD + (l * 8)];
			int shift = (shiftx & 0xfe) + ((shifty & 0xfe) << 7);
			if (nlmap != 0)
				shift = 0;
			int w = scnw;// 256 * 16;
			int h = scnh;// 256 * 8 + 16 + 16;
			//int[] bmpdata = new int[w * h];
			int mx = MousePosition.X - pictureBox1.PointToScreen(new Point(0, 0)).X;
			int my = MousePosition.Y - pictureBox1.PointToScreen(new Point(0, 0)).Y;// - PointToScreen(new Point(0,picti
			//int noPartial = 0;
			//if ((mx < 0) || (mx > w) || (my < 0) || (my > h))
			//	noPartial = 1;
			for (int x = 0; x < w; x++)
			{
				int dh = h - 1; //draw height
				for (int y = h - 1; y >= 0; y--)
				{
					int a = (shift >> 1) + (y >> 4) * 0x80 + (x >> 4);
					if (a >= 0x4000)
						continue;
					int ht = 0;// = hmap[(shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
					if (version == 0)
					{
						ht = tmap[0x4000 + (shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
					}
					else if (version == 1)
					{
						ht = hmap[(shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
					}
					int z = getHeight(x, y, ht);
					int hy = y - z;// x >> 4);
					if ((showhmap & 2) != 0)
						hy = y;
					if ((showhmap & 4) != 0) // && (noPartial == 0))
					{
						if ((my >> 4) < (y >> 4))
							hy = y;
						else
							hy = y - z;
					}
					//if (y > (yp << 4))
					//	hy = y;
					if (hy < 0) //Floor cannot be drawn/goes off map.
						continue;
					if (hy > dh) //Floor is hidden.
					{
						//Quick fix
						if (((dh + 1) * w + x) >= (scnw * scnh))
							continue;
						bmpdata[(dh + 1) * w + x] = 0x000000;
						continue;
					}
					int z2 = 0;// z - (dh - hy);
					while (hy + 1 < dh) //Where to apply wall color for this (x,y) section.
					{
						int pixColor = 0x80 + z2++;
						if (pixColor > 0xF0)
							pixColor = 0xF0;
						else if (pixColor < 0x0)
							pixColor = 0x0;

						if (hy + 12 + 1 < dh) //Red wall
							pixColor = 0x010000 * pixColor; //0xC00000;
						else if (hy + 8 + 1 < dh) //Drop wall
							pixColor = 0x010000 * pixColor + 0x002000; //0xC04000;
						else if (hy + 1 < dh) //Climb wall
							pixColor = 0x010000 * pixColor + 0x004000; //0xC08000;

						//bmpdata[dh * w + x] = pixColor;
						//bmpdata[dh * w + x] = blendit(bmpdata[dh * w + x], pixColor, 0x1, 0x10);
						bmpdata[dh * w + x] = pixColor + ((bmpdata[dh * w + x] >> 4) & 0x0F0F0F); //blendit(bmpdata[dh * w + x], pixColor, 0x1, 0x10);
						dh--;
					}
					if (hy < dh) //Floor color
					{
						int pixColor = 0x80 + z;
						if (pixColor > 0xF0)
							pixColor = 0xF0;
						else if (pixColor <= 0x0)
							pixColor = 0x0;

						if (((htiles[(ht) * 4] & 0xF) == 1) || ((htiles[(ht) * 4] & 0xF) == 2)) //Stairs
							pixColor = 0x010100 * pixColor;
						else //Flat Floor
							pixColor = 0x000100 * pixColor;

						//bmpdata[dh * w + x] = pixColor;
						//bmpdata[dh * w + x] = blendit(bmpdata[dh * w + x], pixColor, 0x1, 0x10); //pixColor;
						bmpdata[dh * w + x] = pixColor + ((bmpdata[dh * w + x] >> 4) & 0x0F0F0F);
						dh--;
					}
				}
			}
			//dispPrioMap();
			return null;// PixelsToImage(bmpdata, w, h);
		}
		Bitmap disptmedit()
		{
			int[] bmpdata = new int[0x80000];
			int w = 0x200;
			int h = 0x400;
			int y = 0;
			//Layer switching
			for (int k = 0; k < 0x40; k += 16)
			{
				for (int i = 0; i < 16; i++)
				{
					for (int j = k; j < k + 16; j++)
					{
						bmpdata[i * w + j] = k << 2;//(j >> 4) << 6;
					}
				}
			}
			y += 0x10;
			//Sprite Priority
			for (int k = 0; k < 0x40; k += 16)
			{
				for (int i = y; i < y+ 16; i++)
				{
					for (int j = k; j < k + 16; j++)
					{
						bmpdata[i * w + j] = k << 2;//(j >> 4) << 6;
					}
				}
			}
			y += 0x10;
			//Event ID
			for (int l = 0; l < 0x100; l += 16)
			{
				for (int k = 0; k < 0x100; k += 16)
				{
					for (int i = y; i < y + 16; i++)
					{
						for (int j = k; j < k + 16; j++)
						{
							bmpdata[i * w + j] = 0x202020 + (l<<15) + (k>>1);//(j >> 4) << 6;
						}
					}
				}
				y += 0x10;
			}
			for (int l = 0; l < 0x80; l += 16)
			{
				for (int k = 0; k < 0x20; k += 16)
				{
					for (int i = y; i < y + 16; i++)
					{
						for (int j = k; j < k + 16; j++)
						{
							bmpdata[i * w + j] = 0x202020 + (l << 15) + (k >> 1);//(j >> 4) << 6;
						}
					}
				}
				y += 0x10;
			}

			return PixelsToImage(bmpdata, 0x200, 0x400); ;
		}
		Bitmap dispPrioMap() //Nevermind Prio, but I'll just use it for now b/c I'm bad with names.
		{
			int l = lastPressedKey - 0x31;
			if ((l < 0) || (l > 2)) { l = 0; } //{ return; }
			int shiftx = rom[pos[0] + 0xC + (l * 8)], shifty = rom[pos[0] + 0xD + (l * 8)];
			int shift = (shiftx & 0xfe) + ((shifty & 0xfe) << 7);
			if (nlmap != 0)
				shift = 0;
			int w = scnw;// 256 * 16;
			int h = scnh;// 256 * 8 + 16 + 16;
			//int[] bmpdata = new int[w * h];
			//int mx = MousePosition.X - pictureBox1.PointToScreen(new Point(0, 0)).X;
			//int my = MousePosition.Y - pictureBox1.PointToScreen(new Point(0, 0)).Y;// - PointToScreen(new Point(0,picti
																					//int noPartial = 0;
																					//if ((mx < 0) || (mx > w) || (my < 0) || (my > h))
																					//	noPartial = 1;

			for (int row = 0; row < (scnh); row += 16)
			{
				for (int col = 0; col < (scnw); col += 16)
				{
					//int t = Bits.getInt16(tmap, 0x8000 + dx + shift + (dy << 7)) & 0xFFF;
					if (0x8000 + (col >> 3) + shift + (row << 4) > 0x10000)
						break;
					int t = 0;// = Bits.getInt16(tmap, 0x8000 + (col >> 3) + shift + (row << 4));// >> 0xE;
							  //try {
							  //if ((t & 0x800) != 0)
							  //	MessageBox.Show(t.ToString("X4"));
					//int color2 = 0;
					if (tey < 0x10)
					{
						int i = 0x8001 + ((col >> 4) << 1) + shift + ((row >> 4) << 8);
						//tmap[i] = (byte)((tmap[i] & 0xCF) | ((tex >> 4) << 4));
						t = (tmap[i] &0x30) << 2;
					}
					else if (tey < 0x20)
					{
						int i = 0x8001 + ((col >> 4) << 1) + shift + ((row >> 4) << 8);
						//tmap[i] = (byte)((tmap[i] & 0x3F) | ((tex >> 4) << 6));
						t = (tmap[i] & 0xC0);
					}
					else if (tey < 0x120)
					{
						//tmap[0x0000 + ((col >> 4)) + shift + ((row >> 4) << 7)] = (byte)(((tey - 0x20) & 0xF0) | (tex >> 4));
						int i = 0x0000 + ((col >> 4)) + (shift >> 1) + ((row >> 4) << 7);
						t = tmap[i];
						//t |= (0xF0 - t) << 16;
						t = ((t & 0xF0) << 15) | ((t & 0xF) << 3);
						//if (tmap[i] == 0xFB)
						//	color2 = 0xFFFFFF;
					}
					else if (tey < 0x1A0)
					{
						int i = 0x4000 + ((col >> 4)) + (shift >> 1) + ((row >> 4) << 7);
						t = (tmap[i] >> ((tey - 0x120) >> 4)) & 1;
						t <<= 6;
					}
					int color = t;//|color2;// & 0x3000;
					//if ((0x8000 + (col >> 3) + shift + (row << 4)) == 0x8e40)//0x9C80) // 0xA440)
					//	if (color == 0)
					//		color = 0xffffff;
					for (int y = row; y < row + 16; y++)
					{
						for (int x = col; x < col + 16; x++)
						{
							//bmpdata[y * w + x] = color;
							bmpdata[y * w + x] = color + ((bmpdata[y * w + x] >> 4) & 0x0F0F0F);
						}
					}
				}
			}
			//for (int x = 0; x < w; x++)
			//{
			//	int dh = h - 1; //draw height
			//	for (int y = h - 1; y >= 0; y--)
			//	{
			//		int ht = hmap[(shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
			//	}
			//}
			return null;// PixelsToImage(bmpdata, w, h);
		}

		int t8ind = 0;
        int t16ind = 0;
		int palind = 0;
		int tex = 0, tey = 0;
        void tsClick(object sender, MouseEventArgs e) {
            if (MouseButtons != MouseButtons.Left) { return; }
			if (tileDisp == 0)
			{
				int a = rom[pos[0] + 7 + (lastPressedKey - 0x31)] * 0x80;
				if ((e.Y < a) || ((a + 0x100) <= e.Y)) { return; }
				if ((e.X < 0) || (0x100 <= e.X)) { return; }
				t8ind = (((e.Y - a) >> 3) << 5) | (e.X >> 3);
			}
			else if (tileDisp == 1)
			{
				t16ind = ((e.Y >> 4) << 5) | (e.X >> 4);
			}
			else if (tileDisp == 2)
			{
				palind = ((e.Y >> 5) << 4) | (e.X >> 5);
				ColorDialog cd = new ColorDialog();
				if (cd.ShowDialog() == DialogResult.OK)
				{
					pal[palind] = cd.Color.ToArgb() & 0x00F8F8F8;

					uint p = (uint)pal[palind];
					p = (p << 0x8 >> 0x1B) | (p << 0x10 >> 0x1B << 5) | (p << 0x18 >> 0x1B << 10);
					Bits.setInt16(paldat, palind * 2, (int)p);
					byte[] temp = new byte[0x10000];
					temp[0] = 1; //Compression Type 01
					int size = Comp.compressFormat1(paldat, 0, temp, 1)[1];
					size = (size + 3) & ~3;
					Array.Resize<byte>(ref temp, size);
					insertMFTfile(palfind, temp);

					tWin(lastPressedKey, 0);
					dispMap(); //loadEntireMap();
				}
			}
			else if (tileDisp == 3)
			{
				hind = e.X >> 4;
				if ((Control.ModifierKeys & Keys.Control) != 0) //if (hind == e.X >> 4)
				{
					((PictureBox)ts.Controls[0]).Invalidate();
					string input = Bits.getInt32(htiles, hind * 4).ToString("X8");
					if (ShowInputDialog(ref input) == DialogResult.OK)
					{
						Bits.setInt32(htiles, hind * 4, Convert.ToInt32(input, 16));
						((PictureBox)ts.Controls[0]).Image = disphtable();
					}
				}
			} else
			{
				tex = e.X;
				tey = e.Y;
				dispMap(); //In case a different overlay should display.
			}
			((PictureBox)ts.Controls[0]).Invalidate();
            //tWin(lastPressedKey, 0);
        }
		int hind = 0;
        void tsPaint(object sender, PaintEventArgs e) {
            if (tileDisp == 0) {
                int a = rom[pos[0] + 7 + (lastPressedKey - 0x31)] * 0x80;
                e.Graphics.DrawRectangle(new Pen(Brushes.Red), (t8ind & 0x1F) << 3, a + (((t8ind >> 5) & 0x1F) << 3), 7, 7);
            } else if (tileDisp == 1) {
                e.Graphics.DrawRectangle(new Pen(Brushes.Red), (t16ind & 0x1F) << 4, ((t16ind >> 5) & 0x1F) << 4, 15, 15);
			}
			else if (tileDisp == 2)
			{
			}
			else if (tileDisp == 3)
			{
				e.Graphics.DrawRectangle(new Pen(Brushes.Cyan), (hind & 0xFF) << 4, 0, 15, 0x104 * 8 - 1);
			}
			else if (tileDisp == 4)
			{
				e.Graphics.DrawRectangle(new Pen(Brushes.Cyan), tex & ~ 0xF, tey & ~0xF, 15, 15);
			}
        }
        int mxp = 0, myp = 0;
		int onClick = 0; //0=No click, 1=Just clicked, 2=Still clicked. //int mxClick = 0, myClick = 0;
		void mapClick(object sender, MouseEventArgs e) { //MouseDown and MouseMove
			if ((MouseButtons == MouseButtons.Left) || (MouseButtons == MouseButtons.Right))
				onClick += 1;
			else
				onClick = 0;
			if (onClick > 2) //0 or 2 if called from MouseMove ; 1 if MouseDown.
				onClick = 2;
			//Return if on same tile, because we don't want to slow things down doing redundant actions.
			if ((mxp >> 4 == e.X >> 4) && (myp >> 4 == e.Y >> 4))
			{
				mxp = e.X; myp = e.Y;
				if (onClick != 1)
					return;
			}
			mxp = e.X; myp = e.Y;
			if ((showhmap & 4) != 0)
				dispMap();
			//Return if not Left mouse button was moved down.

			//if ((mxClick >> 4 == e.X >> 4) && (myClick >> 4 == e.Y >> 4))
			//{
			//	mxClick = e.X; myClick = e.Y;
			//	return;
			//}
			//int layiOffset = 1 << (3 - l);
			//        int shiftx = rom[pos[0] + 0xC + (l * 8)], shifty = rom[pos[0] + 0xD + (l * 8)];
			//        int shift = (shiftx & 0xfe) + ((shifty & 0xfe) << 7);
			//        for (int row = 0; row < (scnh); row += 16) {
			//            for (int col = 0; col < (scnw); col += 16) {
			//                //int t = Bits.getInt16(tmap, 0x8000 + dx + shift + (dy << 7)) & 0xFFF;
			//                int t = (Bits.getInt16(tmap, 0x8000 + (col >> 3) + shift + (row << 4)) & 0xFFF) * 8;

			int l = lastPressedKey - 0x31;
			if ((l < 0) || (l > 2)) { return; }
			int shiftx = rom[pos[0] + 0xC + (l * 8)], shifty = rom[pos[0] + 0xD + (l * 8)];
			int shift = (shiftx & 0xfe) + ((shifty & 0xfe) << 7);
			if (nlmap != 0)
				shift = 0;
			if ((e.X < 0) || (e.Y < 0))
				return;
			if ((e.X >= scnw) || (e.Y >= scnh))
				return;
			int t = 0;
			byte[] mapdata = null; //World Map
			int cSize = 0;
			if (world_map == 1)
			{
				mapdata = new byte[0x400];
				int wmpos = Bits.getInt32(rom, 0x680000 + 4 * 0x197) & 0x1FFFFFF;
				int iOffset = (((e.Y >> 8) * (scnw >> 8)) + (e.X >> 8)) * 4;
				int rptr = Bits.getInt32(rom, wmpos + iOffset); //iOffset += 4;
				if (rptr == 0)
					return;
				cSize = Comp.decompressf(rom, wmpos + rptr, mapdata, 0x0000, 1)[0] - rptr - wmpos; //Map data
				
				//t = Bits.getInt16(mapdata, (((e.X & 0xFF) >> 4) << 2) + shift + (((e.Y & 0xFF) >> 4) << 8));
				t = Bits.getInt16(mapdata, (((e.X & 0xFF) >> 4) << 2) + (((e.Y & 0xFF) >> 4) << 6));
			}
			else
			{
				t = Bits.getInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8));// & 0xF000) | t16ind;
																								//MessageBox.Show(e.X.ToString());
			}
			if (MouseButtons == MouseButtons.Right)
			{
				t16ind = t & 0xFFF;
				((PictureBox)ts.Controls[0]).Invalidate();
				return;
			}
			if (MouseButtons != MouseButtons.Left)
			{
				return;
			}

			if (tileDisp == 0)
			{ //Check if replacing tile is in use, then insert tile if enough space.
			  //if (((xp >> 3) == (e.X >> 3)) && ((yp >> 3) == (e.Y >> 3))) { return; }
			  //xp = e.X; yp = e.Y;

				// Bits.setInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8), t);
				//for (int i = 0; i < 0x8000; i+= 2) Bits.setInt16(tmap, i + 0x8000, i);
				byte[] data = new byte[8];
				for (int i = 0; i < 8; i++)
				{
					data[i] = stiles[((t & 0xFFF) * 8) + i];
				}
				//TODO: CHANGE DATA
				//(t8ind & 0x1F) << 3, a + (((t8ind >> 5) & 0x1F) << 3)
				int j = ((e.Y & 8) >> 1) + ((e.X & 8) >> 2);
				data[j] = (byte)t8ind;
				data[j + 1] = (byte)(t8ind >> 8);

				int t8a = 0; while (t8a < (t8amt * 8))
				{
					int i = 0; while (i < 8)
					{
						if (stiles[t8a + i] != data[i]) { break; }
						i++;
					}
					if (i == 8) { break; }
					//    //if (i < 8) { continue; } break;
					t8a += 8;
				}
				if ((t8a >> 3) < 0x800)
				{
					if (t8a == (t8amt * 8))
					{
						for (int i = 0; i < 8; i++)
						{
							stiles[(t8amt * 8) + i] = data[i];
						}
						t8amt++;
					}
					Bits.setInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8), (t & 0xF000) | (t8a >> 3));
					//Bits.setInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8), (t & 0xF000) | t16ind);
					//ind[t] = t8amt++; //If code is un-commented, make ind[t] = t8a>>3. (Outside if loop.) 
				}
				dispMap();
			}
			else if (tileDisp == 1)
			{ //Just set tile.
			  //if (((xp >> 4) == (e.X >> 4)) && ((yp >> 4) == (e.Y >> 4))) { return; }
			  //xp = e.X; yp = e.Y;
				if ((t & 0xFFF) == t16ind) { return; }
				if (world_map == 1)
				{
					Bits.setInt16(mapdata, (((e.X & 0xFF) >> 4) << 2) + (((e.Y & 0xFF) >> 4) << 6), t16ind);
					//mapdata = new byte[0x800];
					int wmpos = Bits.getInt32(rom, 0x680000 + 4 * 0x197) & 0x1FFFFFF;
					int wmpos2 = Bits.getInt32(rom, 0x680000 + 4 * 0x198) & 0x1FFFFFF;
					int wmsize = wmpos2 - wmpos;
					int iOffset = (((e.Y >> 8) * (scnw >> 8)) + (e.X >> 8)) * 4;
					int rptr = Bits.getInt32(rom, wmpos + iOffset); //iOffset += 4;
					//int rptr = Bits.getInt32(rom, wmpos + iOffset); //iOffset += 4;
					if (rptr == 0)
						return;
					byte[] data = new byte[0x800];
					int cSize2 = Comp.compressFormat1(mapdata, 0, data, 0)[1];
					int relSize = cSize2 - cSize;
					
					if (relSize > 0)
					{
						MessageBox.Show(">0");
						offsetMFT(0x197, wmsize + relSize);
						for (int i = wmsize - 1; (rptr + cSize) < i; i--)
						{
							rom[wmpos + i + relSize] = rom[wmpos + i];
						}
					}
					else
					{
						MessageBox.Show("<0");
						for (int i = (rptr + cSize); i < wmsize; i++)
						{
							rom[wmpos + i + relSize] = rom[wmpos + i];
						}
						offsetMFT(0x197, wmsize + relSize);
					}

					//Change pointers
					for (int i = wmpos + iOffset + 4; i < wmpos + 0x2000; i += 4)
					{
						int j = Bits.getInt32(rom, i);
						if (j != 0)
							Bits.setInt32(rom, i, j + relSize);
					}

					//Insert data
					for (int i = 0; i < cSize2; i++)
					{
						rom[wmpos + rptr + i] = data[i];
					}

					//Comp.decompressf(rom, wmpos + rptr, mapdata, 0x0000, 1); //Map data

					//t = Bits.getInt16(mapdata, (((e.X & 0xFF) >> 4) << 2) + shift + (((e.Y & 0xFF) >> 4) << 8));
					//t = Bits.getInt16(mapdata, (((e.X & 0xFF) >> 4) << 2) + (((e.Y & 0xFF) >> 4) << 6));
				}
				else
				{
					//t = Bits.getInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8));// & 0xF000) | t16ind;
					//MessageBox.Show(e.X.ToString());
					Bits.setInt16(tmap, 0x8000 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8), (t & 0xF000) | t16ind);
				}
				//for (int i = 0; i < 0x8000; i+= 2) Bits.setInt16(tmap, i + 0x8000, i);
				dispMap();
			}
			else if (tileDisp == 2) //Palette
			{
			}
			else if (tileDisp == 3) //H-table
			{
				if (version == 0)
				{
					//ht = tmap[0x4000 + (shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
					tmap[0x4000 + ((e.X >> 4) << 0) + (shift >> 1) + ((e.Y >> 4) << 7)] = (byte)hind;
				}
				else if (version == 1)
				{
					//ht = hmap[(shift >> 1) + (y >> 4) * 0x80 + (x >> 4)];
					hmap[((e.X >> 4) << 0) + (shift >> 1) + ((e.Y >> 4) << 7)] = (byte)hind;
				}
				dispMap();
			}
			else if (tileDisp == 4)
			{
				if (tey < 0x10)
				{
					int i = 0x8001 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8);
					tmap[i] = (byte)((tmap[i] & 0xCF) | ((tex >> 4) << 4));
				}
				else if (tey < 0x20)
				{
					int i = 0x8001 + ((e.X >> 4) << 1) + shift + ((e.Y >> 4) << 8);
					tmap[i] = (byte)((tmap[i] & 0x3F) | ((tex >> 4) << 6));
				}
				else if (tey < 0x120)
				{
					tmap[0x0000 + ((e.X >> 4)) + (shift >> 1) + ((e.Y >> 4) << 7)] = (byte)(((tey - 0x20) & 0xF0) | (tex >> 4));
				}
				else if (tey < 0x1A0)
				{
					int i = 0x4000 + ((e.X >> 4)) + (shift >> 1) + ((e.Y >> 4) << 7);
					tmap[i] = (byte)((tmap[i] & ~(1 << ((tey - 0x120) >> 4))) | ((tex >> 4) << ((tey - 0x120) >> 4)));
				}
				dispMap();
			}
				//sortTiles(); //Re-organize tiles.
				//insertMFTmapfiles();
		}
        int t8amt = 0;
        void sortTiles() {
            int[] freq = new int[0x1000];
            int[] ind = new int[0x1000];
            byte[] stiles2 = new byte[0x10000];
            t8amt = 0;
            for (int ti = 0x8000; ti < 0x10000; ti += 2) {
                int ta = Bits.getInt16(tmap, ti);
                int t = ta & 0xFFF;
                if (freq[t] == 0) {
                    //int t8a = 0; while (t8a < (t8amt * 8)) {
                    //    int i = 0; while (i < 8) {
                    //        if (stiles2[t8a + i] != stiles[t * 8 + i]) { break; }
                    //        i++;
                    //    }
                    //    if (i == 8) { break; }
                    //    //if (i < 8) { continue; } break;
                    //    t8a += 8;
                    //}
                    //if (t8a == (t8amt * 8)) {
                        for (int i = 0; i < 8; i++) {
                            stiles2[(t8amt * 8) + i] = stiles[t * 8 + i];
                        }
                        ind[t] = t8amt++; //If code is un-commented, make ind[t] = t8a>>3. (Outside if loop.) 
                    //}
                    //t = t8a >> 3;
                    //Bits.setInt16(tmap, ti, (ta & 0xF000) | (t8a >> 3));
                    //Bits.setInt16(tmap, ti, (ta & 0xF000) | t8amt);
                }
                Bits.setInt16(tmap, ti, (ta & 0xF000) | ind[t]);
                freq[t]++;
            }
            stiles = stiles2;
        }

		void copy(byte[] src, int srcPos, byte[] des, int desPos, int size)
		{
			for (int i = 0; i < size; i++)
			{
				des[desPos + i] = src[srcPos + i];
			}
			//return desPos + size;
		}
		//void copycompfile(byte[] data, int desPos, int fileRef) //Copy compressed file. //Thought it'd be easier to do in parent function.
		//{
		//	if (fileRef == 0)
		//	{
		//		Bits.setInt32(data, desPos, 0);
		//		return;
		//	}
		//	copy(rom, addr, data, 0, 0x40);
		//}
		void insertMFTmapfiles()
		{ //int index) {
		  int index = mapfind;
		  //indexaddr = 0x680000 + index * 4;
			int addr = Bits.getInt32(rom, 0x680000 + index * 4) & 0x1ffffff;
			//int addr2 = Bits.getInt32(rom, 0x680000 + index * 4 + 4) & 0x1ffffff;

			//byte[] stiles; byte[] tmap;
			byte[] mapdata = new byte[0x10000];
			//byte[] cttable = new byte[0x8000];

			copy(rom, addr, mapdata, 0, 0x40);
			int desPos = 0x40;
			byte[] temp = new byte[0x10000];
			for (int i = 0; i < 7; i++)
			{
				int fileRef = Bits.getInt32(rom, addr + 0x24 + i * 4);
				if (fileRef == 0)
				{
					Bits.setInt32(mapdata, 0x24 + i * 4, 0);
					continue;
				}
				Bits.setInt32(mapdata, 0x24 + i * 4, desPos);
				if (i == 2)
				{
					mapdata[desPos++] = 1;
					desPos = Comp.compressFormat1(tmap, 0, mapdata, desPos)[1];
					desPos += 3;
					desPos &= ~3;
					continue;
				}
				else if (i == 3)
				{
					mapdata[desPos++] = 1;
					desPos = Comp.compressFormat1(hmap, 0, mapdata, desPos)[1];
					desPos += 3;
					desPos &= ~3;
					continue;
				}
				int srcPos1 = addr + fileRef;
				int srcPos2 = Comp.decompress(rom, srcPos1, temp, 0)[0]; //Only care about compressed size. (srcPos2=after compressed file)
				int size = srcPos2 - srcPos1;
				copy(rom, srcPos1, mapdata, desPos, size);
				desPos += size;
				desPos += 3;
				desPos &= ~3;
			}

			Array.Resize<byte>(ref mapdata, desPos);
			insertMFTfile(index, mapdata);
			//mapdata[desPos++] = 1;

			//int ctmsize = Comp.compressFormat1(tmap, 0, ctmap, 1)[1];
			//int cttsize = Comp.compressFormat1(stiles, 0, cttable, 1)[1];

			//decompTileData(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x24)), stiles, 0); //8x8 tile table - Decompress & Deobfuscate.
			//Height Tile Data - Skip for now.
			//decompMap2(rom, pos[0] + (Bits.getInt32(rom, pos[0] + 0x2C)), tmap, 0); //16x16 tilemap - Decompress & Deobfuscate.
			//Height Tilemap - Skip for now.
			//Animation


			//int size = ctmsize + cttsize;
			
			//offsetMFT(index, size);

			//int addr = pos[0];
			//while (true)
			//{

			//}
			
		}

		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			toolStripButton7.Text = "Processing...";
			toolStrip1.Update();
			//toolStripButton7.Invalidate();
			insertMFTmapfiles();
			toolStripButton7.Text = "Done";
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		int offsetMFT(int index, int size) //Makes it so the file at this index has this size.
		{
			//Get info for what data to move and by how much... etc.
			byte[] temp = new byte[0x10000];
			int entryaddr = 0x680000 + 2047 * 4;
			int lastfileaddr = 0;
			while (true) {
				lastfileaddr = Bits.getInt32(rom, entryaddr);
				if (lastfileaddr != 0) break;
				entryaddr -= 4;
			}
			int eofaddr = Comp.decompress(rom, lastfileaddr & 0x1ffffff, temp, 0)[0];
			index = 0x680000 + index * 4;
			int addr = Bits.getInt32(rom, index) & 0x1ffffff;
			int addr2 = Bits.getInt32(rom, index + 4) & 0x1ffffff;
			int offset = size - (addr2 - addr);
			//MessageBox.Show(eofaddr.ToString("x8"));
			//Move contents to either remove the gap or make enough space for the file.
			if (offset < 0)
			{
				int i;
				for (i = addr2; i < eofaddr; i++)
				{
					rom[i + offset] = rom[i];
				}
				while ((i+offset) < eofaddr)
				{
					rom[i++ + offset] = 0;
				}
			}
			else if (offset > 0)
			{
				if (0xF79650 < eofaddr + offset)
				{
					//MEGAROMS Bootleg really shouldn't be in here. Shame! So let's delete it the way it should be! Happy editing!
					if (Bits.getInt32(rom, 0x468) == 0x08F9EE50)
					{
						Bits.setInt32(rom, 0x468, 0x0801319D);
						for (int i = eofaddr; i < 0xF9EFE8; i++)
						{
							rom[i] = 0;
						}
					}
				}
				//Check to make sure we're just overwriting 0s.
				for (int i = eofaddr; i < eofaddr + offset; i++)
				{
					if (rom[i] != 0)
					{
						string hex = "";
						for (int j = i & ~3; j < i + 0x80; j+=0x10)
						{
							hex += "\n" + string.Format("{0} {1} {2} {3}",
							Bits.getInt32(rom, j).ToString("X8"),
							Bits.getInt32(rom, j + 0x4).ToString("X8"),
							Bits.getInt32(rom, j + 0x8).ToString("X8"),
							Bits.getInt32(rom, j + 0xC).ToString("X8"));
						}
						if (MessageBox.Show("Okay to overwrite data at " + i.ToString("X8") + "? (" + eofaddr.ToString("X8") + "-" + (eofaddr + offset).ToString("X8") + ")\nSample data:" + hex,"Overwrite?",MessageBoxButtons.OKCancel) == DialogResult.Cancel)
						{
							return -1;
						}
						else
						{
							break;
						}
					}
				}
				//Move the data.
				for (int i = eofaddr - 1; addr2 <= i; i--)
				{
					rom[i + offset] = rom[i];
				}
			}
            //Update the pointer list!
            //for (int i = index + 4; i <= entryaddr; i += 4)
            //{
            //    Bits.setInt32(rom, i, Bits.getInt32(rom, i) + offset);
            //}

            //Update the pointer list! (v2 - Assumes not all files are in order.)
            int startRange = Bits.getInt32(rom, index);
            int endRange = eofaddr | 0x08000000;
            for (int i = 0x680000; i <= entryaddr; i += 4)
            {
                int ptr = Bits.getInt32(rom, i);
                if ((ptr > startRange) && (ptr < endRange))
                {
                    Bits.setInt32(rom, i, ptr + offset);
                }
            }
            return addr;
		}
		void insertMFTfile(int index, byte[] data)
		{
			int addr = offsetMFT(index, data.Length);
			if (addr == -1)
				return;
			//Actually insert the file for real!
			for (int i = 0; i < data.Length; i++)
			{
				rom[addr + i] = data[i];
			}
		}

		private static DialogResult ShowInputDialog(ref string input)
		{
			System.Drawing.Size size = new System.Drawing.Size(200, 70);
			Form inputBox = new Form();

			inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			inputBox.ClientSize = size;
			inputBox.Text = "Input";

			System.Windows.Forms.TextBox textBox = new TextBox();
			textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
			textBox.Location = new System.Drawing.Point(5, 5);
			textBox.Text = input;
			inputBox.Controls.Add(textBox);

			Button okButton = new Button();
			okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(75, 23);
			okButton.Text = "&OK";
			okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
			inputBox.Controls.Add(okButton);

			Button cancelButton = new Button();
			cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new System.Drawing.Size(75, 23);
			cancelButton.Text = "&Cancel";
			cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
			inputBox.Controls.Add(cancelButton);

			inputBox.AcceptButton = okButton;
			inputBox.CancelButton = cancelButton;

			DialogResult result = inputBox.ShowDialog();
			input = textBox.Text;
			return result;
		}
	}
}