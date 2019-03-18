using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing; //Point, Size

//using System.Drawing; //Bitmap/Rectangle
using System.Drawing.Imaging; //PixelFormat
using System.Runtime.InteropServices; //Marshal

namespace gsmagic
{
	class HexEditor
	{
		Form tt = new Form();
		int scnw = 800;
		int scnh = 600;
		PictureBox pictureBox1 = new PictureBox();
		public void init()
		{
			pictureBox1.Location = new Point(0, 0);// x + tbx.Width, y);
			pictureBox1.Width = scnw;// bn.Width = 50;
			pictureBox1.Height = scnh;// bn.Height = 20;
			//bn.Font = Globals.font;
			//bn.Text = "Edit";
			//bn.Click += button_Click;
			tt.Controls.Add(pictureBox1);
			tt.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyDown);
			tt.Show();
		}
		private void KeyDown(object sender, KeyEventArgs e)
		{
			//MessageBox.Show(e.KeyData.ToString());
			switch (e.KeyCode)
			{
				case (Keys)0x20: //space bar
					//romi -= 0x100;
					if (romi < 0)
						romi = 0;
					//nBytes = 4;
					xStart = 10 + nBytes * 16;
					xa = -16;
					xb = nBytes * -xa * 2 + -xa; //150;
					//xStart = 10 + nBytes * -xa;
					break;
				case (Keys)0x21: //page up
					//romi += 0x100;
					xa = -16;
					xb = -16;
					//xStart = 10 + 16 * -xa;
					xStart = 10 + 16 * 16 + ((16 / nBytes) - 1) * 16; //(16 >> (nBytes - 1)) - 1
					break;
			}
			disp();
		}
		int nBytes = 4;
		int xa = -16;
		int xb = 0;
		int xStart = 0;
		int romi = 0;
		public void disp()
		{
			int[] bmpdata = new int[scnw * scnh];

			//BD
			for (int i = 0; i < (scnw * scnh); i += 0x1)
			{
				bmpdata[i] = 0x006888; // 0x102080;
			}
			//for (int y = 0; y < scnh; y += 0x1)
			//{
			//	for (int x = 0; x < scnw; x += 0x1)
			//	{

			//	}
			//}
			byte[] rom = Globals.mainForm.rom;
			//drawStringI("Say, say, say, hey, hey now, baby! -Adam Levine/Maroon 5", bmpdata, 10, 10);
			//int nBytes = 4;
			////int xStart = 10 + nBytes * 16;
			////int xa = -16;
			////int xb = 150;
			////int xStart = 10 + nBytes * -xa;

			//int xa = -16;
			//int xb = -8;
			//int xStart = 10 + 16 * -xa;

			//int nextNumberX = 150; //224;
			int romi = this.romi;
			for (int y = 0; y < 16; y++)
			{
				int xx = xStart;
				drawStringI(romi.ToString("X8"), bmpdata, 10, 30 + y * 12);
				xx += 100;
				for (int x = 0; x < 16;) //Byte column
				{
					//int xx = 10 + x * nextNumberX;
					//xx += nBytes * 20;
					//xx += nextNumberX;

					//for (int a = nBytes; a > 0; a--)
					//{
					//xx += xa; //-16; // int xx = 10 + x * nextNumberX + a * 8;
					//drawStringI((rom[y * 16 + x]).ToString("X2"), bmpdata, 10 + x * 24, 30 + y * 12);
					drawStringI((rom[romi] & 0xF).ToString("X1"), bmpdata, xx + 8, 30 + y * 12);
					drawStringI((rom[romi++] >> 4).ToString("X1"), bmpdata, xx, 30 + y * 12);
					x++;

					xx += xa;
					//}
					if ((x & (nBytes - 1)) == 0)
						xx += xb; //  nextNumberX;
				}
			}
			pictureBox1.Width = scnw; pictureBox1.Height = scnh;
			pictureBox1.Image = PixelsToImage(bmpdata, scnw, scnh);
		}
		public static Bitmap PixelsToImage(int[] array, int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
			IntPtr pNative = bitmapData.Scan0;
			Marshal.Copy(array, 0, pNative, width * height);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
		void drawStringI(string text, int[] bmpdata, int x, int y)
		{
			//MessageBox.Show(((byte)text[1]).ToString());
			//0805A4E0 = Italics font
			byte[] rom = Globals.mainForm.rom;
			for (int stri = 0; stri < text.Length; stri++)
			{
				int chri = (byte)text[stri] - 0x20;
				for (int yi = 0; yi < 14; yi++)
				{
					int row = Bits.getInt16(rom, 0x5A4E0 + (chri * 0x20) + 2 + (yi * 2));
					for (int xi = 0; xi < 14; xi++)
					{
						if ((row & 0x8000) != 0)
						{
							bmpdata[(y + yi) * scnw + (x + xi)] = 0xF8F8F8;
							bmpdata[(y + yi + 1) * scnw + (x + xi + 1)] = 0;
						}
						row <<= 1;
					}
				}
				x += Bits.getInt16(rom, 0x5A4E0 + (chri * 0x20));
			}
		}
	}
}
