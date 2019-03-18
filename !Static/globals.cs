//using System.Windows.Forms; //Control
using System.Drawing; //Point, Size
namespace gsmagic
{
	static class Globals //Mainly Global Variables. (Probably will be ROM'specific addresses loaded from an xml or json file/not sure.) 
	{
		//Try not to put too much here, as Globals/statics are not that flexible? (Consider storing datatypes in a class/object, and linking from here, if needed?)

		//Wasn't sure about putting this here - Further planning needed? - Either way, the program does need cleanup, so...
		static public Form1 mainForm;
		static public Editors editorsForm;

		//Not sure if doing new Font objects are slow... But either way, this is reused for most Controls, so here is a central place to edit it.
		//static public Font font = new Font("Lucida Console", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
		static public Font font = new Font("Consolas", 8, FontStyle.Regular, GraphicsUnit.Point, 0); // Shorter width, but taller?
	}
}