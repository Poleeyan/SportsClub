using System;
using System.Windows.Forms;

namespace SportsClub
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			ApplicationConfiguration.Initialize();
			Application.Run(new MainForm());
		}
	}
}
