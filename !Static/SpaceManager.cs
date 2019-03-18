using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsmagic
{
	class SpaceManager
	{
		int list_num;
		int[] freeAddr = new int[0x4000];
		int[] freeSize = new int[0x4000];
		//int organizeList(int* list)
		//{
		//	int i, ii, pos, val, size;
		//	for (i = 0; i < list_num - 1; i++)
		//	{
		//		val = list[(i << 1) + 1]; pos = i;
		//		for (ii = i + 1; ii < list_num; ii++)
		//		{
		//			if (list[(ii << 1) + 1] < val)
		//			{
		//				pos = ii; val = list[(ii << 1) + 1];
		//			}
		//		}
		//		if (pos != i)
		//		{
		//			val = list[pos << 1];
		//			size = list[(pos << 1) + 1];
		//			list[pos << 1] = list[i << 1];
		//			list[(pos << 1) + 1] = list[(i << 1) + 1];
		//			if (val == list[i << 1])
		//			{ list[i << 1] = val | 0x40000000; }
		//			else
		//			{ list[i << 1] = val; }
		//			list[(i << 1) + 1] = size;
		//		}
		//		if (list[i << 1] == list[(i - 1) << 1])
		//		{
		//			list_num--;
		//			for (ii = i; ii < list_num; ii++)
		//			{
		//				list[ii << 1] = list[(ii + 1) << 1];
		//				list[(ii << 1) + 1] = list[((ii + 1) << 1) + 1];
		//			}
		//			list[list_num << 1] = 0; list[(list_num << 1) + 1] = 0;
		//			i--;
		//		}
		//	}
		//	//return list_num;
		//	if ((LOAD_INT(FileTable()) >> 24) != 8)
		//	{ return list_num; }
		//	if ((LOAD_INT(FileTable()) & 0x1FFFFFF) != 0)
		//	{
		//		int tlist[32768]; int tnum, tnum2; tnum = list_num;
		//		pos = LOAD_INT(FileTable());
		//		WRITE_INT(FileTable(), pos | 0x70000000);
		//		tnum2 = spacemanager::mapSpace(tlist); list_num = tnum;
		//		spacemanager::freeSpace(list, pos, (tnum2 << 3) + 4);
		//	}
		//	else
		//	{ WRITE_INT(FileTable(), 0x7FFFFFFF); }
		//	pos = spacemanager::findSpace(list, (list_num << 3) + 4);
		//	if (pos > 0)
		//	{
		//		spacemanager::claimSpace(list, pos, (list_num << 3) + 4);
		//		i = 0;
		//		for (i = 0; i < list_num; i++)
		//		{
		//			val = list[i << 1];
		//			WRITE_INT(pos + (i << 3), list[i << 1]);
		//			WRITE_INT(pos + (i << 3) + 4, list[(i << 1) + 1]);
		//		}
		//		WRITE_INT(pos + (list_num << 3), 0x7FFFFFFF);
		//		WRITE_INT((FileTable()), pos);
		//		//WRITE_INT(0,0x12345678);
		//	}
		//	else
		//	{
		//		WRITE_INT(FileTable(), 0x08000000);
		//	}
		//	return list_num;
		//}

		int mapSpace(int* list)
		{
			int pos, val, size;
			for (list_num = 0; list_num < 32768; list_num++)
			{ list[list_num] = 0; }
			list_num = 0;
			pos = LOAD_INT(FileTable()) & 0x09FFFFFF;
			if ((pos & 0x1FFFFFF) != 0x0)
			{
				val = LOAD_INT(pos + (list_num << 3));
				while (val != 0x7FFFFFFF)
				{
					list[list_num << 1] = val;
					list[(list_num << 1) + 1] = LOAD_INT(pos + 4 + (list_num << 3)); list_num++;
					val = LOAD_INT(pos + (list_num << 3));
					if (val == 0)
					{ val = 0x7FFFFFFF; }
				}
				list[list_num << 1] = 0;
				list[(list_num << 1) + 1] = 0;
				return list_num;
			}
			val = 0;
			do
			{
				if (val == 1)
				{ val++; continue; }
				pos = LOAD_INT(FileTable() + 0x4 + (val << 2)) & 0x1FFFFFF;
				if (pos > (FileTable() & 0x1FFFFFF))
				{
					pos = FileSize();
				}
				size = 0;
				do
				{ size++; }
				while (LOAD_BYTE(pos - size) == 0);
				size--;
				size = size & 0xFFFFFFFC;
				pos -= size;
				if (pos + size == FileSize())
				{
					size += (32 << 20) - FileSize();
				}
				if (size > 0)
				{
					list[list_num << 1] = pos | 0x08000000;
					list[(list_num << 1) + 1] = size;
					list_num++;
				}
				val++;
			}
			while (pos <= (FileTable() & 0x1FFFFFF));
			spacemanager::organizeList(list);
			return list_num;
		}
		int freeSpace(int* list, int pos, int size)
		{
			int i, ii, index, pos2, pos3; index = -1; pos2 = pos + size;
			pos |= 0x10000000;
			for (i = 0; i < list_num; i++)
			{
				if ((list[i << 1] & 0x1FFFFFF) > (pos & 0x1FFFFFF))
				{
					if ((pos2 & 0x1FFFFFF) >= (list[i << 1] & 0x1FFFFFF))
					{
						pos = pos & 0x09FFFFFF | (list[i << 1] & 0xF6000000);
						pos3 = list[i << 1] + list[(i << 1) + 1];
						if ((pos3 & 0x1FFFFFF) > (pos2 & 0x1FFFFFF))
						{
							pos2 = pos3;
							size = (pos2 & 0x09FFFFFF) - (pos & 0x09FFFFFF);
						}
						list_num -= 1;
						for (ii = i; ii < list_num; ii++)
						{
							list[ii << 1] = list[(ii + 1) << 1];
							list[(ii << 1) + 1] = list[((ii + 1) << 1) + 1];
						}
						list[list_num << 1] = 0;
						list[(list_num << 1) + 1] = 0;
						i = -1;
					}
				}
				else
				{
					pos3 = list[i << 1] + list[(i << 1) + 1];
					if ((pos & 0x1FFFFFF) <= (pos3 & 0x1FFFFFF))
					{
						pos = list[i << 1];
						if ((pos3 & 0x1FFFFFF) > (pos2 & 0x1FFFFFF))
						{
							pos2 = pos3;
						}
						size = (pos2 & 0x09FFFFFF) - (pos & 0x09FFFFFF);
						list_num -= 1;
						for (ii = i; ii < list_num; ii++)
						{
							list[ii << 1] = list[(ii + 1) << 1];
							list[(ii << 1) + 1] = list[((ii + 1) << 1) + 1];
						}
						list[list_num << 1] = 0;
						list[(list_num << 1) + 1] = 0;
						i = -1;
					}
				}
			}

			list[list_num << 1] = (pos & 0x1FFFFFF) | 0x08000000;
			if ((pos >> 28) == 1)
			{ list[list_num << 1] |= 0x10000000; }
			else
			{
				for (i = 0; i < size; i++)
				{ WRITE_BYTE(pos + i, 0); }
			}
			list[(list_num << 1) + 1] = size;
			list_num++;
			spacemanager::organizeList(list);
			return list_num;
		}
		//int findSpace(int size, int alignment)
		//{

		//	return 0;
		//}
		int claimSpace(int size, int alignment)
		{
			//size=(size+3)&0xFFFFFFFC;
			if (size <= 0)
			{ return -list_num; }

			//Find space
			int pos = -1, i;
			for (i = 0; i < list_num; i++)
			{

				//if ((pos >= 0) //Following operations require this variable, and so to skip them, we have this.
				//	&& (((freeAddr[pos] & alignment) == 0) && ((freeAddr[i] & alignment) != 0)) //Only assign aligned addresses when possible.
				//	)//|| (freeSize[i] <= freeSize[pos])) //Find smallest size acceptable.
				//{
				//	continue;
				//}
				//If/else so I can THINK for once!
				if (pos >= 0) //Following operations require this variable, and so to skip them, we have this.
				{
					if ((freeAddr[i] & alignment) == 0)
					{
						if ((freeAddr[pos] & alignment) != 0) //Only assign aligned addresses when possible.
						{
							if (size <= freeSize[i]) //Makes sure we have enough space here.
							{
								pos = i;
							}
						}
					}
					else
					{
						if (freeSize[i] <= freeSize[pos]) //Find smallest size acceptable.
						{
							if (size <= freeSize[i]) //Makes sure we have enough space here.
							{
								pos = i;
							}
						}
					}
				}
				else
				{
					if (size <= freeSize[i]) //Makes sure we have enough space here.
					{
						pos = i;
					}
				}
			}

		
			i = pos;
			if (pos == -1) //No space found. (Todo: Calculate total free space, and make some room?)
			{ return pos; }
			pos = freeAddr[pos];//Todo: Check alignment

			//if (spacemanager::confirmSpace(list, pos, size) <= 0)
			//{ return -list_num; }

			freeAddr[i] += size;
			freeSize[i] -= size;

			if (freeSize[i] == 0) //Remove entry
			{
				list_num -= 1;
				freeAddr[i] = freeAddr[list_num];
				freeSize[i] = freeSize[list_num];
				//for (i = index; i < list_num; i++)
				//{
				//	list[i << 1] = list[(i + 1) << 1];
				//	list[(i << 1) + 1] = list[((i + 1) << 1) + 1];
				//}
				//list[list_num << 1] = 0; list[(list_num << 1) + 1] = 0;
			}

			//spacemanager::organizeList(list);
			//return list_num;
			return pos;
		}

	}
}
