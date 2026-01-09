namespace RetroVault
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            using (var splash = new Splash())
            {
                splash.Show(); Application.DoEvents(); // allow splash to paint
                Thread.Sleep(2000); // wait 2 seconds 

            }
            Application.Run(new Form1());
        }
    }
}