using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //MessageBox

namespace gsmagic
{
	class Dark_Dawn
	{
		public void extract()
		{
			byte[] buffer = System.IO.File.ReadAllBytes(@"C:\Users\Tea\Desktop\yamata.dmp");// gs3battleram.dmp");
			//int entry = 0;
			int address = 0x23C310;//int address = 0x17726C;
			System.Text.StringBuilder strbuild = new System.Text.StringBuilder(0x1000);

			//int entries = 130;
			while (true)
			{
				int a = Bits.getInt32(buffer, address);
				if (a == -1) break;
				getText(strbuild, buffer, address);
				strbuild.AppendLine();
				address += 4;
			}
			System.IO.File.WriteAllText(@"C:\Users\Tea\Desktop\text.txt", strbuild.ToString());
				//while (address < 0x17BE64) {


				//	byte[] buffer = System.IO.File.ReadAllBytes(@"C:\Users\Tea\Desktop\gs3battleram.dmp");
				//	int entry = 0;
				//	int address = 0x668b8;//int address = 0x17726C;
				//	System.Text.StringBuilder strbuild = new System.Text.StringBuilder(160);

				//	int entries = 130;
				//	while (entries-- > 0) {
				//while (address < 0x17BE64) {
				//int nameID = Bits.getInt16(buffer, address);

				//if (entry + 0xB != nameID)
				//MessageBox.Show(entry + "---" + nameID);
				//int addr2 = Bits.getInt32(buffer, 0x23C328 + ((0x16E + entry) << 2));
				//int length = 20; int pos = addr2 & 0xFFFFFF;
				////MessageBox.Show(addr2.ToString("x8"));
				//while (length-- > 0)
				//{
				//	int c = Bits.getInt16(buffer, pos); pos += 2;
				//	if (c == 0)
				//		break;
				//	if (c < 0x100)
				//		strbuild.Append((char)c);
				//	//else
				//	//	strbuild.Append("["+(char)c+"]");
				//}

				//return strbuild.ToString();
				//MessageBox.Show(strbuild.ToString());
				//There's a hard-limit of <20 for Enemy Names, anyway.
				//strbuild.AppendLine();

				//getText(strbuild, buffer, 0x23C328 + ((0x16E + entry) << 2));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt32(buffer, address));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 4]);
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 5]);
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt16(buffer, address + 6));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt16(buffer, address + 8));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt16(buffer, address + 10));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt16(buffer, address + 12));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + Bits.getInt16(buffer, address + 14));
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 16]);
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 17]);
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 18]);
				//strbuild.Append((char)0x09);
				//strbuild.Append("" + buffer[address + 19]);

				//int itemid = Bits.getInt16(buffer, address + 0x14);
				//getText(strbuild, buffer, 0x220E54 + ((0 + itemid) << 2));
				//int itemamt = buffer[address + 0x1C];
				//if (itemamt != 0)
				//	strbuild.Append(" x" + itemamt);
				//strbuild.Append((char)0x09);
				//itemid = Bits.getInt16(buffer, address + 0x16);
				//getText(strbuild, buffer, 0x220E54 + ((0 + itemid) << 2));
				//itemamt = buffer[address + 0x1D];
				//if (itemamt != 0)
				//	strbuild.Append(" x" + itemamt);
				//strbuild.Append((char)0x09);
				//itemid = Bits.getInt16(buffer, address + 0x18);
				//getText(strbuild, buffer, 0x220E54 + ((0 + itemid) << 2));
				//itemamt = buffer[address + 0x1E];
				//if (itemamt != 0)
				//	strbuild.Append(" x" + itemamt);
				//strbuild.Append((char)0x09);
				//itemid = Bits.getInt16(buffer, address + 0x1A);
				//getText(strbuild, buffer, 0x220E54 + ((0 + itemid) << 2));
				//itemamt = buffer[address + 0x1F];
				//if (itemamt != 0)
				//	strbuild.Append(" x" + itemamt);
				//strbuild.Append((char)0x09);

				//int eType = buffer[address + 0x20];
				//if (eType == 0)
				//	strbuild.Append("Venus");
				//if (eType == 1)
				//	strbuild.Append("Mercury");
				//if (eType == 2)
				//	strbuild.Append("Mars");
				//if (eType == 3)
				//	strbuild.Append("Jupiter");
				//if (eType == 4)
				//	strbuild.Append("Neutral");

				//strbuild.Append(buffer[address + 0x21]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x22]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x23]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x24]);
				//strbuild.Append((char)0x09);

				//if (buffer[address + 0x25] != 0)
				//	strbuild.Append("YAY!!!"); //Making it obvious. / There were no matches.
				//strbuild.Append(buffer[address + 0x25]);

				//strbuild.Append(Bits.getInt16(buffer, address + 0x26));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x28));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x2A));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x2C));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x2E));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x30));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x32));
				//strbuild.Append((char)0x09);
				//strbuild.Append(Bits.getInt16(buffer, address + 0x34));
				//strbuild.Append((char)0x09);

				//strbuild.Append(buffer[address + 0x36]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x37]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x38]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x39]); //All 0s, so likely unused.
				//strbuild.Append((char)0x09);

				//int f = buffer[address + 0x38];
				//for (int i = 0; i < 8; i++)
				//{
				//	if ((f & 1) == 1)
				//		strbuild.Append("*");
				//	f >>= 1;
				//	int psyid = Bits.getInt16(buffer, address + 0x3A + i * 2);
				//	getText(strbuild, buffer, 0x221B5C + ((0 + psyid) << 2));
				//	//strbuild.Append();
				//	strbuild.Append((char)0x09);
				//}

				//strbuild.Append(buffer[address + 0x4A]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x4B]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0x4C]);

				//strbuild.Append(Bits.getInt16(buffer, address + 0x4E));

				//int itemid = Bits.getInt16(buffer, address + 0x50);
				//getText(strbuild, buffer, 0x220E54 + ((0 + itemid) << 2));
				//strbuild.Append((char)0x09);

				//int ic = Bits.getInt16(buffer, address + 0x52);
				//if (ic == 0)
				//	strbuild.Append("Never");
				//if (ic == 1)
				//	strbuild.Append("1/1");
				//if (ic == 2)
				//	strbuild.Append("1/2");
				//if (ic == 3)
				//	strbuild.Append("1/4");
				//if (ic == 4)
				//	strbuild.Append("1/8");
				//if (ic == 5)
				//	strbuild.Append("1/16");
				//if (ic == 6)
				//	strbuild.Append("1/32");
				//if (ic == 7)
				//	strbuild.Append("1/64");
				//if (ic == 8)
				//	strbuild.Append("1/128");
				//if (ic == 9)
				//	strbuild.Append("1/256");
				//strbuild.Append(Bits.getInt16(buffer, address + 0x52));

				//strbuild.Append(Bits.getInt16(buffer, address + 0x54));
				//getText(strbuild, buffer, 0x222EAC + ((0 + entry) << 2));

				//strbuild.Append(Bits.getInt32(buffer, address - 4)); //ID
				//strbuild.Append(Bits.getInt32(buffer, address)); //Class Type
				//strbuild.Append(buffer[address + 4]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 5]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 6]);
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 7]);
				//strbuild.Append((char)0x09);

				//strbuild.Append(buffer[address + 8] + "0%");
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 9] + "0%");
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0xA] + "0%");
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0xB] + "0%");
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0xC] + "0%");
				//strbuild.Append((char)0x09);
				//strbuild.Append(buffer[address + 0xD] + "0%");
				//strbuild.Append((char)0x09);

				//for (int i = 0; i < 16; i++)
				//{
				//	int psyid = Bits.getInt16(buffer, address + 0xE + i * 4);
				//	getText(strbuild, buffer, 0x221B5C + ((0 + psyid) << 2));
				//	strbuild.Append((char)0x09);
				//	strbuild.Append(Bits.getInt16(buffer, address + 0x10 + i * 4));
				//	strbuild.Append((char)0x09);
				//}

				strbuild.Append(buffer[address + 0x4E]);
				strbuild.Append((char)0x09);
				strbuild.Append(buffer[address + 0x4F]);
				strbuild.Append((char)0x09);
				strbuild.Append(buffer[address + 0x50]);

				strbuild.AppendLine(); //0x0D, 0x0A

				//strbuild.AppendLine("Enemy ID: " + Bits.getInt32(buffer, address));
				//strbuild.AppendLine("Level: " + buffer[address + 4]);
				//strbuild.AppendLine("?: " + buffer[address + 5]);
				//strbuild.AppendLine("HP: " + Bits.getInt16(buffer, address + 6));
				//strbuild.AppendLine("PP: " + Bits.getInt16(buffer, address + 8));
				//strbuild.AppendLine("Attack: " + Bits.getInt16(buffer, address + 10));
				//strbuild.AppendLine("Defense: " + Bits.getInt16(buffer, address + 12));
				//strbuild.AppendLine("Agility: " + Bits.getInt16(buffer, address + 14));
				//strbuild.AppendLine("Luck: " + buffer[address + 16]);
				//strbuild.AppendLine("Turns: " + buffer[address + 17]);
				//strbuild.AppendLine("HP Regen: " + buffer[address + 18]);
				//strbuild.AppendLine("PP Regen: " + buffer[address + 19]);

				//strbuild.AppendLine(); //0x0D, 0x0A



				//Class data

				
			//	entry += 1;
			//	address += 0x58; //address += 0x58;
			//}
			//System.IO.File.WriteAllText(@"C:\Users\Tea\Desktop\gs3enemiestab.txt", strbuild.ToString());
			//0x17726C + entry * 0x58;
		}
		//0x16E
		void getText(System.Text.StringBuilder strbuild, byte[] buffer, int address)
		{
			int addr2 = Bits.getInt32(buffer, address);
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
