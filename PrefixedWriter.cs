using System.Text;

namespace AuctionSystem
{
    class PrefixedWriter : TextWriter
    { 
        private TextWriter originalOut;

        public PrefixedWriter()
        {
            if (!Directory.Exists(Program.homeFolder))
            {
                Directory.CreateDirectory(Program.homeFolder);
            }  
            File.WriteAllText(Program.homeFolder + "log.txt", String.Empty);
            File.WriteAllText(Program.homeFolder + "importantLog.txt", String.Empty);
            originalOut = Console.Out;
        }

        public override Encoding Encoding
        {
            get { return new System.Text.ASCIIEncoding(); }
        }
        public override void WriteLine(string message)
        {
            string str = String.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.ffffff"), message);
            File.AppendAllText(Program.homeFolder + "log.txt", str + "\n");
            originalOut.WriteLine(str);
        }
        public override void Write(string message)
        {
            string str = String.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.ffffff"), message);
            File.AppendAllText(Program.homeFolder + "log.txt", str + "\n");
            originalOut.Write(str);
        }

        public static void WriteLineImprtant(string message)
        {
            string str = String.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss.ffffff"), message);
            File.AppendAllText(Program.homeFolder + "importantLog.txt", str + "\n");
            Console.WriteLine(message);
        }
    }
}